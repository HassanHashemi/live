using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Live.Hub
{
    public class WebSocketSocketMiddleware : IMiddleware
    {
        private readonly SocketServer _socketCollection;
        private readonly ILogger<WebSocketSocketMiddleware> _logger;

        public WebSocketSocketMiddleware(SocketServer socketCollection, ILogger<WebSocketSocketMiddleware> logger)
        {
            this._socketCollection = socketCollection;
            this._logger = logger;
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

            await _socketCollection.Process(clientInfo, socket, httpContext.RequestAborted);
        }
    }
}