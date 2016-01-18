using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Whitestone.OpenSerialPortMonitor.Main
{
    public class ShellViewModel : Screen
    {
        string name;
        SecondViewModel _secondView;

        public string Name
        {
            get { return name; }
            set
            {
                name = value;
                NotifyOfPropertyChange(() => Name);
                NotifyOfPropertyChange(() => CanSayHello);
            }
        }

        public bool CanSayHello
        {
            get { return !string.IsNullOrWhiteSpace(Name); }
        }

        public void SayHello()
        {
            MessageBox.Show(string.Format("Hello {0}!", Name)); //Don't do this in real life :)
        }

        protected override void OnInitialize()
        {
            SecondView = new SecondViewModel();
            base.OnInitialize();
        }

        public SecondViewModel SecondView
        {
            get
            {
                return _secondView;
            }
            set
            {
                _secondView = value;
                NotifyOfPropertyChange(() => SecondView);
            }
        }
    }
}
