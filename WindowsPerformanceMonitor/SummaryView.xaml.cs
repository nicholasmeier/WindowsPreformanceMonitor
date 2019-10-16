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
    /// Interaction logic for SummaryView.xaml
    /// </summary>
    public partial class SummaryView : Window
    {

        public Window mainWindowRef = null;

        public SummaryView(Window window)
        {
            InitializeComponent();
            mainWindowRef = window;
        }

        private void DetailedView_Click(object sender, System.EventArgs e)
        {
            this.Close();
            mainWindowRef.Show();
        }
    }
}