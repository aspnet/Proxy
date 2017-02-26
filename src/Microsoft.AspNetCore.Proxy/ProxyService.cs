// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Net.Http;

namespace Microsoft.AspNetCore.Proxy
{
    sealed class ProxyService
    {
        public ProxyService(SharedProxyOptions options)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            Options = options;
#if NET46
            Client = new HttpClient(options.MessageHandler ?? new WinHttpHandler { AutomaticRedirection = false });
#else
            Client = new HttpClient(options.MessageHandler ?? new HttpClientHandler { AllowAutoRedirect = false });
#endif
        }

        public SharedProxyOptions Options { get; private set; }
        public HttpClient Client { get; private set; }
    }
}
