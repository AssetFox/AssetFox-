using BridgeCareCore.Interfaces.DefaultData;
using BridgeCareCore.Services.DefaultData;
using Microsoft.Extensions.DependencyInjection;

namespace BridgeCareCore.StartupExtension
{
    public static class DefaultDataExtension
    {
        public static void AddDefaultData(this IServiceCollection services)
        {
            services.AddScoped<IAnalysisDefaultDataService, AnalysisDefaultDataService>();
            services.AddScoped<IInvestmentDefaultDataService, InvestmentDefaultDataService>();
        }
    }
}
