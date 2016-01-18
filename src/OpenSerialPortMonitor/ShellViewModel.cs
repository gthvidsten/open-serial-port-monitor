using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Whitestone.OpenSerialPortMonitor.Main
{
    [Export(typeof(IShell))]
    public class ShellViewModel : Screen, IShell, IHandle<TestMessage>
    {
        string name;
        SecondViewModel _secondView;

        private readonly IEventAggregator _eventAggregator;

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

        [ImportingConstructor]
        public ShellViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            _eventAggregator.Subscribe(this);
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
            SecondView = new SecondViewModel(_eventAggregator);
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

        public void Handle(TestMessage message)
        {
            MessageBox.Show("Value: " + message.FooBar);
            Name = message.FooBar;
        }
    }
}
