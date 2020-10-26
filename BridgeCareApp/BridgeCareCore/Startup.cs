using System.Collections.Generic;
using AppliedResearchAssociates.iAM.DataAssignment.Aggregation;
using AppliedResearchAssociates.iAM.DataAssignment.Networking;
using AppliedResearchAssociates.iAM.DataMiner;
using AppliedResearchAssociates.iAM.DataMiner.Attributes;
using AppliedResearchAssociates.iAM.DataPersistenceCore.LiteDb;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.FileSystem;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using static Newtonsoft.Json.JsonSerializer;
using LiteDb = AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.LiteDb;

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
            services.AddControllers().AddNewtonsoftJson();
            Create(new JsonSerializerSettings
            {
                PreserveReferencesHandling = PreserveReferencesHandling.Objects
            });

            services.AddScoped<IAttributeMetaDataRepository, AttributeMetaDataRepository>();

            // SQL SERVER SCOPINGS
            services.AddMSSQLServices(Configuration.GetConnectionString("BridgeCareConnex"));

            /*services.AddScoped<IRepository<Network>, NetworkRepository>();
            services.AddScoped<IRepository<MaintainableAsset>, MaintainableAssetRepository>();
            services.AddScoped<IRepository<Attribute>, AttributeRepository>();
            services.AddScoped<IRepository<AttributeDatum<double>>, AttributeDatumRepository<double>>();
            services.AddScoped<IRepository<AttributeDatum<string>>, AttributeDatumRepository<string>>();
            services.AddScoped<IRepository<AggregatedResult<double>>, AggregatedResultRepository<double>>();
            services.AddScoped<IRepository<AggregatedResult<string>>, AggregatedResultRepository<string>>();
            services.AddScoped<IRepository<AttributeMetaDatum>, NetworkDefinitionMetaDataRepository>();*/
            services.AddScoped<INetworkRepository, NetworkRepository>();
            services.AddScoped<IMaintainableAssetRepository, MaintainableAssetRepository>();
            services.AddScoped<IAttributeRepository, AttributeRepository>();
            services.AddScoped<IAttributeDatumRepository, AttributeDatumRepository>();
            services.AddScoped<IAggregatedResultRepository, AggregatedResultRepository>();

            // LITE DB SCOPINGS
            /*services.Configure<LiteDb.LiteDbOptions>(Configuration.GetSection("LiteDbOptions"));
            services.AddSingleton<LiteDb.ILiteDbContext, LiteDb.LiteDbContext>();

            services.AddScoped<IAttributeRepository, LiteDb.AttributeRepository>();
            services.AddScoped<IAggregatedResultRepository, LiteDb.AggregatedResultsRepository>();
            services.AddScoped<INetworkRepository, LiteDb.NetworkRepository>();
            services.AddScoped<IAttributeDatumRepository, LiteDb.AttributeDatumRepository>();
            services.AddScoped<IMaintainableAssetRepository, LiteDb.MaintainableAssetRepository>();*/
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
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
