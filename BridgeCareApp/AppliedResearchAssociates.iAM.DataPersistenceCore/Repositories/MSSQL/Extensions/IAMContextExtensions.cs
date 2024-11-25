using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;
using MoreLinq;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Extensions
{
    public static class IAMContextExtensions
    {
        public static void AddEntity<T>(this IAMContext context, T entity, Guid? userId = null) where T : class
        {
            if (entity == null)
            {
                return;
            }

            if (userId.HasValue)
            {
                SetPropertyValue(entity, BaseEntityProperty.CreatedBy, userId.Value);

                SetPropertyValue(entity, BaseEntityProperty.LastModifiedBy, userId.Value);
            }

            context.Set<T>().Add(entity);

            context.SaveChanges();
        }

        public static void AddAll<T>(this IAMContext context, List<T> entities, Guid? userId = null, int batchSize = 100000) where T : class
        {
            if (!entities.Any())
            {
                return;
            }

            if (userId.HasValue)
            {
                entities.ForEach(entity =>
                {
                    SetPropertyValue(entity, BaseEntityProperty.CreatedBy, userId.Value);

                    SetPropertyValue(entity, BaseEntityProperty.LastModifiedBy, userId.Value);
                });
            }

            var batches = entities.Count() / batchSize;
            var rem = entities.Count() % batchSize;
            var i = 0;

            while (i < batches)
            {
                context.BulkInsert(entities.Skip(i * batchSize).Take(batchSize).ToList(), new BulkConfig { BatchSize = batchSize, BulkCopyTimeout = 1800 });
                context.SaveChanges();
                i++;
            }

            if(rem > 0)
            {
                context.BulkInsert(entities.Skip(i * batchSize).Take(rem).ToList(), new BulkConfig { BatchSize = rem, BulkCopyTimeout = 1800 });
                context.SaveChanges();
            }
        }

        public static void UpdateEntity<T>(this IAMContext context, T entity, Guid key, Guid? userId = null) where T : class
        {
            if (entity == null)
            {
                return;
            }

            var existingEntity = context.Set<T>().Find(key);
            if (existingEntity == null)
            {
                throw new RowNotInTableException($"No {typeof(T).Name} entry found having id {key}.");
            }

            SetPropertyValue(entity, BaseEntityProperty.CreatedBy,
                GetPropertyInfo<T>(BaseEntityProperty.CreatedBy).GetValue(existingEntity));

            SetPropertyValue(entity, BaseEntityProperty.CreatedDate,
                GetPropertyInfo<T>(BaseEntityProperty.CreatedDate).GetValue(existingEntity));

            SetPropertyValue(entity, BaseEntityProperty.LastModifiedBy,
                userId ?? GetPropertyInfo<T>(BaseEntityProperty.LastModifiedBy)
                    .GetValue(existingEntity));

            SetPropertyValue(entity, BaseEntityProperty.LastModifiedDate, DateTime.Now);

            context.Entry(existingEntity).CurrentValues.SetValues(entity);

            context.SaveChanges();
        }

        public static void UpdateEntity<T>(this IAMContext context, T entity, Expression<Func<T, bool>> predicate, Guid? userId = null) where T : class
        {
            if (entity == null)
            {
                return;
            }

            var existingEntity = context.Set<T>().SingleOrDefault(predicate);
            if (existingEntity == null)
            {
                throw new RowNotInTableException($"No {typeof(T).Name} entry found.");
            }

            SetPropertyValue(entity, BaseEntityProperty.CreatedBy,
                GetPropertyInfo<T>(BaseEntityProperty.CreatedBy).GetValue(existingEntity));

            SetPropertyValue(entity, BaseEntityProperty.CreatedDate,
                GetPropertyInfo<T>(BaseEntityProperty.CreatedDate).GetValue(existingEntity));

            SetPropertyValue(entity, BaseEntityProperty.LastModifiedBy,
                userId ?? GetPropertyInfo<T>(BaseEntityProperty.LastModifiedBy)
                    .GetValue(existingEntity));

            SetPropertyValue(entity, BaseEntityProperty.LastModifiedDate, DateTime.Now);

            context.Entry(existingEntity).CurrentValues.SetValues(entity);

            context.SaveChanges();
        }

        public static void UpdateAll<T>(this IAMContext context, List<T> entities, Guid? userId = null, BulkConfig config = null) where T : class
        {
            if (!entities.Any())
            {
                return;
            }

            if (userId.HasValue)
            {
                entities.ForEach(entity =>
                {
                    SetPropertyValue(entity, BaseEntityProperty.LastModifiedBy, userId.Value);
                });
            }

            if (config == null)
            {
                var propsToExclude = new List<string> { "CreatedDate", "CreatedBy" };
                config = new BulkConfig { PropertiesToExclude = propsToExclude };
            }

            context.BulkUpdate(entities, config);

            context.SaveChanges();
        }

        public static T Upsert<T>(this IAMContext context, T entity, Guid key, Guid? userId = null) where T : class
        {
            if (entity == null)
            {
                return null;
            }

            var entities = context.Set<T>();
            T changedEntity = null;

            var existingEntity = entities.Find(key);
            if (existingEntity != null)
            {
                SetPropertyValue(entity, BaseEntityProperty.CreatedBy,
                    GetPropertyInfo<T>(BaseEntityProperty.CreatedBy).GetValue(existingEntity));

                SetPropertyValue(entity, BaseEntityProperty.CreatedDate,
                    GetPropertyInfo<T>(BaseEntityProperty.CreatedDate).GetValue(existingEntity));

                SetPropertyValue(entity, BaseEntityProperty.LastModifiedBy,
                    userId ?? GetPropertyInfo<T>(BaseEntityProperty.LastModifiedBy)
                        .GetValue(existingEntity));

                SetPropertyValue(entity, BaseEntityProperty.LastModifiedDate, DateTime.Now);

                context.Entry(existingEntity).CurrentValues.SetValues(entity);
                changedEntity = entity;
            }
            else
            {
                if (userId != null)
                {
                    SetPropertyValue(entity, BaseEntityProperty.CreatedBy, userId.Value);

                    SetPropertyValue(entity, BaseEntityProperty.LastModifiedBy, userId.Value);
                }

                changedEntity = entities.Add(entity).Entity;
            }

            context.SaveChanges();

            return changedEntity;
        }

        public static void Upsert<T>(this IAMContext context, T entity, Expression<Func<T, bool>> predicate, Guid? userId = null) where T : class
        {
            if (entity == null)
            {
                return;
            }

            var entities = context.Set<T>();

            var existingEntity = entities.SingleOrDefault(predicate);
            if (existingEntity != null)
            {
                SetPropertyValue(entity, BaseEntityProperty.CreatedBy,
                    GetPropertyInfo<T>(BaseEntityProperty.CreatedBy).GetValue(existingEntity));

                SetPropertyValue(entity, BaseEntityProperty.CreatedDate,
                    GetPropertyInfo<T>(BaseEntityProperty.CreatedDate).GetValue(existingEntity));

                SetPropertyValue(entity, BaseEntityProperty.LastModifiedBy,
                    userId ?? GetPropertyInfo<T>(BaseEntityProperty.LastModifiedBy)
                        .GetValue(existingEntity));

                SetPropertyValue(entity, BaseEntityProperty.LastModifiedDate, DateTime.Now);

                context.Entry(existingEntity).CurrentValues.SetValues(entity);
            }
            else
            {
                if (userId.HasValue)
                {
                    SetPropertyValue(entity, BaseEntityProperty.CreatedBy, userId.Value);

                    SetPropertyValue(entity, BaseEntityProperty.LastModifiedBy, userId.Value);
                }

                entities.Add(entity);
            }

            context.SaveChanges();
        }        

        public static void DeleteEntity<T>(this IAMContext context, Expression<Func<T, bool>> predicate) where T : class
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
                context.BulkDelete(entitiesToDelete);                
            }

            context.SaveChanges();
        }

        public static void UpsertOrDelete<T>(this IAMContext context, List<T> entities,
            Dictionary<string, Expression<Func<T, bool>>> predicatesPerCrudOperation, Guid? userId = null) where T : class
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
                        var existingEntity = contextEntities.Find(GetPropertyInfo<T>("Id").GetValue(entity));

                        if (existingEntity != null)
                        {
                            SetPropertyValue(entity, BaseEntityProperty.CreatedBy,
                                GetPropertyInfo<T>(BaseEntityProperty.CreatedBy).GetValue(existingEntity));

                            SetPropertyValue(entity, BaseEntityProperty.CreatedDate,
                                GetPropertyInfo<T>(BaseEntityProperty.CreatedDate).GetValue(existingEntity));

                            SetPropertyValue(entity, BaseEntityProperty.LastModifiedBy,
                                userId ?? GetPropertyInfo<T>(BaseEntityProperty.LastModifiedBy)
                                    .GetValue(existingEntity));

                            SetPropertyValue(entity, BaseEntityProperty.LastModifiedDate, DateTime.Now);

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
                    entitiesToAdd.ForEach(entity =>
                    {
                        if (userId.HasValue)
                        {
                            SetPropertyValue(entity, BaseEntityProperty.CreatedBy, userId.Value);

                            SetPropertyValue(entity, BaseEntityProperty.LastModifiedBy, userId.Value);
                        }

                        contextEntities.Add(entity);
                    });
                }
            }

            context.SaveChanges();
        }

        public static void BulkUpsertOrDelete<T>(this IAMContext context, List<T> entities,
            Dictionary<string, Expression<Func<T, bool>>> predicatesPerCrudOperation, Guid? userId = null) where T : class
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

                    entitiesToUpdate.ForEach(entity =>
                    {
                        if (userId.HasValue)
                        {
                            SetPropertyValue(entity, BaseEntityProperty.LastModifiedBy, userId.Value);
                        }

                        SetPropertyValue(entity, BaseEntityProperty.LastModifiedDate, DateTime.Now);
                    });

                    context.BulkUpdate(entitiesToUpdate.ToList(), config);
                }
            }

            if (predicatesPerCrudOperation.ContainsKey("add"))
            {
                var entitiesToAdd = entities.AsQueryable().Where(predicatesPerCrudOperation["add"]);
                if (entitiesToAdd.Any())
                {
                    entitiesToAdd.ForEach(entity =>
                    {
                        if (userId.HasValue)
                        {
                            SetPropertyValue(entity, BaseEntityProperty.CreatedBy, userId.Value);
                            SetPropertyValue(entity, BaseEntityProperty.LastModifiedBy, userId.Value);
                        }
                    });

                    context.BulkInsert(entitiesToAdd.ToList());
                }
            }

            context.SaveChanges();
        }

        private static void SetPropertyValue<T>(T entity, string property, object value) =>
            GetPropertyInfo<T>(property)?.SetValue(entity, value);

        private static PropertyInfo GetPropertyInfo<T>(string property) =>
            typeof(T).GetProperty(property);

        public static void ReInitializeAllEntityBaseProperties<T>(this IAMContext context, T entity, Guid? userId = null)
        {
            var currentDateTime = DateTime.Now;
            SetPropertyValue(entity, BaseEntityProperty.CreatedDate, currentDateTime);
            SetPropertyValue(entity, BaseEntityProperty.LastModifiedDate, currentDateTime);

            SetPropertyValue(entity, BaseEntityProperty.CreatedBy, userId ?? Guid.Empty);
            SetPropertyValue(entity, BaseEntityProperty.LastModifiedBy, userId ?? Guid.Empty);
        }
    }

    public static class BaseEntityProperty
    {
        public const string CreatedBy = "CreatedBy";
        public const string CreatedDate = "CreatedDate";
        public const string LastModifiedBy = "LastModifiedBy";
        public const string LastModifiedDate = "LastModifiedDate";
    }
}
