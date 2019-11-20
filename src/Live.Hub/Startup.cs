using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace Live.Hub
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHostedService<TestService>();
            services.AddSingleton<SocketServer>();
            services.AddScoped<WebSocketSocketMiddleware>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseExceptionHandler();
            app.UseWebsocketsInternal(env);
        }
    }
}
