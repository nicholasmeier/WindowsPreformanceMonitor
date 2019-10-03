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
        public SummaryView()
        {
            InitializeComponent();
        }
        private void DetailedView_Click(object sender, System.EventArgs e)
        {
            MainWindow wind = new MainWindow();
            wind.Show();
            this.Close();
        }
    }
}
