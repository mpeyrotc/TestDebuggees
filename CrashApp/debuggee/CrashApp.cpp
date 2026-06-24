// A C++ application that crashes due to an integer overflow bug.
// When compiled with /O2, the optimizer keeps intermediate values in registers,
// making the root cause only diagnosable by inspecting registers and disassembly.
//
// Bug: compute_index performs an unchecked multiplication that overflows,
// producing index 2 from large inputs. After the array is freed, readings
// becomes null, so readings[2] dereferences a null pointer -> crash.

#include <cstdio>
#include <cstdint>
#include <cstring>
#include <cstdlib>

struct SensorReading
{
    uint32_t timestamp;
    double   value;
    uint16_t sensorId;
    uint16_t flags;
};

// Computes a buffer index from sensor parameters.
// BUG: the multiplication of 'count * stride' can overflow uint32_t when
// count and stride are both large, wrapping around to a small value
// that passes the bounds check but the underlying pointer is null.
__declspec(noinline)
uint32_t compute_index(uint32_t baseOffset, uint32_t count, uint32_t stride)
{
    uint32_t index = baseOffset + (count * stride);  // bpid:Overflow
    return index;
}

// Reads a sensor value from the data buffer at the computed index.
__declspec(noinline)
double read_sensor_value(const SensorReading* readings, uint32_t totalCount,
                         uint32_t baseOffset, uint32_t sensorCount, uint32_t stride)
{
    uint32_t idx = compute_index(baseOffset, sensorCount, stride);

    // This check looks correct but the real problem is that readings is null
    if (idx < totalCount)
    {
        return readings[idx].value;     // bpid:Crash
    }

    return -1.0;
}

// Sets up sensor data and triggers the overflow scenario.
__declspec(noinline)
int process_sensors()
{
    const uint32_t TOTAL_READINGS = 1024;
    SensorReading* readings = new SensorReading[TOTAL_READINGS];

    // Initialize with test data
    for (uint32_t i = 0; i < TOTAL_READINGS; i++)
    {
        readings[i].timestamp = i * 100;
        readings[i].value = static_cast<double>(i) * 1.5;
        readings[i].sensorId = static_cast<uint16_t>(i % 256);
        readings[i].flags = 0;
    }

    printf("Sensor array at %p, %u entries\n", readings, TOTAL_READINGS);

    // Normal case: works fine
    double val1 = read_sensor_value(readings, TOTAL_READINGS, 0, 10, 4);
    printf("Reading 1: %.2f\n", val1);

    // Simulate a scenario where the data is no longer valid
    // (e.g., another thread freed it, or we processed a stale handle)
    delete[] readings;
    readings = nullptr;

    // BUG TRIGGER: sensorCount=0x80000001, stride=2 causes overflow:
    //   0x80000001 * 2 = 0x100000002, truncated to 0x00000002
    //   baseOffset(0) + 2 = 2, which passes the (idx < 1024) check
    //   But readings is null -> null + offset -> ACCESS VIOLATION
    double val2 = read_sensor_value(readings, TOTAL_READINGS, 0, 0x80000001, 2);
    printf("Reading 2: %.2f\n", val2);  // never reached

    return 0;
}

int main()
{
    printf("CrashApp - Sensor Data Processor\n");
    printf("=================================\n");
    int result = process_sensors();
    printf("Done. Result: %d\n", result);
    return result;
}
