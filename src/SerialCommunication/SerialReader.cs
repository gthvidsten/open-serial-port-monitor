using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Whitestone.OpenSerialPortMonitor.SerialCommunication
{
    public class SerialReader : IDisposable
    {
        // Event handlers
        public event EventHandler<SerialDataReceivedEventArgs> SerialDataReceived;

        // Private variables
        SerialPort _serialPort = null;
        string m_DataBuffer = string.Empty;


        public string[] GetAvailablePorts()
        {
            return SerialPort.GetPortNames();
        }

        public void Start(string portName, int baudRate, Parity parity, int dataBits, StopBits stopBits)
        {
            if (!GetAvailablePorts().Contains(portName))
            {
                throw new Exception(string.Format("Unknown serial port: {0}", portName));
            }

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

        public void Stop()
        {
            if (_serialPort != null)
            {
                _serialPort.DataReceived -= SerialPortDataReceived;
                _serialPort.Close();
                _serialPort.Dispose();
                _serialPort = null;
            }
        }

        private void SerialPortDataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
            // Read the data from COM and empty the COM buffer
            //string comBuffer = _serialPort.ReadExisting();
            //_serialPort.DiscardInBuffer(); // Clear buffer

            SerialPort serialPort = (SerialPort)sender;
            byte[] buffer = new byte[serialPort.BytesToRead];
            serialPort.Read(buffer, 0, buffer.Length);

            OnDataReceived(this, new SerialDataReceivedEventArgs() { Data = buffer });
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
            Stop();
        }
    }
}
