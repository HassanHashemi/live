using System;

namespace Live.Hub
{
    public class ClientConnectedEventArgs : EventArgs
    {
        public ClientInfo Client { get; set; }
    }
}
