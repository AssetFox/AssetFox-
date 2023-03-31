using AppliedResearchAssociates.iAM.TestHelpers;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests.Repositories;
using AppliedResearchAssociates.iAM.UnitTestsCore.TestUtils;
using BridgeCareCore.Services;
using BridgeCareCoreTests.Helpers;
using Xunit;

namespace BridgeCareCoreTests.Tests.Integration
{
    public class PerformanceCurvesServiceIntegrationTests
    {
        private PerformanceCurvesService CreatePerformanceCurvesService()
        {
            var hubService = HubServiceMocks.Default();
            var expressionValidationService = ExpressionValidationServiceMocks.EverythingIsValid();
            var service = new PerformanceCurvesService(TestHelper.UnitOfWork,
                hubService, expressionValidationService.Object);
            return service;
        }

        [Fact]
        public void ExportScenarioPerformanceCurves_SimulationInDb_FilenameStartsWithSimulationName()
        {
            AttributeTestSetup.CreateAttributes(TestHelper.UnitOfWork);
            NetworkTestSetup.CreateNetwork(TestHelper.UnitOfWork);
            var simulationId = Guid.NewGuid();
            var simulationName = RandomStrings.WithPrefix("Simulation");
            SimulationTestSetup.CreateSimulation(TestHelper.UnitOfWork, simulationId, simulationName);
            var service = CreatePerformanceCurvesService();

            var fileInfo = service.ExportScenarioPerformanceCurvesFile(simulationId);

            var filename = fileInfo.FileName;
            Assert.StartsWith(simulationName, filename);
        }
    }
}
