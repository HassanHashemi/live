using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace Live.Hub
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseKestrel((o, c) =>
                    {
                        if (o.HostingEnvironment.IsDevelopment())
                        {
                            c.ListenAnyIP(5000);
                        }
                        else
                        {
                            c.ListenAnyIP(80);
                        }
                    });

                    webBuilder.UseStartup<Startup>();
                });
    }
}
