using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.FileSystem;
using AppliedResearchAssociates.iAM.DataMiner;
using AppliedResearchAssociates.iAM.DataAssignment.Segmentation;
using AppliedResearchAssociates.iAM.DataMiner.Attributes;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Extensions;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Interfaces;
using DataMinerAttribute = AppliedResearchAssociates.iAM.DataMiner.Attributes.Attribute;

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
            services.AddScoped<IRepository<Segment>, SegmentRepository>();
            services.AddScoped<IRepository<AttributeDatum<double>>, AttributeDatumRepository<double>>();
            services.AddScoped<IRepository<AttributeDatum<string>>, AttributeDatumRepository<string>>();
            services.AddScoped<IRepository<AttributeMetaDatum>, SegmentationMetaDataRepository>();
            services.AddScoped<IRepository<AttributeMetaDatum>, AttributeMetaDataRepository>();
            //services.AddScoped<IRepository<DataMinerAttribute>, AttributeRepository>();
            services.AddScoped<INetworkDataRepository, NetworkRepository>();
            services.AddScoped<ISegmentDataRepository, SegmentRepository>();
            services.AddScoped<IAttributeDatumDataRepository, AttributeDatumRepository<double>>();
            services.AddScoped<IAttributeDatumDataRepository, AttributeDatumRepository<string>>();
            services.AddScoped<IAttributeDataRepository, AttributeRepository>();
            services.AddScoped<ISaveChanges, SaveAllChanges>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //UpdateDatabase(app);

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
