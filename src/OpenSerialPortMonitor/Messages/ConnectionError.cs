using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Whitestone.OpenSerialPortMonitor.Main.Messages
{
    public class ConnectionError
    {
        public Exception Exception { get; set; }
    }
}
