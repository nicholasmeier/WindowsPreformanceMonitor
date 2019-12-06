using System;
using System.Collections.Generic;
using System.IO;
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
using System.Diagnostics;
using System.Windows.Shapes;
using System.Management;
using System.Collections.ObjectModel;
using WindowsPerformanceMonitor.Models;
using OpenHardwareMonitor.Hardware;

namespace WindowsPerformanceMonitor
{
    /// <summary>
    /// Interaction logic for HardwareDetails.xaml
    /// </summary>
    public partial class HardwareDetails : UserControl
    {
        private MainWindow mainWindow = null; // Reference to the MainWindow
        CPUHardwareDetails cpuDetails = new CPUHardwareDetails();

        public ListBox List { get; set; }

        #region Initialization

        public HardwareDetails()
        {
            InitializeComponent();
            this.DataContext = this;
            List = lbDetailList;
            Parallel.Invoke(
                () => setLogicalCores(),
                () => setCores(),
                () => setClockSpeed(),
                () => setCacheSize(),
                () => setCPUName(),
                () => setCPUBusSpeed()
                );
            // based on logical cores
            setVirtualization();
            // set the CPU details on load
            setCPUValues(lbDetailList);
        }

        // Get a reference to main windows when it is available.
        // The Loaded Event is set in the XAML code above.
        private void OnControlLoaded(object sender, RoutedEventArgs e)
        {
            mainWindow = Window.GetWindow(this) as MainWindow;
        }

        #endregion

        #region CPU Details

        private void setCPUValues(ListBox listBox)
        {   
            var _clockSpeed = cpuDetails._clockSpeedCPU.ToString() + "GHz";
            List<DetailItem> items = new List<DetailItem>();
            items.Add(new DetailItem() { Title = "Base Speed:", Value = _clockSpeed });
            items.Add(new DetailItem() { Title = "Cores:", Value = cpuDetails._coresCPU.ToString() });
            items.Add(new DetailItem() { Title = "Logical Cores:", Value = cpuDetails._logicalCoresCPU.ToString() });
            items.Add(new DetailItem() { Title = "Virtualization:", Value = cpuDetails._virtualizationCPU.ToString() });
            items.Add(new DetailItem() { Title = "L1 Cache:", Value = cpuDetails._cacheSizesCPU[0].ToString() + " KB" });
            items.Add(new DetailItem() { Title = "L2 Cache:", Value = cpuDetails._cacheSizesCPU[1].ToString() + " MB" });
            items.Add(new DetailItem() { Title = "L3 Cache:", Value = cpuDetails._cacheSizesCPU[2].ToString() + " MB" });
            if (cpuDetails._busSpeedCPU > 0)
                items.Add(new DetailItem() { Title = "Bus Speed:", Value = cpuDetails._busSpeedCPU.ToString() + "MHz" });
            listBox.ItemsSource = items;
            groupBoxDetails.Header = "CPU Details - " + cpuDetails._name;
        }

        private void setLogicalCores()
        {
            cpuDetails._logicalCoresCPU = Environment.ProcessorCount;
        }

        private void setCores()
        {
            int coreCount = 0;
            foreach (var item in new System.Management.ManagementObjectSearcher("Select * from Win32_Processor").Get())
            {
                coreCount += int.Parse(item["NumberOfCores"].ToString());
            }
            cpuDetails._coresCPU = coreCount;
        }

        private void setVirtualization()
        {
            if (cpuDetails._logicalCoresCPU > 0)
            {
                cpuDetails._virtualizationCPU = true;
            }
            else
            {
                cpuDetails._virtualizationCPU = false;
            }
        }

        private void setClockSpeed() // This is stored as Ghz
        {
            ManagementObject Mo = new ManagementObject("Win32_Processor.DeviceID='CPU0'");
            uint sp = (uint)(Mo["CurrentClockSpeed"]);
            double dp = sp / 1000.00;
            Mo.Dispose();
            cpuDetails._clockSpeedCPU = dp;
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

            cpuDetails._cacheSizesCPU = cacheSizes;
        }

        public void setCPUName()
        {
            var cp = new Computer();
            cp.Open();
            cp.HDDEnabled = true;
            cp.FanControllerEnabled = true;
            cp.RAMEnabled = true;
            cp.GPUEnabled = true;
            cp.MainboardEnabled = true;
            cp.CPUEnabled = true;

            for (int i = 0; i < cp.Hardware.Count(); i++)
            {
                var hardware = cp.Hardware[i];
                hardware.Update();

                if (hardware.HardwareType == HardwareType.CPU)
                {
                    cpuDetails._name = hardware.Name;
                }
            }
        }

        public void setCPUBusSpeed()
        {
            var cp = new Computer();
            cp.Open();
            cp.HDDEnabled = true;
            cp.FanControllerEnabled = true;
            cp.RAMEnabled = true;
            cp.MainboardEnabled = true;
            cp.CPUEnabled = true;

            for (int i = 0; i < cp.Hardware.Count(); i++)
            {
                var hardware = cp.Hardware[i];
                hardware.Update();

                if (hardware.HardwareType == HardwareType.CPU)
                {
                    for (int j = 0; j < hardware.Sensors.Count(); j++)
                    {
                        var sensor = hardware.Sensors[j];
                        if ( sensor.SensorType == SensorType.Clock && sensor.Name == "Bus Speed")
                        {
                            cpuDetails._busSpeedCPU = Math.Round((double)sensor.Value, 3);
                        }
                    }
                }
            }
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
            DriveInfo[] drivelist = DriveInfo.GetDrives();

            List<DetailItem> items = new List<DetailItem>();
            items.Add(new DetailItem() { Title = "Number of Disk: ", Value = drivelist.Length.ToString() });
            int cnt = 0;
            foreach (DriveInfo d in drivelist)
            {
                items.Add(new DetailItem() { Title = String.Concat("Drive ", cnt, " - ", d.Name, " - Disk Type : "), Value = d.DriveType.ToString() });
                if (d.IsReady)
                {
                    items.Add(new DetailItem() { Title = String.Concat("Drive ", cnt, " - ", d.Name, " - Total Size : "), Value = String.Concat(d.TotalSize.ToString(), " bytes") });
                    items.Add(new DetailItem() { Title = String.Concat("Drive ", cnt, " - ", d.Name, " - Free Space : "), Value = String.Concat(d.TotalFreeSpace.ToString(), " bytes") });
                }
                else
                {
                    items.Add(new DetailItem() { Title = String.Concat("Drive ", cnt, " - ", d.Name, " - Total Size : "), Value = "N/A" });
                }
                cnt++;
            }
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
                    setCPUValues(List);
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
