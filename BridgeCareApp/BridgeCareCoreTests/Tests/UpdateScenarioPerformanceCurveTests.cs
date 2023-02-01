using AppliedResearchAssociates.iAM.DataPersistenceCore;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests.Repositories;
using AppliedResearchAssociates.iAM.UnitTestsCore.TestUtils;
using BridgeCareCore.Models;
using MoreLinq;
using Xunit;
using Assert = Xunit.Assert;

namespace BridgeCareCoreTests.Tests
{
    public class UpdateScenarioPerformanceCurveTests
    {
        private void Setup()
        {
            AttributeTestSetup.CreateAttributes(TestHelper.UnitOfWork);
            NetworkTestSetup.CreateNetwork(TestHelper.UnitOfWork);
        }

        [Fact]
        public async Task UpsertScenarioPerformanceCurves_CurveInDb_UpdatesShift()
        {
            Setup();
            // Arrange
            var simulationId = Guid.NewGuid();
            var curveId = Guid.NewGuid();
            var simulation = SimulationTestSetup.CreateSimulation(TestHelper.UnitOfWork, simulationId);
            var performanceCurve = ScenarioPerformanceCurveTestSetup.DtoForEntityInDb(TestHelper.UnitOfWork, simulationId, curveId);
            var controller = PerformanceCurveControllerTestSetup.SetupController(EsecSecurityMocks.Admin);
            var scenarioCurves = TestHelper.UnitOfWork.PerformanceCurveRepo.GetScenarioPerformanceCurves(simulationId);
            var performanceCurveDto = scenarioCurves[0];
            performanceCurveDto.Shift = true;

            var request = new PagingSyncModel<PerformanceCurveDTO>()
            {
                UpdateRows = new List<PerformanceCurveDTO> { performanceCurveDto },
                AddedRows = new List<PerformanceCurveDTO>(),
                RowsForDeletion = new List<Guid>()
            };

            // Act
            await controller.UpsertScenarioPerformanceCurves(simulation.Id, request);

            // Assert
            var performanceCurveLibraryDtoAfter = TestHelper.UnitOfWork.PerformanceCurveRepo.GetScenarioPerformanceCurves(simulationId);
            var performanceCurveDtoAfter = performanceCurveLibraryDtoAfter.Single();
            Assert.Equal(performanceCurveDto.Shift, performanceCurveDtoAfter.Shift);
            Assert.Equal(performanceCurveDto.Attribute, performanceCurveDtoAfter.Attribute);
        }

        [Fact]
        public async Task UpsertScenarioPerformanceCurves_CurveInDbWithEquation_EquationUnchanged()
        {
            Setup();
            // Arrange
            var simulationId = Guid.NewGuid();
            var curveId = Guid.NewGuid();
            var simulation = SimulationTestSetup.CreateSimulation(TestHelper.UnitOfWork, simulationId);
            var performanceCurve = ScenarioPerformanceCurveTestSetup.DtoForEntityInDb(TestHelper.UnitOfWork, simulationId, curveId, equation: "2");
            var controller = PerformanceCurveControllerTestSetup.SetupController(EsecSecurityMocks.Admin);
            var scenarioCurves = TestHelper.UnitOfWork.PerformanceCurveRepo.GetScenarioPerformanceCurves(simulationId);

            var request = new PagingSyncModel<PerformanceCurveDTO>()
            {
                UpdateRows = new List<PerformanceCurveDTO> { performanceCurve },
                AddedRows = new List<PerformanceCurveDTO>(),
                RowsForDeletion = new List<Guid>()
            };

            // Act
            await controller.UpsertScenarioPerformanceCurves(simulationId, request);

            // Assert
            var performanceCurveLibraryDtoAfter = TestHelper.UnitOfWork.PerformanceCurveRepo.GetScenarioPerformanceCurves(simulationId);
            var performanceCurveDtoAfter = performanceCurveLibraryDtoAfter.Single();
            Assert.Equal(performanceCurve.Equation.Id, performanceCurveDtoAfter.Equation.Id);
            Assert.Equal(performanceCurve.Attribute, performanceCurveDtoAfter.Attribute);
        }

        [Fact]
        public async Task UpsertScenarioPerformanceCurves_CurveInDbWithEquation_UpdateRemovesEquationFromCurve_EquationRemoved()
        {
            Setup();
            // Arrange
            var simulationId = Guid.NewGuid();
            var curveId = Guid.NewGuid();
            var simulation = SimulationTestSetup.CreateSimulation(TestHelper.UnitOfWork, simulationId);
            var performanceCurve = ScenarioPerformanceCurveTestSetup.DtoForEntityInDb(TestHelper.UnitOfWork, simulationId, curveId, equation: "2");
            var equationId = performanceCurve.Equation.Id;
            var controller = PerformanceCurveControllerTestSetup.SetupController(EsecSecurityMocks.Admin);
            var scenarioCurves = TestHelper.UnitOfWork.PerformanceCurveRepo.GetScenarioPerformanceCurves(simulationId);
            var performanceCurveDto = scenarioCurves[0];
            performanceCurveDto.Equation = null;

            var request = new PagingSyncModel<PerformanceCurveDTO>()
            {
                UpdateRows = new List<PerformanceCurveDTO> { performanceCurveDto },
                AddedRows = new List<PerformanceCurveDTO>(),
                RowsForDeletion = new List<Guid>()
            };

            // Act
            await controller.UpsertScenarioPerformanceCurves(simulationId, request);

            // Assert
            var performanceCurveLibraryDtoAfter = TestHelper.UnitOfWork.PerformanceCurveRepo.GetScenarioPerformanceCurves(simulationId);
            var performanceCurveDtoAfter = performanceCurveLibraryDtoAfter.Single();
            Assert.Null(performanceCurveDtoAfter.Equation?.Expression);
            Assert.Equal(performanceCurveDto.Attribute, performanceCurveDtoAfter.Attribute);
            var equationAfter = TestHelper.UnitOfWork.Context.Equation.SingleOrDefault(e => e.Id == equationId);
            Assert.Null(equationAfter);
        }

        [Fact]
        public async Task UpsertScenarioPerformanceCurves_CurveInDbWithCriterionLibrary_CriterionLibraryUnchanged()
        {
            Setup();
            // Arrange
            var simulationId = Guid.NewGuid();
            var curveId = Guid.NewGuid();
            var simulation = SimulationTestSetup.CreateSimulation(TestHelper.UnitOfWork, simulationId);
            var criterionLibrary = CriterionLibraryTestSetup.TestCriterionLibrary();
            var performanceCurve = ScenarioPerformanceCurveTestSetup.DtoForEntityInDb(TestHelper.UnitOfWork, simulationId, curveId, criterionLibrary);
            var controller = PerformanceCurveControllerTestSetup.SetupController(EsecSecurityMocks.Admin);
            var scenarioCurves = TestHelper.UnitOfWork.PerformanceCurveRepo.GetScenarioPerformanceCurves(simulationId);
            var performanceCurveDto = scenarioCurves[0];

            var request = new PagingSyncModel<PerformanceCurveDTO>()
            {
                UpdateRows = new List<PerformanceCurveDTO> { performanceCurveDto },
                AddedRows = new List<PerformanceCurveDTO>(),
                RowsForDeletion = new List<Guid>()
            };

            // Act
            await controller.UpsertScenarioPerformanceCurves(simulationId, request);

            // Assert
            var scenarioCurvesAfter = TestHelper.UnitOfWork.PerformanceCurveRepo.GetScenarioPerformanceCurves(simulationId);
            var performanceCurveDtoAfter = scenarioCurvesAfter[0];
            Assert.Equal(performanceCurve.CriterionLibrary.Id, performanceCurveDtoAfter.CriterionLibrary.Id);
            Assert.Equal(performanceCurveDto.Attribute, performanceCurveDtoAfter.Attribute);
        }

        [Fact]
        public async Task UpsertScenarioPerformanceCurves_CurveInDbWithCriterionLibrary_UpdateRemovesCriterionLibraryFromCurve_CriterionLibraryRemoved()
        {
            Setup();
            // Arrange
            var simulationId = Guid.NewGuid();
            var curveId = Guid.NewGuid();
            var simulation = SimulationTestSetup.CreateSimulation(TestHelper.UnitOfWork, simulationId);
            var criterionLibrary = CriterionLibraryTestSetup.TestCriterionLibrary();
            var performanceCurve = ScenarioPerformanceCurveTestSetup.DtoForEntityInDb(TestHelper.UnitOfWork, simulationId, curveId, criterionLibrary);
            var controller = PerformanceCurveControllerTestSetup.SetupController(EsecSecurityMocks.Admin);
            var scenarioCurves = TestHelper.UnitOfWork.PerformanceCurveRepo.GetScenarioPerformanceCurves(simulationId);
            var performanceCurveDto = scenarioCurves[0];
            performanceCurveDto.CriterionLibrary = null;

            var request = new PagingSyncModel<PerformanceCurveDTO>()
            {
                UpdateRows = new List<PerformanceCurveDTO> { performanceCurveDto },
                AddedRows = new List<PerformanceCurveDTO>(),
                RowsForDeletion = new List<Guid>()
            };

            // Act
            await controller.UpsertScenarioPerformanceCurves(simulationId, request);

            // Assert
            var scenarioCurvesAfter = TestHelper.UnitOfWork.PerformanceCurveRepo.GetScenarioPerformanceCurves(simulationId);
            var performanceCurveDtoAfter = scenarioCurvesAfter[0];
            Assert.Equal(Guid.Empty, performanceCurveDtoAfter.CriterionLibrary.Id);
            Assert.Equal(performanceCurveDto.Attribute, performanceCurveDtoAfter.Attribute);
            var criterionLibraryJoinAfter = TestHelper.UnitOfWork.Context.CriterionLibraryPerformanceCurve.SingleOrDefault(clpc =>
            clpc.PerformanceCurveId == curveId
            && clpc.CriterionLibraryId == criterionLibrary.Id);
            Assert.Null(criterionLibraryJoinAfter);
        }

        [Fact]
        public async Task UpsertScenarioPerformanceCurves_CurveInDbWithCriterionLibrary_CurveRemovedFromUpsertedLibrary_RemovesCurveAndCriterionJoin()
        {
            Setup();
            // Arrange
            var simulationId = Guid.NewGuid();
            var curveId = Guid.NewGuid();
            var simulation = SimulationTestSetup.CreateSimulation(TestHelper.UnitOfWork, simulationId);
            var criterionLibrary = CriterionLibraryTestSetup.TestCriterionLibrary();
            var performanceCurve = ScenarioPerformanceCurveTestSetup.DtoForEntityInDb(TestHelper.UnitOfWork, simulationId, curveId);
            var controller = PerformanceCurveControllerTestSetup.SetupController(EsecSecurityMocks.Admin);
            var scenarioCurves = TestHelper.UnitOfWork.PerformanceCurveRepo.GetScenarioPerformanceCurves(simulationId);

            var request = new PagingSyncModel<PerformanceCurveDTO>()
            {
                RowsForDeletion = new List<Guid> { curveId},
                AddedRows = new List<PerformanceCurveDTO>(),
                UpdateRows = new List<PerformanceCurveDTO>()
            };
            // Act
            await controller.UpsertScenarioPerformanceCurves(simulationId, request);

            // Assert
            var scenarioCurvesAfter = TestHelper.UnitOfWork.PerformanceCurveRepo.GetScenarioPerformanceCurves(simulationId);
            Assert.Empty(scenarioCurvesAfter);
            var criterionLibraryJoinAfter = TestHelper.UnitOfWork.Context.CriterionLibraryPerformanceCurve.SingleOrDefault(clpc =>
            clpc.PerformanceCurveId == curveId
            && clpc.CriterionLibraryId == simulationId);
            Assert.Null(criterionLibraryJoinAfter);
        }

        [Fact]
        public async Task UpsertScenarioPerformanceCurves_CurveInDbWithEquation_UpdateChangesExpression_ExpressionChangedInDb()
        {
            Setup();
            // Arrange
            var simulationId = Guid.NewGuid();
            var curveId = Guid.NewGuid();
            var simulation = SimulationTestSetup.CreateSimulation(TestHelper.UnitOfWork, simulationId);
            var performanceCurve = ScenarioPerformanceCurveTestSetup.DtoForEntityInDb(TestHelper.UnitOfWork, simulationId, curveId, equation: "2");
            var controller = PerformanceCurveControllerTestSetup.SetupController(EsecSecurityMocks.Admin);
            var scenarioCurves = TestHelper.UnitOfWork.PerformanceCurveRepo.GetScenarioPerformanceCurves(simulationId);
            var performanceCurveDto = scenarioCurves[0];
            performanceCurveDto.Equation.Expression = "123";

            var request = new PagingSyncModel<PerformanceCurveDTO>()
            {
                UpdateRows = new List<PerformanceCurveDTO> { performanceCurveDto },
                AddedRows = new List<PerformanceCurveDTO>(),
                RowsForDeletion = new List<Guid>()
            };

            // Act
            await controller.UpsertScenarioPerformanceCurves(simulationId, request);

            // Assert
            var scenarioCurvesAfter = TestHelper.UnitOfWork.PerformanceCurveRepo.GetScenarioPerformanceCurves(simulationId);
            var performanceCurveDtoAfter = scenarioCurvesAfter[0];
            Assert.Equal("123", performanceCurveDtoAfter.Equation.Expression);
            Assert.Equal(performanceCurveDto.Equation.Id, performanceCurveDtoAfter.Equation.Id);
        }

        [Fact]
        public async Task UpsertScenarioPerformanceCurves_CurveInDbWithEquation_UpdateRemovesCurve_EquationDeleted()
        {
            Setup();
            // Arrange
            var curveId = Guid.NewGuid();
            var simulation = SimulationTestSetup.CreateSimulation(TestHelper.UnitOfWork);
            var simulationId = simulation.Id;
            var performanceCurve = ScenarioPerformanceCurveTestSetup.DtoForEntityInDb(TestHelper.UnitOfWork, simulationId, curveId, equation: "2");
            var controller = PerformanceCurveControllerTestSetup.SetupController(EsecSecurityMocks.Admin);
            var scenarioCurves = TestHelper.UnitOfWork.PerformanceCurveRepo.GetScenarioPerformanceCurves(simulationId);

            var request = new PagingSyncModel<PerformanceCurveDTO>()
            {
                RowsForDeletion = new List<Guid> { curveId},
                AddedRows = new List<PerformanceCurveDTO>(),
                UpdateRows = new List<PerformanceCurveDTO>()
            };

            // Act
            await controller.UpsertScenarioPerformanceCurves(simulationId, request);

            // Assert
            var scenarioCurvesAfter = TestHelper.UnitOfWork.PerformanceCurveRepo.GetScenarioPerformanceCurves(simulationId);
            Assert.Empty(scenarioCurvesAfter);
        }

        [Fact]
        public async Task UpsertSimulationPerformanceCurve_EmptySimulationInDb_Adds()
        {
            Setup();
            // Arrange
            var simulationId = Guid.NewGuid();
            var simulation = SimulationTestSetup.CreateSimulation(TestHelper.UnitOfWork, simulationId);
            var attribute = TestHelper.UnitOfWork.AttributeRepo.GetAttributes().First();
            var performanceCurveDto = new PerformanceCurveDTO
            {
                Attribute = attribute.Name,
                Id = Guid.NewGuid(),
                Name = "Curve",
            };
            var controller = PerformanceCurveControllerTestSetup.SetupController(EsecSecurityMocks.Admin);

            var request = new PagingSyncModel<PerformanceCurveDTO>()
            {
                AddedRows = new List<PerformanceCurveDTO> { performanceCurveDto },
                RowsForDeletion = new List<Guid>(),
                UpdateRows = new List<PerformanceCurveDTO>()
            };

            await controller.UpsertScenarioPerformanceCurves(simulationId, request);

            var scenarioCurvesAfter = TestHelper.UnitOfWork.PerformanceCurveRepo.GetScenarioPerformanceCurves(simulationId);
            Assert.Single(scenarioCurvesAfter);
        }

        [Fact]
        public async Task UpsertSimulationPerformanceCurveWithEquation_EmptyLibraryInDb_AddsCurveAndEquationToLibrary()
        {
            Setup();
            // Arrange
            var curveId = Guid.NewGuid();
            var simulation = SimulationTestSetup.CreateSimulation(TestHelper.UnitOfWork);
            var attribute = TestHelper.UnitOfWork.AttributeRepo.GetAttributes().First();
            var equation = new EquationDTO
            {
                Expression = "3",
                Id = Guid.NewGuid(),
            };
            var performanceCurveDto = new PerformanceCurveDTO
            {
                Attribute = attribute.Name,
                Id = Guid.NewGuid(),
                Name = "Curve",
                Equation = equation,
            };
            var controller = PerformanceCurveControllerTestSetup.SetupController(EsecSecurityMocks.Admin);

            var request = new PagingSyncModel<PerformanceCurveDTO>()
            {
                AddedRows = new List<PerformanceCurveDTO> { performanceCurveDto },
                RowsForDeletion = new List<Guid>(),
                UpdateRows = new List<PerformanceCurveDTO>()
            };

            await controller.UpsertScenarioPerformanceCurves(simulation.Id, request);

            var scenarioCurvesAfter = TestHelper.UnitOfWork.PerformanceCurveRepo.GetScenarioPerformanceCurves(simulation.Id);
            var performanceCurveDtoAfter = scenarioCurvesAfter[0];
            var equationAfter = performanceCurveDtoAfter.Equation;
            Assert.Equal("3", performanceCurveDtoAfter.Equation.Expression);
            var equationEntity = TestHelper.UnitOfWork.Context.Equation.SingleOrDefault(e => e.Id == equationAfter.Id);
            Assert.NotNull(equationEntity);
        }

        [Fact]
        public async Task UpsertScenarioPerformanceCurveWithEquation_EmptySimulationInDb_AddsCurveAndEquationToSimulation()
        {
            Setup();
            // Arrange
            var curveId = Guid.NewGuid();
            var simulation = SimulationTestSetup.CreateSimulation(TestHelper.UnitOfWork);
            var simulationId = simulation.Id;
            var attribute = TestHelper.UnitOfWork.AttributeRepo.GetAttributes().First();
            var equation = new EquationDTO
            {
                Expression = "3",
                Id = Guid.Empty,
            };
            var performanceCurveDto = new PerformanceCurveDTO
            {
                Attribute = attribute.Name,
                Id = Guid.NewGuid(),
                Name = "Curve",
                Equation = equation,
            };
            var controller = PerformanceCurveControllerTestSetup.SetupController(EsecSecurityMocks.Admin);

            var request = new PagingSyncModel<PerformanceCurveDTO>()
            {
                AddedRows = new List<PerformanceCurveDTO> { performanceCurveDto },
                RowsForDeletion = new List<Guid>(),
                UpdateRows = new List<PerformanceCurveDTO>()
            };

            await controller.UpsertScenarioPerformanceCurves(simulationId, request);

            var scenarioCurvesAfter = TestHelper.UnitOfWork.PerformanceCurveRepo.GetScenarioPerformanceCurves(simulationId);
            var performanceCurveDtoAfter = scenarioCurvesAfter[0];
            var equationAfter = performanceCurveDtoAfter.Equation;
            Assert.Null(equationAfter.Expression);
            var equationEntity = TestHelper.UnitOfWork.Context.Equation.SingleOrDefault(e => e.Id == equationAfter.Id);
            Assert.Null(equationEntity);
        }

        [Fact]
        public async Task UpsertScenarioPerformanceCurveWithCriterionLibrary_EmptySimulationInDb_AddsPerformanceCurveAndCriterionLibraryToSimulation()
        {
            Setup();
            // Arrange
            var curveId = Guid.NewGuid();
            var simulation = SimulationTestSetup.CreateSimulation(TestHelper.UnitOfWork);
            var attribute = TestHelper.UnitOfWork.AttributeRepo.GetAttributes().First();
            var criterionLibrary = new CriterionLibraryDTO
            {
                Id = Guid.NewGuid(),
                MergedCriteriaExpression = "MergedCriteriaExpression",
                Description = "Description",
                IsSingleUse = true,
            };
            var performanceCurveDto = new PerformanceCurveDTO
            {
                Attribute = attribute.Name,
                Id = Guid.NewGuid(),
                Name = "Curve",
                CriterionLibrary = criterionLibrary,
            };
            var controller = PerformanceCurveControllerTestSetup.SetupController(EsecSecurityMocks.Admin);

            var request = new PagingSyncModel<PerformanceCurveDTO>()
            {
                AddedRows = new List<PerformanceCurveDTO> { performanceCurveDto },
                RowsForDeletion = new List<Guid>(),
                UpdateRows = new List<PerformanceCurveDTO>()
            };

            await controller.UpsertScenarioPerformanceCurves(simulation.Id, request);

            var scenarioCurvesAfter = TestHelper.UnitOfWork.PerformanceCurveRepo.GetScenarioPerformanceCurves(simulation.Id);
            var performanceCurveDtoAfter = scenarioCurvesAfter[0];
            var criterionLibraryAfter = performanceCurveDtoAfter.CriterionLibrary;
            Assert.Equal("MergedCriteriaExpression", criterionLibraryAfter.MergedCriteriaExpression);
        }

        [Fact]
        public void UpsertOrDeleteScenarioPerformanceCurves_CurveHasInvalidAttribute_Throws()
        {
            Setup();
            var libraryId = Guid.NewGuid();
            var libraryDto = PerformanceCurveLibraryTestSetup.TestPerformanceCurveLibraryInDb(TestHelper.UnitOfWork, libraryId);
            var performanceCurveLibraryDto = TestHelper.UnitOfWork.PerformanceCurveRepo.GetPerformanceCurveLibrary(libraryId);
            var simulation = SimulationTestSetup.CreateSimulation(TestHelper.UnitOfWork);
            var criterionLibrary = new CriterionLibraryDTO
            {
                Id = Guid.NewGuid(),
                MergedCriteriaExpression = "MergedCriteriaExpression",
                Description = "Description",
                IsSingleUse = true,
            };
            var performanceCurveDto = new PerformanceCurveDTO
            {
                Attribute = "Invalid attribute name",
                Id = Guid.NewGuid(),
                Name = "Curve",
                CriterionLibrary = criterionLibrary,
            };
            var performanceCurves = new List<PerformanceCurveDTO> { performanceCurveDto };

            var exception = Assert.ThrowsAny<Exception>(() =>
            TestHelper.UnitOfWork.PerformanceCurveRepo.UpsertOrDeleteScenarioPerformanceCurves(performanceCurves, simulation.Id));

            var message = exception.Message;
            Assert.Contains(ErrorMessageConstants.NoAttributeFoundHavingName, message);
        }
    }
}
