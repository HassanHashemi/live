using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Live.Hub
{
    public class WebSocketSocketMiddleware : IMiddleware
    {
        private readonly SocketServer _socketServer;

        public WebSocketSocketMiddleware(SocketServer socketCollection)
        {
            this._socketServer = socketCollection;
        }

        public async Task InvokeAsync(HttpContext httpContext, RequestDelegate _)
        {
            if (httpContext.Request.Path != "/ws")
            {
                httpContext.Response.StatusCode = 400;
                return;
            }

            if (!httpContext.WebSockets.IsWebSocketRequest)
            {
                httpContext.Response.StatusCode = 400;
                return;
            }

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
            var clientInfo = new ClientInfo(merchantId, userId);

            await _socketServer.Process(clientInfo, socket, httpContext.RequestAborted);
        }
    }
}