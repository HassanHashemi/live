using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace Live.Hub
{
    public class SocketCollection
    {
        private readonly ConcurrentDictionary<ClientInfo, List<WebSocket>> _clients;
        private readonly ILogger<SocketCollection> _logger;

        public SocketCollection(ILogger<SocketCollection> logger)
        {
            this._clients = new ConcurrentDictionary<ClientInfo, List<WebSocket>>();
            this._logger = logger;
        }

        public event EventHandler<MessageReceivedArgs<string>> JsonReceived;
        public event EventHandler<MessageReceivedArgs<byte[]>> BinaryReceived;

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
                clientInfo.Complition.SetResult(null);
            }
            catch (Exception ex)
            {
                clientInfo.Complition.SetException(ex);
            }
            finally
            {
                _clients.TryRemove(clientInfo, out var _);

                LogDisconnect(clientInfo);
            }
        }

        private async Task Read(ClientInfo clientInfo, WebSocket client)
        {
            while (client.State == WebSocketState.Open)
            {
                while (!clientInfo.CancellationToken.IsCancellationRequested)
                {
                    var data = await WebSocketMessageReader.Read(client);

                    if (data.Type == InternalWebsocketMessageType.Hearbeat)
                    {
                        continue;
                    }

                    if (data.Type == InternalWebsocketMessageType.Close)
                    {
                        await client.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);
                    }

                    this.RaiseEvents(clientInfo, data);
                }
            }
        }

        private void RaiseEvents(ClientInfo client, WebSocketMessage message)
        {
            if (message.Type == InternalWebsocketMessageType.Binary)
            {
                this.OnBinaryReceived(new MessageReceivedArgs<byte[]>
                {
                    Client = client,
                    Message = message.ToBinary()
                });
            }
            else if (message.Type == InternalWebsocketMessageType.Json)
            {
                this.OnJsonReceived(new MessageReceivedArgs<string>
                {
                    Client = client,
                    Message = message.ToJson()
                });
            }

            throw new InvalidOperationException("Invlaid message type");
        }

        protected virtual void OnJsonReceived(MessageReceivedArgs<string> e)
        {
            var handler = this.JsonReceived;

            handler?.Invoke(this, e);
        }

        protected virtual void OnBinaryReceived(MessageReceivedArgs<byte[]> e)
        {
            var handler = this.BinaryReceived;

            handler?.Invoke(this, e);
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
