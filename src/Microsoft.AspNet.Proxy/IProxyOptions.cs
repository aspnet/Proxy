using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.AspNet.Proxy
{
    interface IProxyOptions
    {
        string Scheme { get; set; }
        string Host { get; set; }
        string Port { get; set; }
    }
}
