// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Http;

namespace Microsoft.AspNet.Proxy
{
    public class ProxyMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly HttpClient _httpClient;
        private readonly ProxyOptions _options;
        MultiProxyDictionaryOptions _optionsDict;
        public ProxyMiddleware(RequestDelegate next, ProxyOptions options)
        {
            if (next == null)
            {
                throw new ArgumentNullException(nameof(next));
            }

            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            _next = next;
            if (string.IsNullOrEmpty(options.Host))
            {
                throw new ArgumentException("Options parameter must specify host.", "options");
            }

            // Setting default Port and Scheme if not specified
            if (string.IsNullOrEmpty(options.Port))
            {
                if (string.Equals(options.Scheme, "https", StringComparison.OrdinalIgnoreCase))
                {
                    options.Port = "443";
                }
                else
                {
                    options.Port = "80";
                }

            }

            if (string.IsNullOrEmpty(options.Scheme))
            {
                options.Scheme = "http";
            }

            _options = options;

            _httpClient = new HttpClient(_options.BackChannelMessageHandler ?? new HttpClientHandler());
        }
        public ProxyMiddleware(RequestDelegate next, MultiProxyDictionaryOptions options)
        {
            if (next == null)
            {
                throw new ArgumentNullException(nameof(next));
            }

            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            _next = next;
            if (options.Values.Any(c=> string.IsNullOrEmpty(c.Host)))
            {
                throw new ArgumentException("Options parameter must specify host.", "options");
            }

            // Setting default Port and Scheme if not specified
            var defaultPortList = options.Values;
            foreach (var item in defaultPortList)
            {
                if (string.IsNullOrWhiteSpace(item.Port))
                {
                    if (string.Equals(item.Scheme, "https", StringComparison.OrdinalIgnoreCase))
                    {
                        item.Port = "443";
                    }
                    else
                    {
                        item.Port = "80";
                    }
                }
                if (string.IsNullOrEmpty(item.Scheme))
                {
                    item.Scheme = "http";
                }
            }


            _optionsDict = options;

            _options = null;//options;

            _httpClient = new HttpClient(_options.BackChannelMessageHandler ?? new HttpClientHandler());
        }
        public async Task Invoke(HttpContext context)
        {
            var requestMessage = new HttpRequestMessage();
            if (!string.Equals(context.Request.Method, "GET", StringComparison.OrdinalIgnoreCase) &&
                !string.Equals(context.Request.Method, "HEAD", StringComparison.OrdinalIgnoreCase) &&
                !string.Equals(context.Request.Method, "DELETE", StringComparison.OrdinalIgnoreCase) &&
                !string.Equals(context.Request.Method, "TRACE", StringComparison.OrdinalIgnoreCase))
            {
                var streamContent = new StreamContent(context.Request.Body);
                requestMessage.Content = streamContent;
            }

            // Copy the request headers
            foreach (var header in context.Request.Headers)
            {
                if (!requestMessage.Headers.TryAddWithoutValidation(header.Key, header.Value.ToArray()) && requestMessage.Content != null)
                {
                    requestMessage.Content?.Headers.TryAddWithoutValidation(header.Key, header.Value.ToArray());
                }
            }
            IProxyOptions options = _options;
            if (options == null)
            {
                if (_optionsDict.ContainsKey(context.Request.Host.Value))
                {
                    options = _optionsDict[context.Request.Host.Value];
                }
                else
                {
                    options = _optionsDict.DefaultOptions;
                }
            }
            requestMessage.Headers.Host = options.Host + ":" + options.Port;
            var uriString = $"{options.Scheme}://{options.Host}:{options.Port}{context.Request.PathBase}{context.Request.Path}{context.Request.QueryString}";
            requestMessage.RequestUri = new Uri(uriString);

            requestMessage.Method = new HttpMethod(context.Request.Method);
            using (var responseMessage = await _httpClient.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, context.RequestAborted))
            {
                context.Response.StatusCode = (int)responseMessage.StatusCode;
                foreach (var header in responseMessage.Headers)
                {
                    context.Response.Headers[header.Key] = header.Value.ToArray();
                }

                foreach (var header in responseMessage.Content.Headers)
                {
                    context.Response.Headers[header.Key] = header.Value.ToArray();
                }

                // SendAsync removes chunking from the response. This removes the header so it doesn't expect a chunked response.
                context.Response.Headers.Remove("transfer-encoding");
                await responseMessage.Content.CopyToAsync(context.Response.Body);
            }
        }
    }
}