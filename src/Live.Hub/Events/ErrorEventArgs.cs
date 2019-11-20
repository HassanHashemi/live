using System;

namespace Live.Hub
{
    public class ErrorEventArgs : EventArgs
    {
        public ClientInfo Client { get; set; }
        public Exception Exception { get; set; }
    }
}
