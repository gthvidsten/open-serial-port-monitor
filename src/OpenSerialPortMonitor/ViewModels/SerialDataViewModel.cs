using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using Whitestone.OpenSerialPortMonitor.Main.Messages;
using Whitestone.OpenSerialPortMonitor.SerialCommunication;

namespace Whitestone.OpenSerialPortMonitor.Main.ViewModels
{
    public class SerialDataViewModel : PropertyChangedBase, IHandle<SerialPortConnect>, IHandle<SerialPortDisconnect>, IHandle<Autoscroll>, IHandle<SerialPortSend>
    {
        private readonly IEventAggregator _eventAggregator;
        private SerialReader _serialReader;
        private Timer _cacheTimer;
        private int _rawDataCounter = 0;

        public SerialDataViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            _eventAggregator.Subscribe(this);

            _serialReader = new SerialReader();
        }

        private bool _isAutoscroll = true;
        public bool IsAutoscroll
        {
            get { return _isAutoscroll; }
            set
            {
                _isAutoscroll = value;
                NotifyOfPropertyChange(() => IsAutoscroll);
            }
        }

        private StringBuilder _dataViewParsedBuilder = new StringBuilder();
        private string _dataViewParsed = string.Empty;
        public string DataViewParsed
        {
            get { return _dataViewParsed; }
            set
            {
                _dataViewParsed = value;
                NotifyOfPropertyChange(() => DataViewParsed);
            }
        }

        private StringBuilder _dataViewRawBuilder = new StringBuilder();
        private string _dataViewRaw = string.Empty;
        public string DataViewRaw
        {
            get { return _dataViewRaw; }
            set
            {
                _dataViewRaw = value;
                NotifyOfPropertyChange(() => DataViewRaw);
            }
        }

        private StringBuilder _dataViewHexBuilder = new StringBuilder();
        private string _dataViewHex = string.Empty;
        public string DataViewHex
        {
            get { return _dataViewHex; }
            set
            {
                _dataViewHex = value;
                NotifyOfPropertyChange(() => DataViewHex);
            }
        }

        public void Handle(SerialPortConnect message)
        {
            try
            {
                _cacheTimer = new Timer();
                _cacheTimer.Interval = 500;
                _cacheTimer.Elapsed += _cacheTimer_Elapsed;
                _cacheTimer.Start();

                _serialReader.Start(message.PortName, message.BaudRate, message.Parity, message.DataBits, message.StopBits);
                _serialReader.SerialDataReceived += SerialDataReceived;
            }
            catch (Exception ex)
            {
                _eventAggregator.PublishOnUIThread(new ConnectionError() { Exception = ex });
            }
        }

        public void Handle(SerialPortDisconnect message)
        {
            _cacheTimer.Stop();

            _serialReader.Stop();
        }

        void _cacheTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            string dataParsed = _dataViewParsedBuilder.ToString();
            _dataViewParsedBuilder = new StringBuilder();

            string dataHex = _dataViewHexBuilder.ToString();
            _dataViewHexBuilder = new StringBuilder();

            string dataRaw = _dataViewRawBuilder.ToString();
            _dataViewRawBuilder = new StringBuilder();

            DataViewParsed += dataParsed;
            DataViewHex += dataHex;
            DataViewRaw += dataRaw;
        }

        public void Handle(Autoscroll message)
        {
            IsAutoscroll = message.IsTurnedOn;
        }

        void SerialDataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            _dataViewParsedBuilder.Append(System.Text.Encoding.ASCII.GetString(e.Data));

            foreach (byte data in e.Data)
            {
                _rawDataCounter = _rawDataCounter + 1;

                char character = (char)data;
                if (data <= 31 ||
                    data == 127)
                {
                    character = '.';
                }

                _dataViewHexBuilder.Append(string.Format("{0:x2} ", data));
                _dataViewRawBuilder.Append(character);

                if (_rawDataCounter > 0 && _rawDataCounter % 16 == 15)
                {
                    _dataViewHexBuilder.Append("\r\n");
                    _dataViewRawBuilder.Append("\r\n");
                    _rawDataCounter = 0;
                }
            }
        }

        public void Handle(SerialPortSend message)
        {
            _serialReader.Send(message.Data);
        }
    }
}
