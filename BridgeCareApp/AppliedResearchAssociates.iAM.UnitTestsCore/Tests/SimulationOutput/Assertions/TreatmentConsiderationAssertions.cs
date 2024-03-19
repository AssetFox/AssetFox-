using AppliedResearchAssociates.iAM.Analysis.Engine;
using Xunit;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests
{
    internal class TreatmentConsiderationAssertions
    {
        public static void Same(TreatmentConsiderationDetail expected, TreatmentConsiderationDetail actual)
        {
            Assert.Equal(expected.TreatmentName, actual.TreatmentName);
            Assert.Equal(expected.BudgetPriorityLevel, actual.BudgetPriorityLevel);

            Assert.Equal(expected.FundingCalculationInput, actual.FundingCalculationInput);
            Assert.Equal(expected.FundingCalculationOutput, actual.FundingCalculationOutput);

            Assert.Equivalent(expected.CashFlowConsiderations, actual.CashFlowConsiderations, true);
        }
    }
}
