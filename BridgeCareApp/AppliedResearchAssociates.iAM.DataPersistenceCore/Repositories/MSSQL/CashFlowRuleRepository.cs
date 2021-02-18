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
    public class CashFlowRuleRepository : ICashFlowRuleRepository
    {
        public static readonly bool IsRunningFromXUnit = AppDomain.CurrentDomain.GetAssemblies()
            .Any(a => a.FullName.ToLowerInvariant().StartsWith("xunit"));

        private readonly UnitOfWork.UnitOfDataPersistenceWork _unitOfDataPersistenceWork;

        public CashFlowRuleRepository(UnitOfWork.UnitOfDataPersistenceWork unitOfDataPersistenceWork)
        {
            _unitOfDataPersistenceWork = unitOfDataPersistenceWork ?? throw new ArgumentNullException(nameof(unitOfDataPersistenceWork));
        }

        public void CreateCashFlowRuleLibrary(string name, Guid simulationId)
        {
            if (!_unitOfDataPersistenceWork.Context.Simulation.Any(_ => _.Id == simulationId))
            {
                throw new RowNotInTableException($"No simulation found having id {simulationId}.");
            }

            var cashFlowRuleLibraryEntity = new CashFlowRuleLibraryEntity {Id = Guid.NewGuid(), Name = name};

            _unitOfDataPersistenceWork.Context.CashFlowRuleLibrary.Add(cashFlowRuleLibraryEntity);

            _unitOfDataPersistenceWork.Context.CashFlowRuleLibrarySimulation.Add(new CashFlowRuleLibrarySimulationEntity
            {
                CashFlowRuleLibraryId = cashFlowRuleLibraryEntity.Id, SimulationId = simulationId
            });

            _unitOfDataPersistenceWork.Context.SaveChanges();
        }

        public void CreateCashFlowRules(List<CashFlowRule> cashFlowRules, Guid simulationId)
        {
            if (!_unitOfDataPersistenceWork.Context.Simulation.Any(_ => _.Id == simulationId))
            {
                throw new RowNotInTableException($"No simulation found having id {simulationId}.");
            }

            var simulationEntity = _unitOfDataPersistenceWork.Context.Simulation
                .Include(_ => _.CashFlowRuleLibrarySimulationJoin)
                .Single(_ => _.Id == simulationId);

            if (simulationEntity.CashFlowRuleLibrarySimulationJoin == null)
            {
                throw new RowNotInTableException($"No cash flow rule library found for simulation having id {simulationId}.");
            }

            var cashFlowRuleEntities = cashFlowRules
                .Select(_ => _.ToEntity(simulationEntity.CashFlowRuleLibrarySimulationJoin.CashFlowRuleLibraryId))
                .ToList();

            if (IsRunningFromXUnit)
            {
                _unitOfDataPersistenceWork.Context.CashFlowRule.AddRange(cashFlowRuleEntities);
            }
            else
            {
                _unitOfDataPersistenceWork.Context.BulkInsert(cashFlowRuleEntities);
            }

            _unitOfDataPersistenceWork.Context.SaveChanges();

            if (cashFlowRules.Any(_ => _.DistributionRules.Any()))
            {
                var distributionRulesPerCashFlowRuleId = cashFlowRules
                    .Where(_ => _.DistributionRules.Any())
                    .ToDictionary(_ => _.Id, _ => _.DistributionRules.ToList());

                _unitOfDataPersistenceWork.CashFlowDistributionRuleRepo.CreateCashFlowDistributionRules(distributionRulesPerCashFlowRuleId);
            }

            if (cashFlowRules.Any(_ => !_.Criterion.ExpressionIsBlank))
            {
                var cashFlowRuleEntityIdsPerExpression = cashFlowRules
                    .Where(_ => !_.Criterion.ExpressionIsBlank)
                    .GroupBy(_ => _.Criterion.Expression, _ => _.Id)
                    .ToDictionary(_ => _.Key, _ => _.ToList());

                _unitOfDataPersistenceWork.CriterionLibraryRepo.JoinEntitiesWithCriteria(cashFlowRuleEntityIdsPerExpression, "CashFlowRuleEntity", simulationEntity.Name);
            }
        }
    }
}
