using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.TestHelpers;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests.Repositories;
using AppliedResearchAssociates.iAM.UnitTestsCore.TestUtils;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using Xunit;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests
{
    public class TreatmentConsequenceRepositoryTests
    {
        [Fact]
        public void GetTreatmentConsequencesByLibraryIdAndTreatmentName_Does()
        {
            var attributeName = RandomStrings.WithPrefix("attribute");
            AttributeTestSetup.CreateSingleNumericAttribute(TestHelper.UnitOfWork, null, attributeName);
            var treatmentLibraryId = Guid.NewGuid();
            var treatmentLibrary = TreatmentLibraryTestSetup.ModelForEntityInDb(TestHelper.UnitOfWork, treatmentLibraryId);
            var treatmentId = Guid.NewGuid();
            var consequenceId = Guid.NewGuid();
            var treatmentName = RandomStrings.WithPrefix("treatment");
            var treatment = TreatmentTestSetup.ModelForSingleTreatmentOfLibraryInDb(
                TestHelper.UnitOfWork, treatmentLibraryId, treatmentId, treatmentName
                );
            var consequence = TreatmentConsequenceTestSetup.ModelForEntityInDb(
                TestHelper.UnitOfWork, treatmentLibraryId, treatmentId,
                consequenceId, attributeName);

            var result = TestHelper.UnitOfWork.TreatmentConsequenceRepo.GetTreatmentConsequencesByLibraryIdAndTreatmentName(
                treatmentLibraryId, treatmentName);

            var returnedConsequence = result.Single();
            consequence.Equation = new EquationDTO();
            consequence.CriterionLibrary = new CriterionLibraryDTO();
            ObjectAssertions.Equivalent(consequence, returnedConsequence);
        }
    }
}
