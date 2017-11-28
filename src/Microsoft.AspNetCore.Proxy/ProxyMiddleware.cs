// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;

namespace Microsoft.AspNetCore.Proxy
{
    /// <summary>
    /// Proxy Middleware
    /// </summary>
    public class ProxyMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ProxyService _proxyService;

        public ProxyMiddleware(RequestDelegate next, ProxyService proxyService)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
            _proxyService = proxyService ?? throw new ArgumentNullException(nameof(proxyService));
        }

        public async Task Invoke(HttpContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var destination = await _proxyService.Options.GetProxyOptions(context.Request);
            var uri = new Uri(UriHelper.BuildAbsolute(destination.Scheme, destination.Host, destination.PathBase, context.Request.Path, context.Request.QueryString.Add(destination.AppendQuery)));
            await context.ProxyRequest(uri);
        }
    }
}
