using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using Whitestone.OpenSerialPortMonitor.SerialCommunication;
using Whitestone.OpenSerialPortMonitor.Main.Messages;
using System.IO.Ports;
using System.Windows;

namespace Whitestone.OpenSerialPortMonitor.Main.ViewModels
{
    public class SerialConnectorViewModel : PropertyChangedBase, IHandle<ConnectionError>
    {
        private readonly IEventAggregator _eventAggregator;
        
        public BindableCollection<string> ComPorts { get; set; }
        public BindableCollection<int> BaudRates { get; set; }
        public BindableCollection<Parity> Parities { get; set; }
        public BindableCollection<int> DataBits { get; set; }
        public BindableCollection<StopBits> StopBits { get; set; }

        public string SelectedComPort { get; set; }
        public int SelectedBaudRate { get; set; }
        public int SelectedDataBits { get; set; }
        public Parity SelectedParity { get; set; }
        public StopBits SelectedStopBits { get; set; }

        private bool _isConnected = false;
        public bool IsConnected
        {
            get { return _isConnected; }
            set
            {
                _isConnected = value;
                NotifyOfPropertyChange(() => IsConnected);
                NotifyOfPropertyChange(() => IsDisconnected);
            }
        }
        public bool IsDisconnected
        {
            get { return !_isConnected; }
            set
            {
                _isConnected = !value;
                NotifyOfPropertyChange(() => IsConnected);
                NotifyOfPropertyChange(() => IsDisconnected);
            }
        }

        public SerialConnectorViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            _eventAggregator.Subscribe(this);

            BindValues();
        }

        private void BindValues()
        {
            BaudRates = new BindableCollection<int>() { 1200, 2400, 4800, 9600, 19200, 38400, 57600, 115200 };
            SelectedBaudRate = 4800;

            DataBits = new BindableCollection<int>() { 5, 6, 7, 8, 9 };
            SelectedDataBits = 8;

            Parities = new BindableCollection<Parity>();
            foreach (Parity p in Enum.GetValues(typeof(Parity)))
            {
                Parities.Add(p);
            }
            SelectedParity = Parity.None;

            StopBits = new BindableCollection<StopBits>();
            foreach (StopBits s in Enum.GetValues(typeof(StopBits)))
            {
                StopBits.Add(s);
            }
            SelectedStopBits = System.IO.Ports.StopBits.One;


            IEnumerable<string> ports = SerialReader.GetAvailablePorts();
            ComPorts = new BindableCollection<string>();
            ComPorts.AddRange(ports);

            SelectedComPort = ports.FirstOrDefault();
        }

        public void Connect()
        {
            IsConnected = true;

            _eventAggregator.PublishOnUIThread(new SerialPortConnect
            {
                PortName = SelectedComPort,
                BaudRate = SelectedBaudRate,
                DataBits = SelectedDataBits,
                Parity = SelectedParity,
                StopBits = SelectedStopBits
            });
        }

        public void Disconnect()
        {
            IsConnected = false;

            _eventAggregator.PublishOnUIThread(new SerialPortDisconnect());
        }

        public void Handle(ConnectionError message)
        {
            IsConnected = false;

            string errorMessage = message.Exception.Message;
            if (message.Exception.InnerException != null)
            {
                errorMessage = message.Exception.InnerException.Message;
            }
            MessageBox.Show(errorMessage, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
