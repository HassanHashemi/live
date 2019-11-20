using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using System;

namespace Live.Hub
{
    public static class AppExtensions
    {
        public static void UseWebsocketsInternal(this IApplicationBuilder app, IWebHostEnvironment env)
        {
            var options = new WebSocketOptions
            {
                KeepAliveInterval = TimeSpan.FromSeconds(10),
                ReceiveBufferSize = SocketServer.BUFFER_SIZE
            };

            if (!env.IsDevelopment())
            {
                options.AllowedOrigins.Add("mysatrapstage.com");
                options.AllowedOrigins.Add("mysatrap.com");
            }

            app.UseWebSockets(options);
            app.UseMiddleware<WebSocketSocketMiddleware>();
        }
    }
}
