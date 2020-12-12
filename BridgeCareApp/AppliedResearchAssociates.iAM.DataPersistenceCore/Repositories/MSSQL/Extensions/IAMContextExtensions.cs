using System;
using System.Linq;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
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

        public static void AddOrUpdate(this IAMContext context, AttributeEquationCriterionLibraryEntity entity)
        {
            if (entity == null)
            {
                return;
            }

            var existing = context.Set<AttributeEquationCriterionLibraryEntity>()
                .SingleOrDefault(_ => _.AttributeId == entity.AttributeId && _.EquationId == entity.EquationId);

            if (existing != null)
            {
                context.Entry(existing).CurrentValues.SetValues(entity);
            }
            else
            {
                context.Set<AttributeEquationCriterionLibraryEntity>().Add(entity);
            }
        }
    }
}
