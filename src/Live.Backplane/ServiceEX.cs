using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using System;

namespace Live.Backplane
{
    public static class ServiceEx
    {
        public static void AddRedisBackplane(this IServiceCollection services, string config)
        {
            if (string.IsNullOrEmpty(config))
            {
                throw new ArgumentNullException(nameof(config));
            }

            services.AddSingleton<IBackplaine, RedisBackplaine>();
            services.AddSingleton<IConnectionMultiplexer>(
                o => ConnectionMultiplexer.Connect(config));
        }
    }
}
