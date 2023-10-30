using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.TestHelpers;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests.Repositories;
using AppliedResearchAssociates.iAM.UnitTestsCore.TestUtils;
using BridgeCareCore.Services;
using BridgeCareCoreTests.Helpers;
using OfficeOpenXml;
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

        [Fact]
        public void ExportScenarioPerformanceCurves_SimulationInDbWithPerformanceCurve_ThenUpload_SamePerformanceCurve()
        {
            AttributeTestSetup.CreateAttributes(TestHelper.UnitOfWork);
            NetworkTestSetup.CreateNetwork(TestHelper.UnitOfWork);
            var simulationId = Guid.NewGuid();
            var simulationName = RandomStrings.WithPrefix("Simulation");
            SimulationTestSetup.CreateSimulation(TestHelper.UnitOfWork, simulationId, simulationName);
            var curveId = Guid.NewGuid();
            var curve = ScenarioPerformanceCurveTestSetup.DtoForEntityInDb(TestHelper.UnitOfWork,
                simulationId, curveId, equation: "[AGE]");
            var service = CreatePerformanceCurvesService();
            var fileInfo = service.ExportScenarioPerformanceCurvesFile(simulationId);
            var dataAsString = fileInfo.FileData;
            var bytes = Convert.FromBase64String(dataAsString);
            var stream = new MemoryStream(bytes);
            var curves1 = TestHelper.UnitOfWork.PerformanceCurveRepo.GetScenarioPerformanceCurves(simulationId);
            TestHelper.UnitOfWork.PerformanceCurveRepo.UpsertOrDeleteScenarioPerformanceCurves(
                new List<PerformanceCurveDTO>(), simulationId);
            var curves2 = TestHelper.UnitOfWork.PerformanceCurveRepo.GetScenarioPerformanceCurves(simulationId);
            var excelPackage = new ExcelPackage(stream);
            var userCriteria = new UserCriteriaDTO();

            service.ImportScenarioPerformanceCurvesFile(simulationId, excelPackage, userCriteria);

            var curves3 = TestHelper.UnitOfWork.PerformanceCurveRepo.GetScenarioPerformanceCurves(simulationId);
            Assert.Empty(curves2);
            var curve1 = curves1.Single();
            var curve3 = curves3.Single();
            ObjectAssertions.EquivalentExcluding(curve1, curve3, c => c.Id, c => c.Equation.Id);
        }


        [Fact]
        public void ExportLibraryPerformanceCurves_SimulationInDbWithPerformanceCurve_ThenUpload_SamePerformanceCurve()
        {
            AttributeTestSetup.CreateAttributes(TestHelper.UnitOfWork);
            var libraryId = Guid.NewGuid();
            var library = PerformanceCurveLibraryTestSetup.TestPerformanceCurveLibraryInDb(TestHelper.UnitOfWork, libraryId);
            var curveId = Guid.NewGuid();
            var curve = PerformanceCurveTestSetup.TestLibraryPerformanceCurveInDb(TestHelper.UnitOfWork,
                libraryId, curveId, "AGE", "[AGE]");
            var service = CreatePerformanceCurvesService();
            var fileInfo = service.ExportLibraryPerformanceCurvesFile(libraryId);
            var dataAsString = fileInfo.FileData;
            var bytes = Convert.FromBase64String(dataAsString);
            var stream = new MemoryStream(bytes);
            var curves1 = TestHelper.UnitOfWork.PerformanceCurveRepo.GetPerformanceCurvesForLibrary(libraryId);
            TestHelper.UnitOfWork.PerformanceCurveRepo.UpsertOrDeletePerformanceCurves(
                new List<PerformanceCurveDTO>(), libraryId);
            var curves2 = TestHelper.UnitOfWork.PerformanceCurveRepo.GetPerformanceCurvesForLibrary(libraryId);
            Assert.Empty(curves2);
            var excelPackage = new ExcelPackage(stream);
            var userCriteria = new UserCriteriaDTO();

            service.ImportLibraryPerformanceCurvesFile(libraryId, excelPackage, userCriteria);

            var curves3 = TestHelper.UnitOfWork.PerformanceCurveRepo.GetPerformanceCurvesForLibrary(libraryId);
            var curve1 = curves1.Single();
            var curve3 = curves3.Single();
            ObjectAssertions.EquivalentExcluding(curve1, curve3, c => c.Id, c => c.Equation.Id);
        }

    }
}
