using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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

namespace WindowsPerformanceMonitor
{
    /// <summary>

    /// </summary>
    public partial class OverlaySettings : UserControl
    {
        private MainWindow mainWindow = null; // Reference to the MainWindow
        public OverlaySettings()
        {
            this.DataContext = this;
            InitializeComponent();
        }
        private void OnControlLoaded(object sender, RoutedEventArgs e)
        {
            mainWindow = Window.GetWindow(this) as MainWindow;
        }

        private void Sys_stat_checked(object sender, RoutedEventArgs e)
        {

        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox c = (CheckBox)sender;
            switch (c.Name)
            {
                case "CPU":
                    if (c.IsChecked == true)
                    {
                        
                    }
                    else
                    {

                    }
                    break;
                case "GPU":
                    break;
                case "MEM":
                    break;
                case "DIS":
                    break;
                case "NET":
                    break;
                case "TempC":
                    break;
                case "TempG":
                    break;
            }
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
        }

        private void StartOverlay_Click(object sender, RoutedEventArgs e)
        {

        }
        private void Save_Button_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
