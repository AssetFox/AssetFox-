using System;
using System.Collections.Generic;
using AppliedResearchAssociates.iAM.DataPersistenceCore;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.FileSystem;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.Reporting;
using BridgeCareCore.Hubs;
using BridgeCareCore.Interfaces;
using BridgeCareCore.Interfaces.SummaryReport;
using BridgeCareCore.Logging;
using BridgeCareCore.Security;
using BridgeCareCore.Security.Interfaces;
using BridgeCareCore.Services;
using BridgeCareCore.Services.SummaryReport;
using BridgeCareCore.Services.SummaryReport.BridgeData;
using BridgeCareCore.Services.SummaryReport.BridgeWorkSummary;
using BridgeCareCore.Services.SummaryReport.BridgeWorkSummaryByBudget;
using BridgeCareCore.Services.SummaryReport.GraphTabs;
using BridgeCareCore.Services.SummaryReport.GraphTabs.NHSConditionCharts;
using BridgeCareCore.Services.SummaryReport.Parameters;
using BridgeCareCore.Services.SummaryReport.ShortNameGlossary;
using BridgeCareCore.Services.SummaryReport.UnfundedRecommendations;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.AzureADB2C.UI;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;

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
            services.AddCors(o => o.AddPolicy("CorsPolicy", builder =>
            {
                builder
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials()
                .WithOrigins("http://localhost:8080", "https://v2.iam-deploy.com", "https://iam-demo.net/");
            }));

            var securityType = Configuration.GetSection("SecurityType").Value;

            if (securityType == SecurityConstants.SecurityTypes.Esec)
            {
                services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                    .AddJwtBearer(options =>
                    {
                        options.TokenValidationParameters = new TokenValidationParameters
                        {
                            RequireExpirationTime = true,
                            RequireSignedTokens = true,
                            ValidateAudience = false,
                            ValidateIssuer = false,
                            ValidateLifetime = true,
                            IssuerSigningKey = SecurityFunctions.GetPublicKey(Configuration.GetSection("EsecConfig"))
                        };
                    });
            }

            if (securityType == SecurityConstants.SecurityTypes.B2C)
            {
                services.AddAuthentication(AzureADB2CDefaults.BearerAuthenticationScheme)
                    .AddAzureADB2CBearer(options => Configuration.Bind("AzureAdB2C", options));
            }

            services.AddAuthorization(options =>
            {
                options.AddPolicy(SecurityConstants.Policy.AdminOrDistrictEngineer,
                    policy => policy.Requirements.Add(
                        new UserHasAllowedRoleRequirement(Role.Administrator, Role.DistrictEngineer)));
                options.AddPolicy(SecurityConstants.Policy.Admin,
                    policy => policy.Requirements.Add(
                        new UserHasAllowedRoleRequirement(Role.Administrator)));
            });

            services.AddSingleton(Configuration);
            services.AddControllers().AddNewtonsoftJson();

            services.AddSingleton<ILog, LogNLog>();
            services.AddScoped<LegacySimulationSynchronizerService>();
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
            services.AddScoped<CommittedProjectCost>();
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
            services.AddScoped<ISimulationAnalysis, SimulationAnalysisService>();
            services.AddSignalR();
            services.AddSingleton<IEsecSecurity, EsecSecurity>();
            services.AddScoped<AttributeService>();
            services.AddSingleton<IAuthorizationHandler, RestrictAccessHandler>();
            services.AddScoped<ExpressionValidationService>();
            services.AddScoped<IUserCriteriaRepository, UserCriteriaRepository>();
            services.AddScoped<IHubService, HubService>();
            services.AddScoped<IAssetData, PennDOTAssetDataRepository>();
            services.AddScoped<IMaintainableAssetRepository, MaintainableAssetRepository>();
            services.AddScoped<ICommittedProjectService, CommittedProjectService>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            // SQL SERVER SCOPINGS
            //services.AddDbContext<IAMContext>(options =>
            //    options.UseSqlServer(Configuration.GetConnectionString("BridgeCareConnex")));

            services.AddDbContext<IAMContext>(options => options.UseSqlServer(
            Configuration.GetConnectionString("BridgeCareConnex"),
            sqlServerOptions => sqlServerOptions.CommandTimeout(1800))
                );
            services.AddScoped<UnitOfDataPersistenceWork>();

            // Setup reporting
            var reportLookup = new Dictionary<string, Type>();
            reportLookup.Add("HelloWorld", typeof(HelloWorldReport));
            reportLookup.Add("InventoryLookup", typeof(InventoryReport));
            reportLookup.Add("ScenarioOutput", typeof(ScenarioOutputReport));

            services.AddSingleton(service => new ReportLookupLibrary(reportLookup));
            services.AddScoped<IReportGenerator, DictionaryBasedReportGenerator>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILog logger)
        {
            //UpdateDatabase(app);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.ConfigureExceptionHandler(logger);

            app.UseCors("CorsPolicy");

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<BridgeCareHub>("/bridgecarehub");
            });
        }

        private static void UpdateDatabase(IApplicationBuilder app)
        {
            using var serviceScope = app.ApplicationServices
                .GetRequiredService<IServiceScopeFactory>()
                .CreateScope();
            using var context = serviceScope.ServiceProvider.GetService<IAMContext>();
            context.Database.Migrate();
        }
    }
}
