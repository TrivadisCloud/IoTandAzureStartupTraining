using System.Windows;
using Trivadis.IoT.WPF.EventHubClientReceiver.ViewModel;

namespace Trivadis.IoT.WPF.EventHubClientReceiver
{
  public partial class MainWindow : Window
  {
    public MainWindow()
    {
      InitializeComponent();
      DataContext = new MainViewModel();
    }
  }
}
