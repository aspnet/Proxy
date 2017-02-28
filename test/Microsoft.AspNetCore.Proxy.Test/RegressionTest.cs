// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Microsoft.AspNetCore.Proxy.Test
{
    public sealed class RegressionTest
    {
        [Fact]
        public async Task CookiesAreForwarded()
        {
            const string cookieKey = "mykey";
            const string cookieValue = "myvalue";

            using (var server = new WebHostBuilder()
                .UseKestrel()
                .Configure(app => app.Run(ctx =>
                {
                    if (ctx.Request.Cookies[cookieKey] != cookieValue)
                    {
                        ctx.Response.StatusCode = 400;
                    }
                    return Task.FromResult(0);
                })).Start("http://localhost:4004"))
            using (var proxy = new WebHostBuilder()
                .UseKestrel()
                .ConfigureServices(services => services.AddProxy())
                .Configure(app => app.RunProxy(new Uri("http://localhost:4004")))
                .Start("http://localhost:4005"))
#if NET46
            using (var client = new HttpClient(new WinHttpHandler()))
#else
            using (var client = new HttpClient())
#endif
            {
                var request = new HttpRequestMessage(HttpMethod.Post, "http://localhost:4005");
                request.Headers.Add("Cookie", $"{cookieKey}={cookieValue}");
                var response = await client.SendAsync(request);
                Assert.True(response.IsSuccessStatusCode);
            }
        }
    }
}
