using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.Analysis.Engine;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests
{
    public static class BudgetUsageDetails
    {
        public static BudgetUsageDetail Detail(string budgetName)
        {
            var detail = new BudgetUsageDetail(budgetName)
            {
                CoveredCost = 12.34m,
                Status = BudgetUsageStatus.CostCoveredInFull,
            };
            return detail;
        }
    }
}
