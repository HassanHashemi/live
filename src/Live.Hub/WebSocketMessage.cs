using System;
using System.Net.WebSockets;
using System.Text;

namespace Live.Hub
{
    public sealed class WebSocketMessage
    {
        private WebSocketMessage()
        {
        }

        public InternalWebsocketMessageType Type { get; private set; }
        public WebSocketCloseStatus? CloseStatus { get; private set; }
        public object Data { get; private set; }

        public byte[] ToBinary()
        {
            if (!(this.Data is byte[] bytes))
            {
                throw new InvalidOperationException("Data should be byte[]");
            }

            return bytes;
        }

        public string ToJson()
        {
            if (!(this.Data is string json))
            {
                throw new InvalidOperationException("Data should be string");
            }

            return json;
        }

        public static WebSocketMessage Close(WebSocketCloseStatus? status)
            => new WebSocketMessage
            {
                Type = InternalWebsocketMessageType.Close,
                CloseStatus = status
            };

        public static WebSocketMessage Heartbeat()
            => new WebSocketMessage
            {
                Data = null,
                Type = InternalWebsocketMessageType.Hearbeat
            };

        public static WebSocketMessage Binary(byte[] data)
            => new WebSocketMessage
            {
                Data = data,
                Type = InternalWebsocketMessageType.Binary
            };

        public static WebSocketMessage Text(byte[] data)
            => new WebSocketMessage
            {
                Data = Encoding.UTF8.GetString(data),
                Type = InternalWebsocketMessageType.Text
            };

        public bool IsEmpty
            => (Type == InternalWebsocketMessageType.Text || Type == InternalWebsocketMessageType.Binary)
                    &&
               Data == null;
    }
}
