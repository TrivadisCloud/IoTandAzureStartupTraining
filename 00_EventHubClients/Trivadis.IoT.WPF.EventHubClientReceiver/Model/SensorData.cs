﻿using System;

namespace Trivadis.IoT.WPF.EventHubClientReceiver.Model
{
  public class SensorData
  {
    public string DeviceName { get; set; }
    public DateTime ReadTime { get; set; }
    public string SensorType { get; set; }
    public double Value { get; set; }
  }
}
