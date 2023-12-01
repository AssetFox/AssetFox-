using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.LibraryEntities.CashFlow;
using AppliedResearchAssociates.iAM.DTOs;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests.CashFlowRule
{
    public static class CashFlowDistributionRuleDtos
    {
        public static CashFlowDistributionRuleDTO Dto(Guid? id = null, int costCeiling = 500000)
        {
            var resolveId = id ?? Guid.NewGuid();
            var dto = new CashFlowDistributionRuleDTO
            {
                Id = resolveId,
                DurationInYears = 1,
                CostCeiling = costCeiling,
                YearlyPercentages = "100"
            };
            return dto;
        }
    }
}
