using System;
using System.Text;

namespace Live.Hub
{
    public class WebSocketMessage
    {
        private WebSocketMessage()
        {
        }

        public InternalWebsocketMessageType Type { get; set; }
        public object Data { get; set; }

        public byte[] ReadBinary()
        {
            throw new NotImplementedException();
        }

        public T ReadObject<T>()
        {
            throw new NotImplementedException();
        }

        public string ReadJson()
        {
            throw new NotImplementedException();
        }

        public static WebSocketMessage Close => new WebSocketMessage { Type = InternalWebsocketMessageType.Close };

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
