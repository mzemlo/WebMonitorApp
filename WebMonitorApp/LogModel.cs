using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebMonitorApp
{
    class LogModel
    {
        public string UserName { get; set; }
        public string URL { get; set; }
        public DateTime From { get; set; }
        public DateTime To { get; set; }
    }
}
