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
    public class RemainingLifeLimitRepository : IRemainingLifeLimitRepository
    {
        private static readonly bool IsRunningFromXUnit = AppDomain.CurrentDomain.GetAssemblies()
            .Any(a => a.FullName.ToLowerInvariant().StartsWith("xunit"));

        private readonly ICriterionLibraryRepository _criterionLibraryRepo;
        private readonly IAMContext _context;

        public RemainingLifeLimitRepository(ICriterionLibraryRepository criterionLibraryRepo, IAMContext context)
        {
            _criterionLibraryRepo =
                criterionLibraryRepo ?? throw new ArgumentNullException(nameof(criterionLibraryRepo));
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public void CreateRemainingLifeLimitLibrary(string name, Guid simulationId)
        {
            if (!_context.Simulation.Any(_ => _.Id == simulationId))
            {
                throw new RowNotInTableException($"No simulation found having id {simulationId}");
            }

            var remainingLifeLimitLibraryEntity = new RemainingLifeLimitLibraryEntity { Id = Guid.NewGuid(), Name = name };

            _context.RemainingLifeLimitLibrary.Add(remainingLifeLimitLibraryEntity);

            _context.RemainingLifeLimitLibrarySimulation.Add(new RemainingLifeLimitLibrarySimulationEntity
            {
                RemainingLifeLimitLibraryId = remainingLifeLimitLibraryEntity.Id, SimulationId = simulationId
            });
        }

        public void CreateRemainingLifeLimits(List<RemainingLifeLimit> remainingLifeLimits, Guid simulationId)
        {
            if (!_context.Simulation.Any(_ => _.Id == simulationId))
            {
                throw new RowNotInTableException($"No simulation found having id {simulationId}");
            }

            var simulationEntity = _context.Simulation
                .Include(_ => _.RemainingLifeLimitLibrarySimulationJoin)
                .Single(_ => _.Id == simulationId);

            var attributeNames = remainingLifeLimits.Select(_ => _.Attribute.Name).Distinct().ToList();
            var attributeEntities = _context.Attribute
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

            var remainingLifeLimitEntities = remainingLifeLimits
                .Select(_ => _.ToEntity(simulationEntity.RemainingLifeLimitLibrarySimulationJoin.RemainingLifeLimitLibraryId,
                    attributeEntities.Single(__ => __.Name == _.Attribute.Name).Id))
                .ToList();

            if (IsRunningFromXUnit)
            {
                _context.RemainingLifeLimit.AddRange(remainingLifeLimitEntities);
            }
            else
            {
                _context.BulkInsert(remainingLifeLimitEntities);
            }

            if (remainingLifeLimits.Any(_ => !_.Criterion.ExpressionIsBlank))
            {
                var limitIdsPerExpression = remainingLifeLimits
                    .Where(_ => !_.Criterion.ExpressionIsBlank)
                    .GroupBy(_ => _.Criterion.Expression, _ => _)
                    .ToDictionary(_ => _.Key, _ => _.Select(__ => __.Id).ToList());

                _criterionLibraryRepo.JoinEntitiesWithCriteria(limitIdsPerExpression, "RemainingLifeLimitEntity", simulationEntity.Name);
            }
        }
    }
}
