using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WindowsPerformanceMonitor.Models;

namespace WindowsPerformanceMonitor.Backend
{
    public class Hardware
    {
        public double CpuTemp(ComputerObj comp)
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

            return tempTotal / numTotal;
        }

        public double GpuTemp(ComputerObj comp)
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

            return tempTotal / numTotal;
        }
    }
}
