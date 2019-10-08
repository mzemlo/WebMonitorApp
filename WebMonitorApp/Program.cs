using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WebMonitorApp
{
    static class Program
    {
        /// <summary>
        /// Główny punkt wejścia dla aplikacji.
        /// </summary>
        [STAThread]
        static void Main()
        {
            //HERE ADD EXAMPLE USER NAME AND API URL   
            LogsOperations.userName = "";
            DataProvider.URL = "http://www.example.com/";


            RegistryKey rkApp = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);

            //add to autostart
            //if (!IsStartupItem())
            //    rkApp.SetValue("WebMonitorApp", Application.ExecutablePath.ToString());

            //remove from autostart
            //if (IsStartupItem())
            //    rkApp.DeleteValue("WebMonitorApp", false);

            SystemEvents.SessionEnded += new SessionEndedEventHandler(SystemEvents_SessionEnded);

            LogsOperations.InitMonitor();
            
            Application.Run();           
        }

        //Code runs on system shutdown
        static void SystemEvents_SessionEnded(object sender, SessionEndedEventArgs e)
        {           
            string data = System.IO.File.ReadAllText("logs.txt");
            DataProvider.SendData(data);
            System.IO.File.WriteAllText("logs.txt", "");
            LogsOperations._data = new List<LogModel>();
        }
       
        private static bool IsStartupItem()
        {
            RegistryKey rkApp = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);

            if (rkApp.GetValue("WebMonitorApp") == null)
                
                return false;
            else
                
                return true;
        }
    }
}
