using System;

namespace Live.Hub
{
    public class MessageReceivedArgs<T> : EventArgs
    {
        public ClientInfo Client { get; set; }
        public T Message { get; set; }
    }
}
