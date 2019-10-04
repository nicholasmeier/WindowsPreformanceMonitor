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
        public List<uint> _cacheSizesCPU { get; set; }

        public ListBox List { get; set; }

        #region Initialization

        public HardwareDetails()
        {
            InitializeComponent();
            this.DataContext = this;
            List = lbDetailList;
            //setCPUValues(lbDetailList);
        }

        // Get a reference to main windows when it is available.
        // The Loaded Event is set in the XAML code above.
        private void OnControlLoaded(object sender, RoutedEventArgs e)
        {
            mainWindow = Window.GetWindow(this) as MainWindow;
        }

        #endregion

        // TODO: If we make each of these items their own object, we dont have to worry
                // about overwriting them when they change. Then we only need to call each one
                // on load.

        #region CPU Details

        private void setCPUValues(ListBox listBox)
        {   
            setLogicalCores();
            setCores();
            setVirtualization();
            setClockSpeed();
            setCacheSize();
            var _clockSpeed = _clockSpeedCPU.ToString() + "GHz";
            List<DetailItem> items = new List<DetailItem>();
            items.Add(new DetailItem() { Title = "Base Speed:", Value = _clockSpeed });
            items.Add(new DetailItem() { Title = "Cores:", Value = _coresCPU.ToString() });
            items.Add(new DetailItem() { Title = "Logical Cores:", Value = _logicalCoresCPU.ToString() });
            items.Add(new DetailItem() { Title = "Virtualization:", Value = _virtualizationCPU.ToString() });
            items.Add(new DetailItem() { Title = "L1 Cache:", Value = _cacheSizesCPU[0].ToString() + " KB" });
            items.Add(new DetailItem() { Title = "L2 Cache:", Value = _cacheSizesCPU[1].ToString() + " KB" });
            items.Add(new DetailItem() { Title = "L3 Cache:", Value = _cacheSizesCPU[2].ToString() + " KB" });
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

        private void setCacheSize()
        {
            ManagementClass mc = new ManagementClass("Win32_CacheMemory");
            ManagementObjectCollection moc = mc.GetInstances();
            List<uint> cacheSizes = new List<uint>(moc.Count);
            for (int i = 3; i <= 5; i++)
            {
                cacheSizes.AddRange(moc
                  .Cast<ManagementObject>()
                  .Where(p => (ushort)(p.Properties["Level"].Value) == (ushort)i)
                  .Select(p => (uint)(p.Properties["MaxCacheSize"].Value)));
            }

            _cacheSizesCPU = cacheSizes;
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
                    setCPUValues(lbDetailList);
                    groupBoxDetails.Header = "CPU Details";
                    break;
                case "GPU":
                    groupBoxDetails.Header = "GPU Details";
                    setGPUValues(List);
                    break;
                case "Memory":
                    groupBoxDetails.Header = "Memory Details";
                    setMemoryValues(List);
                    break;
                case "Disk":
                    groupBoxDetails.Header = "Disk Details";
                    setDiskValues(List);
                    break;
                case "Network":
                    groupBoxDetails.Header = "Network Details";
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
