using AppliedResearchAssociates.iAM.DataAssignment.Networking;
using AppliedResearchAssociates.iAM.DataMiner;
using AppliedResearchAssociates.iAM.DataMiner.Attributes;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.FileSystem;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Extensions;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
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
            services.AddControllers().AddNewtonsoftJson();

            //It is an extension method in DataPersistenceCore project, which provides the connection to the database
            // This way, BridgeCareCore app doesn't have to know about the provider (eg. EF core)
            services.AddDataAccessServices(Configuration.GetConnectionString("BridgeCareConnex"));

            services.AddScoped<IRepository<Network>, NetworkRepository>();
            services.AddScoped<IRepository<MaintainableAsset>, MaintainableAssetRepository>();
            services.AddScoped<IRepository<Attribute>, AttributeRepository>();
            services.AddScoped<IRepository<AttributeDatum<double>>, AttributeDatumRepository<double>>();
            services.AddScoped<IRepository<AttributeDatum<string>>, AttributeDatumRepository<string>>();
            services.AddScoped<IRepository<AttributeMetaDatum>, NetworkDefinitionMetaDataRepository>();
            services.AddScoped<IRepository<AttributeMetaDatum>, AttributeMetaDataRepository>();
            /*services.AddScoped<INetworkDataRepository, NetworkRepository>();
            services.AddScoped<IMaintainableAssetRepository, MaintainableAssetRepository>();*/
            /*services.AddScoped<IAttributeDatumDataRepository, AttributeDatumRepository<double>>();
            services.AddScoped<IAttributeDatumDataRepository, AttributeDatumRepository<string>>();*/
            services.AddScoped<IAggregatedResultDataRepository, AggregatedResultRepository<double>>();
            services.AddScoped<IAggregatedResultDataRepository, AggregatedResultRepository<string>>();
            //services.AddScoped<IAttributeDataRepository, AttributeRepository>();
            services.AddScoped<ISaveChanges, SaveAllChanges>();
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
