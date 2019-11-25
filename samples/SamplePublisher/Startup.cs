using System;
using System.Collections.Generic;
using System.Linq;
using Live.Backplane;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using StackExchange.Redis;

namespace SamplePublisher
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<PublishMiddleWare>();
            services.AddRedisBackplane("localhost");
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseMiddleware<PublishMiddleWare>();
        }
    }
}
