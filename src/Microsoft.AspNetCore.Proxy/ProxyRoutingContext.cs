// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Proxy;

namespace Microsoft.AspNetCore.Builder
{
    /// <summary>
    /// Context class to facilitate routing decisions
    /// </summary>
    public sealed class ProxyRoutingContext
    {
        private readonly ProxyMiddleware _middleware;

        /// <summary>
        /// Original HttpContext
        /// </summary>
        public HttpContext Context { get; private set; }

        internal ProxyRoutingContext(ProxyMiddleware middleware, HttpContext context)
        {
            _middleware = middleware ?? throw new ArgumentNullException(nameof(middleware));
            Context = context ?? throw new ArgumentNullException(nameof(context));
        }

        /// <summary>
        /// Sends request to the specified server
        /// </summary>
        /// <param name="scheme">Uri scheme</param>
        /// <param name="host">Uri host</param>
        /// <param name="path">Uri path</param>
        /// <param name="query">Uri query string to merge with arguments from request</param>
        /// <returns></returns>
        public Task ForwardRequestTo(string scheme, HostString host, PathString path = default(PathString), QueryString query = default(QueryString))
        {
            if (scheme == null)
            {
                throw new ArgumentNullException(nameof(scheme));
            }
            return _middleware.ForwardRequestTo(Context, scheme, host, path, query);
        }
    }
}
