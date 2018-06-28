// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Net.Http;
using Microsoft.Extensions.Options;

namespace Microsoft.AspNetCore.Proxy
{
    public class ProxyService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly HttpClient _httpClient;

        public ProxyService(IOptions<SharedProxyOptions> options, IHttpClientFactory httpClientFactory)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            Options = options.Value;
            if (Options.MessageHandler != null)
            {
                _httpClient = new HttpClient(Options.MessageHandler);
            }
            else
            {
                _httpClientFactory = httpClientFactory;
            }
        }

        public SharedProxyOptions Options { get; private set; }
        internal HttpClient Client => _httpClientFactory?.CreateClient() ?? _httpClient;
    }
}
