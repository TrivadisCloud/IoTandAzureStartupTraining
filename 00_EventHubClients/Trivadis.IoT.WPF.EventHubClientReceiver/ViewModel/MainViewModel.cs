using System.Collections.ObjectModel;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Threading;
using Trivadis.IoT.WPF.EventHubClientReceiver.Command;
using Trivadis.IoT.WPF.EventHubClientReceiver.DataAccess;
using Trivadis.IoT.WPF.EventHubClientReceiver.Model;

namespace Trivadis.IoT.WPF.EventHubClientReceiver.ViewModel
{
  public class MainViewModel : ViewModelBase
  {
    private string _connectionString;
    private bool _isReceivingDataEnabled;
    private readonly EventHubAccess _eventHubAccess;
    private static readonly object _lock = new object();
    public MainViewModel()
    {
      _eventHubAccess = new EventHubAccess();
      _eventHubAccess.SensorDataReceived += EventHubAccess_SensorDataReceived;
      ReceivedTempSensorDataItems = new ObservableCollection<SensorData>();
      ReceivedHumSensorDataItems = new ObservableCollection<SensorData>();
      BindingOperations.EnableCollectionSynchronization(ReceivedHumSensorDataItems, _lock);
      BindingOperations.EnableCollectionSynchronization(ReceivedTempSensorDataItems, _lock);

      StartReceivingEventsCommand = new DelegateCommand(OnStartReceivingEventsExecute,OnStartReceivingEventsCanExecute);
    }

    public ObservableCollection<SensorData> ReceivedTempSensorDataItems { get; }
    public ObservableCollection<SensorData> ReceivedHumSensorDataItems { get; }

    public ICommand StartReceivingEventsCommand { get; }

    public string ConnectionString
    {
      get { return _connectionString; }
      set
      {
        _connectionString = value;
        OnPropertyChanged();
      }
    }

    private bool _isStarting;
    private bool OnStartReceivingEventsCanExecute(object arg)
    {
      return !_isStarting;
    }

    private void OnStartReceivingEventsExecute(object obj)
    {
      _isStarting = true;
      ((DelegateCommand)StartReceivingEventsCommand).RaiseCanExecuteChanged();
      try
      {
        _eventHubAccess.StartReceivingMessages(ConnectionString);
      }
      finally
      {
        _isStarting = false;
        ((DelegateCommand)StartReceivingEventsCommand).RaiseCanExecuteChanged();
      }
    }

    private void EventHubAccess_SensorDataReceived(object sender, SensorData e)
    {
      lock (_lock)
      {
        if (e.SensorType == "Temp")
        {
          ReceivedTempSensorDataItems.Insert(0, e);
        }
        else
        {
          ReceivedHumSensorDataItems.Insert(0, e);
        }
      }
    }
  }
}
