using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace WebMonitorApp
{
    class DataProvider
    {
        //HERE SET API ADDRESS
        public static string URL;


        public static void SendData(string _data) {
            try
            {
                HttpWebRequest webRequest;

                //request body
                string requestParams = _data;

                webRequest = (HttpWebRequest)WebRequest.Create(URL);

                webRequest.Method = "POST";
                webRequest.ContentType = "application/json";

                byte[] byteArray = Encoding.UTF8.GetBytes(requestParams);
                webRequest.ContentLength = byteArray.Length;
                using (Stream requestStream = webRequest.GetRequestStream())
                {
                    requestStream.Write(byteArray, 0, byteArray.Length);
                }

                // Get the response.
                using (WebResponse response = webRequest.GetResponse())
                {
                    using (Stream responseStream = response.GetResponseStream())
                    {
                        StreamReader rdr = new StreamReader(responseStream, Encoding.UTF8);
                        string Json = rdr.ReadToEnd(); // response from server
                    }
                }
                Console.WriteLine("OK");
                System.IO.File.WriteAllText("logs.txt", "");
                LogsOperations._data = new List<LogModel>();
            }
            catch (Exception)
            {
                Console.WriteLine("REQUEST ERROR");
            }
        }
    }
}
