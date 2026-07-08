// Copyright (c) Microsoft Corporation. All rights reserved.
//
// A tiny native debuggee that contains a deliberate bug so that a crash dump can
// be captured and used by the debugger agent offline-eval tests.
//
// This debuggee is built optimized (/O2) and shipped WITHOUT a PDB, so the
// debugger has no source or symbols for it. Diagnosing the crash therefore
// requires reading the faulting instruction (disassembly) and the CPU registers
// from the dump rather than relying on source lines or locals.
//
// The bug is deliberately constructed so that the exception record alone is NOT
// enough to explain it: the faulting write goes through a NULL base pointer plus
// a large structure-member offset, so the reported faulting address is a large
// non-zero value rather than 0. The only way to discover that the underlying
// cause is a null pointer is to disassemble the faulting store instruction to
// find which register holds the destination base, then read that register from
// the CPU register state and observe that it is zero.

#include <cstdio>

#include "DumpWriter.h"

// A record whose integer payload lives at a large, fixed offset from the start
// of the structure. The oversized reserved header pushes `values` far away from
// the base of the record, so a write through a NULL record pointer faults at
// that offset instead of at address zero.
struct FibonacciBlock
{
    unsigned char reserved[0x2000];
    int values[64];
};

// Returns a pointer to a fixed-size record, or nullptr when the requested count
// does not fit.
//
// BUG: the caller never checks the result for null. When `count` exceeds the
// capacity of `values`, this returns nullptr. The subsequent write through that
// null base pointer targets `&block->values[i]`, i.e. address (0 + 0x2000 + i*4),
// so the access violation reports a large non-zero faulting address. Because the
// build is optimized and stripped of symbols, the null base is only visible by
// disassembling the faulting store instruction to identify the destination base
// register and then reading that register's (zero) value.
__declspec(noinline)
static FibonacciBlock* AcquireBlock(int count)
{
    static FibonacciBlock block;
    if (count > 64)
    {
        return nullptr;
    }

    return &block;
}

// Fills the record's payload with the first `count` Fibonacci numbers. The store
// to block->values[i] faults when `block` is null.
__declspec(noinline)
static void FillFibonacci(FibonacciBlock* block, int count)
{
    for (int i = 0; i < count; ++i)
    {
        block->values[i] = (i < 2) ? i : block->values[i - 1] + block->values[i - 2];
    }
}

int main()
{
    // Install a handler that writes a full crash dump when the (unhandled)
    // access violation is raised, so the dump captures the faulting frame,
    // the register state, and the exception information.
    InstallCrashDumpWriter(L"FibonacciCrash.dmp");

    // Read the count through a volatile so the optimizer cannot fold the call to
    // AcquireBlock or prove the resulting pointer is null. 128 exceeds the
    // 64-slot payload, so AcquireBlock returns nullptr.
    volatile int requestedCount = 128;
    int count = requestedCount;

    FibonacciBlock* block = AcquireBlock(count);
    FillFibonacci(block, count); // access violation writing through the null block

    std::printf("Last Fibonacci number: %d\n", block->values[count - 1]);
    return 0;
}
