// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Proxy;

namespace Microsoft.AspNetCore.Builder
{
    public static class ProxyExtensions
    {
        /// <summary>
        /// Sends request to the specified server
        /// </summary>
        /// <param name="app"></param>
        /// <param name="destinationUri">Destination Uri</param>
        public static void RunProxy(this IApplicationBuilder app, Uri destinationUri)
        {
            if (destinationUri == null)
            {
                throw new ArgumentNullException(nameof(destinationUri));
            }
            app.Run(ctx => ctx.ProxyRequest(destinationUri));
        }

        /// <summary>
        /// Sends request to the specified server
        /// </summary>
        /// <param name="context"></param>
        /// <param name="destinationUri">Destination Uri</param>
        public static async Task ProxyRequest(this HttpContext context, Uri destinationUri)
        {
            if (destinationUri == null)
            {
                throw new ArgumentNullException(nameof(destinationUri));
            }

            if (context.WebSockets.IsWebSocketRequest)
            {
                var wsDestinationUri = new Uri(ProxyUtils.TranslateUriToWebSocketScheme(destinationUri.ToString()));
                await ProxyUtils.ForwardWebSocketRequest(context, wsDestinationUri);
            }
            else
            {
                using (var requestMessage = ProxyUtils.PrepareHttpRequestMessage(context.Request, destinationUri))
                using (var responseMessage = await ProxyUtils.ForwardHttpRequestMessageToServer(context, requestMessage))
                {
                    await ProxyUtils.ForwardHttpResponseMessageToClient(responseMessage, context.Response);
                }
            }
        }
    }
}
