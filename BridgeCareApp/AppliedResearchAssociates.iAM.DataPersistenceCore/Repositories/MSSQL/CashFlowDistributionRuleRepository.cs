using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.ScenarioEntities.CashFlow;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Extensions;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.Domains;
using AppliedResearchAssociates.iAM.DTOs;
using Microsoft.EntityFrameworkCore;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL
{
    public class CashFlowDistributionRuleRepository : ICashFlowDistributionRuleRepository
    {
        private readonly UnitOfDataPersistenceWork _unitOfWork;

        public CashFlowDistributionRuleRepository(UnitOfDataPersistenceWork unitOfWork) =>
            _unitOfWork = unitOfWork ??
                                         throw new ArgumentNullException(nameof(unitOfWork));


        public void UpsertOrDeleteCashFlowDistributionRules(
            Dictionary<Guid, List<CashFlowDistributionRuleDTO>> distributionRulesPerCashFlowRuleId, Guid libraryId)
        {
            var cashFlowDistributionRuleEntities = distributionRulesPerCashFlowRuleId
                .SelectMany(_ => _.Value.Select(distributionRule => distributionRule.ToLibraryEntity(_.Key)))
                .ToList();

            var entityIds = cashFlowDistributionRuleEntities.Select(_ => _.Id).ToList();

            var existingEntityIds = _unitOfWork.Context.CashFlowDistributionRule.AsNoTracking()
                .Where(_ => _.CashFlowRule.CashFlowRuleLibraryId == libraryId && entityIds.Contains(_.Id))
                .Select(_ => _.Id).ToList();

            _unitOfWork.Context
                .DeleteAll<CashFlowDistributionRuleEntity>(_ => _.CashFlowRule.CashFlowRuleLibraryId == libraryId &&
                                                                !entityIds.Contains(_.Id));

            _unitOfWork.Context
                .UpdateAll(cashFlowDistributionRuleEntities.Where(_ => existingEntityIds.Contains(_.Id)).ToList(), _unitOfWork.UserEntity?.Id);

            _unitOfWork.Context
                .AddAll(cashFlowDistributionRuleEntities.Where(_ => !existingEntityIds.Contains(_.Id)).ToList(), _unitOfWork.UserEntity?.Id);
        }

        public void UpsertOrDeleteScenarioCashFlowDistributionRules(
            Dictionary<Guid, List<CashFlowDistributionRuleDTO>> distributionRulesPerCashFlowRuleId, Guid simulationId)
        {
            var cashFlowDistributionRuleEntities = distributionRulesPerCashFlowRuleId
                .SelectMany(_ => _.Value.Select(distributionRule => distributionRule.ToScenarioEntity(_.Key)))
                .ToList();

            var entityIds = cashFlowDistributionRuleEntities.Select(_ => _.Id).ToList();

            var existingEntityIds = _unitOfWork.Context.ScenarioCashFlowDistributionRule.AsNoTracking()
                .Where(_ => _.ScenarioCashFlowRule.SimulationId == simulationId && entityIds.Contains(_.Id))
                .Select(_ => _.Id).ToList();

            _unitOfWork.Context
                .DeleteAll<ScenarioCashFlowDistributionRuleEntity>(_ => _.ScenarioCashFlowRule.SimulationId == simulationId &&
                                                                !entityIds.Contains(_.Id));

            _unitOfWork.Context
                .UpdateAll(cashFlowDistributionRuleEntities.Where(_ => existingEntityIds.Contains(_.Id)).ToList(), _unitOfWork.UserEntity?.Id);

            _unitOfWork.Context
                .AddAll(cashFlowDistributionRuleEntities.Where(_ => !existingEntityIds.Contains(_.Id)).ToList(), _unitOfWork.UserEntity?.Id);
        }
    }
}
