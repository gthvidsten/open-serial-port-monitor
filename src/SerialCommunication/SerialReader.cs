using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace Whitestone.OpenSerialPortMonitor.SerialCommunication
{
    public class SerialReader : IDisposable
    {
        // Constants
        private readonly int MAX_RECEIVE_BUFFER = 64;
        private readonly int BUFFER_TIMER_INTERVAL = 250;

        // Event handlers
        public event EventHandler<SerialDataReceivedEventArgs> SerialDataReceived;

        // Private variables
        private SerialPort _serialPort = null;
        private List<byte> _receiveBuffer = null;
        private Timer _bufferTimer = null;
        private DateTime _lastReceivedData = DateTime.Now;
        private bool _disposed = false;

        public SerialReader()
        {
            _receiveBuffer = new List<byte>(MAX_RECEIVE_BUFFER * 2);
        }

        public static IEnumerable<string> GetAvailablePorts()
        {
            List<string> comPorts = new List<string>();

            string[] availablePorts = SerialPort.GetPortNames();
            foreach (string port in availablePorts)
            {
                comPorts.Add(port);
            }

            return comPorts.OrderBy(port => port);
        }

        public void Start(string portName, int baudRate, Parity parity, int dataBits, StopBits stopBits)
        {
            if (!GetAvailablePorts().Contains(portName))
            {
                throw new Exception(string.Format("Unknown serial port: {0}", portName));
            }

            // Start the timer to empty the receive buffer (in case data smaller than MAX_RECEIVE_BUFFER is received)
            _bufferTimer = new Timer();
            _bufferTimer.Interval = BUFFER_TIMER_INTERVAL;
            _bufferTimer.Elapsed += _bufferTimer_Elapsed;

            // Instantiate new serial port communication
            _serialPort = new SerialPort(portName, baudRate, parity, dataBits, stopBits);

            // Open serial port communication
            _serialPort.Open();

            // Check that it is actually open
            if (!_serialPort.IsOpen)
            {
                throw new Exception(string.Format("Could not open serial port: {0}", portName));
            }

            _serialPort.ReadTimeout = 100; // Milliseconds
            _serialPort.DataReceived += SerialPortDataReceived;
        }

        public void Send(byte[] data)
        {
            if (_serialPort.IsOpen)
            {
                _serialPort.Write(data, 0, data.Length);
            }
        }

        public void Stop()
        {
            // Disconnect from the serial port
            if (_serialPort != null)
            {
                _serialPort.DataReceived -= SerialPortDataReceived;
                _serialPort.Close();
                _serialPort.Dispose();
                _serialPort = null;
            }

            // Stop the timer used for the buffer
            if (_bufferTimer != null)
            {
                _bufferTimer.Stop();
                _bufferTimer = null;
            }

            // Send remaining data in the receive buffer
            lock (_receiveBuffer)
            {
                SendBuffer(ref _receiveBuffer);
            }
        }

        private void SerialPortDataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
            // Read the data from COM and empty the COM buffer
            SerialPort serialPort = (SerialPort)sender;
            byte[] buffer = new byte[serialPort.BytesToRead];
            serialPort.Read(buffer, 0, buffer.Length);

            serialPort.DiscardInBuffer();

            lock (_receiveBuffer)
            {
                _lastReceivedData = DateTime.Now;

                _receiveBuffer.AddRange(buffer);

                if (_receiveBuffer.Count >= MAX_RECEIVE_BUFFER)
                {
                    SendBuffer(ref _receiveBuffer);
                }
            }
        }

        private void _bufferTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            lock (_receiveBuffer)
            {
                // Only send data if the last data received was more than BUFFER_TIMER_INTERVAL milliseconds ago
                // This is to ensure it only empties the buffer when not a lot of data has been received lately
                if ((DateTime.Now - _lastReceivedData).TotalMilliseconds > BUFFER_TIMER_INTERVAL)
                {
                    SendBuffer(ref _receiveBuffer);
                }
            }
        }

        private void SendBuffer(ref List<byte> buffer)
        {
            byte[] byteBuffer = buffer.ToArray();
            buffer.Clear();

            OnDataReceived(this, new SerialDataReceivedEventArgs() { Data = byteBuffer });
        }

        private void OnDataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            EventHandler<SerialDataReceivedEventArgs> handler = SerialDataReceived;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                Stop();
            }

            _disposed = true;
        }
    }
}
