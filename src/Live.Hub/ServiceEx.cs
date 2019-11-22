using Live.Backplane;
using Microsoft.Extensions.DependencyInjection;

namespace Live.Hub
{
    public static class ServiceEx
    {
        public static void AddSocketServer(this IServiceCollection services)
        {
            services.AddSingleton<SocketServer>();
            services.AddHostedService<SocketServiceHost>();
            services.AddScoped<WebSocketSocketMiddleware>();
            services.AddSingleton<IBackplaine, RedisBackplaine>();
        }
    }
}
