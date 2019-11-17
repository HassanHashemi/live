using System;

namespace Live.Backplane
{
    public class BackplaneMessageReceivedArgs : EventArgs
    {
        public BackplaneMessage Message { get; set; }
    }
}
