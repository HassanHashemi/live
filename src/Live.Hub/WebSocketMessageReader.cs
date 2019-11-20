using System;
using System.IO;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace Live.Hub
{
    public static class WebSocketMessageReader
    {
        public const int READ_TIME_OUT = 5000;
        public const int BUFFER_SIZE = 4 * 1024;

        public static Task<WebSocketMessage> Read(WebSocket socket)
        {
            return ReadInternal(socket);
        }

        private static async Task<WebSocketMessage> ReadInternal(WebSocket socket)
        {
            var buffer = WebSocket.CreateClientBuffer(BUFFER_SIZE, BUFFER_SIZE);

            using var cts = new CancellationTokenSource();
            cts.CancelAfter(READ_TIME_OUT);

            WebSocketReceiveResult result = null;

            using var stream = new MemoryStream();

            do
            {
                result = await socket.ReceiveAsync(buffer, cts.Token);

                if (result.CloseStatus.HasValue)
                {
                    return WebSocketMessage.Close((WebSocketCloseStatus)result.CloseStatus);
                }

                stream.Write(buffer.Array, 0, result.Count);

            } while (!result.EndOfMessage);

            return ParseData(result, stream.ToArray());
        }

        private static WebSocketMessage ParseData(WebSocketReceiveResult result, byte[] bytes)
        {
            if (bytes.IsHeartbeat())
            {
                return WebSocketMessage.Heartbeat();
            }

            return result.MessageType switch
            {
                WebSocketMessageType.Text => WebSocketMessage.Text(bytes),
                WebSocketMessageType.Binary => WebSocketMessage.Binary(bytes),
                _ => throw new InvalidOperationException("Invalid socket message type")
            };
        }
    }
}
