using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Live.Hub
{
    public class SocketServer : IAsyncDisposable
    {
        public const int BUFFER_SIZE = 4 * 1024;

        private readonly ILogger<SocketServer> _logger;

        public SocketServer(ILogger<SocketServer> logger)
        {
            ConnectedClients = new SocketCollection();
            _logger = logger;
        }

        public event EventHandler<MessageReceivedArgs<string>> TextReceived;
        public event EventHandler<MessageReceivedArgs<byte[]>> BinaryReceived;
        public event EventHandler<ClientConnectedEventArgs> ClientConnected;
        public event EventHandler<ClientDisconnectedEventArgs> ClientDisconnected;
        public event EventHandler<ErrorEventArgs> ClientError;
        public event EventHandler<HeartBeatEventArgs> ClientHeartBeat;

        public SocketCollection ConnectedClients { get; }

        public async Task Process(ClientInfo clientInfo, WebSocket socket, CancellationToken requestAborted)
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

            Connect(clientInfo, socket);

            try
            {
                await Read(clientInfo, socket, requestAborted);
            }
            catch (Exception ex)
            {
                OnClientError(new ErrorEventArgs { Client = clientInfo, Exception = ex });
            }
            finally
            {
                Disconnect(clientInfo, socket);
            }
        }

        public Task SendJson(string userId, object message, CancellationToken cancellationToken = default)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            return SendString(userId, JsonSerializer.Serialize(message), cancellationToken);
        }

        public Task SendString(string userId, string message, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(message))
            {
                throw new ArgumentNullException(nameof(message));
            }

            return SendBytes(userId, Encoding.UTF8.GetBytes(message), WebSocketMessageType.Text, cancellationToken);
        }

        public Task SendBytes(string userId, byte[] message, WebSocketMessageType type = WebSocketMessageType.Binary, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(userId))
            {
                throw new ArgumentNullException(nameof(userId));
            }

            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            var sockets = this.ConnectedClients.GetConnections(userId);

            return Send(sockets, message, type, cancellationToken);
        }

        private async Task Send(IEnumerable<WebSocket> sockets, byte[] message, WebSocketMessageType type, CancellationToken cancellationToken)
        {
            var tasks = new List<Task>();

            foreach (var connection in sockets)
            {
                var sendTask = SendBytes(connection, message, type, cancellationToken);

                tasks.Add(sendTask);
            }

            try
            {
                await Task.WhenAll(tasks);
            }
            catch
            { }

            Task.WaitAll(tasks.ToArray());
        }

        protected virtual void OnClientError(ErrorEventArgs e)
        {
            var handlers = ClientError;
            handlers?.Invoke(this, e);

            _logger.LogError(e.Exception.Message);
        }

        private Task SendBytes(WebSocket socket, byte[] data, WebSocketMessageType type, CancellationToken cancellationToken)
        {
            var dataToSend = new ArraySegment<byte>(data, 0, data.Length);

            return socket.SendAsync(dataToSend, type, true, cancellationToken);
        }

        private void Disconnect(ClientInfo clientInfo, WebSocket socket)
        {
            OnClientDisconnect(new ClientDisconnectedEventArgs { Client = clientInfo });

            ConnectedClients.Remove(clientInfo, socket);
            LogDisconnect(clientInfo);
        }

        private void Connect(ClientInfo clientInfo, WebSocket socket)
        {
            OnClientConnected(new ClientConnectedEventArgs { Client = clientInfo });

            ConnectedClients.AddConnection(clientInfo, socket);
            LogConnection(clientInfo);
        }

        protected virtual void OnClientDisconnect(ClientDisconnectedEventArgs e)
        {
            var handlers = this.ClientDisconnected;
            handlers?.Invoke(this, e);
        }

        protected virtual void OnClientConnected(ClientConnectedEventArgs e)
        {
            var handlers = this.ClientConnected;
            handlers?.Invoke(this, e);
        }

        private async Task Read(ClientInfo clientInfo, WebSocket client, CancellationToken requestAborted)
        {
            while (client.State == WebSocketState.Open && !requestAborted.IsCancellationRequested)
            {
                var data = await WebSocketMessageReader.Read(client);

                if (data.Type == InternalWebsocketMessageType.Hearbeat)
                {
                    this.OnHeartBeat(new HeartBeatEventArgs { Client = clientInfo });
                    continue;
                }

                if (data.Type == InternalWebsocketMessageType.Close)
                {
                    await client.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);
                    break;
                }

                this.RaiseMessageEvents(clientInfo, data);
            }
        }

        protected virtual void OnHeartBeat(HeartBeatEventArgs e)
        {
            var handlers = this.ClientHeartBeat;
            handlers?.Invoke(this, e);
        }

        private void RaiseMessageEvents(ClientInfo client, WebSocketMessage message)
        {
            if (message.Type == InternalWebsocketMessageType.Binary)
            {
                this.OnBinaryReceived(new MessageReceivedArgs<byte[]>
                {
                    Client = client,
                    Message = message.ToBinary()
                });
            }
            else if (message.Type == InternalWebsocketMessageType.Text)
            {
                this.OnJsonReceived(new MessageReceivedArgs<string>
                {
                    Client = client,
                    Message = message.ToJson()
                });
            }
            else
            {
                throw new InvalidOperationException("Invlaid message type");
            }
        }

        protected virtual void OnJsonReceived(MessageReceivedArgs<string> e)
        {
            var handler = this.TextReceived;

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

        public async ValueTask DisposeAsync()
        {
            using var cts = new CancellationTokenSource();
            cts.CancelAfter(4000);

            foreach (var socket in this.ConnectedClients.SelectMany(c => c.Value))
            {
                try
                {
                    await socket.CloseAsync(WebSocketCloseStatus.NormalClosure,
                        "Server shutdown",
                        cts.Token);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message);
                }
            }
        }
    }
}
