using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddDataAccessServices(this IServiceCollection services, string connectionString)
        {
            services.AddDbContext<IAMContext>(options =>
            {
                options.UseSqlServer(connectionString)
                .EnableSensitiveDataLogging();
            });
        }
    }
}
