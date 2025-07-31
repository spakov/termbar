using LibreHardwareMonitor.Hardware;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Spakov.TermBar.Models
{
    /// <summary>
    /// The performance model.
    /// </summary>
    /// <remarks>
    /// <para>Exposes sensors from <c>LibreHardwareMonitorLib</c> as properties
    /// that can be read.</para>
    /// <para>For non-x86/x64, falls back to the Windows performance API, since
    /// <c>LibreHardwareMonitorLib</c> appears to be incompatible with
    /// ARM64. In this case, GPU performance monitoring is not
    /// supported, since the <see cref="PerformanceCounter"/> API does not
    /// support GPU metrics well.</para>
    /// <para>Intended to be interfaced with via polling.</para>
    /// </remarks>
#pragma warning disable IDE0079 // Remove unnecessary suppression
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CsWinRT1028:Class is not marked partial", Justification = "Not a view")]
#pragma warning restore IDE0079 // Remove unnecessary suppression
    internal class Performance : IDisposable
    {
        /// <summary>
        /// The string to display in place of an invalid value.
        /// </summary>
        public const string ErrorIcon = "⚠️";

        /// <summary>
        /// The name of the <c>LibreHardwareMonitorLib</c> "CPU Total" sensor.
        /// </summary>
        private const string CpuTotal = "CPU Total";

        /// <summary>
        /// The name of the <c>LibreHardwareMonitorLib</c> "CPU Core" sensor.
        /// </summary>
        private const string GpuCore = "GPU Core";

        /// <summary>
        /// The name of the <c>LibreHardwareMonitorLib</c> "Memory" sensor.
        /// </summary>
        private const string Memory = "Memory";

        private readonly UpdateVisitor? _updateVisitor;
        private readonly Computer? _computer;

        private readonly ISensor? _cpuPercent;
        private readonly ISensor? _gpuPercent;
        private readonly ISensor? _memoryPercent;

        private readonly Dictionary<Identifier, DateTime>? _lastUpdates;

        private readonly PerformanceCounter? _legacyCpuPercent;
        private readonly PerformanceCounter? _legacyMemoryPercent;

        private static readonly Performance s_instance = new();

        /// <summary>
        /// The singleton instance.
        /// </summary>
        public static Performance Instance => s_instance;

        /// <summary>
        /// The running process's architecture.
        /// </summary>
        internal static Architecture Architecture => RuntimeInformation.ProcessArchitecture;

        /// <summary>
        /// The CPU percent.
        /// </summary>
        public static float? CpuPercent
        {
            get
            {
                return Architecture is Architecture.X86 or Architecture.X64
                    ? Instance.GetSensorValue(Instance._cpuPercent)
                    : Instance._legacyCpuPercent!.NextValue();
            }
        }

        /// <summary>
        /// The GPU percent.
        /// </summary>
        public static float? GpuPercent
        {
            get
            {
                return Architecture is Architecture.X86 or Architecture.X64
                    ? Instance.GetSensorValue(Instance._gpuPercent)
                    : null;
            }
        }

        /// <summary>
        /// The memory percent.
        /// </summary>
        public static float? MemoryPercent
        {
            get
            {
                return Architecture is Architecture.X86 or Architecture.X64
                    ? Instance.GetSensorValue(Instance._memoryPercent)
                    : Instance._legacyMemoryPercent!.NextValue();
            }
        }

        /// <summary>
        /// Initializes a <see cref="Performance"/>.
        /// </summary>
        private Performance()
        {
            if (Architecture is Architecture.X86 or Architecture.X64)
            {
                _updateVisitor = new();
                _computer = new() { IsCpuEnabled = true, IsMemoryEnabled = true, IsGpuEnabled = true };
                _computer.Open();
                _computer.Accept(_updateVisitor);

                foreach (IHardware hardware in _computer.Hardware)
                {
                    if (hardware.HardwareType == HardwareType.Cpu)
                    {
                        foreach (ISensor sensor in hardware.Sensors)
                        {
                            if (sensor.SensorType == SensorType.Load && sensor.Name == CpuTotal)
                            {
                                _cpuPercent = sensor;
                            }
                        }
                    }
                    else if (hardware.HardwareType == HardwareType.GpuNvidia)
                    {
                        foreach (ISensor sensor in hardware.Sensors)
                        {
                            if (sensor.SensorType == SensorType.Load && sensor.Name == GpuCore)
                            {
                                _gpuPercent = sensor;
                            }
                        }
                    }
                    else if (hardware.HardwareType == HardwareType.Memory)
                    {
                        foreach (ISensor sensor in hardware.Sensors)
                        {
                            if (sensor.SensorType == SensorType.Load && sensor.Name == Memory)
                            {
                                _memoryPercent = sensor;
                            }
                        }
                    }
                }

                _lastUpdates = [];
            }
            else
            {
                // Fall back to Windows performance monitoring
                _legacyCpuPercent = new("Processor", "% Processor Time", "_Total");
                _legacyMemoryPercent = new("Memory", "% Committed Bytes In Use");
            }
        }

        /// <summary>
        /// Gets the value of <paramref name="sensor"/>.
        /// </summary>
        /// <param name="sensor">An <see cref="ISensor"/>.</param>
        /// <returns><paramref name="sensor"/>'s value.</returns>
        internal float? GetSensorValue(ISensor? sensor)
        {
            if (sensor is not null)
            {
                UpdateSensorHardware(sensor);
                _updateVisitor!.VisitSensor(sensor);
            }

            return sensor?.Value;
        }

        /// <summary>
        /// Updates the hardware providing <paramref name="sensor"/>.
        /// </summary>
        /// <remarks>Only updates if it has been longer than 100 milliseconds
        /// since the last update.</remarks>
        /// <param name="sensor">An <see cref="ISensor"/>.</param>
        private void UpdateSensorHardware(ISensor sensor)
        {
            if (!_lastUpdates!.TryGetValue(sensor.Identifier, out DateTime lastUpdate))
            {
                _lastUpdates.Add(sensor.Identifier, DateTime.MinValue);
            }

            if (DateTime.Now.Subtract(lastUpdate).TotalMilliseconds > 100)
            {
                _updateVisitor!.VisitHardware(sensor.Hardware);
                _lastUpdates[sensor.Identifier] = DateTime.Now;
            }
        }

        public void Dispose() => _computer!.Close();
    }

    /// <summary>
    /// A visitor for updating hardware sensor values.
    /// </summary>
    internal class UpdateVisitor : IVisitor
    {
        /// <summary>
        /// Initializes an <see cref="UpdateVisitor"/>.
        /// </summary>
        /// <exception cref="PlatformNotSupportedException"></exception>
        public UpdateVisitor()
        {
            if (Performance.Architecture is not Architecture.X86 and not Architecture.X64)
            {
                throw new PlatformNotSupportedException("LibreHardwareMonitor only supports x86 and x64.");
            }
        }

        public void VisitComputer(IComputer computer) => computer.Traverse(this);

        public void VisitHardware(IHardware hardware)
        {
            hardware.Update();

            foreach (IHardware subHardware in hardware.SubHardware)
            {
                subHardware.Accept(this);
            }
        }

        public void VisitSensor(ISensor sensor)
        {
        }

        public void VisitParameter(IParameter parameter)
        {
        }
    }
}