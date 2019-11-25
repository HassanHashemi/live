using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Live.Backplane
{
    public static class ServiceEx
    {
        public static void AddRedisBackplane(this IServiceCollection services, IConfiguration config)
        {
            services.Configure<RedisConfig>(o => config.GetSection("Redis").Bind(o));
            services.AddSingleton<IBackplaine, RedisBackplaine>();
        }
    }
}
