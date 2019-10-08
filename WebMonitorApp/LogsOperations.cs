using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace WebMonitorApp
{
    class LogsOperations
    {
        private static string lastUrl = null;
        private static string lastTitle = null;
        private static LogModel objToSave = new LogModel();
        public static List<LogModel> _data = new List<LogModel>();
        public static string userName;
        private static string activeProcessName = null;
        public static void InitMonitor()
        {
            //Get logged windows username
            //WindowsPrincipal wp = new WindowsPrincipal(WindowsIdentity.GetCurrent());
            //string identity = wp.Identity.Name;
            //userName =  identity.Substring(identity.IndexOf(@"\") + 1);
            System.IO.File.WriteAllText("logs.txt", "");
            
            //Set username for log object          
            objToSave.UserName = userName;

            //Check the active url every 2s 
            System.Timers.Timer time = new System.Timers.Timer
            {
                Interval = 2000
            };
            time.Elapsed += new System.Timers.ElapsedEventHandler(SaveUrl);
            time.Start();
            SaveUrl(time, null);

            //Check the active process every 0.5s 
            System.Timers.Timer timeProcess = new System.Timers.Timer
            {
                Interval = 500
            };
            timeProcess.Elapsed += new System.Timers.ElapsedEventHandler(GetActiveProcessName);
            timeProcess.Start();
            GetActiveProcessName(timeProcess, null);

            //Send the data to server every 5min
            System.Timers.Timer timeToRequest = new System.Timers.Timer
            {
                Interval = 300000
            };
            timeToRequest.Elapsed += new System.Timers.ElapsedEventHandler(PushLogs);
            timeToRequest.Start();
            PushLogs(timeToRequest, null);
        }

        [DllImport("user32.dll")]
        public static extern IntPtr GetWindowThreadProcessId(IntPtr hWnd, out uint ProcessId);

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        //Get active process name
        private static void GetActiveProcessName(object sender, System.Timers.ElapsedEventArgs e)
        {
            IntPtr hwnd = GetForegroundWindow();
            GetWindowThreadProcessId(hwnd, out uint pid);
            Process p = Process.GetProcessById((int)pid);
            activeProcessName = p.ProcessName;
        }

        //Save objects array to json file
        public static void SaveLogToFile(LogModel _objectToSave)
        {
            try
            {
                LogModel logToSave = new LogModel
                {
                    UserName = _objectToSave.UserName,
                    URL = _objectToSave.URL,
                    From = _objectToSave.From,
                    To = _objectToSave.To
                };
                _data.Add(logToSave);
                string json = JsonConvert.SerializeObject(_data.ToArray());

                //write string to file
                System.IO.File.WriteAllText("logs.txt", json);
            }
            catch (Exception)
            {
                Console.WriteLine("Can't save logs to file!!!");
                SaveLogToFile(_objectToSave);
            }

        }

        public static void PushLogs(object sender, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                string data = System.IO.File.ReadAllText("logs.txt");
                if (data != "")
                {
                    DataProvider.SendData(data);
                }
            }
            catch (Exception)
            {
                Console.WriteLine("Can't clear logs file!!!");
                //Try to send the data after 5 seconds
                System.Timers.Timer timeToNextRequest = new System.Timers.Timer
                {
                    Interval = 5000
                };
                timeToNextRequest.Elapsed += new System.Timers.ElapsedEventHandler(PushLogs);
                timeToNextRequest.Start();
                PushLogs(timeToNextRequest, null);
                timeToNextRequest.Stop();
            }

        }

        //Get the active Url 
        public static void SaveUrl(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (activeProcessName == "chrome")
            {
                foreach (Process process in Process.GetProcessesByName("chrome"))
                {
                    string url = BrowserLocation.GetChromeUrl(process);
                    if (url == null)
                        continue;
                    if (lastTitle != process.MainWindowTitle)
                    {

                        if (lastTitle != null && lastUrl != null && lastUrl != "" && lastUrl.Length > 4)
                        {
                            objToSave.To = DateTime.Now;
                            if (lastUrl.Substring(0, 4) == "http")
                            {
                                SaveLogToFile(objToSave);
                            }
                            lastUrl = BrowserLocation.GetChromeUrl(process);
                            lastTitle = process.MainWindowTitle;
                            objToSave.From = DateTime.Now;
                            objToSave.URL = lastUrl;
                        }
                        else
                        {
                            lastUrl = BrowserLocation.GetChromeUrl(process);
                            lastTitle = process.MainWindowTitle;
                            objToSave.URL = lastUrl;
                            objToSave.From = DateTime.Now;
                        }
                    }
                }
            }
            else if (activeProcessName == "firefox")
            {
                foreach (Process process in Process.GetProcessesByName("firefox"))
                {
                    string url = BrowserLocation.GetFirefoxUrl(process);
                    if (url == null)
                        continue;
                    if (lastUrl != url)
                    {
                        if (lastTitle != null && lastUrl != null && lastUrl != "")
                        {
                            if (lastUrl.Substring(0, 4) == "http")
                            {
                                objToSave.To = DateTime.Now;
                                SaveLogToFile(objToSave);
                            }
                        }
                        lastUrl = BrowserLocation.GetFirefoxUrl(process);
                        lastTitle = process.MainWindowTitle;
                        objToSave.URL = lastUrl;
                        objToSave.From = DateTime.Now;

                    }
                }
            }
            else if (activeProcessName == "iexplore")
            {
                foreach (Process process in Process.GetProcessesByName("iexplore"))
                {
                    string url = BrowserLocation.GetInternetExplorerUrl(process);
                    if (url == null)
                        continue;
                    if (lastUrl != url)
                    {
                        if (lastTitle != null && lastUrl != null && lastUrl != "" && lastUrl.Length > 4)
                        {
                            if (lastUrl.Substring(0, 4) == "http")
                            {
                                objToSave.To = DateTime.Now;
                                SaveLogToFile(objToSave);
                            }
                        }
                        lastUrl = BrowserLocation.GetInternetExplorerUrl(process);
                        lastTitle = process.MainWindowTitle;
                        objToSave.URL = lastUrl;
                        objToSave.From = DateTime.Now;

                    }
                }
            }
            else
            {
                if (lastUrl != null)
                {
                    objToSave.To = DateTime.Now;
                    if (lastUrl != "" && lastUrl.Length > 4)
                    {
                        SaveLogToFile(objToSave);
                    }

                    lastUrl = null;
                    lastTitle = null;
                }

            }
        }
    }
}
