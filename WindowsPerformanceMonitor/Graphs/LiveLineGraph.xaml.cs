using System;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using LiveCharts;
using LiveCharts.Configurations;
using WindowsPerformanceMonitor.Models;

namespace WindowsPerformanceMonitor.Graphs
{
    public partial class LiveLineGraph : UserControl, INotifyPropertyChanged
    {
        private double _axisMax;
        private double _axisMin;
        private double _trend;
        public ChartValues<MeasureModel> ChartValues { get; set; }
        public Func<double, string> DateTimeFormatter { get; set; }
        public double AxisStep { get; set; }
        public double AxisUnit { get; set; }
        public int ProcessPid { get; set; }

        public LiveLineGraph()
        {
            InitializeComponent();

            HardwareObserver observer = new HardwareObserver(UpdateValues);
            Globals.provider.Subscribe(observer);
            this.DataContext = this;

            ProcessPid = 0;

            var mapper = Mappers.Xy<MeasureModel>()
                .X(model => model.DateTime.Ticks)   //use DateTime.Ticks as X
                .Y(model => model.Value);           //use the value property as Y

            // Save the mapper globally.
            Charting.For<MeasureModel>(mapper);

            // The values property will store our values array
            ChartValues = new ChartValues<MeasureModel>();

            // Set how to display the X Labels
            DateTimeFormatter = value => new DateTime((long)value).ToString("mm:ss");

            // AxisStep forces the distance between each separator in the X axis
            AxisStep = TimeSpan.FromSeconds(1).Ticks;

            // AxisUnit forces lets the axis know that we are plotting seconds
            AxisUnit = TimeSpan.TicksPerSecond;

            SetAxisLimits(DateTime.Now);
        }

        public void UpdateValues(ComputerObj comp)
        {
            Read(comp);
        }

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

        private void Read(ComputerObj comp)
        {
            var now = DateTime.Now;


            if (ProcessPid != 0)
            {
                _trend = comp.ProcessList.First(p => p.Pid == ProcessPid).Cpu;
            }
            else
            {
                _trend = comp.TotalCpu;

            }

            ChartValues.Add(new MeasureModel
            {
                DateTime = now,
                Value = _trend
            });

            SetAxisLimits(now);

            //lets only use the last 150 values
            if (ChartValues.Count > 150) ChartValues.RemoveAt(0);
       
        }

        private void SetAxisLimits(DateTime now)
        {
            AxisMax = now.Ticks + TimeSpan.FromSeconds(1).Ticks; // lets force the axis to be 1 second ahead
            AxisMin = now.Ticks - TimeSpan.FromSeconds(8).Ticks; // and 8 seconds behind
        }

        #region INotifyPropertyChanged implementation

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName = null)
        {
            if (PropertyChanged != null)
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}