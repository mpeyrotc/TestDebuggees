// Copyright (c) Microsoft Corporation. All rights reserved.

#ifndef FIBONACCICRASH_DUMPWRITER_H
#define FIBONACCICRASH_DUMPWRITER_H

// Installs a process-wide handler that writes a full crash dump to `dumpFileName`
// (in the current working directory) the first time an unhandled exception is
// raised, then terminates the process. Intended for capturing a deterministic
// crash dump for offline debugger tests.
void InstallCrashDumpWriter(const wchar_t* dumpFileName);

#endif // FIBONACCICRASH_DUMPWRITER_H
