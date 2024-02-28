using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebCrawlerProxy.Entidades
{
    public class ProxyModel
    {
        public string IP { get; set; }
        public string Port { get; set; }
        public string Country { get; set; }
        public string Protocol { get; set; }
        public string Type { get; set; }
        public string Anonymity {get;set;}
        public string LastSeen {get;set;}
    }
}
