using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Live.Hub
{
    public class SatrapSocketServer
    {
        private readonly SocketCollection _socketCollection;

        public SatrapSocketServer(SocketCollection socketCollection)
        {
            this._socketCollection = socketCollection;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            if (httpContext.WebSockets.IsWebSocketRequest)
            {
                var merchantId = httpContext.Request.Query["merchantId"];
                var userId = httpContext.Request.Query["userId"];

                if (string.IsNullOrEmpty(merchantId))
                {
                    return;
                }

                if (string.IsNullOrEmpty(userId))
                {
                    return;
                }

                var socket = await httpContext.WebSockets.AcceptWebSocketAsync();
                var tcs = new TaskCompletionSource<object>();

                var clientInfo = new ClientInfo(merchantId, userId, tcs, httpContext.RequestAborted);
                _socketCollection.Process(clientInfo, socket);

                await tcs.Task;
            }
            else
            {
                // TODO: return 404
            }
        }
    }
}
