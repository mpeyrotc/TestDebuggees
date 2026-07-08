// Copyright (c) Microsoft Corporation. All rights reserved.

#include "DumpWriter.h"

#include <windows.h>
#include <dbghelp.h>
#include <cstdio>
#include <cwchar>

#pragma comment(lib, "dbghelp.lib")

namespace
{
    const wchar_t* g_dumpFileName = L"crash.dmp";

    // Top-level SEH filter. An access violation is a Windows SEH exception
    // (code 0xC0000005); when it is left unhandled, this filter runs with
    // EXCEPTION_POINTERS pointing at the faulting instruction, so the dump
    // captures the faulting frame, the register state, and the exception record.
    LONG WINAPI WriteCrashDump(EXCEPTION_POINTERS* exceptionPointers)
    {
        HANDLE dumpFile = ::CreateFileW(
            g_dumpFileName,
            GENERIC_WRITE,
            0,
            nullptr,
            CREATE_ALWAYS,
            FILE_ATTRIBUTE_NORMAL,
            nullptr);

        if (dumpFile != INVALID_HANDLE_VALUE)
        {
            MINIDUMP_EXCEPTION_INFORMATION exceptionInfo{};
            exceptionInfo.ThreadId = ::GetCurrentThreadId();
            exceptionInfo.ExceptionPointers = exceptionPointers;
            exceptionInfo.ClientPointers = FALSE;

            // A full memory dump so the debugger can inspect the faulting
            // instruction bytes and the stack of the crashing thread.
            const MINIDUMP_TYPE dumpType = static_cast<MINIDUMP_TYPE>(
                MiniDumpWithFullMemory |
                MiniDumpWithHandleData |
                MiniDumpWithThreadInfo);

            ::MiniDumpWriteDump(
                ::GetCurrentProcess(),
                ::GetCurrentProcessId(),
                dumpFile,
                dumpType,
                &exceptionInfo,
                nullptr,
                nullptr);

            ::CloseHandle(dumpFile);
            std::wprintf(L"Wrote crash dump to %s\n", g_dumpFileName);
        }

        // Terminate the process; the crash has been captured.
        return EXCEPTION_EXECUTE_HANDLER;
    }
}

void InstallCrashDumpWriter(const wchar_t* dumpFileName)
{
    if (dumpFileName != nullptr)
    {
        g_dumpFileName = dumpFileName;
    }

    ::SetUnhandledExceptionFilter(WriteCrashDump);
}
