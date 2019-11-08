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
