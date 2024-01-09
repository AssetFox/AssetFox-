using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.TestHelpers;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests.Repositories;
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
            var treatmentCost = LibraryTreatmentCostTestSetup.ModelForEntityInDb(
                TestHelper.UnitOfWork, treatmentId, treatmentLibraryId);

            var costs = TestHelper.UnitOfWork.TreatmentCostRepo.GetTreatmentCostsWithEquationJoinsByLibraryIdAndTreatmentName(
                treatmentLibraryId, "treatment");

            var foundCost = costs.Single();
            ObjectAssertions.EquivalentExcluding(treatmentCost, foundCost, x => x.CriterionLibrary, x => x.Equation.Id);
        }

        [Fact]
        public void UpsertOrDeleteTreatmentCosts_TreatmentInDbWithLibrary_Does()
        {
            var networkId = Guid.NewGuid();
            var treatmentLibraryId = Guid.NewGuid();
            var treatmentLibrary = TreatmentLibraryTestSetup.ModelForEntityInDb(TestHelper.UnitOfWork, treatmentLibraryId);
            var treatmentId = Guid.NewGuid();
            var treatment = TreatmentTestSetup.ModelForSingleTreatmentOfLibraryInDb(
                TestHelper.UnitOfWork, treatmentLibraryId, treatmentId, "treatment");
            var treatmentCost = TreatmentCostDtos.Dto();
            var treatmentCosts = new List<TreatmentCostDTO> { treatmentCost };
            var costDictionary = new Dictionary<Guid, List<TreatmentCostDTO>> { { treatmentId, treatmentCosts } };

            TestHelper.UnitOfWork.TreatmentCostRepo.UpsertOrDeleteTreatmentCosts(costDictionary, treatmentLibraryId);

            var costsAfter = TestHelper.UnitOfWork.TreatmentCostRepo.GetTreatmentCostByTreatmentId(treatmentId);
            var costAfter = costsAfter.Single();
            ObjectAssertions.EquivalentExcluding(treatmentCost, costAfter, c => c.Equation, c => c.CriterionLibrary);
            ObjectAssertions.Equivalent(new EquationDTO(), costAfter.Equation);
            ObjectAssertions.Equivalent(new CriterionLibraryDTO(), costAfter.CriterionLibrary);
        }

        [Fact]
        public void UpsertOrDeleteTreatmentCosts_CostHasChildren_TreatmentInDbWithLibrary_Does()
        {
            var networkId = Guid.NewGuid();
            var treatmentLibraryId = Guid.NewGuid();
            var treatmentLibrary = TreatmentLibraryTestSetup.ModelForEntityInDb(TestHelper.UnitOfWork, treatmentLibraryId);
            var treatmentId = Guid.NewGuid();
            var treatment = TreatmentTestSetup.ModelForSingleTreatmentOfLibraryInDb(
                TestHelper.UnitOfWork, treatmentLibraryId, treatmentId, "treatment");
            var treatmentCost = TreatmentCostDtos.WithEquationAndCriterionLibrary();
            var treatmentCosts = new List<TreatmentCostDTO> { treatmentCost };
            var costDictionary = new Dictionary<Guid, List<TreatmentCostDTO>> { { treatmentId, treatmentCosts } };

            TestHelper.UnitOfWork.TreatmentCostRepo.UpsertOrDeleteTreatmentCosts(costDictionary, treatmentLibraryId);

            var costsAfter = TestHelper.UnitOfWork.TreatmentCostRepo.GetTreatmentCostByTreatmentId(treatmentId);
            var costAfter = costsAfter.Single();
            ObjectAssertions.EquivalentExcluding(treatmentCost, costAfter, tc => tc.CriterionLibrary.Name, tc => tc.CriterionLibrary.Id, tc => tc.CriterionLibrary.Owner, tc => tc.Equation.Id);
        }

        [Fact]
        public void GetTreatmentCostByScenarioTreatmentId_ScenarioTreatmentInDbWithCosts_Gets()
        {
            AttributeTestSetup.CreateAttributes(TestHelper.UnitOfWork);
            NetworkTestSetup.CreateNetwork(TestHelper.UnitOfWork);
            var simulationId = Guid.NewGuid();
            SimulationTestSetup.ModelForEntityInDb(TestHelper.UnitOfWork, simulationId, networkId: NetworkTestSetup.NetworkId);
            var treatmentId = Guid.NewGuid();
            TreatmentTestSetup.ModelForSingleTreatmentOfSimulationInDb(TestHelper.UnitOfWork, simulationId, treatmentId);
            var treatmentCost = ScenarioTreatmentCostTestSetup.CostForTreatmentInDb(TestHelper.UnitOfWork, treatmentId, simulationId);

            var treatmentCosts = TestHelper.UnitOfWork.TreatmentCostRepo.GetTreatmentCostByScenarioTreatmentId(treatmentId);

            var actual = treatmentCosts.Single();
            ObjectAssertions.EquivalentExcluding(treatmentCost, actual,
                tc => tc.CriterionLibrary.Id,
                tc => tc.CriterionLibrary.Name,
                tc => tc.CriterionLibrary.Owner,
                tc => tc.Equation.Id);
        }
    }
}
