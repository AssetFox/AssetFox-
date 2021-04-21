using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Extensions;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers;
using AppliedResearchAssociates.iAM.Domains;
using AppliedResearchAssociates.iAM.DTOs;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL
{
    public class CashFlowDistributionRuleRepository : ICashFlowDistributionRuleRepository
    {
        public static readonly bool IsRunningFromXUnit = AppDomain.CurrentDomain.GetAssemblies()
            .Any(a => a.FullName.ToLowerInvariant().StartsWith("xunit"));

        private readonly UnitOfWork.UnitOfDataPersistenceWork _unitOfWork;

        public CashFlowDistributionRuleRepository(UnitOfWork.UnitOfDataPersistenceWork unitOfWork) =>
            _unitOfWork = unitOfWork ??
                                         throw new ArgumentNullException(nameof(unitOfWork));

        public void CreateCashFlowDistributionRules(Dictionary<Guid, List<CashFlowDistributionRule>> distributionRulesPerCashFlowRuleEntityId)
        {
            var cashFlowDistributionRuleEntities = distributionRulesPerCashFlowRuleEntityId
                .SelectMany(_ => _.Value.Select((__, index) => __.ToEntity(_.Key, ++index)))
                .ToList();

            _unitOfWork.Context.AddAll(cashFlowDistributionRuleEntities);
        }

        public void UpsertOrDeleteCashFlowDistributionRules(
            Dictionary<Guid, List<CashFlowDistributionRuleDTO>> distributionRulesPerCashFlowRuleId, Guid libraryId)
        {
            var cashFlowDistributionRuleEntities = distributionRulesPerCashFlowRuleId.SelectMany(_ => _.Value.Select(__ => __.ToEntity(_.Key)))
                .ToList();

            var entityIds = cashFlowDistributionRuleEntities.Select(_ => _.Id).ToList();

            var existingEntityIds = _unitOfWork.Context.CashFlowDistributionRule
                .Where(_ => _.CashFlowRule.CashFlowRuleLibraryId == libraryId && entityIds.Contains(_.Id))
                .Select(_ => _.Id).ToList();

            var predicatesPerCrudOperation = new Dictionary<string, Expression<Func<CashFlowDistributionRuleEntity, bool>>>
            {
                {"delete", _ => _.CashFlowRule.CashFlowRuleLibraryId == libraryId && !entityIds.Contains(_.Id)},
                {"update", _ => existingEntityIds.Contains(_.Id)},
                {"add", _ => !existingEntityIds.Contains(_.Id)}
            };

            _unitOfWork.Context.BulkUpsertOrDelete(cashFlowDistributionRuleEntities, predicatesPerCrudOperation, _unitOfWork.UserEntity?.Id);
        }
    }
}
