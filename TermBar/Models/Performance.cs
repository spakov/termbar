using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Spakov.TermBar.Models {
  /// <summary>
  /// The performance model.
  /// </summary>
  /// <remarks>Intended to be interfaced with via polling.</remarks>
  internal class Performance {
    private readonly PerformanceCounter cpuPercent;
    private readonly PerformanceCounter memoryPercent;

    private PerformanceCounterCategory? gpuPerformanceCounterCategory;
    private readonly Dictionary<string, PerformanceCounter> gpuPerformanceCounters;

    private static readonly Performance instance = new();

    /// <summary>
    /// The singleton instance.
    /// </summary>
    public static Performance Instance => instance;

    /// <summary>
    /// The CPU percent.
    /// </summary>
    public static float CpuPercent => Instance.cpuPercent.NextValue();

    /// <summary>
    /// The GPU percent.
    /// </summary>
    public static float GpuPercent => Instance.GetGpuPercent();

    /// <summary>
    /// The memory percent.
    /// </summary>
    public static float MemoryPercent => Instance.memoryPercent.NextValue();

    /// <summary>
    /// Initializes a <see cref="Performance"/>.
    /// </summary>
    private Performance() {
      cpuPercent = new("Processor", "% Processor Time", "_Total");
      memoryPercent = new("Memory", "% Committed Bytes In Use");

      gpuPerformanceCounters = [];
    }

    /// <summary>
    /// Returns the maximum of all 3D GPU performance counters.
    /// </summary>
    /// <returns>A <see langword="float"/> average.</returns>
    private float GetGpuPercent() {
      gpuPerformanceCounterCategory ??= new PerformanceCounterCategory("GPU Engine");

      float maxValue = 0.0f;

      foreach (string instance in gpuPerformanceCounterCategory.GetInstanceNames()) {
        if (instance.EndsWith("3D")) {
          if (!gpuPerformanceCounters.ContainsKey(instance)) {
            gpuPerformanceCounters.Add(instance, new("GPU Engine", "Utilization Percentage", instance));
          }
        }
      }

      List<string> instancesToRemove = [];

      foreach (KeyValuePair<string, PerformanceCounter> performanceCounter in gpuPerformanceCounters) {
        float value;

        if (!gpuPerformanceCounterCategory.InstanceExists(performanceCounter.Key)) {
          instancesToRemove.Add(performanceCounter.Key);
          continue;
        }

        value = performanceCounter.Value.NextValue();

        if (maxValue < value) {
          maxValue = value;
        }
      }

      foreach (string instance in instancesToRemove) {
        gpuPerformanceCounters.Remove(instance);
      }

      return maxValue;
    }
  }
}