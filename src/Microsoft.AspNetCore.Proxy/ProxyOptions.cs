// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Microsoft.AspNetCore.Builder
{
    /// <summary>
    /// Options to configure Proxy
    /// </summary>
    public class ProxyOptions
    {
        private int? _webSocketBufferSize;

        public HttpMessageHandler BackChannelMessageHandler { get; set; }
        public TimeSpan? WebSocketKeepAliveInterval { get; set; }
        public int? WebSocketBufferSize
        {
            get => _webSocketBufferSize;
            set
            {
                if (value.HasValue && value.Value <= 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(value));
                }
                _webSocketBufferSize = value;
            }
        }

        public Func<ProxyRoutingContext, Task> RouteHandler { get; set; }
    }
}
