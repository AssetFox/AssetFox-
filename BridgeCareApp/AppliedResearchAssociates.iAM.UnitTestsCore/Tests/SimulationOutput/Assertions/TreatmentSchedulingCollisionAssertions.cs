using AppliedResearchAssociates.iAM.Analysis.Engine;
using Xunit;

namespace AppliedResearchAssociates.iAM.UnitTestsCore
{
    internal class TreatmentSchedulingCollisionAssertions
    {
        public static void Same(TreatmentSchedulingCollisionDetail expected, TreatmentSchedulingCollisionDetail actual)
        {
            Assert.Equal(expected.NameOfUnscheduledTreatment, actual.NameOfUnscheduledTreatment);
            Assert.Equal(expected.Year, actual.Year);
        }
    }
}
