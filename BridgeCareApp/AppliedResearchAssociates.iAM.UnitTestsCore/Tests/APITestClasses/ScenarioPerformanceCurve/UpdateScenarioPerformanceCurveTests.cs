﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.UnitTestsCore.TestUtils;
using BridgeCareCore.Security;
using MoreLinq;
using Xunit;
using Assert = Xunit.Assert;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests.APITestClasses
{
    public class UpdateScenarioPerformanceCurveTests
    {
        private TestHelper _testHelper => TestHelper.Instance;

        private void Setup()
        {
            _testHelper.CreateAttributes();
            _testHelper.CreateNetwork();
            _testHelper.SetupDefaultHttpContext();
        }

        [Fact]
        public async Task UpsertScenarioPerformanceCurves_CurveInDb_UpdatesShift()
        {
            Setup();
            // Arrange
            var simulationId = Guid.NewGuid();
            var curveId = Guid.NewGuid();
            var simulationEntity = _testHelper.CreateSimulation(simulationId);
            var simulation = _testHelper.UnitOfWork.SimulationRepo.GetSimulation(simulationEntity.Id);
            var performanceCurve = ScenarioPerformanceCurveTestSetup.EntityInDb(_testHelper.UnitOfWork, simulationId, curveId);
            var controller = PerformanceCurveControllerTestSetup.SetupController(_testHelper, _testHelper.MockEsecSecurityAdmin);
            var scenarioCurves = _testHelper.UnitOfWork.PerformanceCurveRepo.GetScenarioPerformanceCurves(simulationId);
            var performanceCurveDto = scenarioCurves[0];
            performanceCurveDto.Shift = true;

            // Act
            await controller.UpsertScenarioPerformanceCurves(simulation.Id, scenarioCurves);

            // Assert
            var performanceCurveLibraryDtoAfter = _testHelper.UnitOfWork.PerformanceCurveRepo.GetScenarioPerformanceCurves(simulationId);
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
            var simulationEntity = _testHelper.CreateSimulation(simulationId);
            var simulation = _testHelper.UnitOfWork.SimulationRepo.GetSimulation(simulationEntity.Id);
            var performanceCurve = ScenarioPerformanceCurveTestSetup.EntityInDb(_testHelper.UnitOfWork, simulationId, curveId);
            var equation = EquationTestSetup.TwoWithScenarioJoinInDb(_testHelper.UnitOfWork, null, curveId);
            var controller = PerformanceCurveControllerTestSetup.SetupController(_testHelper, _testHelper.MockEsecSecurityAdmin);
            var scenarioCurves = _testHelper.UnitOfWork.PerformanceCurveRepo.GetScenarioPerformanceCurves(simulationId);
            var performanceCurveDto = scenarioCurves[0];
            performanceCurveDto.Equation = equation.ToDto();

            // Act
            await controller.UpsertScenarioPerformanceCurves(simulationId, scenarioCurves);

            // Assert
            var performanceCurveLibraryDtoAfter = _testHelper.UnitOfWork.PerformanceCurveRepo.GetScenarioPerformanceCurves(simulationId);
            var performanceCurveDtoAfter = performanceCurveLibraryDtoAfter.Single();
            Assert.Equal(performanceCurveDto.Equation.Id, performanceCurveDtoAfter.Equation.Id);
            Assert.Equal(performanceCurveDto.Attribute, performanceCurveDtoAfter.Attribute);
        }

        [Fact]
        public async Task UpsertScenarioPerformanceCurves_CurveInDbWithEquation_UpdateRemovesEquationFromCurve_EquationRemoved()
        {
            Setup();
            // Arrange
            var simulationId = Guid.NewGuid();
            var curveId = Guid.NewGuid();
            var simulationEntity = _testHelper.CreateSimulation(simulationId);
            var simulation = _testHelper.UnitOfWork.SimulationRepo.GetSimulation(simulationEntity.Id);
            var performanceCurve = ScenarioPerformanceCurveTestSetup.EntityInDb(_testHelper.UnitOfWork, simulationId, curveId);
            var equation = EquationTestSetup.TwoWithScenarioJoinInDb(_testHelper.UnitOfWork, null, curveId);
            var controller = PerformanceCurveControllerTestSetup.SetupController(_testHelper, _testHelper.MockEsecSecurityAdmin);
            var scenarioCurves = _testHelper.UnitOfWork.PerformanceCurveRepo.GetScenarioPerformanceCurves(simulationId);
            var performanceCurveDto = scenarioCurves[0];
            performanceCurveDto.Equation = null;

            // Act
            await controller.UpsertScenarioPerformanceCurves(simulationId, scenarioCurves);

            // Assert
            var performanceCurveLibraryDtoAfter = _testHelper.UnitOfWork.PerformanceCurveRepo.GetScenarioPerformanceCurves(simulationId);
            var performanceCurveDtoAfter = performanceCurveLibraryDtoAfter.Single();
            Assert.Null(performanceCurveDtoAfter.Equation?.Expression);
            Assert.Equal(performanceCurveDto.Attribute, performanceCurveDtoAfter.Attribute);
            var equationAfter = _testHelper.UnitOfWork.Context.Equation.SingleOrDefault(e => e.Id == equation.Id);
            Assert.Null(equationAfter);
        }

        [Fact]
        public async Task UpsertScenarioPerformanceCurves_CurveInDbWithCriterionLibrary_CriterionLibraryUnchanged()
        {
            Setup();
            // Arrange
            var simulationId = Guid.NewGuid();
            var curveId = Guid.NewGuid();
            var simulationEntity = _testHelper.CreateSimulation(simulationId);
            var simulation = _testHelper.UnitOfWork.SimulationRepo.GetSimulation(simulationEntity.Id);
            var performanceCurve = ScenarioPerformanceCurveTestSetup.EntityInDb(_testHelper.UnitOfWork, simulationId, curveId);
            var controller = PerformanceCurveControllerTestSetup.SetupController(_testHelper, _testHelper.MockEsecSecurityAdmin);
            var criterionLibrary = CriterionLibraryTestSetup.TestCriterionLibraryInDb(_testHelper.UnitOfWork);
            CriterionLibraryScenarioPerformanceCurveJoinTestSetup.JoinCurveToCriterionLibrary(_testHelper.UnitOfWork, performanceCurve.Id, criterionLibrary.Id);
            var scenarioCurves = _testHelper.UnitOfWork.PerformanceCurveRepo.GetScenarioPerformanceCurves(simulationId);
            var performanceCurveDto = scenarioCurves[0];
            performanceCurveDto.CriterionLibrary = criterionLibrary.ToDto();

            // Act
            await controller.UpsertScenarioPerformanceCurves(simulationId, scenarioCurves);

            // Assert
            var scenarioCurvesAfter = _testHelper.UnitOfWork.PerformanceCurveRepo.GetScenarioPerformanceCurves(simulationId);
            var performanceCurveDtoAfter = scenarioCurvesAfter[0];
            Assert.Equal(criterionLibrary.Id, performanceCurveDtoAfter.CriterionLibrary.Id);
            Assert.Equal(performanceCurveDto.Attribute, performanceCurveDtoAfter.Attribute);
        }

        [Fact]
        public async Task UpsertScenarioPerformanceCurves_CurveInDbWithCriterionLibrary_UpdateRemovesCriterionLibraryFromCurve_CriterionLibraryRemoved()
        {
            Setup();
            // Arrange
            var simulationId = Guid.NewGuid();
            var curveId = Guid.NewGuid();
            var simulationEntity = _testHelper.CreateSimulation(simulationId);
            var simulation = _testHelper.UnitOfWork.SimulationRepo.GetSimulation(simulationEntity.Id);
            var performanceCurve = ScenarioPerformanceCurveTestSetup.EntityInDb(_testHelper.UnitOfWork, simulationId, curveId);
            var controller = PerformanceCurveControllerTestSetup.SetupController(_testHelper, _testHelper.MockEsecSecurityAdmin);
            var criterionLibrary = CriterionLibraryTestSetup.TestCriterionLibraryInDb(_testHelper.UnitOfWork);
            CriterionLibraryScenarioPerformanceCurveJoinTestSetup.JoinCurveToCriterionLibrary(_testHelper.UnitOfWork, performanceCurve.Id, criterionLibrary.Id);
            var scenarioCurves = _testHelper.UnitOfWork.PerformanceCurveRepo.GetScenarioPerformanceCurves(simulationId);
            var performanceCurveDto = scenarioCurves[0];
            performanceCurveDto.CriterionLibrary = null;

            // Act
            await controller.UpsertScenarioPerformanceCurves(simulationId, scenarioCurves);

            // Assert
            var scenarioCurvesAfter = _testHelper.UnitOfWork.PerformanceCurveRepo.GetScenarioPerformanceCurves(simulationId);
            var performanceCurveDtoAfter = scenarioCurvesAfter[0];
            Assert.Equal(Guid.Empty, performanceCurveDtoAfter.CriterionLibrary.Id);
            Assert.Equal(performanceCurveDto.Attribute, performanceCurveDtoAfter.Attribute);
            var criterionLibraryJoinAfter = _testHelper.UnitOfWork.Context.CriterionLibraryPerformanceCurve.SingleOrDefault(clpc =>
            clpc.PerformanceCurveId == curveId
            && clpc.CriterionLibraryId == criterionLibrary.Id);
            Assert.Null(criterionLibraryJoinAfter);
        }

        private const string Reason = "Not updated";
        [Fact]
        public async Task UpsertScenarioPerformanceCurves_CurveInDbWithCriterionLibrary_CurveRemovedFromUpsertedLibrary_RemovesCurveAndCriterionJoin()
        {
            Setup();
            // Arrange
            var simulationId = Guid.NewGuid();
            var curveId = Guid.NewGuid();
            var simulationEntity = _testHelper.CreateSimulation(simulationId);
            var simulation = _testHelper.UnitOfWork.SimulationRepo.GetSimulation(simulationEntity.Id);
            var performanceCurve = ScenarioPerformanceCurveTestSetup.EntityInDb(_testHelper.UnitOfWork, simulationId, curveId);
            var controller = PerformanceCurveControllerTestSetup.SetupController(_testHelper, _testHelper.MockEsecSecurityAdmin);
            var criterionLibrary = CriterionLibraryTestSetup.TestCriterionLibraryInDb(_testHelper.UnitOfWork);
            CriterionLibraryScenarioPerformanceCurveJoinTestSetup.JoinCurveToCriterionLibrary(_testHelper.UnitOfWork, curveId, criterionLibrary.Id);
            var scenarioCurves = _testHelper.UnitOfWork.PerformanceCurveRepo.GetScenarioPerformanceCurves(simulationId);
            scenarioCurves.RemoveAt(0);
            // Act
            await controller.UpsertScenarioPerformanceCurves(simulationId, scenarioCurves);

            // Assert
            var scenarioCurvesAfter = _testHelper.UnitOfWork.PerformanceCurveRepo.GetScenarioPerformanceCurves(simulationId);
            Assert.Empty(scenarioCurvesAfter);
            var criterionLibraryJoinAfter = _testHelper.UnitOfWork.Context.CriterionLibraryPerformanceCurve.SingleOrDefault(clpc =>
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
            var simulationEntity = _testHelper.CreateSimulation(simulationId);
            var simulation = _testHelper.UnitOfWork.SimulationRepo.GetSimulation(simulationEntity.Id);
            var performanceCurve = ScenarioPerformanceCurveTestSetup.EntityInDb(_testHelper.UnitOfWork, simulationId, curveId);
            var controller = PerformanceCurveControllerTestSetup.SetupController(_testHelper, _testHelper.MockEsecSecurityAdmin);
            var equation = EquationTestSetup.TwoWithScenarioJoinInDb(_testHelper.UnitOfWork, null, curveId);
            var scenarioCurves = _testHelper.UnitOfWork.PerformanceCurveRepo.GetScenarioPerformanceCurves(simulationId);
            var performanceCurveDto = scenarioCurves[0];
            performanceCurveDto.Equation.Expression = "123";

            // Act
            await controller.UpsertScenarioPerformanceCurves(simulationId, scenarioCurves);

            // Assert
            var scenarioCurvesAfter = _testHelper.UnitOfWork.PerformanceCurveRepo.GetScenarioPerformanceCurves(simulationId);
            var performanceCurveDtoAfter = scenarioCurvesAfter[0];
            Assert.Equal("123", performanceCurveDtoAfter.Equation.Expression);
            Assert.Equal(performanceCurveDto.Equation.Id, performanceCurveDtoAfter.Equation.Id);
        }

        [Fact]
        public async Task UpsertScenarioPerformanceCurves_CurveInDbWithEquation_UpdateRemovesCurve_EquationDeleted()
        {
            Setup();
            // Arrange
            var simulationId = Guid.NewGuid();
            var curveId = Guid.NewGuid();
            var simulationEntity = _testHelper.CreateSimulation(simulationId);
            var simulation = _testHelper.UnitOfWork.SimulationRepo.GetSimulation(simulationEntity.Id);
            var performanceCurve = ScenarioPerformanceCurveTestSetup.EntityInDb(_testHelper.UnitOfWork, simulationId, curveId);
            var equation = EquationTestSetup.TwoWithScenarioJoinInDb(_testHelper.UnitOfWork, null, curveId);
            var controller = PerformanceCurveControllerTestSetup.SetupController(_testHelper, _testHelper.MockEsecSecurityAdmin);
            var scenarioCurves = _testHelper.UnitOfWork.PerformanceCurveRepo.GetScenarioPerformanceCurves(simulationId);
            scenarioCurves.RemoveAt(0);

            // Act
            await controller.UpsertScenarioPerformanceCurves(simulationId, scenarioCurves);

            // Assert
            var scenarioCurvesAfter = _testHelper.UnitOfWork.PerformanceCurveRepo.GetScenarioPerformanceCurves(simulationId);
            Assert.Empty(scenarioCurvesAfter);
        }

        [Fact]
        public async Task UpsertSimulationPerformanceCurve_EmptySimulationInDb_Adds()
        {
            Setup();
            // Arrange
            var simulationId = Guid.NewGuid();
            var curveId = Guid.NewGuid();
            var simulationEntity = _testHelper.CreateSimulation(simulationId);
            var simulation = _testHelper.UnitOfWork.SimulationRepo.GetSimulation(simulationEntity.Id);
            var attribute = _testHelper.UnitOfWork.AttributeRepo.GetAttributes().First();
            var performanceCurveDto = new PerformanceCurveDTO
            {
                Attribute = attribute.Name,
                Id = Guid.NewGuid(),
                Name = "Curve",
            };
            var controller = PerformanceCurveControllerTestSetup.SetupController(_testHelper, _testHelper.MockEsecSecurityAdmin);
            var scenarioCurves = new List<PerformanceCurveDTO> { performanceCurveDto };

            await controller.UpsertScenarioPerformanceCurves(simulationId, scenarioCurves);

            var scenarioCurvesAfter = _testHelper.UnitOfWork.PerformanceCurveRepo.GetScenarioPerformanceCurves(simulationId);
            Assert.Single(scenarioCurvesAfter);
        }

        [Fact]
        public async Task UpsertSimulationPerformanceCurveWithEquation_EmptyLibraryInDb_AddsCurveAndEquationToLibrary()
        {
            Setup();
            // Arrange
            var simulationId = Guid.NewGuid();
            var curveId = Guid.NewGuid();
            var simulationEntity = _testHelper.CreateSimulation(simulationId);
            var simulation = _testHelper.UnitOfWork.SimulationRepo.GetSimulation(simulationEntity.Id);
            var attribute = _testHelper.UnitOfWork.AttributeRepo.GetAttributes().First();
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
            var controller = PerformanceCurveControllerTestSetup.SetupController(_testHelper, _testHelper.MockEsecSecurityAdmin);
            var scenarioCurves = new List<PerformanceCurveDTO> { performanceCurveDto };

            await controller.UpsertScenarioPerformanceCurves(simulationId, scenarioCurves);

            var scenarioCurvesAfter = _testHelper.UnitOfWork.PerformanceCurveRepo.GetScenarioPerformanceCurves(simulationId);
            var performanceCurveDtoAfter = scenarioCurvesAfter[0];
            var equationAfter = performanceCurveDtoAfter.Equation;
            Assert.Equal("3", performanceCurveDtoAfter.Equation.Expression);
            var equationEntity = _testHelper.UnitOfWork.Context.Equation.SingleOrDefault(e => e.Id == equationAfter.Id);
            Assert.NotNull(equationEntity);
        }

        [Fact]
        public async Task UpsertScenarioPerformanceCurveWithEquation_EmptySimulationInDb_AddsCurveAndEquationToSimulation()
        {
            Setup();
            // Arrange
            var simulationId = Guid.NewGuid();
            var curveId = Guid.NewGuid();
            var simulationEntity = _testHelper.CreateSimulation(simulationId);
            var simulation = _testHelper.UnitOfWork.SimulationRepo.GetSimulation(simulationEntity.Id);
            var attribute = _testHelper.UnitOfWork.AttributeRepo.GetAttributes().First();
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
            var controller = PerformanceCurveControllerTestSetup.SetupController(_testHelper, _testHelper.MockEsecSecurityAdmin);
            var scenarioCurves = new List<PerformanceCurveDTO> { performanceCurveDto };

            await controller.UpsertScenarioPerformanceCurves(simulationId, scenarioCurves);

            var scenarioCurvesAfter = _testHelper.UnitOfWork.PerformanceCurveRepo.GetScenarioPerformanceCurves(simulationId);
            var performanceCurveDtoAfter = scenarioCurvesAfter[0];
            var equationAfter = performanceCurveDtoAfter.Equation;
            Assert.Null(equationAfter.Expression);
            var equationEntity = _testHelper.UnitOfWork.Context.Equation.SingleOrDefault(e => e.Id == equationAfter.Id);
            Assert.Null(equationEntity);
        }

        [Fact]
        public async Task UpsertScenarioPerformanceCurveWithCriterionLibrary_EmptySimulationInDb_AddsPerformanceCurveAndCriterionLibraryToSimulation()
        {
            Setup();
            // Arrange
            var simulationId = Guid.NewGuid();
            var curveId = Guid.NewGuid();
            var simulationEntity = _testHelper.CreateSimulation(simulationId);
            var simulation = _testHelper.UnitOfWork.SimulationRepo.GetSimulation(simulationEntity.Id);
            var attribute = _testHelper.UnitOfWork.AttributeRepo.GetAttributes().First();
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
            var performanceCurveDtos = new List<PerformanceCurveDTO> { performanceCurveDto };
            var controller = PerformanceCurveControllerTestSetup.SetupController(_testHelper, _testHelper.MockEsecSecurityAdmin);

            await controller.UpsertScenarioPerformanceCurves(simulationId, performanceCurveDtos);

            var scenarioCurvesAfter = _testHelper.UnitOfWork.PerformanceCurveRepo.GetScenarioPerformanceCurves(simulationId);
            var performanceCurveDtoAfter = scenarioCurvesAfter[0];
            var criterionLibraryAfter = performanceCurveDtoAfter.CriterionLibrary;
            Assert.Equal("MergedCriteriaExpression", criterionLibraryAfter.MergedCriteriaExpression);
        }
    }
}