using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.TestHelpers;
using AppliedResearchAssociates.iAM.UnitTestsCore.TestUtils;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests
{
    public class TreatmentConsequenceRepositoryTests
    {
        public void GetTreatmentConsequencesByLibraryIdAndTreatmentName_Does()
        {
            var networkId = Guid.NewGuid();
            var treatmentLibraryId = Guid.NewGuid();
            var treatmentLibrary = TreatmentLibraryTestSetup.ModelForEntityInDb(TestHelper.UnitOfWork, treatmentLibraryId);
            var treatmentId = Guid.NewGuid();
            var treatmentName = RandomStrings.WithPrefix("treatment");
            var treatment = TreatmentTestSetup.ModelForSingleTreatmentOfLibraryInDb(
                TestHelper.UnitOfWork, treatmentLibraryId, treatmentId, treatmentName);
            //var consequence = TreatmentConsequenceTestSetup

            var result = TestHelper.UnitOfWork.TreatmentConsequenceRepo.GetTreatmentConsequencesByLibraryIdAndTreatmentName(
                treatmentLibraryId, treatmentName);
        }
    }
}
