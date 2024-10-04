using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.FileSystem;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.WorkQueue;
using BridgeCareCore.Interfaces;
using BridgeCareCore.Models;
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

            services.AddSingleton<SequentialWorkQueue<WorkQueueMetadata>>();
            services.AddSingleton<FastSequentialworkQueue<WorkQueueMetadata>>();
            services.AddHostedService<SequentialWorkBackgroundService>();
            services.AddHostedService<FastSequentialWorkBackgroundService>();
            services.AddHostedService<AttributeValueCacheBuildLaunchingService>();
            //services.AddHostedService<AggregatedSelectValuesResultDtoCacheService>();

            services.AddScoped<IGeneralWorkQueueService, GeneralWorkQueueService>();
            services.AddScoped<AttributeService>();
            services.AddScoped<IExcelRawDataImportService, ExcelRawDataImportService>();
            services.AddScoped<IExpressionValidationService, ExpressionValidationService>();
            services.AddScoped<IUserCriteriaRepository, UserCriteriaRepository>();
            services.AddScoped<IMaintainableAssetRepository, MaintainableAssetRepository>();
            services.AddScoped<IInvestmentBudgetsService, InvestmentBudgetsService>();
            services.AddScoped<IPerformanceCurvesService, PerformanceCurvesService>();
            services.AddScoped<ITreatmentService, TreatmentService>();
            services.AddScoped<ICommittedProjectService, CommittedProjectService>();
            services.AddScoped<ExcelTreatmentLoader>();
            services.AddScoped<UnitOfDataPersistenceWork>();
            services.AddScoped<IUnitOfWork>(x => x.GetRequiredService<UnitOfDataPersistenceWork>());

            services.AddScoped<ISimulationRepository, SimulationRepository>();
        }
    }
}
