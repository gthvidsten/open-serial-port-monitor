using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using Whitestone.OpenSerialPortMonitor.SerialCommunication;
using Whitestone.OpenSerialPortMonitor.Main.Messages;
using System.IO.Ports;

namespace Whitestone.OpenSerialPortMonitor.Main.ViewModels
{
    public class SerialConnectorViewModel : PropertyChangedBase
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

        private string _buttonText = "Connect";
        public string ButtonText
        {
            get { return _buttonText; }
            set
            {
                _buttonText = value;
                NotifyOfPropertyChange(() => ButtonText);
            }
        }

        private bool _canEdit = true;
        public bool CanEdit
        {
            get { return _canEdit; }
            set
            {
                _canEdit = value;
                NotifyOfPropertyChange(() => CanEdit);
            }
        }

        public SerialConnectorViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            
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
            if (CanEdit)
            {
                ButtonText = "Disconnect";
                CanEdit = false;

                _eventAggregator.PublishOnBackgroundThread(new SerialPortConnect
                {
                    PortName = SelectedComPort,
                    BaudRate = SelectedBaudRate,
                    DataBits = SelectedDataBits,
                    Parity = SelectedParity,
                    StopBits = SelectedStopBits
                });

                //int[] a = new int[2];
                //a[5] = 0;
            }
            else
            {
                ButtonText = "Connect";
                CanEdit = true;

                _eventAggregator.PublishOnBackgroundThread(new SerialPortDisconnect());
            }
        }
    }
}
