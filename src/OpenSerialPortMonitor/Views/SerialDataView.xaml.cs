using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Whitestone.OpenSerialPortMonitor.Main.Messages;

namespace Whitestone.OpenSerialPortMonitor.Main.Views
{
    /// <summary>
    /// Interaction logic for SerialDataView.xaml
    /// </summary>
    public partial class SerialDataView : UserControl
    {
        public SerialDataView()
        {
            InitializeComponent();

            DataViewParsed.TextChanged += TextChanged;
            DataViewRaw.TextChanged += TextChanged;
        }

        void TextChanged(object sender, TextChangedEventArgs e)
        {
            ScrollViewer scrollviewer;
            TextBox textbox = (TextBox)sender;
            if (textbox.Parent is Grid)
            {
                Grid grid = (Grid)textbox.Parent;
                scrollviewer = (ScrollViewer)grid.Parent;
            }
            else
            {
                scrollviewer = (ScrollViewer)textbox.Parent;
            }

            bool isAutoscroll = true;
            if (scrollviewer.Tag != null &&
                scrollviewer.Tag is bool)
            {
                isAutoscroll = (bool)scrollviewer.Tag;
            }

            if (isAutoscroll)
            {
                scrollviewer.ScrollToEnd();
            }   
        }
    }
}
