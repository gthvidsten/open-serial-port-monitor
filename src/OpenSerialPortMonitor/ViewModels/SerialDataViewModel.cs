using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using Whitestone.OpenSerialPortMonitor.Main.Messages;
using Whitestone.OpenSerialPortMonitor.SerialCommunication;

namespace Whitestone.OpenSerialPortMonitor.Main.ViewModels
{
    public class SerialDataViewModel : PropertyChangedBase, IHandle<SerialPortConnect>, IHandle<SerialPortDisconnect>, IHandle<Autoscroll>, IHandle<SendTestData>
    {
        private readonly IEventAggregator _eventAggregator;
        private SerialReader _serialReader;
        private Timer _cacheTimer;

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
                _eventAggregator.PublishOnBackgroundThread(new ConnectionError() { Exception = ex });
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

        private void SendTestData()
        {
            byte[] data = System.Text.Encoding.ASCII.GetBytes(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ffff") + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ffff") + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ffff") + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ffff") + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ffff") + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ffff") + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ffff") + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ffff") + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ffff") + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ffff") + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ffff") + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ffff") + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ffff") + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ffff") + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ffff") + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ffff") + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ffff") + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ffff") + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ffff") + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ffff") + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ffff") + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ffff") + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ffff") + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ffff") + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ffff") + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ffff") + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ffff") + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ffff") + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ffff") + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ffff") + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ffff") + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ffff") + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ffff") + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ffff") + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ffff") + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ffff") + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ffff") + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ffff") + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ffff") + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ffff") + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ffff") + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ffff") + "\r\n");
            //byte[] data = new byte[] { 0x5d, 0x43, 0x31, 0x39, 0x39, 0x30, 0x30, 0x30, 0x30, 0x30, 0x30, 0x30, 0x30, 0x33, 0x30, 0x30, 0x36, 0x34, 0x39, 0x39, 0x33, 0x35, 0x39, 0x37, 0x39, 0x36, 0x0d, 0x0a };
            SerialDataReceivedEventArgs args = new SerialDataReceivedEventArgs()
            {
                Data = data
            };

            SerialDataReceived(null, args);
        }

        void SerialDataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            _dataViewParsedBuilder.Append(System.Text.Encoding.ASCII.GetString(e.Data));

            for (int i = 0; i < e.Data.Length; i++)
            {
                char character = (char)e.Data[i];
                if (e.Data[i] <= 31 ||
                    e.Data[i] == 127)
                {
                    character = '.';
                }

                _dataViewHexBuilder.Append(string.Format("{0:x2} ", e.Data[i]));
                _dataViewRawBuilder.Append(character);

                if (i > 0 && i % 16 == 15)
                {
                    _dataViewHexBuilder.Append("\r\n");
                    _dataViewRawBuilder.Append("\r\n");
                }
            }
        }

        public void Handle(SendTestData message)
        {
            SendTestData();
        }
    }
}
