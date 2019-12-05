using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;

namespace WindowsPerformanceMonitor
{
    public class UserSettings
    {
        public struct Settings
        {
            public bool IsEncryption;
            public string LogFilePath;
            public string LogFileFormat;

            public bool Hibernation;
            public string CpuThreshold;
            public string GpuThreshold;
            public string MemoryThreshold;
            public string Email;

            //overlay settings
            public double ovly_opac;
            public bool ovly_sys;
            public bool ovly_cpu;
            public bool ovly_gpu;
            public bool ovly_mem;
            public bool ovly_net;
            public bool ovly_dis;
            public bool ovly_tcpu;
            public bool ovly_tgpu;

        }

        public Settings settings;
        public string path { get; private set; }

        public UserSettings()
        {
            settings = new Settings();
            path = Path.Combine(Environment.ExpandEnvironmentVariables("%userprofile%"), "Documents", "WindowsPerformanceMonitorSettings");
        }

        public void Save()
        {
            String json = JsonConvert.SerializeObject(settings);
            String fileName = "\\UserSettings.txt";
            if (!System.IO.Directory.Exists(path))
                System.IO.Directory.CreateDirectory(path);


            System.IO.File.WriteAllText(path + fileName, json);
        }

        public void Delete()
        {
            File.Delete(path + "\\UserSettings.txt");
        }

        private Settings Read()
        {
            string json = new StreamReader(path + "\\UserSettings.txt").ReadToEnd();
            return JsonConvert.DeserializeObject<Settings>(json);
        }

        public bool Exists()
        {
            return System.IO.File.Exists(path + "\\UserSettings.txt");
        }

        public void Load()
        {
            settings = Read();
        }
    }
}
