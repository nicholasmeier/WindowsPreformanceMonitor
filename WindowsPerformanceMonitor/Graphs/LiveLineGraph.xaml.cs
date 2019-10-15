using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using LiveCharts;
using LiveCharts.Configurations;
using LiveCharts.Wpf;
using WindowsPerformanceMonitor.Backend;
using WindowsPerformanceMonitor.Models;

namespace WindowsPerformanceMonitor.Graphs
{
    public partial class LiveLineGraph : UserControl, INotifyPropertyChanged
    {
        private double _axisMax;
        private double _axisMin;
        private double _trend;
        private string _chartColor;
        private bool _cpuSeriesVisibility;
        private bool _gpuSeriesVisibility;
        private bool _memorySeriesVisibility;
        private bool _diskSeriesVisibility;
        private bool _networkSeriesVisibility;
        private bool _cpuTempSeriesVisibility;
        private bool _gpuTempSeriesVisibility;

        private ObservableCollection<bool> _seriesVisibility;

        public SeriesCollection SeriesCollection { get; set; }
        public AxesCollection AxisYCollection { get; set; }
        public Func<double, string> DateTimeFormatter { get; set; }
        public double AxisStep { get; set; }
        public double AxisUnit { get; set; }
        public int ProcessPid { get; set; }
        public LineSeries CpuSeries { get; set; }
        public LineSeries GpuSeries { get; set; }
        public LineSeries MemorySeries { get; set; }
        public LineSeries DiskSeries { get; set; }
        public LineSeries NetworkSeries { get; set; }
        public LineSeries CpuTempSeries { get; set; }
        public LineSeries GpuTempSeries { get; set; }


        public LiveLineGraph()
        {
            InitLineSeries();
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

            // Set how to display the X Labels
            DateTimeFormatter = value => new DateTime((long)value).ToString("mm:ss");

            // AxisStep forces the distance between each separator in the X axis
            AxisStep = TimeSpan.FromSeconds(10).Ticks;

            // AxisUnit forces lets the axis know that we are plotting seconds
            AxisUnit = TimeSpan.TicksPerSecond;

            ChartColor = "Red";

            SetAxisLimits(DateTime.Now);
        }
        
        private void InitLineSeries()
        {
            SeriesVisibility = new ObservableCollection<bool>();
            for (int i = 0; i < 7; i++)
            {
                SeriesVisibility.Add(false);
            }

            SeriesVisibility[(int)Series.Cpu] = true;
            SeriesVisibility[(int)Series.Gpu] = false;
            SeriesVisibility[(int)Series.Memory] = false;
            SeriesVisibility[(int)Series.Disk] = false;
            SeriesVisibility[(int)Series.Network] = false;
            SeriesVisibility[(int)Series.CpuTemp] = false;
            SeriesVisibility[(int)Series.GpuTemp] = false;

            CpuSeries = new LineSeries { Title = "Cpu", Values = new ChartValues<MeasureModel>(), ScalesYAt = 0 };
            GpuSeries = new LineSeries { Title = "Gpu", Values = new ChartValues<MeasureModel>(), ScalesYAt = 0 };
            MemorySeries = new LineSeries { Title = "Memory", Values = new ChartValues<MeasureModel>(), ScalesYAt = 0 };
            DiskSeries = new LineSeries { Title = "Disk", Values = new ChartValues<MeasureModel>(), ScalesYAt = 0 };
            NetworkSeries = new LineSeries { Title = "Network", Values = new ChartValues<MeasureModel>(), ScalesYAt = 0 };
            CpuTempSeries = new LineSeries { Title = "CpuTemp", Values = new ChartValues<MeasureModel>(), ScalesYAt = 0 };
            GpuTempSeries = new LineSeries { Title = "GpuTemp", Values = new ChartValues<MeasureModel>(), ScalesYAt = 0 };

            SeriesCollection = new SeriesCollection
            {
                CpuSeries,
                GpuSeries,
                MemorySeries,
                DiskSeries,
                NetworkSeries,
                CpuTempSeries,
                GpuTempSeries
            };
        }

        private void UpdateValues(ComputerObj comp)
        {
            Read(comp);
        }

        public int i = 0;
        public void Read(ComputerObj comp)
        {
            var now = DateTime.Now;
            Trend trend = GetTrend(comp);
            AddChartValues(trend, now);
            SetAxisLimits(now);
            ClearChartValues();
        }

        #region OnChange Events
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

        public string ChartColor
        {
            get { return _chartColor; }
            set
            {
                _chartColor = value;
                OnPropertyChanged("ChartColor");
            }
        }

        public ObservableCollection<bool> SeriesVisibility
        {
            get { return _seriesVisibility; }
            set
            {
                _seriesVisibility = value;
                OnPropertyChanged("SeriesVisibility");
            }
        }

        #endregion

        private void AddChartValues(Trend _trend, DateTime now)
        {
            Console.WriteLine(CpuSeries);
            CpuSeries.Values.Add(new MeasureModel { DateTime = now, Value = _trend.Cpu });
            GpuSeries.Values.Add(new MeasureModel { DateTime = now, Value = _trend.Gpu });
            MemorySeries.Values.Add(new MeasureModel { DateTime = now, Value = _trend.Memory });
            DiskSeries.Values.Add(new MeasureModel { DateTime = now, Value = _trend.Disk });
            NetworkSeries.Values.Add(new MeasureModel { DateTime = now, Value = _trend.Network });
            CpuTempSeries.Values.Add(new MeasureModel { DateTime = now, Value = _trend.CpuTemp });
            GpuTempSeries.Values.Add(new MeasureModel { DateTime = now, Value = _trend.GpuTemp });
        }
        private void ClearChartValues()
        {
            for (int i = 0; i < SeriesCollection.Count; i++)
            {
                if (SeriesCollection[i].Values.Count > 25)
                {
                    SeriesCollection[i].Values.RemoveAt(0);
                }
            }
        }

        private Trend GetTrend(ComputerObj comp)
        {
            Trend _trend = new Trend();
            Hardware hw = new Hardware();

            try
            {
                if (ProcessPid > 0)
                {
                        _trend.Cpu = comp.ProcessList.First(p => p.Pid == ProcessPid).Cpu;
                        _trend.Gpu = comp.ProcessList.First(p => p.Pid == ProcessPid).Gpu;
                        _trend.Memory = comp.ProcessList.First(p => p.Pid == ProcessPid).Memory;
                        _trend.Disk = comp.ProcessList.First(p => p.Pid == ProcessPid).Disk;
                        _trend.Network = comp.ProcessList.First(p => p.Pid == ProcessPid).Network;
                        _trend.CpuTemp = hw.CpuTemp(comp);
                        _trend.GpuTemp = hw.GpuTemp(comp);
                }
                else
                {
                    _trend.Cpu = comp.TotalCpu;
                    _trend.Gpu = comp.TotalGpu;
                    _trend.Memory = comp.TotalMemory;
                    _trend.Disk = comp.TotalDisk;
                    _trend.Network = comp.TotalNetwork;
                    _trend.CpuTemp = hw.CpuTemp(comp);
                    _trend.GpuTemp = hw.GpuTemp(comp);
                }
            }
            catch (Exception) // Process was killed but still trying to graph.
            {
                _trend.Cpu = 0;
                _trend.CpuTemp = 0;
                _trend.Gpu = 0;
                _trend.GpuTemp = 0;
                _trend.Memory = 0;
                _trend.Disk = 0;
                _trend.Network = 0;
            }
          
            return _trend;
        }

        private void SetAxisLimits(DateTime now)
        {
            AxisMax = now.Ticks + TimeSpan.FromSeconds(1).Ticks; // lets force the axis to be 1 second ahead
            AxisMin = now.Ticks - TimeSpan.FromSeconds(30).Ticks; // and  seconds behind
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