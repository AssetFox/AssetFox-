using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace BridgeCareCore
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((hostcontext, config) =>
                {
                    var env = hostcontext.HostingEnvironment;
#if MSSQLDEBUG
                    config.AddJsonFile("coreConnection.Development.json", optional: true, reloadOnChange: true);
#else
                    config.AddJsonFile("coreConnection.json", optional: true, reloadOnChange: true);
#endif
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
