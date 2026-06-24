# CrashApp - Integer Overflow Crash Debuggee

A C++ application that crashes due to an integer overflow bug. When compiled with `/O2` optimizations, the optimizer keeps intermediate values in registers, making the root cause only diagnosable by inspecting CPU registers and disassembly.

## The Bug

`compute_index` performs an unchecked `uint32_t` multiplication that overflows:
- `sensorCount=0x80000001`, `stride=2` → `0x80000001 * 2 = 0x100000002`
- Truncated to `uint32_t`: `0x00000002`
- `baseOffset(0) + 2 = 2`, which passes the `(idx < 1024)` bounds check
- But `readings` has been set to `nullptr` → **access violation**

## Build

Requires MSVC (Visual Studio C++ tools):

```
cl /O2 /EHsc CrashApp.cpp /Fe:CrashApp.exe
```

Or open `CrashApp.vcxproj` in Visual Studio and build Release|x64.

## Usage

Used as a test debuggee for Copilot debugger agent offline evaluation tests. The crash is designed to exercise register inspection and disassembly tools.
