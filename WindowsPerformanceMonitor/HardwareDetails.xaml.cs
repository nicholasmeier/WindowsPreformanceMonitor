using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using PerformanceMonitor.Cpp.CLI;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Diagnostics;
using System.Windows.Shapes;
using System.Management;
using System.Collections.ObjectModel;
using System.Management;

namespace WindowsPerformanceMonitor
{
    /// <summary>
    /// Interaction logic for HardwareDetails.xaml
    /// </summary>
    public partial class HardwareDetails : UserControl
    {
        private MainWindow mainWindow = null; // Reference to the MainWindow
        public double _clockSpeedCPU { get; set; }
        public int _coresCPU { get; set; }
        public int _logicalCoresCPU { get; set; }
        public Boolean _virtualizationCPU { get; set; }

        public ListBox List { get; set; }

        #region Initialization
        public HardwareDetails()
        {
            InitializeComponent();
            this.DataContext = this;

        }

        // Get a reference to main windows when it is available.
        // The Loaded Event is set in the XAML code above.
        private void OnControlLoaded(object sender, RoutedEventArgs e)
        {
            mainWindow = Window.GetWindow(this) as MainWindow;

            List = lbDetailList;
            setCPUValues(lbDetailList);
        }
        #endregion

        #region CPU Details
        private void setCPUValues(ListBox listBox)
        {   
            setLogicalCores();
            setCores();
            setVirtualization();
            setClockSpeed();
            var _clockSpeed = _clockSpeedCPU.ToString() + "GHz";
            List<DetailItem> items = new List<DetailItem>();
            items.Add(new DetailItem() { Title = "Base Speed:", Value = _clockSpeed });
            items.Add(new DetailItem() { Title = "Cores:", Value = _coresCPU.ToString() });
            items.Add(new DetailItem() { Title = "Logical Cores:", Value = _logicalCoresCPU.ToString() });
            items.Add(new DetailItem() { Title = "Virtualization:", Value = _virtualizationCPU.ToString() });

            listBox.ItemsSource = items;
            groupBoxDetails.Header = "CPU Details";
        }

        private void setLogicalCores()
        {
            _logicalCoresCPU = Environment.ProcessorCount;
        }
        private void setCores()
        {
            int coreCount = 0;
            foreach (var item in new System.Management.ManagementObjectSearcher("Select * from Win32_Processor").Get())
            {
                coreCount += int.Parse(item["NumberOfCores"].ToString());
            }
            _coresCPU = coreCount;
        }
        private void setVirtualization()
        {
            if (_logicalCoresCPU > 0)
            {
                _virtualizationCPU = true;
            }
            else
            {
                _virtualizationCPU = false;
            }
        }
        private void setClockSpeed() // This is stored as Ghz
        {
            ManagementObject Mo = new ManagementObject("Win32_Processor.DeviceID='CPU0'");
            uint sp = (uint)(Mo["CurrentClockSpeed"]);
            double dp = sp / 1000.00;
            Mo.Dispose();
            _clockSpeedCPU = dp;
        }
        #endregion
        #region GPU Details
        private void setGPUValues(ListBox listBox)
        {
            List<DetailItem> items = new List<DetailItem>();
            items.Add(new DetailItem() { Title = "Base Speed:", Value = "1.8GHz" });
            items.Add(new DetailItem() { Title = "Cores:", Value = "2" });
            items.Add(new DetailItem() { Title = "Logical Cores:", Value = "4" });
            items.Add(new DetailItem() { Title = "Virtualization:", Value = "True" });

            listBox.ItemsSource = items;
            groupBoxDetails.Header = "GPU Details";
        }
        #endregion
        #region Memory Details
        private void setMemoryValues(ListBox listBox)
        {
            List<DetailItem> items = new List<DetailItem>();
            items.Add(new DetailItem() { Title = "Base Speed:", Value = "1.5GHz" });
            items.Add(new DetailItem() { Title = "Cores:", Value = "1" });
            items.Add(new DetailItem() { Title = "Logical Cores:", Value = "0" });
            items.Add(new DetailItem() { Title = "Virtualization:", Value = "False" });

            listBox.ItemsSource = items;
            groupBoxDetails.Header = "Memory Details";
        }
        #endregion
        #region Disk Details
        private void setDiskValues(ListBox listBox)
        {
            List<DetailItem> items = new List<DetailItem>();
            items.Add(new DetailItem() { Title = "Base Speed:", Value = "1.8GHz" });
            items.Add(new DetailItem() { Title = "Cores:", Value = "2" });
            items.Add(new DetailItem() { Title = "Logical Cores:", Value = "4" });
            items.Add(new DetailItem() { Title = "Virtualization:", Value = "True" });

            listBox.ItemsSource = items;
            groupBoxDetails.Header = "Disk Details";
        }
        #endregion
        #region Network Details
        private void setNetworkValues(ListBox listBox)
        {
            List<DetailItem> items = new List<DetailItem>();
            items.Add(new DetailItem() { Title = "Base Speed:", Value = "1.8GHz" });
            items.Add(new DetailItem() { Title = "Cores:", Value = "2" });
            items.Add(new DetailItem() { Title = "Logical Cores:", Value = "4" });
            items.Add(new DetailItem() { Title = "Virtualization:", Value = "True" });

            listBox.ItemsSource = items;
            groupBoxDetails.Header = "Network Details";
        }
        #endregion
        #region UI Events
        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            var radioButton = sender as RadioButton;
            if (radioButton == null|| List == null) return;
            switch (radioButton.Content.ToString())
            {
                case "CPU":
                    setCPUValues(List);
                    break;
                case "GPU":
                    setGPUValues(List);
                    break;
                case "Memory":
                    setMemoryValues(List);
                    break;
                case "Disk":
                    setDiskValues(List);
                    break;
                case "Network":
                    setNetworkValues(List);
                    break;
                default:
                    break;
            }
            
        }
        #endregion
    }

    #region Detail class
    public class DetailItem
    {
        public string Title { get; set; }
        public string Value { get; set; }
    }
    #endregion
}
