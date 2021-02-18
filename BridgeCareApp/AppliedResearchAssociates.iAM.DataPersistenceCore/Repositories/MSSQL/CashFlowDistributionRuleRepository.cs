using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappings;
using AppliedResearchAssociates.iAM.Domains;
using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore.Internal;
using MoreLinq;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL
{
    public class CashFlowDistributionRuleRepository : ICashFlowDistributionRuleRepository
    {
        public static readonly bool IsRunningFromXUnit = AppDomain.CurrentDomain.GetAssemblies()
            .Any(a => a.FullName.ToLowerInvariant().StartsWith("xunit"));

        private readonly UnitOfWork.UnitOfDataPersistenceWork _unitOfDataPersistenceWork;

        public CashFlowDistributionRuleRepository(UnitOfWork.UnitOfDataPersistenceWork unitOfDataPersistenceWork) => _unitOfDataPersistenceWork = unitOfDataPersistenceWork ?? throw new ArgumentNullException(nameof(unitOfDataPersistenceWork));

        public void CreateCashFlowDistributionRules(Dictionary<Guid, List<CashFlowDistributionRule>> distributionRulesPerCashFlowRuleEntityId)
        {
            var cashFlowDistributionRuleEntities = distributionRulesPerCashFlowRuleEntityId
                .SelectMany(_ => _.Value.Select((__, index) => __.ToEntity(_.Key, ++index)))
                .ToList();

            if (IsRunningFromXUnit)
            {
                _unitOfDataPersistenceWork.Context.CashFlowDistributionRule.AddRange(cashFlowDistributionRuleEntities);
            }
            else
            {
                _unitOfDataPersistenceWork.Context.BulkInsert(cashFlowDistributionRuleEntities);
            }

            _unitOfDataPersistenceWork.Context.SaveChanges();
        }
    }
}
