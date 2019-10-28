using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WindowsPerformanceMonitor.Backend;
using WindowsPerformanceMonitor.Graphs;
using WindowsPerformanceMonitor.Models;

namespace WindowsPerformanceMonitor
{
    class Log
    {
        public struct data {
            public double Cpu;
            public double Gpu;
            public double Memory;
            public double Disk;
            public double Network;
            public double CpuTemp;
            public double GpuTemp;
            public List<ProcessEntry> ProcessList { get; set; }
        }

        public struct payload
        {
            public List<data> mydata;
            public List<DateTime> mytimes;
            public DateTime mystart;
        }

        payload mypayload;
        String logPath;
        HardwareObserver observer;
        public Log()
        {
            logPath = Directory.GetCurrentDirectory();//DEFAULT LOG PATH
            mypayload = new payload();
            mypayload.mydata = new List<data>();
            mypayload.mytimes = new List<DateTime>();
            StartLog();
        }
        
        public Log(String path)
        {
            logPath = path;
            mypayload = new payload();
            mypayload.mydata = new List<data>();
            mypayload.mytimes = new List<DateTime>();
        }

        public void StartLog()
        {
            mypayload.mystart = DateTime.Now;
            observer = new HardwareObserver(UpdateValues);
            Globals.provider.Subscribe(observer);
        }

        public void UpdateValues(ComputerObj comp)
        {
            Hardware hw = new Hardware();
            data NewData = new data();
            NewData.CpuTemp = hw.CpuTemp(comp);
            NewData.GpuTemp = hw.GpuTemp(comp);
            NewData.Cpu = comp.TotalCpu;
            NewData.Gpu = comp.TotalGpu;
            NewData.Memory = comp.TotalMemory;
            NewData.Network = comp.TotalNetwork;
            NewData.Disk = comp.TotalDisk;
            NewData.ProcessList = new List<ProcessEntry>(comp.ProcessList);
            mypayload.mydata.Add(NewData);
            mypayload.mytimes.Add(DateTime.Now);
        }

        public void WriteIt()
        {
            // Note: might need to compress these using System.IO.Compression.
            //       might need to force a write after so much memory is used to avoid running out.
            String json = JsonConvert.SerializeObject(mypayload);
            String fileName = mypayload.mystart.Date.Month.ToString() + "-" + mypayload.mystart.Date.Day.ToString() + "-" + mypayload.mystart.Date.Year.ToString();
            if(!System.IO.Directory.Exists(Path.Combine(Environment.ExpandEnvironmentVariables("%userprofile%"), "Documents", "WindowsPerformanceMonitor")))
                System.IO.Directory.CreateDirectory(Path.Combine(Environment.ExpandEnvironmentVariables("%userprofile%"), "Documents", "WindowsPerformanceMonitor"));
            int i = 0;
            string appendage = "";
            while (File.Exists(Path.Combine(Environment.ExpandEnvironmentVariables("%userprofile%"), "Documents", "WindowsPerformanceMonitor", fileName + appendage +".txt")))
            {
                i++;
                appendage = "(" + i + ")";
            }
        
            System.IO.File.WriteAllText(Path.Combine(Environment.ExpandEnvironmentVariables("%userprofile%"), "Documents", "WindowsPerformanceMonitor", fileName + appendage +".txt"), json);
            observer.Unsubscribe();
        }

        public payload ReadIt(String path)
        {
            string json = new StreamReader(path).ReadToEnd();
            return JsonConvert.DeserializeObject<payload>(json);
        }
    }
}
