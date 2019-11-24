using SharpPcap;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

// You might need to download WinPcap_4_1_3 to run this
// without errors.
namespace WindowsPerformanceMonitor.Backend
{
    public class ProcessPerformanceInfo
    {
        public int ProcessID { get; set; }
        public long NetSendBytes { get; set; }
        public long NetRecvBytes { get; set; }
        public long NetTotalBytes { get; set; }

    }
    public class NetworkInfo
    {
        public ProcessPerformanceInfo ProcInfo;

        public void Run(int _pid)
        {
            ProcInfo = new ProcessPerformanceInfo()
            {
                ProcessID = _pid
            };

            int pid = ProcInfo.ProcessID;
            List<int> ports = new List<int>();

            #region get ports for certain process through executing the netstat -ano command  in cmd
            Process pro = new Process();
            pro.StartInfo.FileName = "cmd.exe";
            pro.StartInfo.UseShellExecute = false;
            pro.StartInfo.RedirectStandardInput = true;
            pro.StartInfo.RedirectStandardOutput = true;
            pro.StartInfo.RedirectStandardError = true;
            pro.StartInfo.CreateNoWindow = true;
            pro.Start();
            pro.StandardInput.WriteLine("netstat -ano");
            pro.StandardInput.WriteLine("exit");
            Regex reg = new Regex("\\s+", RegexOptions.Compiled);
            string line = null;
            ports.Clear();
            while ((line = pro.StandardOutput.ReadLine()) != null)
            {
                line = line.Trim();
                if (line.StartsWith("TCP", StringComparison.OrdinalIgnoreCase))
                {
                    line = reg.Replace(line, ",");
                    string[] arr = line.Split(',');
                    if (arr[4] == pid.ToString())
                    {
                        string soc = arr[1];
                        int pos = soc.LastIndexOf(':');
                        int pot = int.Parse(soc.Substring(pos + 1));
                        ports.Add(pot);
                    }
                }
                else if (line.StartsWith("UDP", StringComparison.OrdinalIgnoreCase))
                {
                    line = reg.Replace(line, ",");
                    string[] arr = line.Split(',');
                    if (arr[3] == pid.ToString())
                    {
                        string soc = arr[1];
                        int pos = soc.LastIndexOf(':');
                        int pot = int.Parse(soc.Substring(pos + 1));
                        ports.Add(pot);
                    }
                }
            }
            pro.Close();
            #endregion

            // Get ip address
            IPAddress[] addrList = Dns.GetHostByName(Dns.GetHostName()).AddressList;
            string IP = addrList[0].ToString();


            // var devices = SharpPcap.WinPcap.WinPcapDeviceList.Instance;
            var devices = CaptureDeviceList.Instance;

            // differentiate based upon types

            int count = devices.Count;
            if (count < 1)
            {
                Console.WriteLine("No device found on this machine");
                return;
            }

            for (int i = 0; i < count; ++i)
            {
                for (int j = 0; j < ports.Count; ++j)
                {
                    CaptureFlowRecv(IP, ports[j], i);
                    CaptureFlowSend(IP, ports[j], i);
                }
            }
            while (true)
            {
                Console.WriteLine(_pid + " proc NetTotalBytes : " + ProcInfo.NetTotalBytes);
                Console.WriteLine(_pid + " proc NetSendBytes : " + ProcInfo.NetSendBytes);
                Console.WriteLine(_pid + " proc NetRecvBytes : " + ProcInfo.NetRecvBytes);

                //Call refresh function every 1s 
                RefreshInfo();
            }

        }

        private void CaptureFlowSend(string IP, int portID, int deviceID)
        {
            ICaptureDevice device = CaptureDeviceList.New()[deviceID];

            device.OnPacketArrival += new PacketArrivalEventHandler(device_OnPacketArrivalSend);
            int readTimeoutMilliseconds = 1000;
            device.Open(DeviceMode.Promiscuous, readTimeoutMilliseconds);
            string filter = "src host " + IP + " and src port " + portID;
            device.Filter = filter;
            device.StartCapture();

        }

        private void device_OnPacketArrivalSend(object sender, CaptureEventArgs e)
        {
            //DateTime time = e.Packet.Timeval.Date;
            //int len = e.Packet.Data.Length;
            //Console.WriteLine("{0}:{1}:{2},{3} Len={4}",
            //    time.Hour, time.Minute, time.Second, time.Millisecond, len);
            var len = e.Packet.Data.Length;
            ProcInfo.NetSendBytes += len;

        }

        private void CaptureFlowRecv(string IP, int portID, int deviceID)
        {
            ICaptureDevice device = CaptureDeviceList.New()[deviceID];
            device.OnPacketArrival += new PacketArrivalEventHandler(device_OnPacketArrivalRecv);

            int readTimeoutMilliseconds = 1000;
            device.Open(DeviceMode.Promiscuous, readTimeoutMilliseconds);

            string filter = "dst host " + IP + " and dst port " + portID;
            device.Filter = filter;
            device.StartCapture();

        }

        private void device_OnPacketArrivalRecv(object sender, CaptureEventArgs e)
        {
            var len = e.Packet.Data.Length;
            ProcInfo.NetRecvBytes += len;
        }

        public void RefreshInfo()
        {
            ProcInfo.NetRecvBytes = 0;
            ProcInfo.NetSendBytes = 0;
            ProcInfo.NetTotalBytes = 0;
            Thread.Sleep(1000);
            ProcInfo.NetTotalBytes = ProcInfo.NetRecvBytes + ProcInfo.NetSendBytes;
        }
    }
}
