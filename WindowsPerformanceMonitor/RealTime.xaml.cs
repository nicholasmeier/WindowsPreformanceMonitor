using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
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
using LiveCharts;
using LiveCharts.Configurations;
using LiveCharts.Wpf;
using WindowsPerformanceMonitor.Models;

namespace WindowsPerformanceMonitor
{
    /// <summary>
    /// Interaction logic for RealTime.xaml
    /// </summary>
    public partial class RealTime : UserControl
    {
        private MainWindow mainWindow = null; // Reference to the MainWindow

        public RealTime()
        {
            InitializeComponent();
            InitializeComboBox();
        }

        #region Initialization

        private void OnControlLoaded(object sender, RoutedEventArgs e)
        {
            mainWindow = Window.GetWindow(this) as MainWindow;
        }

        private void InitializeComboBox()
        {
            comboBox1.Items.Add("System");

            for (var i = 0; i < 5; i++)
            {
                comboBox1.Items.Add($"Process {i}");
            }

            comboBox1.SelectedItem = "System";
        }

        #endregion

        #region Events

        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // TODO
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // TODO
        }

        private void ProcessList_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            // TODO
        }

        #endregion
    }
}
