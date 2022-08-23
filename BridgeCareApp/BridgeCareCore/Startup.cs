using System;
using System.Collections.Generic;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL;
using AppliedResearchAssociates.iAM.Reporting;
using AppliedResearchAssociates.iAM.Hubs;
using AppliedResearchAssociates.iAM.Hubs.Interfaces;
using AppliedResearchAssociates.iAM.Hubs.Services;
using BridgeCareCore.Logging;
using BridgeCareCore.Services.Aggregation;
using BridgeCareCore.StartupExtension;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using AppliedResearchAssociates.iAM.Reporting.Interfaces;
using System.Security.Claims;

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
            //services.Configure<ForwardedHeadersOptions>(options =>
            //{
            //    options.ForwardedHeaders =
            //        ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
            //});
            var urlsKeyValue = Configuration.GetSection("AllowedOrigins:urls").GetChildren();
            var urls = new List<string>();

            foreach (var item in urlsKeyValue)
            {
                urls.Add(item.Value);
            }

            services.AddCors(o => o.AddPolicy("CorsPolicy", builder =>
            {
                builder
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials()
                .WithOrigins(urls.ToArray());
            }));

            services.AddSecurityConfig(Configuration);

            services.AddSingleton(Configuration);
            services.AddControllers().AddNewtonsoftJson();

            services.AddSimulationData();
            services.AddDefaultData();

            services.AddSingleton<ILog, LogNLog>();

            services.AddSignalR();
            services.AddScoped<IHubService, HubService>();

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            var connectionString = Configuration.GetConnectionString("BridgeCareConnex");

            services.AddDbContext<IAMContext>(options => options.UseSqlServer(
                connectionString,
                sqlServerOptions => sqlServerOptions.CommandTimeout(1800))
                );

            SetupReporting(services);
            var reportLookup = new Dictionary<string, Type>();

            reportLookup.Add("PAMSSummaryReport", typeof(PAMSSummaryReport));

            services.AddScoped<IReportGenerator, DictionaryBasedReportGenerator>();
            services.AddScoped<IAggregationService, AggregationService>();

            services.AddAuthorization(options =>
            {
                // Deficient Condition Goal
                options.AddPolicy("ViewDeficientConditionGoalFromlLibrary",
                    policy => policy.RequireClaim(ClaimTypes.Name, "DeficientConditionGoalViewPermittedFromLibraryAccess", "DeficientConditionGoalViewAnyFromLibraryAccess"));
                options.AddPolicy("ViewDeficientConditionGoalFromScenario",
                    policy => policy.RequireClaim(ClaimTypes.Name, "DeficientConditionGoalViewAnyFromScenarioAccess", "DeficientConditionGoalViewPermittedFromScenarioAccess"));
                options.AddPolicy("ModifyDeficientConditionGoalFromLibrary",
                    policy => policy.RequireClaim(ClaimTypes.Name, "DeficientConditionGoalModifyAnyFromLibraryAccess", "DeficientConditionGoalModifyPermittedFromLibraryAccess"));
                options.AddPolicy("ModifyDeficientConditionGoalFromScenario",
                    policy => policy.RequireClaim(ClaimTypes.Name, "DeficientConditionGoalModifyAnyFromScenarioAccess", "DeficientConditionGoalModifyPermittedFromScenarioAccess"));

                //Investment
                options.AddPolicy("ViewInvestmentFromScenario",
                    policy => policy.RequireClaim(ClaimTypes.Name, "InvestmentViewAnyFromScenarioAccess", "InvestmentViewPermittedFromScenarioAccess"));
                options.AddPolicy("ViewInvestmentFromLibrary",
                    policy => policy.RequireClaim(ClaimTypes.Name, "InvestmentViewAnyFromLibraryAccess", "InvestmentViewPermittedFromLibraryAccess"));
                options.AddPolicy("ModifyInvestmentFromScenario",
                    policy => policy.RequireClaim(ClaimTypes.Name, "InvestmentModifyAnyFromScenarioAccess", "InvestmentModifyPermittedFromScenarioAccess"));
                options.AddPolicy("ModifyInvestmentFromLibrary",
                    policy => policy.RequireClaim(ClaimTypes.Name, "InvestmentModifyAnyFromLibraryAccess", "InvestmentModifyPermittedFromLibraryAccess"));
                options.AddPolicy("ImportInvestmentFromLibrary",
                    policy => policy.RequireClaim(ClaimTypes.Name, "InvestmentImportAnyFromLibraryAccess", "InvestmentImportPermittedFromLibraryAccess"));
                options.AddPolicy("ImportInvestmentFromScenario",
                    policy => policy.RequireClaim(ClaimTypes.Name, "InvestmentImportAnyFromScenarioAccess", "InvestmentImportPermittedFromScenarioAccess"));

                // Performance Curve
                options.AddPolicy("ViewPerformanceCurveFromLibrary",
                    policy => policy.RequireClaim(ClaimTypes.Name, "PerformanceCurveViewAnyFromLibraryAccess", "PerformanceCurveViewPermittedFromLibraryAccess"));
                options.AddPolicy("ViewPerformanceCurveFromScenario",
                    policy => policy.RequireClaim(ClaimTypes.Name, "PerformanceCurveViewAnyFromScenarioAccess", "PerformanceCurveViewPermittedFromScenarioAccess"));
                options.AddPolicy("ModifyPerformanceCurveFromLibrary",
                    policy => policy.RequireClaim(ClaimTypes.Name, "PerformanceCurveAddAnyFromLibraryAccess",
                                                                   "PerformanceCurveUpdateAnyFromLibraryAccess",
                                                                   "PerformanceCurveAddPermittedFromLibraryAccess",
                                                                   "PerformanceCurveUpdatePermittedFromLibraryAccess"));
                options.AddPolicy("ModifyPerformanceCurveFromScenario",
                    policy => policy.RequireClaim(ClaimTypes.Name, "PerformanceCurveModifyAnyFromScenarioAccess", "PerformanceCurveModifyPermittedFromScenarioAccess"));
                options.AddPolicy("DeletePerformanceCurveFromLibrary",
                    policy => policy.RequireClaim(ClaimTypes.Name, "PerformanceCurveDeleteAnyFromLibraryAccess", "PerformanceCurveDeletePermittedFromLibraryAccess"));
                options.AddPolicy("ImportPerformanceCurveFromLibrary",
                    policy => policy.RequireClaim(ClaimTypes.Name, "PerformanceCurveImportAnyFromLibraryAccess", "PerformanceCurveImportPermittedFromLibraryAccess"));
                options.AddPolicy("ImportPerformanceCurveFromScenario",
                    policy => policy.RequireClaim(ClaimTypes.Name, "PerformanceCurveImportAnyFromScenarioAccess", "PerformanceCurveImportPermittedFromScenarioAccess"));

                //  Reamining Life Limit
                options.AddPolicy("ViewRemainingLifeLimitFromLibrary",
                    policy => policy.RequireClaim(ClaimTypes.Name, "RemainingLifeLimitViewAnyFromLibraryAccess", "RemainingLifeLimitViewPermittedFromLibraryAccess"));
                options.AddPolicy("ViewRemainingLifeLimitFromScenario",
                    policy => policy.RequireClaim(ClaimTypes.Name, "RemainingLifeLimitViewAnyFromScenarioAccess", "RemainingLifeLimitViewPermittedFromScenarioAccess"));
                options.AddPolicy("ModifyRemainingLifeLimitFromLibrary",
                    policy => policy.RequireClaim(ClaimTypes.Name, "RemainingLifeLimitAddAnyFromLibraryAccess",
                                                                   "RemainingLifeLimitUpdateAnyFromLibraryAccess",
                                                                   "RemainingLifeLimitAddPermittedFromLibraryAccess",
                                                                   "RemainingLifeLimitUpdatePermittedFromLibraryAccess"));
                options.AddPolicy("ModifyRemainingLifeLimitFromScenario",
                    policy => policy.RequireClaim(ClaimTypes.Name, "RemainingLifeLimitModifyAnyFromScenarioAccess", "RemainingLifeLimitModifyPermittedFromScenarioAccess"));
                options.AddPolicy("DeleteRemainingLifeLimitFromLibrary",
                    policy => policy.RequireClaim(ClaimTypes.Name, "RemainingLifeLimitDeleteAnyFromLibraryAccess", "RemainingLifeLimitDeletePermittedFromLibraryAccess"));

                // Target Condition Goal
                options.AddPolicy("ViewTargetConditionGoalFromLibrary",
                    policy => policy.RequireClaim(ClaimTypes.Name, "TargetConditionGoalViewAnyFromLibraryAccess", "TargetConditionGoalViewPermittedFromLibraryAccess"));
                options.AddPolicy("ViewTargetConditionGoalFromScenario",
                    policy => policy.RequireClaim(ClaimTypes.Name, "TargetConditionGoalViewAnyFromScenarioAccess", "TargetConditionGoalViewPermittedFromScenarioAccess"));
                options.AddPolicy("ModifyTargetConditionGoalFromLibrary",
                    policy => policy.RequireClaim(ClaimTypes.Name, "TargetConditionGoalAddAnyFromLibraryAccess",
                                                                   "TargetConditionGoalUpdateAnyFromLibraryAccess",
                                                                   "TargetConditionGoalAddPermittedFromLibraryAccess",
                                                                   "TargetConditionGoalUpdatePermittedFromLibraryAccess"));
                options.AddPolicy("ModifyTargetConditionGoalFromScenario",
                    policy => policy.RequireClaim(ClaimTypes.Name, "TargetConditionGoalModifyAnyFromScenarioAccess", "TargetConditionGoalModifyPermittedFromScenarioAccess"));
                options.AddPolicy("DeleteTargetConditionGoalFromLibrary",
                    policy => policy.RequireClaim(ClaimTypes.Name, "TargetConditionGoalDeleteAnyFromLibraryAccess", "TargetConditionGoalDeletePermittedFromLibraryAccess"));

                // Treatment
                options.AddPolicy("ViewTreatmentFromLibrary",
                    policy => policy.RequireClaim(ClaimTypes.Name, "TreatmentViewAnyFromLibraryAccess", "TreatmentViewPermittedFromLibraryAccess"));
                options.AddPolicy("ViewTreatmentFromScenario",
                    policy => policy.RequireClaim(ClaimTypes.Name, "TreatmentViewAnyFromScenarioAccess", "TreatmentViewPermittedFromScenarioAccess"));
                options.AddPolicy("ModifyTreatmentFromLibrary",
                    policy => policy.RequireClaim(ClaimTypes.Name, "TreatmentAddAnyFromLibraryAccess",
                                                                   "TreatmentUpdateAnyFromLibraryAccess",
                                                                   "TreatmentAddPermittedFromLibraryAccess",
                                                                   "TreatmentUpdatePermittedFromLibraryAccess"));
                options.AddPolicy("ModifyTreatmentFromScenario",
                    policy => policy.RequireClaim(ClaimTypes.Name, "TreatmentModifyAnyFromScenarioAccess",
                                                                   "TreatmentModifyPermittedFromScenarioAccess"));
                options.AddPolicy("DeleteTreatmentFromLibrary",
                    policy => policy.RequireClaim(ClaimTypes.Name, "TreatmentDeleteAnyFromLibraryAccess", "TreatmentDeletePermittedFromLibraryAccess"));
                options.AddPolicy("ImportTreatmentFromLibrary",
                    policy => policy.RequireClaim(ClaimTypes.Name, "TreatmentImportAnyFromLibraryAccess", "TreatmentImportPermittedFromLibraryAccess"));
                options.AddPolicy("ImportTreatmentFromScenario",
                    policy => policy.RequireClaim(ClaimTypes.Name, "TreatmentImportAnyFromScenarioAccess", "TreatmentImportPermittedFromScenarioAccess"));

                // Analysis Method
                options.AddPolicy("ViewAnalysisMethod",
                    policy => policy.RequireClaim(ClaimTypes.Name, "AnalysisMethodViewAnyAccess", "AnalysisMethodViewPermittedAccess"));
                options.AddPolicy("ModifyAnalysisMethod",
                    policy => policy.RequireClaim(ClaimTypes.Name, "AnalysisMethodModifyAnyAccess", "AnalysisMethodModifyPermittedAccess"));

                // Attributes
                options.AddPolicy("ModifyAttributes", policy => policy.RequireClaim(ClaimTypes.Name, "AttributesAddAccess", "AttributesUpdateAccess"));

                // Simulation
                options.AddPolicy("ViewSimulation",
                    policy => policy.RequireClaim(ClaimTypes.Name, "SimulationViewPermittedAccess", "SimulationViewAnyAccess"));
                options.AddPolicy("DeleteSimulation",
                    policy => policy.RequireClaim(ClaimTypes.Name, "SimulationDeletePermittedAccess", "SimulationDeleteAnyAccess"));
                options.AddPolicy("UpdateSimulation",
                    policy => policy.RequireClaim(ClaimTypes.Name, "SimulationUpdatePermittedAccess", "SimulationUpdateAnyAccess"));
                options.AddPolicy("RunSimulation",
                    policy => policy.RequireClaim(ClaimTypes.Name, "SimulationRunPermittedAccess", "SimulationRunAnyAccess"));

                // Budget Priority
                options.AddPolicy("ViewBudgetPriorityFromLibrary",
                    policy => policy.RequireClaim(ClaimTypes.Name, "BudgetPriorityViewAnyFromLibraryAccess", "BudgetPriorityViewPermittedFromLibraryAccess"));
                options.AddPolicy("ModifyBudgetPriorityFromLibrary",
                    policy => policy.RequireClaim(ClaimTypes.Name, "BudgetPriorityUpdateAnyFromLibraryAccess",
                                                                   "BudgetPriorityUpdatePermittedFromLibraryAccess",
                                                                   "BudgetPriorityAddPermittedFromLibraryAccess",
                                                                   "BudgetPriorityAddAnyFromLibraryAccess"));
                options.AddPolicy("DeleteBudgetPriorityFromLibrary",
                    policy => policy.RequireClaim(ClaimTypes.Name, "BudgetPriorityDeleteAnyFromLibraryAccess", "BudgetPriorityDeletePermittedFromLibraryAccess"));
                options.AddPolicy("ViewBudgetPriorityFromScenario",
                    policy => policy.RequireClaim(ClaimTypes.Name, "BudgetPriorityViewPermittedFromScenarioAccess", "BudgetPriorityViewAnyFromScenarioAccess"));
                options.AddPolicy("ModifyBudgetPriorityFromScenario",
                    policy => policy.RequireClaim(ClaimTypes.Name, "BudgetPriorityModifyPermittedFromScenarioAccess", "BudgetPriorityModifyAnyFromScenarioAccess"));

                // Calculated Attributes
                options.AddPolicy("ModifyCalculatedAttributesFromLibrary",
                    policy => policy.RequireClaim(ClaimTypes.Name, "CalculatedAttributesModifyFromLibraryAccess", "CalculatedAttributesChangeInLibraryAccess"));
                options.AddPolicy("ModifyCalculatedAttributesFromScenario",
                    policy => policy.RequireClaim(ClaimTypes.Name, "CalculatedAttributesModifyFromScenarioAccess", "CalculatedAttributesChangeInScenarioAccess"));

                // Cash Flow
                options.AddPolicy("ViewCashFlowFromLibrary",
                    policy => policy.RequireClaim(ClaimTypes.Name, "CashFlowViewAnyFromLibraryAccess", "CashFlowViewPermittedFromLibraryAccess"));
                options.AddPolicy("ViewCashFlowFromScenario",
                    policy => policy.RequireClaim(ClaimTypes.Name, "CashFlowViewAnyFromScenarioAccess", "CashFlowViewPermittedFromScenarioAccess"));
                options.AddPolicy("ModifyCashFlowFromLibrary",
                    policy => policy.RequireClaim(ClaimTypes.Name, "CashFlowModifyAnyFromLibraryAccess", "CashFlowModifyPermittedFromLibraryAccess"));
                options.AddPolicy("ModifyCashFlowFromScenario",
                    policy => policy.RequireClaim(ClaimTypes.Name, "CashFlowModifyAnyFromScenarioAccess", "CashFlowModifyPermittedFromScenarioAccess"));

                // Committed Projects
                options.AddPolicy("ImportCommittedProjects",
                    policy => policy.RequireClaim(ClaimTypes.Name, "CommittedProjectImportAnyAccess", "CommittedProjectImportPermittedAccess"));
                options.AddPolicy("ModifyCommittedProjects",
                    policy => policy.RequireClaim(ClaimTypes.Name, "CommittedProjectModifyPermittedAccess", "CommittedProjectModifyAnyAccess"));
                options.AddPolicy("ViewCommittedProjects",
                    policy => policy.RequireClaim(ClaimTypes.Name, "CommittedProjectViewPermittedAccess", "CommittedProjectViewAnyAccess"));
            });
        }

        private void SetupReporting(IServiceCollection services)
        {
            var reportFactoryList = new List<IReportFactory>();
            reportFactoryList.Add(new HelloWorldReportFactory());
            reportFactoryList.Add(new InventoryReportFactory());
            reportFactoryList.Add(new BAMSSummaryReportFactory());
            reportFactoryList.Add(new ScenarioOutputReportFactory());
            reportFactoryList.Add(new PAMSSummaryReportFactory());
            services.AddSingleton<IReportLookupLibrary>(service => new ReportLookupLibrary(reportFactoryList));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILog logger)
        {

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                //app.UseForwardedHeaders();
            }

            app.ConfigureExceptionHandler(logger);
            //app.UseForwardedHeaders();

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
