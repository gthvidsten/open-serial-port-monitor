using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Timers;

namespace Whitestone.OpenSerialPortMonitor.SerialCommunication
{
    public class SerialReader : IDisposable
    {
        // Constants
        private readonly int MAX_RECEIVE_BUFFER = 128;
        private readonly int BUFFER_TIMER_INTERVAL = 100;

        // Event handlers
        public event EventHandler<SerialDataReceivedEventArgs> SerialDataReceived;

        // Private variables
        private SerialPort _serialPort = null;
        private List<byte> _receiveBuffer = null;
        private Timer _bufferTimer = null;
        private DateTime _lastReceivedData = DateTime.Now;
        private bool _disposed = false;
        private System.Threading.Thread _readThread = null;
        private bool _readThreadRunning = false;

        public SerialReader()
        {
            _receiveBuffer = new List<byte>(MAX_RECEIVE_BUFFER * 3);
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
            _bufferTimer.Start();

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

            _readThread = new System.Threading.Thread(ReadThread);
            _readThreadRunning = true;
            _readThread.Start();
        }

        private async void ReadThread()
        {
            while (_readThreadRunning)
            {
                try
                {
                    // This is the proper way to read data, according to http://www.sparxeng.com/blog/software/must-use-net-system-io-ports-serialport
                    // Though he uses BeginRead/EndRead, ReadAsync is the preferred way in .NET 4.5
                    byte[] buffer = new byte[MAX_RECEIVE_BUFFER * 3];
                    int bytesRead = await _serialPort.BaseStream.ReadAsync(buffer, 0, buffer.Length);

                    byte[] received = new byte[bytesRead];
                    Buffer.BlockCopy(buffer, 0, received, 0, bytesRead);
                    lock (_receiveBuffer)
                    {
                        _receiveBuffer.AddRange(received);
                    }
                }
                catch
                {

                }
            }
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
            _readThreadRunning = false;

            // Disconnect from the serial port
            if (_serialPort != null)
            {
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

        private void _bufferTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            lock (_receiveBuffer)
            {
                // Only send data if the last data received was more than BUFFER_TIMER_INTERVAL milliseconds ago
                // This is to ensure it only empties the buffer when not a lot of data has been received lately
                if ((DateTime.Now - _lastReceivedData).TotalMilliseconds > BUFFER_TIMER_INTERVAL && _receiveBuffer.Count > 0)
                {
                    SendBuffer(ref _receiveBuffer);
                }
            }
        }

        private void SendBuffer(ref List<byte> buffer)
        {
            byte[] byteBuffer = buffer.ToArray();
            buffer.Clear();

            EventHandler<SerialDataReceivedEventArgs> handler = SerialDataReceived;
            if (handler != null)
            {
                handler(this, new SerialDataReceivedEventArgs() { Data = byteBuffer });
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
