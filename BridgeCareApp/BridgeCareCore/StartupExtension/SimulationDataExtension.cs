using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.FileSystem;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;

using AppliedResearchAssociates.iAM.Reporting.Interfaces.BAMSSummaryReport;
using AppliedResearchAssociates.iAM.Reporting.Services.BAMSSummaryReport.BridgeData;

using AppliedResearchAssociates.iAM.Reporting.Interfaces.PAMSSummaryReport;
using AppliedResearchAssociates.iAM.Reporting.Services.PAMSSummaryReport.PamsData;

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
            services.AddScoped<ISimulationOutputFileRepository, SimulationOutputFileRepository>();

            services.AddSingleton<SequentialWorkQueue>();
            services.AddHostedService<SequentialWorkBackgroundService>();
            services.AddScoped<ISimulationAnalysis, SimulationAnalysisService>();
            services.AddScoped<AttributeService>();

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
        }
    }
}
