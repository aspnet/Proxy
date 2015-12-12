using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Microsoft.AspNet.Proxy
{
    /// <summary>
    /// Options to configure multi domain to multi proxy host
    /// </summary>
    public class MultiProxyDictionaryOptions : Dictionary<string, MultiProxyOptionsValue>
    {
        public MultiProxyDictionaryOptions(ProxyOptions defaultOptions)
        {
            DefaultOptions = defaultOptions;
        }
        public HttpMessageHandler BackChannelMessageHandler { get; set; }

        public ProxyOptions DefaultOptions { get; set; }
        
    }


}
