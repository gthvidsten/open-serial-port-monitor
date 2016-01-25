using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Whitestone.OpenSerialPortMonitor.Main.Messages
{
    [DataContract]
    public class SerialPortSend
    {
        [DataMember]
        public byte[] Data { get; set; }
    }
}
