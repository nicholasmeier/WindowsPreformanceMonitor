using Microsoft.Win32;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Forms;

namespace WindowsPerformanceMonitor
{
    /// <summary>
    /// Interaction logic for Logging.xaml
    /// </summary>
    /// 

    public partial class Logging : System.Windows.Controls.UserControl
    {
        Log temp;
        private MainWindow mainWindow = null;
        private String path = "";
        public Logging()
        {
            InitializeComponent();
        }

        private void OnControlLoaded(object sender, RoutedEventArgs e)
        {
            mainWindow = Window.GetWindow(this) as MainWindow;
        }

        private void InitializeComboBox()
        {
            // TODO
        }

        private void StartLog_Click(object sender, RoutedEventArgs e)
        {
            if (path.Equals(""))
                temp = new Log();
            else
                temp = new Log(path);
                temp.StartLog();
            // TODO
        }

        private void StopLog_Click(object sender, RoutedEventArgs e)
        {
            temp.WriteIt();
            // TODO
        }

        private void DeleteLog_Click(object sender, RoutedEventArgs e)
        {
            // TODO
        }

        private void ChooseLocation_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog Fbd = new FolderBrowserDialog();
            if(Fbd.ShowDialog() == DialogResult.OK)
            {
                path = Fbd.SelectedPath;
            }

        }

        private void ComboBox_Graph_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void logList_Click(object sender, RoutedEventArgs e)
        {
            var item = (sender as System.Windows.Controls.ListView).SelectedItem;
            if (item != null)
            {
                // TODO
            }
        }
    }
}
