using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using WindowsPerformanceMonitor.Graphs;
using WindowsPerformanceMonitor.Models;

namespace WindowsPerformanceMonitor
{
    /// <summary>
    /// Interaction logic for SummaryView.xaml
    /// </summary>
    public partial class SummaryView : Window
    {

        public Window mainWindowRef = null;
        private double _totalCpu;
        private double _totalGpu;
        private double _totalMemory;

        public SummaryView(Window window)
        {
            InitializeComponent();
            HardwareObserver observer = new HardwareObserver(UpdateValues);
            Globals.provider.Subscribe(observer);
            mainWindowRef = window;
            InitText();
            InitGraphs();
        }

        public void InitText()
        {
            t1.Text = $"CPU: 0%";
            t2.Text = $"GPU: 0%";
            t3.Text = $"Memory: 0%";
        }

        public void InitGraphs()
        {
            liveGraph1.connect();
            liveGraph2.connect();
            liveGraph3.connect();
            liveGraph1.ProcessPid = 0;
            liveGraph2.ProcessPid = 0;
            liveGraph3.ProcessPid = 0;
            SetVisibilityToFalseExecept(liveGraph1, (int)Series.Cpu);
            SetVisibilityToFalseExecept(liveGraph2, (int)Series.Gpu);
            SetVisibilityToFalseExecept(liveGraph3, (int)Series.Memory);
            liveGraph1.SeriesVisibility[(int)Series.Cpu] = true;
            liveGraph2.SeriesVisibility[(int)Series.Gpu] = true;
            liveGraph3.SeriesVisibility[(int)Series.Memory] = true;
        }

        private void SetVisibilityToFalseExecept(LiveLineGraph graph, int statIndex)
        {
            for (int k = 0; k < 7; k++)
            {
                if (statIndex == k) continue;
                graph.SeriesVisibility[k] = false;
            }
        }

        public void UpdateValues(ComputerObj comp)
        {
            this.Dispatcher.Invoke(() =>
            {
                t1.Text = $"CPU: {comp.TotalCpu}%";
                t2.Text = $"GPU: {comp.TotalGpu}%";
                t3.Text = $"Memory: {comp.TotalMemory}%";
                TotalMemory = comp.TotalMemory;
            });
        }

        private void DetailedView_Click(object sender, System.EventArgs e)
        {
            this.Close();
            mainWindowRef.Show();
        }

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public double TotalCpu
        {
            get { return _totalCpu; }
            set
            {
                _totalCpu = value;
                OnPropertyChanged(nameof(TotalCpu));
            }
        }

        public double TotalGpu
        {
            get { return _totalGpu; }
            set
            {
                _totalGpu = value;
                OnPropertyChanged(nameof(TotalGpu));
            }
        }

        public double TotalMemory
        {
            get { return _totalMemory; }
            set
            {
                _totalMemory = value;
                OnPropertyChanged(nameof(TotalMemory));
            }
        }

        #endregion

        private void SummaryView_Closing(object sender, CancelEventArgs e)
        {
            mainWindowRef.Show();
        }
    }
}