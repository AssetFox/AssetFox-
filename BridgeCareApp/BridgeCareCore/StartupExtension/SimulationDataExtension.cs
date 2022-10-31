using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.FileSystem;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using BridgeCareCore.Interfaces;
using BridgeCareCore.Services;
using BridgeCareCore.Services.Treatment;
using Microsoft.Extensions.DependencyInjection;

namespace BridgeCareCore.StartupExtension
{
    public static class SimulationDataExtension
    {
        public static void AddSimulationData(this IServiceCollection services)
        {
            services.AddScoped<IAttributeMetaDataRepository, AttributeMetaDataRepository>();

            services.AddScoped<ISimulationOutputFileRepository, SimulationOutputFileRepository>();

            services.AddSingleton<SequentialWorkQueue>();
            services.AddHostedService<SequentialWorkBackgroundService>();
            services.AddScoped<ISimulationAnalysis, SimulationAnalysisService>();
            services.AddScoped<AttributeService>();
            services.AddScoped<AttributeImportService>();
            services.AddScoped<IExcelRawDataImportService, ExcelRawDataImportService>();
            services.AddScoped<IExpressionValidationService, ExpressionValidationService>();
            services.AddScoped<IUserCriteriaRepository, UserCriteriaRepository>();
            services.AddScoped<IMaintainableAssetRepository, MaintainableAssetRepository>();
            services.AddScoped<IInvestmentBudgetsService, InvestmentBudgetsService>();
            services.AddScoped<IPerformanceCurvesService, PerformanceCurvesService>();
            services.AddScoped<ITreatmentService, TreatmentService>();
            services.AddScoped<ICommittedProjectService, CommittedProjectService>();
            services.AddScoped<ExcelTreatmentLoader>();
            services.AddScoped<IUnitOfWork, UnitOfDataPersistenceWork>();
            services.AddScoped<UnitOfDataPersistenceWork>();
            services.AddScoped<ISimulationRepository, SimulationRepository>();
            services.AddScoped<ISimulationService,SimulationService>();
            services.AddScoped<ISimulationQueueService, SimulationQueueService>();
            services.AddScoped<ICalculatedAttributeService, CalculatedAttributeService>();
            services.AddScoped<IBudgetPriortyService, BudgetPriortyService>();
            services.AddScoped<ITargetConditionGoalService, TargetConditionGoalService>();
            services.AddScoped<IRemainingLifeLimitService, RemainingLifeLimitService>();
            services.AddScoped<ICashFlowService, CashFlowService>();
        }
    }
}
