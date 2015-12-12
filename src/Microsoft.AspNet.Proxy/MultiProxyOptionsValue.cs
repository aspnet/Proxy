using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.AspNet.Proxy
{
    public class MultiProxyOptionsValue : IProxyOptions
    {
        public string Scheme { get; set; }
        public string Host { get; set; }
        public string Port { get; set; }
    }
}
