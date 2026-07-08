# FibonacciCrashApp - Null-Pointer Access Violation Debuggee

A C++ application that crashes with an access violation when writing through a
null pointer. When compiled optimized (`/O2`) and shipped **without a PDB**, the
null base is invisible from source or locals: the reported faulting address is a
large non-zero value (a struct-member offset from a null base), so diagnosing the
root cause requires reading the faulting instruction (disassembly), the CPU
registers, and raw memory from the crash dump.

## The Bug

`AcquireBlock` returns `nullptr` when the requested `count` exceeds the fixed
64-slot payload, but the caller never checks the result:

- `requestedCount = 128` (read through a `volatile` so the optimizer cannot fold
  the call away or prove the pointer is null).
- `AcquireBlock(128)` returns `nullptr` because `128 > 64`.
- `FillFibonacci` writes to `block->values[i]`, i.e. `(0 + 0x2000 + i*4)`, through
  the null base &rarr; **access violation** (`EXCEPTION_ACCESS_VIOLATION`,
  `0xC0000005`).

`FibonacciBlock` has an oversized `reserved[0x2000]` header so the faulting
address is `0x2000`, not `0`. Only by disassembling the faulting store to find
its destination base register and then reading that register (which is zero) can
the null pointer be identified.

## Files

- `debuggee/Tests.cpp` - the crashing program.
- `debuggee/DumpWriter.h` / `debuggee/DumpWriter.cpp` - installs a
  `SetUnhandledExceptionFilter` handler that calls `MiniDumpWriteDump`
  (full memory + handles + thread info) to capture `FibonacciCrash.dmp` on crash.
- `debuggee/FibonacciCrash.vcxproj` / `FibonacciCrash.slnx` - the project. It uses
  a deterministic + `\__assets\` pathmap so the dump's source records resolve
  relative to the test's assets folder.

## Build

Requires MSVC (Visual Studio C++ tools). Build **Release|x64** so the binary is
optimized:

```
msbuild debuggee\FibonacciCrash.vcxproj /p:Configuration=Release /p:Platform=x64
```

Or open `debuggee\FibonacciCrash.slnx` in Visual Studio and build Release|x64.

## Usage

Run `FibonacciCrash.exe`; it crashes and writes `FibonacciCrash.dmp` next to the
exe. Delete the accompanying `FibonacciCrash.pdb` so symbols are unavailable, then
upload the dump to a drop for the offline-eval test.

Used as a test debuggee for the Copilot debugger agent offline evaluation tests.
The crash is designed to exercise the register, disassembly, and memory-read tools.
