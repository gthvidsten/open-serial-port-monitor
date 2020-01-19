﻿using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Whitestone.OpenSerialPortMonitor.Main.Messages;

namespace Whitestone.OpenSerialPortMonitor.Main.ViewModels
{
    public class SerialDataSendViewModel : PropertyChangedBase, IHandle<SerialPortConnect>, IHandle<SerialPortDisconnect>, IHandle<ConnectionError>
    {
        private readonly IEventAggregator _eventAggregator;

        private string _dataToSend = string.Empty;
        public string DataToSend
        {
            get { return _dataToSend; }
            set
            {
                _dataToSend = value;
                NotifyOfPropertyChange(() => DataToSend);
                NotifyOfPropertyChange(() => IsValidData);
            }
        }

        private bool _isText = true;
        public bool IsText
        {
            get { return _isText; }
            set
            {
                _isText = value;
                NotifyOfPropertyChange(() => IsText);
                NotifyOfPropertyChange(() => IsValidData);
            }
        }

        private bool _isHex = false;
        public bool IsHex
        {
            get { return _isHex; }
            set
            {
                _isHex = value;
                if (value){
                    LineEnd = LENONE;
                }
                NotifyOfPropertyChange(() => IsHex);
                NotifyOfPropertyChange(() => IsValidData);
            }
        }

        public bool IsValidData
        {
            get
            {
                if (!IsConnected)
                {
                    return false;
                }

                if (DataToSend.Length <= 0)
                {
                    return false;
                }

                if (IsText) // Left this here for readability even though it is not needed. Otherwise someone might think it was missing or forgotten about.
                {
                }

                if (IsHex)
                {
                    try
                    {
                        string[] characters = DataToSend.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                        foreach (string hex in characters)
                        {
                            byte value = Convert.ToByte(hex, 16);
                        }
                    }
                    catch
                    {
                        return false;
                    }
                }

                return true;
            }
        }

        private bool _isConnected = false;
        public bool IsConnected {
            get { return _isConnected; }
            set {
                _isConnected = value;
                NotifyOfPropertyChange(() => IsConnected);
                NotifyOfPropertyChange(() => IsValidData);
            }
        }
        private bool _clearAfterSend = false;
        public bool ClearAfterSend {
            get { return _clearAfterSend; }
            set {
                _clearAfterSend = value;
                NotifyOfPropertyChange(() => ClearAfterSend);
            }
        }
        private string _lineEnd;
        public string LineEnd {
            get { return _lineEnd; }
            set {
                Console.WriteLine(value.ToString());
                _lineEnd = value;
                NotifyOfPropertyChange(() => LineEnd);
            }
        }
        private const string LENONE = "No Line End";
        private const string LECR = "add CR";
        private const string LELF = "add LF";
        private const string LECRLD = "add CR + LF";
        public string[] LineEndsValues { get; } = {LENONE, LECR, LELF, LECRLD};
        public SerialDataSendViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            _eventAggregator.Subscribe(this);
            LineEnd = LENONE;
        }
        public void DoSend()
        {
            List<byte> data = new List<byte>();

            if (IsHex)
            {
                string[] characters = DataToSend.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                foreach (string hex in characters)
                {
                    byte value = Convert.ToByte(hex, 16);
                    data.Add(value);
                }
            }

            if (IsText)
            {
                string parsed = DataToSend.Replace("\\\\r", "\r").Replace("\\\\n", "\n");
                switch (LineEnd){
                    case LECR:
                        parsed += "\r";
                        break;
                    case LELF:
                        parsed += "\n";
                        break;
                    case LECRLD:
                        parsed += "\r\n";
                        break;
                }
                data.AddRange(System.Text.Encoding.ASCII.GetBytes(parsed));
                if (ClearAfterSend){
                    DataToSend = String.Empty;
                }
            }

            _eventAggregator.PublishOnUIThread(new Messages.SerialPortSend() { Data = data.ToArray() });
        }

        public void Handle(SerialPortConnect message)
        {
            IsConnected = true;
        }

        public void Handle(SerialPortDisconnect message)
        {
            IsConnected = false;
        }

        public void Handle(ConnectionError message)
        {
            IsConnected = false;
        }
    }
}
