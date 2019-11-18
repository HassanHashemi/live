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
    public enum InternalWebsocketMessageType
    {
        Hearbeat = 1,
        Json = 2,
        Binary = 3,
        Close = 4
    }
}
