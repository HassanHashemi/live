using System;

namespace Live.Hub
{
    public class HeartBeatEventArgs : EventArgs
    {
        public ClientInfo Client { get; set; }
    }
}