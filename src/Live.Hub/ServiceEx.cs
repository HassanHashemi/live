using Live.Backplane;
using Microsoft.Extensions.DependencyInjection;

namespace Live.Hub
{
    public static class ServiceEx
    {
        public static void AddSocketServer(this IServiceCollection services)
        {
            services.AddSingleton<IBackplaine, RedisBackplaine>();
            services.AddHostedService<SocketServiceHost>();
            services.AddSingleton<SocketServer>();
            services.AddScoped<WebSocketSocketMiddleware>();
        }
    }
}
