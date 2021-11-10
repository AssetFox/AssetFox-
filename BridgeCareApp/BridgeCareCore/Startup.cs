using System;
using System.Collections.Generic;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL;
using AppliedResearchAssociates.iAM.Reporting;
using BridgeCareCore.Hubs;
using BridgeCareCore.Interfaces;
using BridgeCareCore.Logging;
using BridgeCareCore.Services;
using BridgeCareCore.StartupExtension;
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
            var urlsKeyValue = Configuration.GetSection("AllowedOrigins:urls").GetChildren();
            var urls = new List<string>();

            foreach (var item in urlsKeyValue)
            {
                urls.Add(item.Value);
            }

            services.AddCors(o => o.AddPolicy("CorsPolicy", builder =>
            {
                builder
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials()
                //.WithOrigins(urls.ToArray());
            }));

            services.AddSecurityConfig(Configuration);

            services.AddSingleton(Configuration);
            services.AddControllers().AddNewtonsoftJson();

            services.AddSimulationData();
            services.AddSummaryReportDataTABs();
            services.AddSummaryReportGraphTABs();
            services.AddDefaultData();

            services.AddSingleton<ILog, LogNLog>();
            services.AddScoped<LegacySimulationSynchronizerService>();

            services.AddSignalR();
            services.AddScoped<IHubService, HubService>();

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddDbContext<IAMContext>(options => options.UseSqlServer(
                Configuration.GetConnectionString("BridgeCareConnex"),
                sqlServerOptions => sqlServerOptions.CommandTimeout(1800))
                );

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
