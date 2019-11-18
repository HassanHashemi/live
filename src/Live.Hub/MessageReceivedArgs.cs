using Microsoft.Extensions.Logging;
using System;
using System.Buffers;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace Live.Hub
{

    public class MessageReceivedArgs<T> : EventArgs
    {
        public ClientInfo Client { get; set; }
        public T Message { get; set; }
    }
}
