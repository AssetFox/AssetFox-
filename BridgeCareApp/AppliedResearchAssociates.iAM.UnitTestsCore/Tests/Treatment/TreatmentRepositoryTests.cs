using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DTOs.Enums;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests.User;
using AppliedResearchAssociates.iAM.UnitTestsCore.TestUtils;
using Xunit;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests.Repositories;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests.SelectableTreatment;
using AppliedResearchAssociates.iAM.TestHelpers;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests.Treatment
{
    public class TreatmentRepositoryTests
    {

        [Fact]
        public void GetDefaultTreatment_NoneIsDefined_ReturnsNull()
        {
            AttributeTestSetup.CreateAttributes(TestHelper.UnitOfWork);
            NetworkTestSetup.CreateNetwork(TestHelper.UnitOfWork);
            var simulationId = Guid.NewGuid();
            var simulation = SimulationTestSetup.ModelForEntityInDb(TestHelper.UnitOfWork, simulationId);

            var defaultTreatment = TestHelper.UnitOfWork.SelectableTreatmentRepo.GetDefaultTreatment(simulationId);

            Assert.Null(defaultTreatment);
        }

        [Fact]
        public void GetDefaultTreatment_OneIsDefined_ReturnsIt()
        {
            AttributeTestSetup.CreateAttributes(TestHelper.UnitOfWork);
            NetworkTestSetup.CreateNetwork(TestHelper.UnitOfWork);
            var simulationId = Guid.NewGuid();
            var simulation = SimulationTestSetup.ModelForEntityInDb(TestHelper.UnitOfWork, simulationId);
            var treatment = TreatmentDtos.DtoWithEmptyCostsAndConsequencesLists();
            var treatments = new List<TreatmentDTO> { treatment };
            TestHelper.UnitOfWork.SelectableTreatmentRepo.UpsertOrDeleteScenarioSelectableTreatment(treatments, simulationId);

            var defaultTreatment = TestHelper.UnitOfWork.SelectableTreatmentRepo.GetDefaultTreatment(simulationId);

            Assert.Equal(treatment.Id, defaultTreatment.Id);
            Assert.Equal(treatment.Name, defaultTreatment.Name);
            Assert.Equal(simulationId, defaultTreatment.SimulationId);
            Assert.Equal(treatment.AssetType.ToString(), defaultTreatment.AssetType.ToString());
        }


        [Fact]
        public void GetDefaultNoTreatment_NoneIsDefined_ReturnsNull()
        {
            AttributeTestSetup.CreateAttributes(TestHelper.UnitOfWork);
            NetworkTestSetup.CreateNetwork(TestHelper.UnitOfWork);
            var simulationId = Guid.NewGuid();
            var simulation = SimulationTestSetup.ModelForEntityInDb(TestHelper.UnitOfWork, simulationId);

            var defaultNoTreatment = TestHelper.UnitOfWork.SelectableTreatmentRepo.GetDefaultNoTreatment(simulationId);

            Assert.Null(defaultNoTreatment);
        }

        [Fact]
        public void GetDefaultNoTreatment_OneIsDefined_ReturnsIt()
        {
            AttributeTestSetup.CreateAttributes(TestHelper.UnitOfWork);
            NetworkTestSetup.CreateNetwork(TestHelper.UnitOfWork);
            var simulationId = Guid.NewGuid();
            var simulation = SimulationTestSetup.ModelForEntityInDb(TestHelper.UnitOfWork, simulationId);
            var treatment = TreatmentDtos.DtoWithEmptyCostsAndConsequencesLists();
            treatment.Budgets = new List<TreatmentBudgetDTO>();
            var treatments = new List<TreatmentDTO> { treatment };
            TestHelper.UnitOfWork.SelectableTreatmentRepo.UpsertOrDeleteScenarioSelectableTreatment(treatments, simulationId);

            var defaultNoTreatment = TestHelper.UnitOfWork.SelectableTreatmentRepo.GetDefaultNoTreatment(simulationId);

            ObjectAssertions.EquivalentExcluding(defaultNoTreatment, treatment, t => t.CriterionLibrary);
        }

        [Fact]
        public void AddDefaultPerformanceFactors_PerformanceFactorInDb_Adds()
        {
            AttributeTestSetup.CreateAttributes(TestHelper.UnitOfWork);
            NetworkTestSetup.CreateNetwork(TestHelper.UnitOfWork);
            var simulationId = Guid.NewGuid();
            var simulation = SimulationTestSetup.ModelForEntityInDb(TestHelper.UnitOfWork, simulationId);
            var curveId = Guid.NewGuid();
            var performanceCurveDto = ScenarioPerformanceCurveTestSetup.DtoForEntityInDb(
                TestHelper.UnitOfWork, simulationId, curveId, attribute: TestAttributeNames.SupDurationN);
            var treatment = TreatmentDtos.DtoWithEmptyCostsAndConsequencesLists();
            var treatments = new List<TreatmentDTO> { treatment };

            TestHelper.UnitOfWork.SelectableTreatmentRepo.AddDefaultPerformanceFactors(simulationId, treatments);

            var performanceFactorsAfter = treatment.PerformanceFactors;
            var performanceFactorAfter = performanceFactorsAfter.Single();
            var expectedPerformanceFactorAfter = new TreatmentPerformanceFactorDTO
            {
                Attribute = TestAttributeNames.SupDurationN,
                PerformanceFactor = 1,
            };
            ObjectAssertions.EquivalentExcluding(performanceFactorAfter, expectedPerformanceFactorAfter, pf => pf.Id);
        }
    }
}
