using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Whitestone.OpenSerialPortMonitor.Main
{
    public class SecondViewModel : PropertyChangedBase
    {
        public void SecondButton()
        {
            MessageBox.Show("Second box");
        }
    }
}
