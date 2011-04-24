using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EHaskins.Frc.Communication.DriverStation;

namespace EStopTest
{
    class Program
    {
        static void Main(string[] args)
        {
            var monitor = new KeyboardMonitor();

            monitor.Start();
            monitor.EStopPressed += EStopPressed;
            monitor.EnablePressed += EnablePressed;
            monitor.DisablePressed += DisablePressed;
            while (true)
            {
                monitor.Poll();

                System.Threading.Thread.Sleep(100);
            }
        }
        private static void DisablePressed(object sender, EventArgs e)
        {
            Console.WriteLine("Disabled");
        }
        private static void EnablePressed(object sender, EventArgs e)
        {
            Console.WriteLine("Enabled");
        }
        private static void EStopPressed(object sender, EventArgs e)
        {
            Console.WriteLine("EStop");
        }

    }
}
