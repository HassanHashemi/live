using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Live.Hub
{
    public class WebSocketSocketMiddleware
    {
        private readonly SocketCollection _socketCollection;
        private readonly ILogger<WebSocketSocketMiddleware> _logger;

        public WebSocketSocketMiddleware(SocketCollection socketCollection, ILogger<WebSocketSocketMiddleware> logger)
        {
            this._socketCollection = socketCollection;
            this._logger = logger;
        }

        public async Task InvokeAsync(HttpContext httpContext)
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
            var clientInfo = new ClientInfo(merchantId, userId, httpContext.RequestAborted);

            try
            {
                await _socketCollection.Process(clientInfo, socket);
            }
            catch (Exception)
            {
                _logger.LogInformation("client disconnected");
            }
        }
    }
}
