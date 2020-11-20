﻿using System;
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
    public class TargetConditionGoalRepository : MSSQLRepository, ITargetConditionGoalRepository
    {
        private static readonly bool IsRunningFromXUnit = AppDomain.CurrentDomain.GetAssemblies()
            .Any(a => a.FullName.ToLowerInvariant().StartsWith("xunit"));

        private readonly ICriterionLibraryRepository _criterionLibraryRepo;

        public TargetConditionGoalRepository(ICriterionLibraryRepository criterionLibraryRepo, IAMContext context) : base(context) =>
            _criterionLibraryRepo = criterionLibraryRepo ?? throw new ArgumentNullException(nameof(criterionLibraryRepo));

        public void CreateTargetConditionGoalLibrary(string name, string simulationName)
        {
            if (!Context.Simulation.Any(_ => _.Name == simulationName))
            {
                throw new RowNotInTableException($"No simulation found having name {simulationName}");
            }

            var simulationEntity = Context.Simulation.Single(_ => _.Name == simulationName);

            var targetConditionGoalLibraryEntity = new TargetConditionGoalLibraryEntity { Id = Guid.NewGuid(), Name = name };

            Context.TargetConditionGoalLibrary.Add(targetConditionGoalLibraryEntity);

            Context.TargetConditionGoalLibrarySimulation.Add(new TargetConditionGoalLibrarySimulationEntity
            {
                TargetConditionGoalLibraryId = targetConditionGoalLibraryEntity.Id,
                SimulationId = simulationEntity.Id
            });

            Context.SaveChanges();
        }

        public void CreateTargetConditionGoals(List<TargetConditionGoal> targetConditionGoals, string simulationName)
        {
            if (!Context.Simulation.Any(_ => _.Name == simulationName))
            {
                throw new RowNotInTableException($"No simulation found having name {simulationName}");
            }

            var simulationEntity = Context.Simulation
                .Include(_ => _.TargetConditionGoalLibrarySimulationJoin)
                .Single(_ => _.Name == simulationName);

            var attributeNames = targetConditionGoals.Select(_ => _.Attribute.Name).Distinct().ToList();
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

            var targetConditionGoalEntities = targetConditionGoals
                .Select(_ => _.ToEntity(simulationEntity.TargetConditionGoalLibrarySimulationJoin.TargetConditionGoalLibraryId,
                    attributeEntities.Single(__ => __.Name == _.Attribute.Name).Id))
                .ToList();

            if (IsRunningFromXUnit)
            {
                Context.TargetConditionGoal.AddRange(targetConditionGoalEntities);
            }
            else
            {
                Context.BulkInsert(targetConditionGoalEntities);
            }

            Context.SaveChanges();

            if (targetConditionGoals.Any(_ => !_.Criterion.ExpressionIsBlank))
            {
                var targetConditionGoalEntityIdsPerExpression = targetConditionGoals
                    .Where(_ => !_.Criterion.ExpressionIsBlank)
                    .GroupBy(_ => _.Criterion.Expression, _ => _)
                    .ToDictionary(_ => _.Key, _ =>
                    {
                        var targetConditionGoalNames = _.Select(__ => __.Name).ToList();
                        return targetConditionGoalEntities.Where(__ => targetConditionGoalNames.Contains(__.Name))
                            .Select(__ => __.Id).ToList();
                    });

                _criterionLibraryRepo.JoinEntitiesWithCriteria(targetConditionGoalEntityIdsPerExpression,
                    "TargetConditionGoalEntity", simulationName);
            }
        }
    }
}
