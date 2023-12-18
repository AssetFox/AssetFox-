using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests.Repositories;
using AppliedResearchAssociates.iAM.UnitTestsCore.TestUtils;
using MoreLinq;
using Xunit;
using Assert = Xunit.Assert;

namespace AppliedResearchAssociates.iAM.UnitTestsCore
{
    public class PerformanceCurveRepositoryTests
    {

        [Fact]
        public void Delete_PerformanceCurveLibraryExistsWithCurveAndEquation_DeletesAll()
        {
            AttributeTestSetup.CreateAttributes(TestHelper.UnitOfWork);
            var performanceCurveLibraryId = Guid.NewGuid();
            var performanceCurveId = Guid.NewGuid();
            var testLibrary = PerformanceCurveLibraryTestSetup.TestPerformanceCurveLibraryInDb(TestHelper.UnitOfWork, performanceCurveLibraryId);
            var curveDto = PerformanceCurveTestSetup.TestLibraryPerformanceCurveInDb(TestHelper.UnitOfWork, performanceCurveLibraryId, performanceCurveId, TestAttributeNames.ActionType);
            var criterionLibrary = CriterionLibraryTestSetup.TestCriterionLibraryInDb(TestHelper.UnitOfWork);
            var dtos = TestHelper.UnitOfWork.PerformanceCurveRepo.GetPerformanceCurveLibraries();

            var performanceCurveLibraryDTO = dtos.Single(dto => dto.Id == performanceCurveLibraryId);
            curveDto.CriterionLibrary = criterionLibrary;
            var equationInDb = TestHelper.UnitOfWork.Context.Equation
                .SingleOrDefault(e => e.PerformanceCurveEquationJoin.PerformanceCurve.PerformanceCurveLibraryId == performanceCurveLibraryId);
            Assert.NotNull(equationInDb);
            TestHelper.UnitOfWork.Context.ChangeTracker.Clear();

            // Act
            TestHelper.UnitOfWork.PerformanceCurveRepo.DeletePerformanceCurveLibrary(performanceCurveLibraryId);

            Assert.False(TestHelper.UnitOfWork.Context.PerformanceCurveLibrary.Any(_ => _.Id == performanceCurveLibraryId));
            Assert.False(TestHelper.UnitOfWork.Context.PerformanceCurve.Any(_ => _.Id == performanceCurveId));
            Assert.False(
                TestHelper.UnitOfWork.Context.CriterionLibraryPerformanceCurve.Any(_ =>
                    _.PerformanceCurveId == performanceCurveId));
            Assert.False(
                TestHelper.UnitOfWork.Context.PerformanceCurveEquation.Any(_ =>
                    _.PerformanceCurveId == performanceCurveId));
            var equationInDbAfter = TestHelper.UnitOfWork.Context.Equation
                .SingleOrDefault(e => e.PerformanceCurveEquationJoin.PerformanceCurve.PerformanceCurveLibraryId == performanceCurveLibraryId);
            Assert.Null(equationInDbAfter);
        }

        [Fact]
        public void GetPerformanceCurveLibrariesNoPerformanceCurves_Does()
        {
            Setup();
            var libraryId = Guid.NewGuid();
            var library = PerformanceCurveLibraryTestSetup.TestPerformanceCurveLibraryInDb(TestHelper.UnitOfWork, libraryId);
            var attribute = TestAttributeNames.CulvDurationN;
            var curveId = Guid.NewGuid();
            PerformanceCurveTestSetup.TestLibraryPerformanceCurveInDb(TestHelper.UnitOfWork, libraryId, curveId, attribute);

            var libraries = TestHelper.UnitOfWork.PerformanceCurveRepo.GetPerformanceCurveLibrariesNoPerformanceCurves();

            var relevantLibrary = libraries.Single(l => l.Id == libraryId);
            Assert.Empty(relevantLibrary.PerformanceCurves);
            var relevantLibraryWithChildren = TestHelper.UnitOfWork.PerformanceCurveRepo.GetPerformanceCurveLibrary(libraryId);
            Assert.NotEmpty(relevantLibraryWithChildren.PerformanceCurves);
        }

        private void Setup()
        {
            AttributeTestSetup.CreateAttributes(TestHelper.UnitOfWork);
            NetworkTestSetup.CreateNetwork(TestHelper.UnitOfWork);
        }

        [Fact]
        public void UpsertOrDeleteScenarioPerformanceCurves_CurveInDb_UpdatesShift()
        {
            Setup();
            // Arrange
            var simulationId = Guid.NewGuid();
            var curveId = Guid.NewGuid();
            var simulation = SimulationTestSetup.CreateSimulation(TestHelper.UnitOfWork, simulationId);
            var performanceCurve = ScenarioPerformanceCurveTestSetup.DtoForEntityInDb(TestHelper.UnitOfWork, simulationId, curveId);
            var scenarioCurves = TestHelper.UnitOfWork.PerformanceCurveRepo.GetScenarioPerformanceCurves(simulationId);
            var performanceCurveDto = scenarioCurves[0];
            performanceCurveDto.Shift = true;
            var UpdateRows = new List<PerformanceCurveDTO> { performanceCurveDto };

            // Act
            TestHelper.UnitOfWork.PerformanceCurveRepo.UpsertOrDeleteScenarioPerformanceCurvesNonAtomic(UpdateRows, simulation.Id);

            // Assert
            var performanceCurveLibraryDtoAfter = TestHelper.UnitOfWork.PerformanceCurveRepo.GetScenarioPerformanceCurves(simulationId);
            var performanceCurveDtoAfter = performanceCurveLibraryDtoAfter.Single();
            Assert.Equal(performanceCurveDto.Shift, performanceCurveDtoAfter.Shift);
            Assert.Equal(performanceCurveDto.Attribute, performanceCurveDtoAfter.Attribute);
        }

        [Fact]
        public void UpsertOrDeleteScenarioPerformanceCurves_CurveInDbWithEquation_EquationUnchanged()
        {
            Setup();
            // Arrange
            var simulationId = Guid.NewGuid();
            var curveId = Guid.NewGuid();
            var simulation = SimulationTestSetup.CreateSimulation(TestHelper.UnitOfWork, simulationId);
            var performanceCurve = ScenarioPerformanceCurveTestSetup.DtoForEntityInDb(TestHelper.UnitOfWork, simulationId, curveId, equation: "2");
            var updateRows = new List<PerformanceCurveDTO> { performanceCurve };

            // Act
            TestHelper.UnitOfWork.PerformanceCurveRepo.UpsertOrDeleteScenarioPerformanceCurvesNonAtomic(updateRows, simulation.Id);

            // Assert
            var performanceCurveLibraryDtoAfter = TestHelper.UnitOfWork.PerformanceCurveRepo.GetScenarioPerformanceCurves(simulationId);
            var performanceCurveDtoAfter = performanceCurveLibraryDtoAfter.Single();
            Assert.Equal(performanceCurve.Equation.Id, performanceCurveDtoAfter.Equation.Id);
            Assert.Equal(performanceCurve.Attribute, performanceCurveDtoAfter.Attribute);
        }

        [Fact]
        public void UpsertOrDeleteScenarioPerformanceCurves_CurveInDbWithEquation_UpdateRemovesEquationFromCurve_EquationRemoved()
        {
            Setup();
            // Arrange
            var simulationId = Guid.NewGuid();
            var curveId = Guid.NewGuid();
            var simulation = SimulationTestSetup.CreateSimulation(TestHelper.UnitOfWork, simulationId);
            var performanceCurve = ScenarioPerformanceCurveTestSetup.DtoForEntityInDb(TestHelper.UnitOfWork, simulationId, curveId, equation: "2");
            var equationId = performanceCurve.Equation.Id;
            var scenarioCurves = TestHelper.UnitOfWork.PerformanceCurveRepo.GetScenarioPerformanceCurves(simulationId);
            var performanceCurveDto = scenarioCurves[0];
            performanceCurveDto.Equation = null;
            var updateRows = new List<PerformanceCurveDTO> { performanceCurveDto };

            // Act
            TestHelper.UnitOfWork.PerformanceCurveRepo.UpsertOrDeleteScenarioPerformanceCurvesNonAtomic(updateRows, simulation.Id);

            // Assert
            var performanceCurveLibraryDtoAfter = TestHelper.UnitOfWork.PerformanceCurveRepo.GetScenarioPerformanceCurves(simulationId);
            var performanceCurveDtoAfter = performanceCurveLibraryDtoAfter.Single();
            Assert.Null(performanceCurveDtoAfter.Equation?.Expression);
            Assert.Equal(performanceCurveDto.Attribute, performanceCurveDtoAfter.Attribute);
            var equationAfter = TestHelper.UnitOfWork.Context.Equation.SingleOrDefault(e => e.Id == equationId);
            Assert.Null(equationAfter);
        }

        [Fact]
        public void UpsertOrDeleteScenarioPerformanceCurves_CurveInDbWithCriterionLibrary_CriterionLibraryUnchanged()
        {
            Setup();
            // Arrange
            var simulationId = Guid.NewGuid();
            var curveId = Guid.NewGuid();
            var simulation = SimulationTestSetup.CreateSimulation(TestHelper.UnitOfWork, simulationId);
            var criterionLibrary = CriterionLibraryTestSetup.TestCriterionLibrary();
            var performanceCurve = ScenarioPerformanceCurveTestSetup.DtoForEntityInDb(TestHelper.UnitOfWork, simulationId, curveId, criterionLibrary);
            var scenarioCurves = TestHelper.UnitOfWork.PerformanceCurveRepo.GetScenarioPerformanceCurves(simulationId);
            var performanceCurveDto = scenarioCurves[0];
            var updateRows = new List<PerformanceCurveDTO> { performanceCurveDto };
            // Act
            TestHelper.UnitOfWork.PerformanceCurveRepo.UpsertOrDeleteScenarioPerformanceCurvesNonAtomic(updateRows, simulation.Id);

            // Assert
            var scenarioCurvesAfter = TestHelper.UnitOfWork.PerformanceCurveRepo.GetScenarioPerformanceCurves(simulationId);
            var performanceCurveDtoAfter = scenarioCurvesAfter[0];
            Assert.Equal(performanceCurve.CriterionLibrary.Id, performanceCurveDtoAfter.CriterionLibrary.Id);
            Assert.Equal(performanceCurveDto.Attribute, performanceCurveDtoAfter.Attribute);
        }

        [Fact]
        public void UpsertOrDeleteScenarioPerformanceCurves_CurveInDbWithCriterionLibrary_UpdateRemovesCriterionLibraryFromCurve_CriterionLibraryRemoved()
        {
            Setup();
            // Arrange
            var simulationId = Guid.NewGuid();
            var curveId = Guid.NewGuid();
            var simulation = SimulationTestSetup.CreateSimulation(TestHelper.UnitOfWork, simulationId);
            var criterionLibrary = CriterionLibraryTestSetup.TestCriterionLibrary();
            var performanceCurve = ScenarioPerformanceCurveTestSetup.DtoForEntityInDb(TestHelper.UnitOfWork, simulationId, curveId, criterionLibrary);
            var scenarioCurves = TestHelper.UnitOfWork.PerformanceCurveRepo.GetScenarioPerformanceCurves(simulationId);
            var performanceCurveDto = scenarioCurves[0];
            performanceCurveDto.CriterionLibrary = null;

            var updateRows = new List<PerformanceCurveDTO> { performanceCurveDto };

            // Act
            TestHelper.UnitOfWork.PerformanceCurveRepo.UpsertOrDeleteScenarioPerformanceCurvesNonAtomic(updateRows, simulation.Id);

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
        public void UpsertOrDeleteScenarioPerformanceCurves_CurveInDbWithCriterionLibrary_CurveRemovedFromUpsertedLibrary_RemovesCurveAndCriterionJoin()
        {
            Setup();
            // Arrange
            var simulationId = Guid.NewGuid();
            var curveId = Guid.NewGuid();
            var simulation = SimulationTestSetup.CreateSimulation(TestHelper.UnitOfWork, simulationId);
            var criterionLibrary = CriterionLibraryTestSetup.TestCriterionLibrary();
            var performanceCurve = ScenarioPerformanceCurveTestSetup.DtoForEntityInDb(TestHelper.UnitOfWork, simulationId, curveId);
            var scenarioCurves = TestHelper.UnitOfWork.PerformanceCurveRepo.GetScenarioPerformanceCurves(simulationId);

            var updateRows = new List<PerformanceCurveDTO>();

            // Act
            TestHelper.UnitOfWork.PerformanceCurveRepo.UpsertOrDeleteScenarioPerformanceCurvesNonAtomic(updateRows, simulation.Id);

            // Assert
            var scenarioCurvesAfter = TestHelper.UnitOfWork.PerformanceCurveRepo.GetScenarioPerformanceCurves(simulationId);
            Assert.Empty(scenarioCurvesAfter);
            var criterionLibraryJoinAfter = TestHelper.UnitOfWork.Context.CriterionLibraryPerformanceCurve.SingleOrDefault(clpc =>
            clpc.PerformanceCurveId == curveId
            && clpc.CriterionLibraryId == simulationId);
            Assert.Null(criterionLibraryJoinAfter);
        }

        [Fact]
        public void UpsertOrDeleteScenarioPerformanceCurves_CurveInDbWithEquation_UpdateChangesExpression_ExpressionChangedInDb()
        {
            Setup();
            // Arrange
            var simulationId = Guid.NewGuid();
            var curveId = Guid.NewGuid();
            var simulation = SimulationTestSetup.CreateSimulation(TestHelper.UnitOfWork, simulationId);
            var performanceCurve = ScenarioPerformanceCurveTestSetup.DtoForEntityInDb(TestHelper.UnitOfWork, simulationId, curveId, equation: "2");
            var scenarioCurves = TestHelper.UnitOfWork.PerformanceCurveRepo.GetScenarioPerformanceCurves(simulationId);
            var performanceCurveDto = scenarioCurves[0];
            performanceCurveDto.Equation.Expression = "123";

            var updateRows = new List<PerformanceCurveDTO> { performanceCurveDto };

            // Act
            TestHelper.UnitOfWork.PerformanceCurveRepo.UpsertOrDeleteScenarioPerformanceCurvesNonAtomic(updateRows, simulation.Id);

            // Assert
            var scenarioCurvesAfter = TestHelper.UnitOfWork.PerformanceCurveRepo.GetScenarioPerformanceCurves(simulationId);
            var performanceCurveDtoAfter = scenarioCurvesAfter[0];
            Assert.Equal("123", performanceCurveDtoAfter.Equation.Expression);
            Assert.Equal(performanceCurveDto.Equation.Id, performanceCurveDtoAfter.Equation.Id);
        }

        [Fact]
        public void UpsertOrDeleteScenarioPerformanceCurves_CurveInDbWithEquation_UpdateRemovesCurve_EquationDeleted()
        {
            Setup();
            // Arrange
            var curveId = Guid.NewGuid();
            var simulation = SimulationTestSetup.CreateSimulation(TestHelper.UnitOfWork);
            var simulationId = simulation.Id;
            var performanceCurve = ScenarioPerformanceCurveTestSetup.DtoForEntityInDb(TestHelper.UnitOfWork, simulationId, curveId, equation: "2");
            var updateRows = new List<PerformanceCurveDTO>();

            // Act
            TestHelper.UnitOfWork.PerformanceCurveRepo.UpsertOrDeleteScenarioPerformanceCurvesNonAtomic(updateRows, simulation.Id);

            // Assert
            var scenarioCurvesAfter = TestHelper.UnitOfWork.PerformanceCurveRepo.GetScenarioPerformanceCurves(simulationId);
            Assert.Empty(scenarioCurvesAfter);
        }

        [Fact]
        public void UpsertOrDeleteScenarioPerformanceCurves_EmptySimulationInDb_Adds()
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
            var updateRows = new List<PerformanceCurveDTO> { performanceCurveDto };

            TestHelper.UnitOfWork.PerformanceCurveRepo.UpsertOrDeleteScenarioPerformanceCurvesNonAtomic(updateRows, simulation.Id);

            var scenarioCurvesAfter = TestHelper.UnitOfWork.PerformanceCurveRepo.GetScenarioPerformanceCurves(simulationId);
            Assert.Single(scenarioCurvesAfter);
        }

        [Fact]
        public void UpsertOrDeleteScenarioPerformanceCurves_EmptyLibraryInDb_AddsCurveAndEquationToLibrary()
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
            var updateRows = new List<PerformanceCurveDTO> { performanceCurveDto };

            TestHelper.UnitOfWork.PerformanceCurveRepo.UpsertOrDeleteScenarioPerformanceCurvesNonAtomic(updateRows, simulation.Id);

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
            var updateRows = new List<PerformanceCurveDTO> { performanceCurveDto };

            TestHelper.UnitOfWork.PerformanceCurveRepo.UpsertOrDeleteScenarioPerformanceCurvesNonAtomic(updateRows, simulation.Id);

            var scenarioCurvesAfter = TestHelper.UnitOfWork.PerformanceCurveRepo.GetScenarioPerformanceCurves(simulationId);
            var performanceCurveDtoAfter = scenarioCurvesAfter[0];
            var equationAfter = performanceCurveDtoAfter.Equation;
            Assert.Null(equationAfter.Expression);
            var equationEntity = TestHelper.UnitOfWork.Context.Equation.SingleOrDefault(e => e.Id == equationAfter.Id);
            Assert.Null(equationEntity);
        }

        [Fact]
        public void UpsertOrDeleteScenarioPerformanceCurves_CurveInListWithCriterionLibrary_EmptySimulationInDb_AddsPerformanceCurveAndCriterionLibraryToSimulation()
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

            var updateRows = new List<PerformanceCurveDTO> { performanceCurveDto };

            TestHelper.UnitOfWork.PerformanceCurveRepo.UpsertOrDeleteScenarioPerformanceCurvesNonAtomic(updateRows, simulation.Id);

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
            TestHelper.UnitOfWork.PerformanceCurveRepo.UpsertOrDeleteScenarioPerformanceCurvesNonAtomic(performanceCurves, simulation.Id));

            var message = exception.Message;
            Assert.Contains(ErrorMessageConstants.NoAttributeFoundHavingName, message);
        }
    }
}
