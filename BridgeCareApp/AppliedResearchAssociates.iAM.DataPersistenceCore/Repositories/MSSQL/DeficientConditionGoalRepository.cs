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
    public class DeficientConditionGoalRepository : MSSQLRepository, IDeficientConditionGoalRepository
    {
        private static readonly bool IsRunningFromXUnit = AppDomain.CurrentDomain.GetAssemblies()
            .Any(a => a.FullName.ToLowerInvariant().StartsWith("xunit"));

        private readonly ICriterionLibraryRepository _criterionLibraryRepo;

        public DeficientConditionGoalRepository(ICriterionLibraryRepository criterionLibraryRepo, IAMContext context) : base(context) =>
            _criterionLibraryRepo = criterionLibraryRepo ?? throw new ArgumentNullException(nameof(criterionLibraryRepo));

        public void CreateDeficientConditionGoalLibrary(string name, string simulationName)
        {
            if (!Context.Simulation.Any(_ => _.Name == simulationName))
            {
                throw new RowNotInTableException($"No simulation found having name {simulationName}");
            }

            var simulationEntity = Context.Simulation.Single(_ => _.Name == simulationName);

            var deficientConditionGoalLibraryEntity = new DeficientConditionGoalLibraryEntity { Id = Guid.NewGuid(), Name = name };

            Context.DeficientConditionGoalLibrary.Add(deficientConditionGoalLibraryEntity);

            Context.DeficientConditionGoalLibrarySimulation.Add(new DeficientConditionGoalLibrarySimulationEntity
            {
                DeficientConditionGoalLibraryId = deficientConditionGoalLibraryEntity.Id,
                SimulationId = simulationEntity.Id
            });

            Context.SaveChanges();
        }

        public void CreateDeficientConditionGoals(List<DeficientConditionGoal> deficientConditionGoals, string simulationName)
        {
            if (!Context.Simulation.Any(_ => _.Name == simulationName))
            {
                throw new RowNotInTableException($"No simulation found having name {simulationName}");
            }

            var simulationEntity = Context.Simulation
                .Include(_ => _.DeficientConditionGoalLibrarySimulationJoin)
                .Single(_ => _.Name == simulationName);

            var attributeNames = deficientConditionGoals.Select(_ => _.Attribute.Name).Distinct().ToList();
            var attributeEntities = Context.Attribute.Where(_ => attributeNames.Contains(_.Name)).ToList();

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

            var deficientConditionGoalEntities = deficientConditionGoals
                .Select(_ => _.ToEntity(simulationEntity.DeficientConditionGoalLibrarySimulationJoin.DeficientConditionGoalLibraryId,
                    attributeEntities.Single(__ => __.Name == _.Attribute.Name).Id))
                .ToList();

            if (IsRunningFromXUnit)
            {
                Context.DeficientConditionGoal.AddRange(deficientConditionGoalEntities);
            }
            else
            {
                Context.BulkInsert(deficientConditionGoalEntities);
            }

            Context.SaveChanges();

            if (deficientConditionGoals.Any(_ => !_.Criterion.ExpressionIsBlank))
            {
                var deficientConditionGoalEntityIdsPerExpression = deficientConditionGoals
                    .Where(_ => !_.Criterion.ExpressionIsBlank)
                    .GroupBy(_ => _.Criterion.Expression, _ => _)
                    .ToDictionary(_ => _.Key, _ =>
                    {
                        var deficientConditionGoalNames = _.Select(__ => __.Name).ToList();
                        return deficientConditionGoalEntities.Where(__ => deficientConditionGoalNames.Contains(__.Name))
                            .Select(__ => __.Id).ToList();
                    });

                _criterionLibraryRepo.JoinEntitiesWithCriteria(deficientConditionGoalEntityIdsPerExpression,
                    "DeficientConditionGoalEntity", simulationName);
            }
        }
    }
}
