using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.TestHelpers;
using AppliedResearchAssociates.iAM.UnitTestsCore.TestUtils;
using Xunit;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests.TreatmentCost
{
    public class TreatmentCostRepositoryTests
    {

        [Fact]
        public void GetTreatmentCostsWithEquationJoinsByLibraryIdAndTreatmentName_ObjectsInDb_GetsTheCost()
        {
            var networkId = Guid.NewGuid();
            var treatmentLibraryId = Guid.NewGuid();
            var treatmentLibrary = TreatmentLibraryTestSetup.ModelForEntityInDb(TestHelper.UnitOfWork, treatmentLibraryId);
            var treatmentId = Guid.NewGuid();
            var treatment = TreatmentTestSetup.ModelForSingleTreatmentOfLibraryInDb(
                TestHelper.UnitOfWork, treatmentLibraryId, treatmentId, "treatment");
            var treatmentCost = TreatmentCostTestSetup.ModelForEntityInDb(
                TestHelper.UnitOfWork, treatmentId, treatmentLibraryId);

            var costs = TestHelper.UnitOfWork.TreatmentCostRepo.GetTreatmentCostsWithEquationJoinsByLibraryIdAndTreatmentName(
                treatmentLibraryId, "treatment");

            var foundCost = costs.Single();
            ObjectAssertions.EquivalentExcluding(treatmentCost, foundCost, x => x.CriterionLibrary, x => x.Equation.Id);
        }
    }
}
