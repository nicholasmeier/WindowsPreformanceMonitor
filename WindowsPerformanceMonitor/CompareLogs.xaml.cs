using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace WindowsPerformanceMonitor
{
    /// <summary>
    /// Interaction logic for CompareLogs.xaml
    /// </summary>
    public partial class CompareLogs : Window
    {
        public Window mainWindowRef = null;
        public CompareLogs(Window window)
        {
            InitializeComponent();
            mainWindowRef = window;
            this.Closed += new EventHandler(Window_Closed);
        }

        void Window_Closed(object sender, EventArgs e)
        {
            mainWindowRef.Show();
        }

    }
}
