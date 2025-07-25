using LibreHardwareMonitor.Hardware;
using System;
using System.Collections.Generic;

namespace Spakov.TermBar.Models
{
    /// <summary>
    /// The performance model.
    /// </summary>
    /// <remarks>
    /// <para>Exposes sensors from <c>LibreHardwareMonitorLib</c> as properties
    /// that can be read.</para>
    /// <para>Intended to be interfaced with via polling.</para>
    /// </remarks>
#pragma warning disable IDE0079 // Remove unnecessary suppression
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CsWinRT1028:Class is not marked partial", Justification = "Not a view")]
#pragma warning restore IDE0079 // Remove unnecessary suppression
    internal class Performance : IDisposable
    {
        private const string CpuTotal = "CPU Total";
        private const string GpuCore = "GPU Core";
        private const string Memory = "Memory";

        private readonly UpdateVisitor _updateVisitor;
        private readonly Computer _computer;

        private readonly ISensor? _cpuPercent;
        private readonly ISensor? _gpuPercent;
        private readonly ISensor? _memoryPercent;

        private readonly Dictionary<Identifier, DateTime> _lastUpdates;

        private static readonly Performance s_instance = new();

        /// <summary>
        /// The singleton instance.
        /// </summary>
        public static Performance Instance => s_instance;

        /// <summary>
        /// The CPU percent.
        /// </summary>
        public static float? CpuPercent => Instance.GetSensorValue(Instance._cpuPercent);

        /// <summary>
        /// The GPU percent.
        /// </summary>
        public static float? GpuPercent => Instance.GetSensorValue(Instance._gpuPercent);

        /// <summary>
        /// The memory percent.
        /// </summary>
        public static float? MemoryPercent => Instance.GetSensorValue(Instance._memoryPercent);

        /// <summary>
        /// Initializes a <see cref="Performance"/>.
        /// </summary>
        private Performance()
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
                _updateVisitor.VisitSensor(sensor);
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
            if (!_lastUpdates.TryGetValue(sensor.Identifier, out DateTime lastUpdate))
            {
                _lastUpdates.Add(sensor.Identifier, DateTime.MinValue);
            }

            if (DateTime.Now.Subtract(lastUpdate).TotalMilliseconds > 100)
            {
                _updateVisitor.VisitHardware(sensor.Hardware);
                _lastUpdates[sensor.Identifier] = DateTime.Now;
            }
        }

        public void Dispose() => _computer.Close();
    }

    /// <summary>
    /// A visitor for updating hardware sensor values.
    /// </summary>
    internal class UpdateVisitor : IVisitor
    {
        public void VisitComputer(IComputer computer) => computer.Traverse(this);

        public void VisitHardware(IHardware hardware)
        {
            hardware.Update();

            foreach (IHardware subHardware in hardware.SubHardware)
            {
                subHardware.Accept(this);
            }
        }

        public void VisitSensor(ISensor sensor) {
        }

        public void VisitParameter(IParameter parameter) { 
        }
    }
}