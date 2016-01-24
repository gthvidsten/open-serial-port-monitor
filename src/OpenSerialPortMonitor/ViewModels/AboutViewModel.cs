using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Whitestone.OpenSerialPortMonitor.Main.ViewModels
{
    public class AboutViewModel : Screen
    {
        public string Version
        {
            get
            {
                return Assembly.GetExecutingAssembly().GetName().Version.ToString();
            }
        }

        public void CloseWindow()
        {
            TryClose();
        }
    }
}
