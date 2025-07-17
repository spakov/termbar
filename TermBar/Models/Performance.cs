using LibreHardwareMonitor.Hardware;
using System;
using System.Collections.Generic;

namespace Spakov.TermBar.Models {
  /// <summary>
  /// The performance model.
  /// </summary>
  /// <remarks>Intended to be interfaced with via polling.</remarks>
#pragma warning disable IDE0079 // Remove unnecessary suppression
  [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CsWinRT1028:Class is not marked partial", Justification = "Not a view")]
#pragma warning restore IDE0079 // Remove unnecessary suppression
  internal class Performance : IDisposable {
    private const string cpuTotal = "CPU Total";
    private const string gpuCore = "GPU Core";
    private const string memory = "Memory";

    private readonly UpdateVisitor updateVisitor;
    private readonly Computer computer;

    private readonly ISensor? cpuPercent;
    private readonly ISensor? gpuPercent;
    private readonly ISensor? memoryPercent;

    private readonly Dictionary<Identifier, DateTime> lastUpdates;

    private static readonly Performance instance = new();

    /// <summary>
    /// The singleton instance.
    /// </summary>
    public static Performance Instance => instance;

    /// <summary>
    /// The CPU percent.
    /// </summary>
    public static float? CpuPercent => Instance.GetSensorValue(Instance.cpuPercent);

    /// <summary>
    /// The GPU percent.
    /// </summary>
    public static float? GpuPercent => Instance.GetSensorValue(Instance.gpuPercent);

    /// <summary>
    /// The memory percent.
    /// </summary>
    public static float? MemoryPercent => Instance.GetSensorValue(Instance.memoryPercent);

    /// <summary>
    /// Initializes a <see cref="Performance"/>.
    /// </summary>
    private Performance() {
      updateVisitor = new();
      computer = new() { IsCpuEnabled = true, IsMemoryEnabled = true, IsGpuEnabled = true };
      computer.Open();
      computer.Accept(updateVisitor);

      foreach (IHardware hardware in computer.Hardware) {
        if (hardware.HardwareType == HardwareType.Cpu) {
          foreach (ISensor sensor in hardware.Sensors) {
            if (sensor.SensorType == SensorType.Load && sensor.Name == cpuTotal) {
              cpuPercent = sensor;
            }
          }
        } else if (hardware.HardwareType == HardwareType.GpuNvidia) {
          foreach (ISensor sensor in hardware.Sensors) {
            if (sensor.SensorType == SensorType.Load && sensor.Name == gpuCore) {
              gpuPercent = sensor;
            }
          }
        } else if (hardware.HardwareType == HardwareType.Memory) {
          foreach (ISensor sensor in hardware.Sensors) {
            if (sensor.SensorType == SensorType.Load && sensor.Name == memory) {
              memoryPercent = sensor;
            }
          }
        }
      }

      lastUpdates = [];
    }

    /// <summary>
    /// Gets the value of <paramref name="sensor"/>.
    /// </summary>
    /// <param name="sensor">An <see cref="ISensor"/>.</param>
    /// <returns><paramref name="sensor"/>'s value.</returns>
    internal float? GetSensorValue(ISensor? sensor) {
      if (sensor is not null) {
        UpdateSensorHardware(sensor);
        updateVisitor.VisitSensor(sensor);
      }

      return sensor?.Value;
    }

    /// <summary>
    /// Updates the hardware providing <paramref name="sensor"/>.
    /// </summary>
    /// <remarks>Only updates if it has been longer than 100 milliseconds since
    /// the last update.</remarks>
    /// <param name="sensor">An <see cref="ISensor"/>.</param>
    private void UpdateSensorHardware(ISensor sensor) {
      if (!lastUpdates.TryGetValue(sensor.Identifier, out DateTime lastUpdate)) {
        lastUpdates.Add(sensor.Identifier, DateTime.MinValue);
      }

      if (DateTime.Now.Subtract(lastUpdate).TotalMilliseconds > 100) {
        updateVisitor.VisitHardware(sensor.Hardware);
        lastUpdates[sensor.Identifier] = DateTime.Now;
      }
    }

    public void Dispose() => computer.Close();
  }

  /// <summary>
  /// A visitor for updating hardware sensor values.
  /// </summary>
  internal class UpdateVisitor : IVisitor {
    public void VisitComputer(IComputer computer) => computer.Traverse(this);

    public void VisitHardware(IHardware hardware) {
      hardware.Update();
      foreach (IHardware subHardware in hardware.SubHardware) subHardware.Accept(this);
    }

    public void VisitSensor(ISensor sensor) { }

    public void VisitParameter(IParameter parameter) { }
  }
}