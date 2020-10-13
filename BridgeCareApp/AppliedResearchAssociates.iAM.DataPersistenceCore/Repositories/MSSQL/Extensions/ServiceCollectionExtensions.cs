using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.LiteDb;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddSqlServerDataAccessServices(this IServiceCollection services, string connectionString)
        {
            services.AddDbContext<IAMContext>(options =>
            {
                options.UseSqlServer(connectionString)
                .EnableSensitiveDataLogging();
            });
        }

        public static void AddLiteDbDataAccessServices(this IServiceCollection services, string databaseFilePath)
        {
            services.AddDbContext<LiteDbContext>(options =>
            {
                options
            })
        }
    }
}
