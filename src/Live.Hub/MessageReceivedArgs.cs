using System;

namespace Live.Hub
{
    public class ClientConnectedEventArgs : EventArgs
    {
        public ClientInfo Client { get; set; }
    }

    public class ClientDisconnectedEventArgs : EventArgs
    {
        public ClientInfo Client { get; set; }
    }

    public class MessageReceivedArgs<T> : EventArgs
    {
        public ClientInfo Client { get; set; }
        public T Message { get; set; }
    }
}
