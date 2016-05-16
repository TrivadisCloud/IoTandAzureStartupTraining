using System.Windows;
using Trivadis.IoT.WPF.EventHubClientSimulator.DataAccess;
using Trivadis.IoT.WPF.EventHubClientSimulator.ViewModel;

namespace Trivadis.IoT.WPF.EventHubClientSimulator
{
  public partial class MainWindow : Window
  {
    public MainWindow()
    {
      InitializeComponent();

      DataContext = new MainViewModel(new EventPublisher());
    }
  }
}
