using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappings;
using AppliedResearchAssociates.iAM.Domains;
using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL
{
    public class RemainingLifeLimitRepository : MSSQLRepository, IRemainingLifeLimitRepository
    {
        private static readonly bool IsRunningFromXUnit = AppDomain.CurrentDomain.GetAssemblies()
            .Any(a => a.FullName.ToLowerInvariant().StartsWith("xunit"));

        private readonly ICriterionLibraryRepository _criterionLibraryRepo;

        public RemainingLifeLimitRepository(ICriterionLibraryRepository criterionLibraryRepo, IAMContext context) : base(context) => _criterionLibraryRepo = criterionLibraryRepo ?? throw new ArgumentNullException(nameof(criterionLibraryRepo));

        public void CreateRemainingLifeLimitLibrary(string name, string simulationName)
        {
            if (!Context.Simulation.Any(_ => _.Name == simulationName))
            {
                throw new RowNotInTableException($"No simulation found having name {simulationName}");
            }

            var simulationEntity = Context.Simulation.Single(_ => _.Name == simulationName);

            var remainingLifeLimitLibraryEntity = new RemainingLifeLimitLibraryEntity { Id = Guid.NewGuid(), Name = name };

            Context.RemainingLifeLimitLibrary.Add(remainingLifeLimitLibraryEntity);

            Context.RemainingLifeLimitLibrarySimulation.Add(new RemainingLifeLimitLibrarySimulationEntity
            {
                RemainingLifeLimitLibraryId = remainingLifeLimitLibraryEntity.Id,
                SimulationId = simulationEntity.Id
            });
        }

        public void CreateRemainingLifeLimits(List<RemainingLifeLimit> remainingLifeLimits, string simulationName)
        {
            if (!Context.Simulation.Any(_ => _.Name == simulationName))
            {
                throw new RowNotInTableException($"No simulation found having name {simulationName}");
            }

            var simulationEntity = Context.Simulation
                .Include(_ => _.RemainingLifeLimitLibrarySimulationJoin)
                .Single(_ => _.Name == simulationName);

            var attributeNames = remainingLifeLimits.Select(_ => _.Attribute.Name).Distinct().ToList();
            var attributeEntities = Context.Attribute
                .Where(_ => attributeNames.Contains(_.Name)).ToList();

            if (!attributeEntities.Any())
            {
                throw new RowNotInTableException("Could not find matching attributes for given performance curves.");
            }

            var attributeNamesFromDataSource = attributeEntities.Select(_ => _.Name).ToList();
            if (!attributeNames.All(ruleName => attributeNamesFromDataSource.Contains(ruleName)))
            {
                var attributeNamesNotFound = attributeNames.Except(attributeNamesFromDataSource).ToList();
                if (attributeNamesNotFound.Count() == 1)
                {
                    throw new RowNotInTableException($"No attribute found having name {attributeNamesNotFound[0]}.");
                }

                throw new RowNotInTableException(
                    $"No attributes found found having names: {string.Join(", ", attributeNamesNotFound)}.");
            }

            var remainingLifeLimitEntityIdsPerExpression = new Dictionary<string, List<Guid>>();

            var remainingLifeLimitEntities = remainingLifeLimits
                .Select(_ =>
                {
                    var entity = _.ToEntity(
                        simulationEntity.RemainingLifeLimitLibrarySimulationJoin.RemainingLifeLimitLibraryId,
                        attributeEntities.Single(__ => __.Name == _.Attribute.Name).Id);

                    if (!_.Criterion.ExpressionIsBlank)
                    {
                        if (remainingLifeLimitEntityIdsPerExpression.ContainsKey(_.Criterion.Expression))
                        {
                            remainingLifeLimitEntityIdsPerExpression[_.Criterion.Expression].Add(entity.Id);
                        }
                        else
                        {
                            remainingLifeLimitEntityIdsPerExpression.Add(_.Criterion.Expression, new List<Guid>
                            {
                                entity.Id
                            });
                        }
                    }

                    return entity;
                })
                .ToList();

            if (IsRunningFromXUnit)
            {
                Context.RemainingLifeLimit.AddRange(remainingLifeLimitEntities);
            }
            else
            {
                Context.BulkInsert(remainingLifeLimitEntities);
            }

            Context.SaveChanges();

            if (remainingLifeLimitEntityIdsPerExpression.Values.Any())
            {
                _criterionLibraryRepo
                    .JoinEntitiesWithCriteria(remainingLifeLimitEntityIdsPerExpression, "RemainingLifeLimitEntity", simulationName);
            }
        }
    }
}
