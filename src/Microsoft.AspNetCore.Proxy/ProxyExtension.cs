// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Proxy;
using Microsoft.Extensions.Options;

namespace Microsoft.AspNetCore.Builder
{
    public static class ProxyExtension
    {
        /// <summary>
        /// Sends request to remote server determined by the route handler
        /// </summary>
        /// <param name="app"></param>
        /// <param name="routeHandler">Handler deciding where to route specific request</param>
        public static void RunProxy(this IApplicationBuilder app, Func<ProxyRoutingContext, Task> routeHandler) => RunProxy(app, new ProxyOptions
        {
            RouteHandler = routeHandler
        });

        /// <summary>
        /// Sends request to the specified server
        /// </summary>
        /// <param name="app"></param>
        /// <param name="scheme">Uri scheme</param>
        /// <param name="host">Uri host</param>
        /// <param name="path">Uri path</param>
        public static void RunProxy(this IApplicationBuilder app, string scheme, HostString host, PathString path = default(PathString))
        {
            if (scheme == null)
            {
                throw new ArgumentNullException(nameof(scheme));
            }
            RunProxy(app, ctx => ctx.ForwardRequestTo(scheme, host, path));
        }

        /// <summary>
        /// Sends request to remote server as specified in options
        /// </summary>
        /// <param name="app"></param>
        /// <param name="options">Options for setting port, host, and scheme</param>
        public static void RunProxy(this IApplicationBuilder app, ProxyOptions options)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            app.UseMiddleware<ProxyMiddleware>(Options.Create(options));
        }
    }
}
