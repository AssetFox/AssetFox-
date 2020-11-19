using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappings;
using AppliedResearchAssociates.iAM.Domains;
using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;
using MoreLinq;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL
{
    public class CashFlowRuleRepository : MSSQLRepository, ICashFlowRuleRepository
    {
        public static readonly bool IsRunningFromXUnit = AppDomain.CurrentDomain.GetAssemblies()
            .Any(a => a.FullName.ToLowerInvariant().StartsWith("xunit"));

        private readonly ICashFlowDistributionRuleRepository _cashFlowDistributionRuleRepo;
        private readonly ICriterionLibraryRepository _criterionLibraryRepo;

        public CashFlowRuleRepository(ICashFlowDistributionRuleRepository cashFlowDistributionRuleRepo,
            ICriterionLibraryRepository criterionLibraryRepo,
            IAMContext context) : base(context)
        {
            _cashFlowDistributionRuleRepo = cashFlowDistributionRuleRepo ??
                                            throw new ArgumentNullException(nameof(cashFlowDistributionRuleRepo));
            _criterionLibraryRepo = criterionLibraryRepo ??
                                    throw new ArgumentNullException(nameof(criterionLibraryRepo));
        }

        public void CreateCashFlowRuleLibrary(string name, string simulationName)
        {
            if (!Context.Simulation.Any(_ => _.Name == simulationName))
            {
                throw new RowNotInTableException($"No simulation found having name {simulationName}");
            }

            var simulationEntity = Context.Simulation.Single(_ => _.Name == simulationName);

            var cashFlowRuleLibraryEntity = new CashFlowRuleLibraryEntity {Id = Guid.NewGuid(), Name = name};

            Context.CashFlowRuleLibrary.Add(cashFlowRuleLibraryEntity);

            Context.CashFlowRuleLibrarySimulation.Add(new CashFlowRuleLibrarySimulationEntity
            {
                CashFlowRuleLibraryId = cashFlowRuleLibraryEntity.Id, SimulationId = simulationEntity.Id
            });

            Context.SaveChanges();
        }

        public void CreateCashFlowRules(List<CashFlowRule> cashFlowRules, string simulationName)
        {
            if (!Context.Simulation.Any(_ => _.Name == simulationName))
            {
                throw new RowNotInTableException($"No simulation found having name {simulationName}");
            }

            var simulationEntity = Context.Simulation
                .Include(_ => _.CashFlowRuleLibrarySimulationJoin)
                .Single(_ => _.Name == simulationName);

            var cashFlowRuleEntities = cashFlowRules
                .Select(_ => _.ToEntity(simulationEntity.CashFlowRuleLibrarySimulationJoin.CashFlowRuleLibraryId))
                .ToList();

            // save the data models in the data source
            if (IsRunningFromXUnit)
            {
                Context.CashFlowRule.AddRange(cashFlowRuleEntities);
            }
            else
            {
                Context.BulkInsert(cashFlowRuleEntities);
            }

            Context.SaveChanges();

            if (cashFlowRules.Any(_ => _.DistributionRules.Any()))
            {
                var distributionRulesPerCashFlowRuleEntityId = cashFlowRules
                    .Where(_ => _.DistributionRules.Any()).ToList()
                    .ToDictionary(_ => cashFlowRuleEntities.Single(__ => __.Name == _.Name).Id,
                        _ => _.DistributionRules.ToList());

                _cashFlowDistributionRuleRepo.CreateCashFlowDistributionRules(distributionRulesPerCashFlowRuleEntityId);
            }

            if (cashFlowRules.Any(_ => !_.Criterion.ExpressionIsBlank))
            {
                var cashFlowRuleEntityIdsPerExpression = cashFlowRules
                    .Where(_ => !_.Criterion.ExpressionIsBlank)
                    .GroupBy(_ => _.Criterion.Expression, _ => _)
                    .ToDictionary(_ => _.Key, _ =>
                    {
                        var cashFlowRuleNames = _.Select(__ => __.Name).ToList();
                        return cashFlowRuleEntities.Where(__ => cashFlowRuleNames.Contains(__.Name))
                            .Select(__ => __.Id).ToList();
                    });

                _criterionLibraryRepo.JoinEntitiesWithCriteria(cashFlowRuleEntityIdsPerExpression, "CashFlowRuleEntity", simulationName);
            }
        }
    }
}
