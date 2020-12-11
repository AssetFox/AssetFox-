using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Extensions
{
    public static class IAMContextExtensions
    {
        public static void AddOrUpdate<T>(this IAMContext context, T entity, Guid key) where T : class
        {
            if (entity == null)
            {
                return;
            }

            var existing = context.Set<T>().Find(key);

            if (existing != null)
            {
                context.Entry(existing).CurrentValues.SetValues(entity);
            }
            else
            {
                context.Set<T>().Add(entity);
            }
        }
    }
}
