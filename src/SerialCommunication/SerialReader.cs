using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Whitestone.OpenSerialPortMonitor.SerialCommunication
{
    public class SerialReader
    {
        SerialPort m_SerialPort = null;
        string m_DataBuffer = string.Empty;


        public SerialReader()
        {
        }

        void SerialPortDataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            // Read the data from COM and empty the COM buffer
            string comBuffer = m_SerialPort.ReadExisting();
            m_SerialPort.DiscardInBuffer(); // Clear buffer
        }


        public void Start()
        {
            // Instantiate new serial port communication
            m_SerialPort = new SerialPort("COM1", 9600, Parity.None, 8, StopBits.One);

            // Open serial port communication
            m_SerialPort.Open();

            if (m_SerialPort.IsOpen)
            {
                m_SerialPort.ReadTimeout = 100; // Milliseconds
                m_SerialPort.DataReceived += SerialPortDataReceived;
            }
            else
            {
                throw new Exception("Could not open serial port COM1");
            }
        }

        public void Stop()
        {
            if (m_SerialPort != null)
            {
                m_SerialPort.Close();
                m_SerialPort.Dispose();
                m_SerialPort = null;
            }
        }
    }
}
