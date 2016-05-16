using Newtonsoft.Json;
using Sensors.Dht;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Trivadis.IoT.Uwp.SensorMonitor.Model;
using Trivadis.IoT.Uwp.SensorMonitor.ServiceAccess;
using Windows.Devices.Gpio;
using Windows.Security.ExchangeActiveSyncProvisioning;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Trivadis.IoT.Uwp.SensorMonitor
{
  public sealed partial class MainPage : Page
  {
    private Dht22 _dht22;
    private GpioPin _pinDht22;
    private GpioPin _pinRedLed;
    private GpioPin _pinGreenLed;
    private EventHubClient _eventHubClient;
    private bool _isRedLedOn;
    private bool _isGreenLedOn;
    private string _deviceName;

    public MainPage()
    {
      this.InitializeComponent();
      _eventHubClient = new EventHubClient();
      this.Loaded += MainPage_Loaded;
    }

    public float Temperature { get; private set; }
    public float Humidity { get; private set; }

    public bool IsGreenLedOn
    {
      get { return _isGreenLedOn; }
      set
      {
        _isGreenLedOn = value;
        _pinGreenLed.Write(_isGreenLedOn ?
           GpioPinValue.Low :
           GpioPinValue.High);
      }
    }

    public bool IsRedLedOn
    {
      get { return _isRedLedOn; }
      set
      {
        _isRedLedOn = value;
        _pinRedLed.Write(_isRedLedOn ?
          GpioPinValue.Low :
          GpioPinValue.High);
      }
    }


    private void MainPage_Loaded(object sender, RoutedEventArgs e)
    {
      var deviceInfo = new EasClientDeviceInformation();
      _deviceName = deviceInfo.FriendlyName;

      var gpio = GpioController.GetDefault();

      if (gpio == null)
      {
        Debug.WriteLine("GPIO is null");
        return;
      }

      // Initialize the DHT22 sensor
      _pinDht22 = gpio.OpenPin(22, GpioSharingMode.Exclusive);
      _dht22 = new Dht22(_pinDht22, GpioPinDriveMode.Input);

      // Initialize the green led
      _pinGreenLed = gpio.OpenPin(17);
      _pinGreenLed.Write(GpioPinValue.High);
      _pinGreenLed.SetDriveMode(GpioPinDriveMode.Output);

      // Initialize the red led
      _pinRedLed = gpio.OpenPin(27);
      _pinRedLed.Write(GpioPinValue.High);
      _pinRedLed.SetDriveMode(GpioPinDriveMode.Output);

      // Start a timer with 3 seconds interval
      DispatcherTimer timer = new DispatcherTimer();
      timer.Tick += Timer_Tick;
      timer.Interval = TimeSpan.FromSeconds(3);
      timer.Start();
    }

    private async void Timer_Tick(object sender, object e)
    {
      // Turn the LEDs off
      IsGreenLedOn = false;
      IsRedLedOn = false;
      this.Bindings.Update();
      await Task.Delay(500);

      // Read the DHT22 sensor data
      var reading = await _dht22.GetReadingAsync(1).AsTask();

      if (reading.IsValid)
      {
        Temperature = Convert.ToSingle(reading.Temperature);
        Humidity = Convert.ToSingle(reading.Humidity);

        Debug.WriteLine("Temp: " + Temperature);
        Debug.WriteLine("Hum: " + Humidity);
        IsGreenLedOn = true;

        // TODO: Comment this in after you've configured the Event Hub Client to point to your event hub
        //await SendToEventHubAsync();
      }
      else
      {
        IsRedLedOn = true;
      }

      this.Bindings.Update();
      Debug.WriteLine("Reading is valid: " + reading.IsValid);
    }

    private async Task SendToEventHubAsync()
    {
      var tempSensorData = new SensorData { DeviceName = _deviceName, SensorType = "Temp", Value = Temperature, ReadTime = DateTime.UtcNow };
      var humiditySensorData = new SensorData { DeviceName = _deviceName, SensorType = "Hum", Value = Humidity, ReadTime = DateTime.UtcNow };

      var tempJson = JsonConvert.SerializeObject(tempSensorData);
      var humidityJson = JsonConvert.SerializeObject(humiditySensorData);

      await _eventHubClient.PostDataAsync(tempJson);
      await _eventHubClient.PostDataAsync(humidityJson);
    }
  }
}
