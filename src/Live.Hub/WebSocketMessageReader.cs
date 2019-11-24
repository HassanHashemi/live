using System;
using System.IO;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using static Live.Hub.WebSocketMessage;

namespace Live.Hub
{
    public static class WebSocketMessageReader
    {
        public const int READ_TIME_OUT = 5000;
        public const int BUFFER_SIZE = 4 * 1024;

        public static async Task<WebSocketMessage> Read(WebSocket socket)
        {
            var buffer = WebSocket.CreateServerBuffer(BUFFER_SIZE);

            using var cts = new CancellationTokenSource(READ_TIME_OUT);

            WebSocketReceiveResult result = null;

            using var stream = new MemoryStream();

            do
            {
                result = await socket.ReceiveAsync(buffer, cts.Token);

                if (result.CloseStatus.HasValue)
                {
                    return Close(result.CloseStatus);
                }

                stream.Write(buffer.Array, 0, result.Count);

            } while (!result.EndOfMessage);

            return ParseData(result, stream.ToArray());
        }

        private static WebSocketMessage ParseData(WebSocketReceiveResult result, byte[] bytes)
            => bytes.IsHeartbeat() ? Heartbeat() : result.MessageType switch
            {
                WebSocketMessageType.Text => Text(bytes),
                WebSocketMessageType.Binary => Binary(bytes),
                _ => throw new InvalidOperationException("Invalid socket message type")
            };
    }
}
