using System;

namespace Live.Hub
{
    public class ClientDisconnectedEventArgs : EventArgs
    {
        public ClientInfo Client { get; set; }
    }
}
