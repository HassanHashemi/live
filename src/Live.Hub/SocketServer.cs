using Microsoft.Extensions.Logging;
using System;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace Live.Hub
{
    public class SocketServer
    {
        public const int BUFFER_SIZE = 4 * 1024;

        private readonly SocketCollection _clients;
        private readonly ILogger<SocketServer> _logger;

        public SocketServer(ILogger<SocketServer> logger)
        {
            _clients = new SocketCollection();
            _logger = logger;
        }

        public event EventHandler<MessageReceivedArgs<string>> JsonReceived;
        public event EventHandler<MessageReceivedArgs<byte[]>> BinaryReceived;
        public event EventHandler<ClientConnectedEventArgs> ClientConnected;
        public event EventHandler<ClientDisconnectedEventArgs> ClientDisconnected;

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

            Exception thrownException = null;

            try
            {
                await Read(clientInfo, socket, requestAborted);
            }
            catch (Exception ex)
            {
                thrownException = ex;
                _logger.LogError(ex.Message);
            }
            finally
            {
                Disconnect(clientInfo, socket);
            }

            if (thrownException != null)
            {
                throw thrownException;
            }
        }

        private void Disconnect(ClientInfo clientInfo, WebSocket socket)
        {
            OnClientDisconnect(new ClientDisconnectedEventArgs { Client = clientInfo });

            _clients.Remove(clientInfo, socket);

            LogDisconnect(clientInfo);
        }

        private void Connect(ClientInfo clientInfo, WebSocket socket)
        {
            OnClientConnected(new ClientConnectedEventArgs { Client = clientInfo });

            _clients.AddConnection(clientInfo, socket);

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
            while (client.State == WebSocketState.Open)
            {
                while (!requestAborted.IsCancellationRequested)
                {
                    var data = await WebSocketMessageReader.Read(client);

                    if (data.Type == InternalWebsocketMessageType.Hearbeat)
                    {
                        continue;
                    }

                    if (data.Type == InternalWebsocketMessageType.Close)
                    {
                        await client.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);
                        break;
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
