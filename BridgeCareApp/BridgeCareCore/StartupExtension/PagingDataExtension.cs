using BridgeCareCore.Interfaces;
using BridgeCareCore.Interfaces.DefaultData;
using BridgeCareCore.Services;
using BridgeCareCore.Services.DefaultData;
using Microsoft.Extensions.DependencyInjection;

namespace BridgeCareCore.StartupExtension
{
    public static class PagingDataExtension
    {
        public static void AddPagingData(this IServiceCollection services)
        {
            services.AddScoped<IDeficientConditionGoalPagingService, DeficientConditionGoalPagingService>();
        }
    }
}
