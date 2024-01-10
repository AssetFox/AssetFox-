using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.TestHelpers;
using AppliedResearchAssociates.iAM.UnitTestsCore.Assertions;
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
            var equationId = Guid.NewGuid();
            var consequence = LibraryTreatmentConsequenceTestSetup.ModelForEntityInDb(
                TestHelper.UnitOfWork, treatmentLibraryId, treatmentId,
                consequenceId, attributeName, equationId);

            var result = TestHelper.UnitOfWork.TreatmentConsequenceRepo.GetTreatmentConsequencesByLibraryIdAndTreatmentName(
                treatmentLibraryId, treatmentName);

            var returnedConsequence = result.Single();
            ObjectAssertions.EquivalentExcluding(consequence, returnedConsequence, x => x.CriterionLibrary, x => x.Equation);
            CriterionLibraryDtoAssertions.AssertValidUpsertResult(consequence.CriterionLibrary, returnedConsequence.CriterionLibrary);
        }


        [Fact]
        public void GetTreatmentConsequencesByLibraryIdAndTreatmentName_DoesNotGetEquation()
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
            var consequence = LibraryTreatmentConsequenceTestSetup.ModelForEntityInDb(
                TestHelper.UnitOfWork, treatmentLibraryId, treatmentId,
                consequenceId, attributeName);
            Assert.NotEqual(Guid.Empty, consequence.Equation.Id);

            var result = TestHelper.UnitOfWork.TreatmentConsequenceRepo.GetTreatmentConsequencesByLibraryIdAndTreatmentName(
                treatmentLibraryId, treatmentName);

            var returnedConsequence = result.Single();
            var returnedEquation = returnedConsequence.Equation;
            ObjectAssertions.Equivalent(new EquationDTO(), returnedEquation);
        }

        [Fact]
        public void GetTreatmentConsequencesByTreatmentId_ConsequenceInDbWithTreatment_Gets()
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
            var consequence = LibraryTreatmentConsequenceTestSetup.ModelForEntityInDb(
                TestHelper.UnitOfWork, treatmentLibraryId, treatmentId,
                consequenceId, attributeName);

            var actual = TestHelper.UnitOfWork.TreatmentConsequenceRepo.GetTreatmentConsequencesByTreatmentId(treatmentId);

            var actualConsequence = actual.Single();
            ObjectAssertions.EquivalentExcluding(consequence, actualConsequence, c => c.CriterionLibrary, c => c.Equation.Id);
            CriterionLibraryDtoAssertions.AssertValidUpsertResult(consequence.CriterionLibrary, actualConsequence.CriterionLibrary);
        }

        [Fact]
        public void GetScenarioTreatmentConsequencesByTreatmentId_ConsequenceInDbWithTreatment_Gets()
        {
            var attributeName = RandomStrings.WithPrefix("attribute");
            AttributeTestSetup.CreateAttributes(TestHelper.UnitOfWork);
            NetworkTestSetup.CreateNetwork(TestHelper.UnitOfWork);
            AttributeTestSetup.CreateSingleNumericAttribute(TestHelper.UnitOfWork, null, attributeName);
            var simulationId = Guid.NewGuid();
            var simulation = SimulationTestSetup.ModelForEntityInDb(TestHelper.UnitOfWork, simulationId, networkId: NetworkTestSetup.NetworkId);
            var consequenceId = Guid.NewGuid();
            var treatmentName = RandomStrings.WithPrefix("treatment");
            var treatment = TreatmentTestSetup.ModelForSingleTreatmentOfSimulationInDb(TestHelper.UnitOfWork, simulationId);
            var treatmentId = treatment.Id;
            var consequence = ScenarioTreatmentConsequenceTestSetup.ModelForEntityInDb(
                TestHelper.UnitOfWork, simulationId, treatmentId,
                consequenceId, attributeName);

            var actual = TestHelper.UnitOfWork.TreatmentConsequenceRepo.GetScenarioTreatmentConsequencesByTreatmentId(treatmentId);

            var actualConsequence = actual.Single();
            ObjectAssertions.EquivalentExcluding(consequence, actualConsequence, c => c.CriterionLibrary, c => c.Equation);
            CriterionLibraryDtoAssertions.AssertValidUpsertResult(consequence.CriterionLibrary, actualConsequence.CriterionLibrary);
        }
    }
}
