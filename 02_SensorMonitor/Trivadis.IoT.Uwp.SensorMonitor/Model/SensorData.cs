using System;

namespace Trivadis.IoT.Uwp.SensorMonitor.Model
{
  public class SensorData
  {
    public string DeviceName { get; set; }
    public DateTime ReadTime { get; internal set; }
    public string SensorType { get; set; }
    public double Value { get; set; }
  }
}
