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
    public partial class RealTime : UserControl, INotifyPropertyChanged
    {
        private MainWindow mainWindow = null; // Reference to the MainWindow

        public RealTime()
        {
            InitializeComponent();
            InitializeComboBox();
            InitializeGraph();
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

        private void InitializeGraph()
        {

            var mapper = Mappers.Xy<MeasureModel>()
                .X(model => model.DateTime.Ticks)   //use DateTime.Ticks as X
                .Y(model => model.Value);           //use the value property as Y

            Charting.For<MeasureModel>(mapper);                     // Save the mapper globally.
            ChartValues = new ChartValues<MeasureModel>();          // Store chart values.
            DateTimeFormatter = value => new DateTime((long)value).ToString("mm:ss"); // X-Axis labels

            AxisStep = TimeSpan.FromSeconds(1).Ticks;             //AxisStep forces the distance between each separator in the X axis
            AxisUnit = TimeSpan.TicksPerSecond;             //AxisUnit forces lets the axis know that we are plotting seconds
                                                            //this is not always necessary, but it can prevent wrong labeling
            SetAxisLimits(DateTime.Now);
            IsReading = true;
            DataContext = this;
            Task.Factory.StartNew(Read);
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

        #region Graph

        public ChartValues<MeasureModel> ChartValues { get; set; }
        public Func<double, string> DateTimeFormatter { get; set; }
        public double AxisStep { get; set; }
        public double AxisUnit { get; set; }

        private double _axisMax;
        private double _axisMin;
        private double _trend;

        public double AxisMax
        {
            get { return _axisMax; }
            set
            {
                _axisMax = value;
                OnPropertyChanged("AxisMax");
            }
        }
        public double AxisMin
        {
            get { return _axisMin; }
            set
            {
                _axisMin = value;
                OnPropertyChanged("AxisMin");
            }
        }

        public bool IsReading { get; set; }

        private void Read()
        {
            var r = new Random();

            while (IsReading)
            {
                Thread.Sleep(150);
                var now = DateTime.Now;

                _trend += r.Next(-8, 10);

                ChartValues.Add(new MeasureModel
                {
                    DateTime = now,
                    Value = _trend
                });

                SetAxisLimits(now);

                //lets only use the last 150 values
                if (ChartValues.Count > 150) ChartValues.RemoveAt(0);
            }
        }

        private void SetAxisLimits(DateTime now)
        {
            AxisMax = now.Ticks + TimeSpan.FromSeconds(1).Ticks; // lets force the axis to be 1 second ahead
            AxisMin = now.Ticks - TimeSpan.FromSeconds(8).Ticks; // and 8 seconds behind
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName = null)
        {
            if (PropertyChanged != null)
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
