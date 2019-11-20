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

        public InternalWebsocketMessageType Type { get; set; }
        public WebSocketCloseStatus? CloseStatus { get; set; }
        public object Data { get; set; }

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

        public static WebSocketMessage Close(WebSocketCloseStatus status)
            => new WebSocketMessage { Type = InternalWebsocketMessageType.Close, CloseStatus = status };

        public static WebSocketMessage Heartbeat()
        {
            return new WebSocketMessage
            {
                Data = null,
                Type = InternalWebsocketMessageType.Hearbeat
            };
        }

        public static WebSocketMessage Binary(byte[] data)
        {
            return new WebSocketMessage
            {
                Data = data,
                Type = InternalWebsocketMessageType.Binary
            };
        }

        public static WebSocketMessage Json(byte[] data)
        {
            return new WebSocketMessage
            {
                Data = Encoding.UTF8.GetString(data),
                Type = InternalWebsocketMessageType.Json
            };
        }
    }
}
