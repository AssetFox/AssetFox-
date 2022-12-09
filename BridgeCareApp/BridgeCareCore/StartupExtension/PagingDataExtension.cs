using BridgeCareCore.Interfaces;
using BridgeCareCore.Interfaces.DefaultData;
using BridgeCareCore.Services;
using BridgeCareCore.Services.DefaultData;
using BridgeCareCore.Services.Paging;
using Microsoft.Extensions.DependencyInjection;

namespace BridgeCareCore.StartupExtension
{
    public static class PagingDataExtension
    {
        public static void AddPagingData(this IServiceCollection services)
        {
            services.AddScoped<IDeficientConditionGoalPagingService, DeficientConditionGoalPagingService>();
            services.AddScoped<ITargetConditionGoalPagingService, TargetConditionGoalPagingService>();
            services.AddScoped<IBudgetPriortyPagingService, BudgetPriortyPagingService>();
            services.AddScoped<ICalculatedAttributePagingService, CalculatedAttributePagingService>();
            services.AddScoped<ICommittedProjectPagingService, CommittedProjectPagingService>();
            services.AddScoped<IInvestmentPagingService, InvestmentPagingService>();
            services.AddScoped<IPerformanceCurvesPagingService, PerformanceCurvesPagingService>();
            services.AddScoped<IRemainingLifeLimitPagingService, RemainingLifeLimitPagingService>();
            services.AddScoped<ISimulationPagingService, SimulationPagingService>();
            services.AddScoped<ITreatmentPagingService, TreatmentPagingService>();
        }
    }
}
