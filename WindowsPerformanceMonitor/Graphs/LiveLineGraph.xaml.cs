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
        private string _chartColor;

        public ChartValues<MeasureModel> ChartValues { get; set; }
        public Func<double, string> DateTimeFormatter { get; set; }
        public double AxisStep { get; set; }
        public double AxisUnit { get; set; }
        public int ProcessPid { get; set; }
        public string StatToGraph { get; set; }

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
            AxisStep = TimeSpan.FromSeconds(10).Ticks;

            // AxisUnit forces lets the axis know that we are plotting seconds
            AxisUnit = TimeSpan.TicksPerSecond;

            ChartColor = "Red";

            SetAxisLimits(DateTime.Now);
        }

        private void UpdateValues(ComputerObj comp)
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

        public string ChartColor
        {
            get { return _chartColor; }
            set
            {
                _chartColor = value;
                OnPropertyChanged("ChartColor");
            }
        }

        public bool IsReading { get; set; }

        public void Read(ComputerObj comp)
        {
            var now = DateTime.Now;

            _trend = GetTrend(comp);

            ChartValues.Add(new MeasureModel
            {
                DateTime = now,
                Value = _trend
            });

            SetAxisLimits(now);

            if (ChartValues.Count > 150) ChartValues.RemoveAt(0);
       
        }

        private double GetTrend(ComputerObj comp)
        {
            // Probably not the best way, but it works.
            double _trend = 0;

            try
            {
                if (ProcessPid > 0)
                {
                    if (StatToGraph == "CPU")
                    {
                        _trend = comp.ProcessList.First(p => p.Pid == ProcessPid).Cpu;
                        ChartColor = "Red";
                    }
                    else if (StatToGraph == "GPU")
                    {
                        _trend = comp.ProcessList.First(p => p.Pid == ProcessPid).Gpu;
                        ChartColor = "Orange";
                    }
                    else if (StatToGraph == "Memory")
                    {
                        _trend = comp.ProcessList.First(p => p.Pid == ProcessPid).Memory;
                        ChartColor = "Green";
                    }
                    else if (StatToGraph == "Disk")
                    {
                        _trend = comp.ProcessList.First(p => p.Pid == ProcessPid).Disk;
                        ChartColor = "Blue";
                    }
                    else if (StatToGraph == "Network")
                    {
                        _trend = comp.ProcessList.First(p => p.Pid == ProcessPid).Network;
                        ChartColor = "Purple";
                    }
                }
                else
                {
                    if (StatToGraph == "CPU")
                    {
                        _trend = comp.TotalCpu;
                        ChartColor = "Red";
                    }
                    if (StatToGraph == "CPU Temperature")
                    {
                        int cpuindex = 0;
                        for (int i = 0; i < comp.Computer.Hardware.Length; i++)
                        {
                            if (comp.Computer.Hardware[i].HardwareType.ToString().Equals("CPU"))
                            {
                                cpuindex = i;
                            }
                        }
                        var mycpu = comp.Computer.Hardware[cpuindex];
                        double tempTotal = 0;
                        int numTotal = 0;
                        for (int i = 0; i < mycpu.Sensors.Length; i++)
                        {
                            if (mycpu.Sensors[i].SensorType.ToString().Equals("Temperature"))
                            {
                                if (mycpu.Sensors[i].Value != null)
                                {
                                    numTotal++;
                                    tempTotal += Convert.ToDouble(mycpu.Sensors[i].Value.ToString());
                                }
                            }
                        }
                        _trend = tempTotal / numTotal;
                        ChartColor = "Yellow";
                    }
                    else if (StatToGraph == "GPU")
                    {
                        _trend = comp.TotalGpu;
                        ChartColor = "Orange";
                    }
                    else if (StatToGraph == "GPU Temperature")
                    {
                        int gpuindex = 0;
                        for (int i = 0; i < comp.Computer.Hardware.Length; i++)
                        {
                            String tempGPU = comp.Computer.Hardware[i].HardwareType.ToString();
                            if (tempGPU.Contains("gpu") || tempGPU.Contains("Gpu") || tempGPU.Contains("GPU"))
                            {
                                gpuindex = i;
                            }
                        }
                        var mygpu = comp.Computer.Hardware[gpuindex];
                        double tempTotal = 0;
                        int numTotal = 0;
                        for (int i = 0; i < mygpu.Sensors.Length; i++)
                        {
                            if (mygpu.Sensors[i].SensorType.ToString().Equals("Temperature"))
                            {
                                if (mygpu.Sensors[i].Value != null)
                                {
                                    numTotal++;
                                    tempTotal += Convert.ToDouble(mygpu.Sensors[i].Value.ToString());
                                }
                            }
                        }
                        _trend = tempTotal / numTotal;
                        ChartColor = "Pink";
                    }
                    else if (StatToGraph == "Memory")
                    {
                        _trend = comp.TotalMemory;
                        ChartColor = "Green";
                    }
                    else if (StatToGraph == "Disk")
                    {
                        _trend = comp.TotalDisk;
                        ChartColor = "Blue";
                    }
                    else if (StatToGraph == "Network")
                    {
                        _trend = comp.TotalNetwork;
                        ChartColor = "Purple";
                    }
                }
            }
            catch (Exception) // Process was killed but still trying to graph.
            {
                _trend = 0;
            }
           

            return _trend;
        }

        public void Clear()
        {
            ChartValues.Clear();
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