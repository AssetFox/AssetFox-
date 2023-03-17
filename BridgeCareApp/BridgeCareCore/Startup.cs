using System;
using System.Collections.Generic;
using AppliedResearchAssociates.iAM.Common;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL;
using AppliedResearchAssociates.iAM.Hubs;
using AppliedResearchAssociates.iAM.Hubs.Interfaces;
using AppliedResearchAssociates.iAM.Hubs.Services;
using AppliedResearchAssociates.iAM.Reporting;
using AppliedResearchAssociates.iAM.Reporting.Interfaces;
using AppliedResearchAssociates.iAM.Reporting.Logging;
using BridgeCareCore.Security;
using BridgeCareCore.Services.Aggregation;
using BridgeCareCore.StartupExtension;
using BridgeCareCore.GraphQL;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

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
            services.AddTransient<IClaimsTransformation, ClaimsTransformation>();

            services.AddSingleton(Configuration);
            services.AddControllers().AddNewtonsoftJson();

            services.AddSimulationData();
            services.AddPagingData();
            services.AddDefaultData();

            services.AddSingleton<ILog, LogNLog>();

            services.AddSignalR();
            services.AddScoped<IHubService, HubService>();

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<IAuthorizationMiddlewareResultHandler, SecurityAuthorizationMiddlewareResultHandler>();
            var connectionString = Configuration.GetConnectionString("BridgeCareConnex");

            services.AddDbContext<IAMContext>(options => options.UseSqlServer(
                connectionString,
                sqlServerOptions => sqlServerOptions.CommandTimeout(1800))
                );

            services.AddGraphQLServer()
                .AddQueryType<Query>()
                .AddFiltering()
                .AddSorting();

            SetupReporting(services);
            var reportLookup = new Dictionary<string, Type>();

            reportLookup.Add("PAMSSummaryReport", typeof(PAMSSummaryReport));

            services.AddScoped<IReportGenerator, DictionaryBasedReportGenerator>();
            services.AddScoped<IAggregationService, AggregationService>();
        }

        private void SetupReporting(IServiceCollection services)
        {
            var reportFactoryList = new List<IReportFactory>();
            reportFactoryList.Add(new HelloWorldReportFactory());
            reportFactoryList.Add(new InventoryReportFactory());
            reportFactoryList.Add(new BAMSSummaryReportFactory());
            reportFactoryList.Add(new ScenarioOutputReportFactory());
            reportFactoryList.Add(new PAMSSummaryReportFactory());
            reportFactoryList.Add(new BAMSAuditReportFactory());
            reportFactoryList.Add(new BAMSPBExportReportFactory());
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
                endpoints.MapGraphQL();
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
