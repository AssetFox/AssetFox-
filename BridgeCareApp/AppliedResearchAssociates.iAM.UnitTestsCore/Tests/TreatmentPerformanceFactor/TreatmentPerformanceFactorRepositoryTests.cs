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

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests.TreatmentPerformanceFactor
{
    public class TreatmentPerformanceFactorRepositoryTests
    {
        [Fact]
        public void UpsertScenarioTreatmentPerformanceFactors_Does()
        {
            AttributeTestSetup.CreateAttributes(TestHelper.UnitOfWork);
            NetworkTestSetup.CreateNetwork(TestHelper.UnitOfWork);
            var simulationId = Guid.NewGuid();
            var treatmentId = Guid.NewGuid();
            var simulation = SimulationTestSetup.ModelForEntityInDb(TestHelper.UnitOfWork, simulationId);
            var treatment = TreatmentTestSetup.ModelForSingleTreatmentOfSimulationInDb(
                TestHelper.UnitOfWork, simulationId, treatmentId);
            var performanceFactorId = Guid.NewGuid();
            var performanceFactor = TreatmentPerformanceFactorDtos.Dto("[AGE]", performanceFactorId);
            var performanceFactors = new List<TreatmentPerformanceFactorDTO> { performanceFactor };
            var dictionary = new Dictionary<Guid, List<TreatmentPerformanceFactorDTO>> { { treatmentId, performanceFactors } };

            TestHelper.UnitOfWork.TreatmentPerformanceFactorRepo.UpsertScenarioTreatmentPerformanceFactors(dictionary, simulationId);

            var treatmentAfter = TestHelper.UnitOfWork.SelectableTreatmentRepo.GetScenarioSelectableTreatmentById(treatmentId);
            var performanceFactorAfter = treatmentAfter.Treatment.PerformanceFactors.Single();
            ObjectAssertions.Equivalent(performanceFactor, performanceFactorAfter);
        }
    }
}
