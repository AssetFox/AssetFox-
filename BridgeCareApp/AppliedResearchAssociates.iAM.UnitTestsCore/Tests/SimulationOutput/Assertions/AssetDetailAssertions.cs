using System;
using AppliedResearchAssociates.iAM.Analysis.Engine;
using AppliedResearchAssociates.iAM.Common;
using Xunit;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests
{
    public static class AssetDetailAssertions
    {
        internal static void Same(AssetDetail expected, AssetDetail actual)
        {
            Assert.Equal(expected.AppliedTreatment, actual.AppliedTreatment);
            Assert.Equal(expected.TreatmentStatus, actual.TreatmentStatus);
            Assert.Equal(expected.TreatmentCause, actual.TreatmentCause);
            Assert.Equal(expected.TreatmentFundingIgnoresSpendingLimit, actual.TreatmentFundingIgnoresSpendingLimit);
            Assert.Equal(expected.TreatmentStatus, actual.TreatmentStatus);
            Assert.Equal(expected.TreatmentOptions.Count, actual.TreatmentOptions.Count);
            expected.TreatmentOptions.Sort(to => to.TreatmentName);
            actual.TreatmentOptions.Sort(to => to.TreatmentName);
            for (int i=0; i< expected.TreatmentOptions.Count; i++)
            {
                TreatmentOptionAssertions.Same(expected.TreatmentOptions[i], actual.TreatmentOptions[i]);
            }
            Assert.Equal(expected.TreatmentSchedulingCollisions.Count, actual.TreatmentSchedulingCollisions.Count);
            expected.TreatmentSchedulingCollisions.Sort(tsc => tsc.NameOfUnscheduledTreatment);
            actual.TreatmentSchedulingCollisions.Sort(tsc => tsc.NameOfUnscheduledTreatment);
            for (int i=0; i<expected.TreatmentSchedulingCollisions.Count; i++)
            {
                TreatmentSchedulingCollisionAssertions.Same(expected.TreatmentSchedulingCollisions[i], actual.TreatmentSchedulingCollisions[i]);
            }
            Assert.Equal(expected.TreatmentConsiderations.Count, actual.TreatmentConsiderations.Count);
            expected.TreatmentConsiderations.Sort(tc => tc.TreatmentName + tc.BudgetPriorityLevel);
            actual.TreatmentConsiderations.Sort(tc => tc.TreatmentName + tc.BudgetPriorityLevel);
            for (int i=0; i<expected.TreatmentConsiderations.Count; i++)
            {
                TreatmentConsiderationAssertions.Same(expected.TreatmentConsiderations[i], actual.TreatmentConsiderations[i]);
            }
            AssetSummaryDetailAssertions.Same(expected, actual);
       }
    }
}
