using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.Analysis.Engine;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests
{
    public static class TreatmentConsiderationDetails
    {
        public static TreatmentConsiderationDetail Detail(string treatmentName, string budgetName)
        {
            var detail = new TreatmentConsiderationDetail(treatmentName)
            {
                BudgetPriorityLevel = 10,
            };
            //var budgetUsageDetail = BudgetUsageDetails.Detail(budgetName);
            //detail.BudgetUsages.Add(budgetUsageDetail);
            var cashFlowConsiderationDetail = CashFlowConsiderationDetails.Detail();
            detail.CashFlowConsiderations.Add(cashFlowConsiderationDetail);
            return detail;
        }

        public static TreatmentConsiderationDetail Detail(SimulationOutputSetupContext setupContext)
        {
            var detail = Detail(setupContext.TreatmentName, setupContext.BudgetName);
            return detail;
        }
    }
}
