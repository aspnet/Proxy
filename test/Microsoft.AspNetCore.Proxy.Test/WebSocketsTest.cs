using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.TestHost;
using Xunit;

namespace Microsoft.AspNetCore.Proxy.Test
{
    public class WebSocketsTest
    {
        private static async Task<string> ReceiveTextMessage(WebSocket socket, int maxLen = 4096)
        {
            var recvBuff = new byte[maxLen];
            var received = 0;
            while (true)
            {
                var res = await socket.ReceiveAsync(new ArraySegment<byte>(recvBuff, received, maxLen - received), CancellationToken.None);
                Assert.Equal(WebSocketMessageType.Text, res.MessageType);
                received += res.Count;
                if (res.EndOfMessage)
                    return Encoding.UTF8.GetString(recvBuff, 0, received);
                Assert.InRange(received, 0, maxLen);
            }
        }

        [Fact]
        public async Task SmokeTest1()
        {
            var server = new WebHostBuilder()
                .UseKestrel()
                .Configure(app => app.UseWebSockets().Run(async ctx =>
                {
                    var socket = await ctx.WebSockets.AcceptWebSocketAsync("myproto2");
                    var message1 = await ReceiveTextMessage(socket);
                    var message2 = await ReceiveTextMessage(socket);
                    Assert.Equal("TEST MESSAGE 1", message1);
                    Assert.Equal("TEST MSG 2", message2);
                    await socket.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes("TEST MESSAGE 3")), WebSocketMessageType.Text, true, CancellationToken.None);
                    await socket.CloseOutputAsync(WebSocketCloseStatus.NormalClosure, "My Status1", CancellationToken.None);
                })).Start("http://localhost:4001");

            var proxy = new WebHostBuilder()
                .UseKestrel()
                .Configure(app => app.UseWebSockets().RunProxy(new ProxyOptions { Scheme = "http", Host = "localhost", Port = "4001" })).Start("http://localhost:4002");

            using (var client = new ClientWebSocket())
            {
                client.Options.AddSubProtocol("myproto1");
                client.Options.AddSubProtocol("myproto2");
                await client.ConnectAsync(new Uri("ws://localhost:4002"), CancellationToken.None);
                Assert.Equal("myproto2", client.SubProtocol);
                await client.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes("TEST MESSAGE 1")), WebSocketMessageType.Text, true, CancellationToken.None);
                await client.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes("TEST MSG 2")), WebSocketMessageType.Text, true, CancellationToken.None);
                var message3 = await ReceiveTextMessage(client);
                Assert.Equal("TEST MESSAGE 3", message3);
                var recv = await client.ReceiveAsync(new ArraySegment<byte>(new byte[4096]), CancellationToken.None);
                Assert.Equal(WebSocketMessageType.Close, recv.MessageType);
                Assert.Equal(WebSocketCloseStatus.NormalClosure, recv.CloseStatus);
                Assert.Equal("My Status1", recv.CloseStatusDescription);
                await client.CloseOutputAsync(WebSocketCloseStatus.NormalClosure, "My Status2", CancellationToken.None);
            }

            server.Dispose();
            proxy.Dispose();
        }
    }
}
