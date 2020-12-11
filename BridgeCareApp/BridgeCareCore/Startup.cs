using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.FileSystem;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Extensions;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQLLegacy;
using BridgeCareCore.Hubs;
using BridgeCareCore.Interfaces.SummaryReport;
using BridgeCareCore.Services.SummaryReport;
using BridgeCareCore.Services.SummaryReport.BridgeData;
using BridgeCareCore.Services.SummaryReport.UnfundedRecommendations;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using LiteDb = AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.LiteDb;
using FileSystemRepository = AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.FileSystem;
using BridgeCareCore.Services.SummaryReport.BridgeWorkSummary;
using BridgeCareCore.Services.SummaryReport.BridgeWorkSummaryByBudget;
using BridgeCareCore.Services.SummaryReport.ShortNameGlossary;
using BridgeCareCore.Services.SummaryReport.GraphTabs;
using BridgeCareCore.Services.SummaryReport.GraphTabs.NHSConditionCharts;
using BridgeCareCore.Services.SummaryReport.Parameters;

namespace BridgeCareCore
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton(Configuration);

            services.AddControllers().AddNewtonsoftJson();

            services.AddScoped<IAttributeMetaDataRepository, AttributeMetaDataRepository>();
            services.AddScoped<ISimulationOutputFileRepository, SimulationOutputFileRepository>();
            services.AddScoped<ISummaryReportGenerator, SummaryReportGenerator>();
            services.AddScoped<IExcelHelper, ExcelHelper>();
            services.AddScoped<IBridgeDataForSummaryReport, BridgeDataForSummaryReport>();
            services.AddScoped<IHighlightWorkDoneCells, HighlightWorkDoneCells>();
            services.AddScoped<IUnfundedRecommendations, UnfundedRecommendations>();
            services.AddScoped<IBridgeWorkSummary, BridgeWorkSummary>();
            services.AddScoped<IBridgeWorkSummaryByBudget, BridgeWorkSummaryByBudget>();
            services.AddScoped<SummaryReportGlossary>();
            services.AddScoped<SummaryReportParameters>();

            services.AddScoped<CostBudgetsWorkSummary>();
            services.AddScoped<BridgesCulvertsWorkSummary>();
            services.AddScoped<BridgeRateDeckAreaWorkSummary>();
            services.AddScoped<NHSBridgeDeckAreaWorkSummary>();
            services.AddScoped<DeckAreaBridgeWorkSummary>();
            services.AddScoped<PostedClosedBridgeWorkSummary>();
            services.AddScoped<BridgeWorkSummaryCommon>();
            services.AddScoped<BridgeWorkSummaryComputationHelper>();

            services.AddScoped<CulvertCost>();
            services.AddScoped<BridgeWorkCost>();

            // Summary report Graph TABS
            services.AddScoped<IAddGraphsInTabs, AddGraphsInTabs>();
            services.AddScoped<NHSConditionChart>();
            services.AddScoped<NonNHSConditionBridgeCount>();
            services.AddScoped<NonNHSConditionDeckArea>();
            services.AddScoped<ConditionBridgeCount>();
            services.AddScoped<ConditionDeckArea>();
            services.AddScoped<PoorBridgeCount>();
            services.AddScoped<PoorBridgeDeckArea>();
            services.AddScoped<PoorBridgeDeckAreaByBPN>();

            services.AddScoped<StackedColumnChartCommon>();
            services.AddSignalR();

#if MsSqlDebug
            // SQL SERVER SCOPINGS
            //services.AddMSSQLServices(Configuration.GetConnectionString("BridgeCareConnex"));
            services.AddDbContext<IAMContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("BridgeCareConnex")));

            services.AddScoped<INetworkRepository, NetworkRepository>();
            services.AddScoped<IMaintainableAssetRepository, MaintainableAssetRepository>();
            services.AddScoped<IAttributeRepository, AttributeRepository>();
            services.AddScoped<IAttributeDatumRepository, AttributeDatumRepository>();
            services.AddScoped<IAggregatedResultRepository, AggregatedResultRepository>();
            services.AddScoped<ISimulationRepository, SimulationRepository>();

            // Repository for legacy database
            services.AddMSSQLLegacyServices(Configuration.GetConnectionString("BridgeCareLegacyConnex"));
            services.AddScoped<IPennDotReportARepository, PennDotReportARepository>();
            services.AddScoped<IYearlyInvestmentRepository, YearlyInvestmentRepository>();

            services.AddScoped<ISimulationRepository, SimulationRepository>();
#elif LiteDbDebug
            // LITE DB SCOPINGS
            services.Configure<LiteDb.LiteDbOptions>(Configuration.GetSection("LiteDbOptions"));
            services.AddSingleton<LiteDb.ILiteDbContext, LiteDb.LiteDbContext>();

            services.AddScoped<IAttributeRepository, LiteDb.AttributeRepository>();
            services.AddScoped<IAggregatedResultRepository, LiteDb.AggregatedResultsRepository>();
            services.AddScoped<INetworkRepository, LiteDb.NetworkRepository>();
            services.AddScoped<IAttributeDatumRepository, LiteDb.AttributeDatumRepository>();
            services.AddScoped<IMaintainableAssetRepository, LiteDb.MaintainableAssetRepository>();
#endif
            services.AddCors(o => o.AddPolicy("CorsPolicy", builder => {
                builder
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials()
                .WithOrigins("http://localhost:8080", "https://v2.iam-deploy.com");
            }));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCors("CorsPolicy");

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<BridgeCareHub>("/bridgecarehub");
            });
        }

        private static void UpdateDatabase(IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices
                .GetRequiredService<IServiceScopeFactory>()
                .CreateScope())
            {
                using (var context = serviceScope.ServiceProvider.GetService<IAMContext>())
                {
                    context.Database.Migrate();
                }
            }
        }
    }
}
