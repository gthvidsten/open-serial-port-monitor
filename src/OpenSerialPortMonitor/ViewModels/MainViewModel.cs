using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Whitestone.OpenSerialPortMonitor.Main.Framework;
using Whitestone.OpenSerialPortMonitor.Main.Messages;

namespace Whitestone.OpenSerialPortMonitor.Main.ViewModels
{
    [Export(typeof(IShell))]
    public class MainViewModel : Screen, IShell, IHandle<SerialPortConnect>
    {
        private readonly IEventAggregator _eventAggregator;

        private SerialConnectorViewModel _serialConnectorView;
        public SerialConnectorViewModel SerialConnectorView
        {
            get
            {
                return _serialConnectorView;
            }
            set
            {
                _serialConnectorView = value;
                NotifyOfPropertyChange(() => SerialConnectorView);
            }
        }

        [ImportingConstructor]
        public MainViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            _eventAggregator.Subscribe(this);
        }

        protected override void OnInitialize()
        {
            SerialConnectorView = new SerialConnectorViewModel(_eventAggregator);
            base.OnInitialize();
        }

        public void Handle(SerialPortConnect message)
        {
            MessageBox.Show("Value: " + message.PortName + ", " + message.BaudRate + ", " + message.DataBits + ", " + message.Parity + ", " + message.StopBits);
        }
    }
}
