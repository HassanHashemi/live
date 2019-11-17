using Microsoft.Extensions.Logging;
using System;
using System.Buffers;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace Live.Hub
{
    public sealed class SocketCollection
    {
        private readonly ConcurrentDictionary<ClientInfo, List<WebSocket>> _clients;
        private readonly ILogger<SocketCollection> _logger;

        public SocketCollection(ILogger<SocketCollection> logger)
        {
            this._clients = new ConcurrentDictionary<ClientInfo, List<WebSocket>>();
            this._logger = logger;
        }

        public async void Process(ClientInfo clientInfo, WebSocket socket)
        {
            if (clientInfo == null)
            {
                throw new ArgumentNullException(nameof(clientInfo));
            }

            if (socket == null)
            {
                throw new ArgumentNullException(nameof(socket));
            }

            if (socket.State != WebSocketState.Open)
            {
                throw new InvalidOperationException("invalid state");
            }

            if (!this._clients.ContainsKey(clientInfo))
            {
                _clients.TryAdd(clientInfo, new List<WebSocket>() { socket });
            }
            else
            {
                _clients[clientInfo].Add(socket);
            }

            LogConnection(clientInfo);

            try
            {
                await this.Read(clientInfo, socket);
            }
            finally
            {
                clientInfo.Complition.SetResult(null);
                _clients.TryRemove(clientInfo, out var _);

                LogDisconnect(clientInfo);
            }
        }

        private async Task Read(ClientInfo clientInfo, WebSocket client)
        {
            var buffer = ArrayPool<byte>.Shared.Rent(4 * 1024);

            while (client.State == WebSocketState.Open)
            {
                while (!clientInfo.CancellationToken.IsCancellationRequested)
                {
                    using var cts = new CancellationTokenSource();
                    cts.CancelAfter(3000);

                    var result = await client.ReceiveAsync(new ArraySegment<byte>(buffer), cts.Token);

                    while (!result.CloseStatus.HasValue)
                    {
                        await client.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);
                        break;
                    }
                }
            }
        }

        private void LogConnection(ClientInfo clientInfo)
        {
            _logger.LogInformation($"{clientInfo.UserId}:{clientInfo.MerchantId} Connected");
        }

        private void LogDisconnect(ClientInfo clientInfo)
        {
            _logger.LogInformation($"{clientInfo.UserId}:{clientInfo.MerchantId} Disconnected");
        }
    }
}
