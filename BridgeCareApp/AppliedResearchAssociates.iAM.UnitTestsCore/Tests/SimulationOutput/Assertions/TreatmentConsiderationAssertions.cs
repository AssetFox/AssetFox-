using AppliedResearchAssociates.iAM.Analysis.Engine;
using AppliedResearchAssociates.iAM.Common;
using Xunit;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests
{
    internal class TreatmentConsiderationAssertions
    {
        public static void Same(TreatmentConsiderationDetail expected, TreatmentConsiderationDetail actual)
        {
            Assert.Equal(expected.TreatmentName, actual.TreatmentName);
            Assert.Equal(expected.BudgetPriorityLevel, actual.BudgetPriorityLevel);
            Assert.Equal(expected.BudgetUsages.Count, actual.BudgetUsages.Count);
            Assert.Equal(expected.CashFlowConsiderations.Count, actual.CashFlowConsiderations.Count);
            expected.BudgetUsages.Sort(bu => bu.BudgetName);
            actual.BudgetUsages.Sort(bu => bu.BudgetName);
            for (int i=0; i<expected.BudgetUsages.Count; i++)
            {
                BudgetUsageAssertions.Same(expected.BudgetUsages[i], actual.BudgetUsages[i]);
            }
            expected.CashFlowConsiderations.Sort(cfc => cfc.CashFlowRuleName + cfc.ReasonAgainstCashFlow);
            actual.CashFlowConsiderations.Sort(cfc => cfc.CashFlowRuleName + cfc.ReasonAgainstCashFlow);
            for (int i=0; i<actual.CashFlowConsiderations.Count; i++)
            {
                CashFlowConsiderationAssertions.Same(expected.CashFlowConsiderations[i], actual.CashFlowConsiderations[i]);
            }
        }
    }
}
