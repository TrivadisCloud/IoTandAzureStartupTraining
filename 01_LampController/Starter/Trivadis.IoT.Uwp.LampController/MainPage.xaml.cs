using Windows.Devices.Gpio;
using Windows.UI.Xaml.Controls;

namespace Trivadis.IoT.Uwp.LampController
{
  public sealed partial class MainPage : Page
  {
    private GpioPin _greenLEDPin;

    public MainPage()
    {
      InitializeComponent();
      this.Loaded += MainPage_Loaded;

    }

    private void MainPage_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
    {
      var gpio = GpioController.GetDefault();

      if (gpio != null)
      {
        _greenLEDPin = gpio.OpenPin(17);

        _greenLEDPin.SetDriveMode(GpioPinDriveMode.Output);

        _greenLEDPin.Write(GpioPinValue.High);
      }
    }

    private void ToggleGreenLight_Unchecked(object sender, Windows.UI.Xaml.RoutedEventArgs e)
    {
      _greenLEDPin.Write(GpioPinValue.High);
    }
    private void ToggleGreenLight_Checked(object sender, Windows.UI.Xaml.RoutedEventArgs e)
    {
      _greenLEDPin.Write(GpioPinValue.Low);
    }

    private void ToggleRedLight_Unchecked(object sender, Windows.UI.Xaml.RoutedEventArgs e)
    {
      // TODO: Turn the red led off
    }
    private void ToggleRedLight_Checked(object sender, Windows.UI.Xaml.RoutedEventArgs e)
    {
      // TODO: Turn the red led on
    }
  }
}
