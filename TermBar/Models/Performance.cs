using System.Diagnostics;

namespace Spakov.TermBar.Models {
  /// <summary>
  /// The performance model.
  /// </summary>
  /// <remarks>Intended to be interfaced with via polling.</remarks>
  internal class Performance {
    private readonly PerformanceCounter cpuPercent;
    private readonly PerformanceCounter memoryPercent;

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
    /// The memory percent.
    /// </summary>
    public static float MemoryPercent => Instance.memoryPercent.NextValue();

    /// <summary>
    /// Initializes a <see cref="Performance"/>.
    /// </summary>
    private Performance() {
      cpuPercent = new("Processor", "% Processor Time", "_Total");
      memoryPercent = new("Memory", "% Committed Bytes In Use");
    }
  }
}