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
            services.AddScoped<FileSystemRepository.ISimulationOutputRepository, FileSystemRepository.SimulationOutputRepository>();
            services.AddScoped<ISummaryReportGenerator, SummaryReportGenerator>();
            services.AddScoped<IExcelHelper, ExcelHelper>();
            services.AddScoped<IBridgeDataForSummaryReport, BridgeDataForSummaryReport>();
            services.AddScoped<IHighlightWorkDoneCells, HighlightWorkDoneCells>();
            services.AddScoped<IUnfundedRecommendations, UnfundedRecommendations>();
            services.AddSignalR();

#if MsSqlDebug
            // SQL SERVER SCOPINGS
            services.AddMSSQLServices(Configuration.GetConnectionString("BridgeCareConnex"));

            services.AddScoped<INetworkRepository, NetworkRepository>();
            services.AddScoped<IMaintainableAssetRepository, MaintainableAssetRepository>();
            services.AddScoped<IAttributeRepository, AttributeRepository>();
            services.AddScoped<IAttributeDatumRepository, AttributeDatumRepository>();
            services.AddScoped<IAggregatedResultRepository, AggregatedResultRepository>();
            services.AddScoped<ISimulationRepository, SimulationRepository>();

            // Repository for legacy database
            services.AddMSSQLLegacyServices(Configuration.GetConnectionString("BridgeCareLegacyConnex"));
            services.AddScoped<IPennDotReportARepository, PennDotReportARepository>();

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
