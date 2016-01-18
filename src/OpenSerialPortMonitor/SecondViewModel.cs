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
        private readonly IEventAggregator _eventAggregator;

        public SecondViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
        }

        public void SecondButton()
        {
            _eventAggregator.PublishOnBackgroundThread(new TestMessage
            {
                FooBar = "From SecondViewModel"
            });
        }
    }
}
