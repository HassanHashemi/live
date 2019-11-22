using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
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
                .ConfigureLogging((o, c) =>
                {
                    if (o.HostingEnvironment.IsDevelopment())
                    {
                        c.AddConsole();
                    }
                })
                .ConfigureKestrel((o, c) =>
                {
                    if (o.HostingEnvironment.IsDevelopment())
                    {
                        c.ListenLocalhost(5000);
                    }
                    else
                    {
                        c.ListenAnyIP(80);
                    }
                });
    }
}