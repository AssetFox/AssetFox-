using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using EFCore.BulkExtensions;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Extensions
{
    public static class IAMContextExtensions
    {
        private static readonly bool IsRunningFromXUnit = AppDomain.CurrentDomain.GetAssemblies()
            .Any(a => a.FullName.ToLowerInvariant().StartsWith("xunit"));

        public static void Upsert<T>(this IAMContext context, T entity, Guid key) where T : class
        {
            if (entity == null)
            {
                return;
            }

            var entities = context.Set<T>();

            var existingEntity = entities.Find(key);
            if (existingEntity != null)
            {
                var createdDatePropertyInfo = typeof(T).GetProperty("CreatedDate");
                createdDatePropertyInfo.SetValue(entity, createdDatePropertyInfo.GetValue(existingEntity));
                var lastModifiedDatePropertyInfo = typeof(T).GetProperty("LastModifiedDate");
                lastModifiedDatePropertyInfo.SetValue(entity, DateTime.Now);
                context.Entry(existingEntity).CurrentValues.SetValues(entity);
            }
            else
            {
                entities.Add(entity);
            }

            context.SaveChanges();
        }

        public static void Upsert<T>(this IAMContext context, T entity, Expression<Func<T, bool>> predicate) where T : class
        {
            if (entity == null)
            {
                return;
            }

            var entities = context.Set<T>();

            var existingEntity = entities.SingleOrDefault(predicate);
            if (existingEntity != null)
            {
                var createdDatePropertyInfo = typeof(T).GetProperty("CreatedDate");
                createdDatePropertyInfo.SetValue(entity, createdDatePropertyInfo.GetValue(existingEntity));
                var lastModifiedDatePropertyInfo = typeof(T).GetProperty("LastModifiedDate");
                lastModifiedDatePropertyInfo.SetValue(entity, DateTime.Now);
                context.Entry(existingEntity).CurrentValues.SetValues(entity);
            }
            else
            {
                entities.Add(entity);
            }

            context.SaveChanges();
        }

        public static void Delete<T>(this IAMContext context, Expression<Func<T, bool>> predicate) where T : class
        {
            var entities = context.Set<T>();

            var entityToDelete = entities.SingleOrDefault(predicate);
            if (entityToDelete != null)
            {
                entities.Remove(entityToDelete);
            }

            context.SaveChanges();
        }

        public static void DeleteAll<T>(this IAMContext context, Expression<Func<T, bool>> predicate) where T : class
        {
            var entities = context.Set<T>();

            var entitiesToDelete = entities.Where(predicate).ToList();
            if (entitiesToDelete.Any())
            {
                if (IsRunningFromXUnit)
                {
                    entities.RemoveRange(entitiesToDelete);
                }
                else
                {
                    context.BulkDelete(entitiesToDelete);
                }
            }

            context.SaveChanges();
        }

        public static void UpsertOrDelete<T>(this IAMContext context, List<T> entities,
            Dictionary<string, Expression<Func<T, bool>>> predicatesPerCrudOperation) where T : class
        {
            var contextEntities = context.Set<T>();

            if (predicatesPerCrudOperation.ContainsKey("delete"))
            {
                var entitiesToDelete = contextEntities
                    .Where(predicatesPerCrudOperation["delete"])
                    .ToList();

                if (entitiesToDelete.Any())
                {
                    contextEntities.RemoveRange(entitiesToDelete);
                }
            }

            if (predicatesPerCrudOperation.ContainsKey("update"))
            {
                var entitiesToUpdate = entities.AsQueryable()
                    .Where(predicatesPerCrudOperation["update"])
                    .ToList();

                if (entitiesToUpdate.Any())
                {
                    entitiesToUpdate.ForEach(entity =>
                    {
                        var idPropertyInfo = typeof(T).GetProperty("Id");
                        var existingEntity = contextEntities.Find(idPropertyInfo.GetValue(entity));

                        if (existingEntity != null)
                        {
                            var createdDatePropertyInfo = typeof(T).GetProperty("CreatedDate");
                            createdDatePropertyInfo.SetValue(entity, createdDatePropertyInfo.GetValue(existingEntity));
                            var lastModifiedDatePropertyInfo = typeof(T).GetProperty("LastModifiedDate");
                            lastModifiedDatePropertyInfo.SetValue(entity, DateTime.Now);
                            context.Entry(existingEntity).CurrentValues.SetValues(entity);
                        }
                    });
                }
            }

            if (predicatesPerCrudOperation.ContainsKey("add"))
            {
                var entitiesToAdd = entities.AsQueryable()
                    .Where(predicatesPerCrudOperation["add"])
                    .ToList();

                if (entitiesToAdd.Any())
                {
                    contextEntities.AddRange(entitiesToAdd);
                }
            }

            context.SaveChanges();
        }

        public static void BulkUpsertOrDelete<T>(this IAMContext context, List<T> entities,
            Dictionary<string, Expression<Func<T, bool>>> predicatesPerCrudOperation) where T : class
        {
            var contextEntities = context.Set<T>();

            if (predicatesPerCrudOperation.ContainsKey("delete"))
            {
                var entitiesToDelete = contextEntities.Where(predicatesPerCrudOperation["delete"]);
                if (entitiesToDelete.Any())
                {
                    context.BulkDelete(entitiesToDelete.ToList());
                }
            }

            if (predicatesPerCrudOperation.ContainsKey("update"))
            {
                var entitiesToUpdate = entities.AsQueryable().Where(predicatesPerCrudOperation["update"]);
                if (entitiesToUpdate.Any())
                {
                    var propsToExclude = new List<string> { "CreatedDate", "CreatedBy" };
                    var config = new BulkConfig { PropertiesToExclude = propsToExclude };
                    context.BulkUpdate(entitiesToUpdate.ToList(), config);
                }
            }

            if (predicatesPerCrudOperation.ContainsKey("add"))
            {
                var entitiesToAdd = entities.AsQueryable().Where(predicatesPerCrudOperation["add"]);
                if (entitiesToAdd.Any())
                {
                    context.BulkInsert(entitiesToAdd.ToList());
                }
            }

            context.SaveChanges();
        }
    }
}
