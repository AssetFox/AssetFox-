using AppliedResearchAssociates.iAM.DataAssignment.Networking;
using AppliedResearchAssociates.iAM.DataMiner;
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
            services.AddMSSQLServices(Configuration.GetConnectionString("BridgeCareConnex"));

            services.Configure<LiteDb.LiteDbOptions>(Configuration.GetSection("LiteDbOptions"));
            services.AddSingleton<LiteDb.ILiteDbContext, LiteDb.LiteDbContext>();

            services.AddScoped<IRepository<Network>, LiteDb.NetworkRepository>();
            services.AddScoped<IAttributeDatumRepository, LiteDb.AttributeDatumRepository<double>>();

            services.AddScoped<IRepository<AttributeMetaDatum>, NetworkDefinitionMetaDataRepository>();
            services.AddScoped<IRepository<AttributeMetaDatum>, AttributeMetaDataRepository>();
            services.AddScoped<ISaveChanges, SaveAllChanges>();

            services.AddControllers().AddNewtonsoftJson();
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
