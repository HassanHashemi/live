using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Live.Hub
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder<Startup>(args)
                .ConfigureLogging((context, log) =>
                {
                    log.ClearProviders();

                    if (context.HostingEnvironment.IsDevelopment())
                    {
                        log.AddConsole();
                    }
                })
            .ConfigureAppConfiguration((o, c) =>
            {
                c.AddJsonFile("appsettings.Development.json", false);
            })
                .ConfigureKestrel((context, kestrel) =>
                {
                    if (context.HostingEnvironment.IsDevelopment())
                    {
                        kestrel.ListenLocalhost(5000);
                    }
                    else
                    {
                        kestrel.ListenAnyIP(80);
                    }
                });
    }
}