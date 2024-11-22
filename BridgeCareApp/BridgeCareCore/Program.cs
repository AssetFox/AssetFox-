using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace BridgeCareCore
{
    public class Program
    {
        public static void Main(string[] args) => CreateHostBuilder(args).Build().Run();

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((hostContext, config) =>
                {
                    config.AddJsonFile("appsettings.json", true, true);
                    config.AddJsonFile("esec.json", true, true);
                    config.AddJsonFile("b2c.json", true, true);
#if MsSqlDebug
                    config.AddJsonFile("coreConnection.Development.json", true, true);
#elif Release
                    config.AddJsonFile("coreConnection.json", true, true);
#endif
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
#if MsSqlDebug
                    webBuilder.UseStartup<Startup>().UseKestrel(options => { options.Limits.MaxRequestBodySize = null; });
#elif Release
                    webBuilder.UseStartup<Startup>();
#endif
                });
    }
}
