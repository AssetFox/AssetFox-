using System;
using System.Linq;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.LibraryEntities.PerformanceCurve;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.ScenarioEntities.PerformanceCurve;
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
        // Uncomment the below when we fix the repo
        //Assert.Equal(performanceCurveDto.Equation.Id, performanceCurveDtoAfter.Equation.Id);
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
            // uncomment the below when fixing the repo wjwjwj
    //        Assert.Equal(criterionLibrary.Id, performanceCurveDtoAfter.CriterionLibrary.Id);
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
        [Fact (Skip =Reason)]
        public async Task UpsertPerformanceCurveLibrary_CurveInDbWithCriterionLibrary_CurveRemovedFromUpsertedLibrary_RemovesCurveAndCriterionJoin()
        {
            Setup();
            // Arrange
            var libraryId = Guid.NewGuid();
            var curveId = Guid.NewGuid();
            var library = PerformanceCurveLibraryTestSetup.TestPerformanceCurveLibraryInDb(_testHelper.UnitOfWork, libraryId);
            var performanceCurve = PerformanceCurveTestSetup.TestPerformanceCurveInDb(_testHelper.UnitOfWork, libraryId, curveId);
            var controller = PerformanceCurveControllerTestSetup.SetupController(_testHelper, _testHelper.MockEsecSecurityAdmin);
            var criterionLibrary = CriterionLibraryTestSetup.TestCriterionLibraryInDb(_testHelper.UnitOfWork);
            var criterionCurveJoin = new CriterionLibraryPerformanceCurveEntity
            {
                PerformanceCurveId = performanceCurve.Id,
                CriterionLibraryId = criterionLibrary.Id
            };
            _testHelper.UnitOfWork.Context.Add(criterionCurveJoin);
            var performanceCurveLibraryDto = _testHelper.UnitOfWork.PerformanceCurveRepo.GetPerformanceCurveLibrary(libraryId);
            performanceCurveLibraryDto.PerformanceCurves.RemoveAt(0);

            // Act
            await controller.UpsertPerformanceCurveLibrary(performanceCurveLibraryDto);

            // Assert
            var performanceCurveLibraryDtoAfter = _testHelper.UnitOfWork.PerformanceCurveRepo.GetPerformanceCurveLibrary(libraryId);
            Assert.Empty(performanceCurveLibraryDtoAfter.PerformanceCurves);
            var criterionLibraryJoinAfter = _testHelper.UnitOfWork.Context.CriterionLibraryPerformanceCurve.SingleOrDefault(clpc =>
            clpc.PerformanceCurveId == curveId
            && clpc.CriterionLibraryId == libraryId);
            Assert.Null(criterionLibraryJoinAfter);
        }

        [Fact(Skip = Reason)]
        public async Task UpsertPerformanceCurveLibrary_CurveInDbWithEquation_UpdateChangesExpression_ExpressionChangedInDb()
        {
            Setup();
            // Arrange
            var libraryId = Guid.NewGuid();
            var curveId = Guid.NewGuid();
            var library = PerformanceCurveLibraryTestSetup.TestPerformanceCurveLibraryInDb(_testHelper.UnitOfWork, libraryId);
            var performanceCurve = PerformanceCurveTestSetup.TestPerformanceCurveInDb(_testHelper.UnitOfWork, libraryId, curveId);
            var equation = EquationTestSetup.TwoWithJoinInDb(_testHelper.UnitOfWork, null, curveId);
            var controller = PerformanceCurveControllerTestSetup.SetupController(_testHelper, _testHelper.MockEsecSecurityAdmin);
            var performanceCurveLibraryDto = _testHelper.UnitOfWork.PerformanceCurveRepo.GetPerformanceCurveLibrary(libraryId);
            var performanceCurveDto = performanceCurveLibraryDto.PerformanceCurves[0];
            performanceCurveDto.Equation.Expression = "123";

            // Act
            await controller.UpsertPerformanceCurveLibrary(performanceCurveLibraryDto);

            // Assert
            var performanceCurveLibraryDtoAfter = _testHelper.UnitOfWork.PerformanceCurveRepo.GetPerformanceCurveLibrary(libraryId);
            var performanceCurveAfter = performanceCurveLibraryDtoAfter.PerformanceCurves[0];
            Assert.Equal("123", performanceCurveAfter.Equation.Expression);
            Assert.Equal(performanceCurveDto.Equation.Id, performanceCurveAfter.Equation.Id);
        }

        [Fact(Skip = Reason)]
        public async Task UpsertPerformanceCurveLibrary_CurveInDbWithEquation_UpdateRemovesCurve_EquationDeleted()
        {
            Setup();
            // Arrange
            var libraryId = Guid.NewGuid();
            var curveId = Guid.NewGuid();
            var library = PerformanceCurveLibraryTestSetup.TestPerformanceCurveLibraryInDb(_testHelper.UnitOfWork, libraryId);
            var performanceCurve = PerformanceCurveTestSetup.TestPerformanceCurveInDb(_testHelper.UnitOfWork, libraryId, curveId);
            var equation = EquationTestSetup.TwoWithJoinInDb(_testHelper.UnitOfWork, null, curveId);
            var controller = PerformanceCurveControllerTestSetup.SetupController(_testHelper, _testHelper.MockEsecSecurityAdmin);
            var performanceCurveLibraryDto = _testHelper.UnitOfWork.PerformanceCurveRepo.GetPerformanceCurveLibrary(libraryId);
            performanceCurveLibraryDto.PerformanceCurves.RemoveAt(0);

            // Act
            await controller.UpsertPerformanceCurveLibrary(performanceCurveLibraryDto);

            // Assert
            var performanceCurveLibraryDtoAfter = _testHelper.UnitOfWork.PerformanceCurveRepo.GetPerformanceCurveLibrary(libraryId);
            Assert.Empty(performanceCurveLibraryDtoAfter.PerformanceCurves);
        }

        [Fact(Skip = Reason)]
        public async Task UpsertPerformanceCurve_EmptyLibraryInDb_Adds()
        {
            Setup();
            var libraryId = Guid.NewGuid();
            var library = PerformanceCurveLibraryTestSetup.TestPerformanceCurveLibraryInDb(_testHelper.UnitOfWork, libraryId);
            var performanceCurveLibraryDto = _testHelper.UnitOfWork.PerformanceCurveRepo.GetPerformanceCurveLibrary(libraryId);
            var attribute = _testHelper.UnitOfWork.AttributeRepo.GetAttributes().First();
            var performanceCurveDto = new PerformanceCurveDTO
            {
                Attribute = attribute.Name,
                Id = Guid.NewGuid(),
                Name = "Curve",
            };
            Assert.Empty(performanceCurveLibraryDto.PerformanceCurves);
            performanceCurveLibraryDto.PerformanceCurves.Add(performanceCurveDto); ;
            var controller = PerformanceCurveControllerTestSetup.SetupController(_testHelper, _testHelper.MockEsecSecurityAdmin);

            await controller.UpsertPerformanceCurveLibrary(performanceCurveLibraryDto);

            var performanceCurveLibraryDtoAfter = _testHelper.UnitOfWork.PerformanceCurveRepo.GetPerformanceCurveLibrary(libraryId);
            Assert.Single(performanceCurveLibraryDtoAfter.PerformanceCurves);
        }

        [Fact(Skip = Reason)]
        public async Task UpsertPerformanceCurveWithEquation_EmptyLibraryInDb_AddsCurveAndEquationToLibrary()
        {
            Setup();
            var libraryId = Guid.NewGuid();
            var library = PerformanceCurveLibraryTestSetup.TestPerformanceCurveLibraryInDb(_testHelper.UnitOfWork, libraryId);
            var performanceCurveLibraryDto = _testHelper.UnitOfWork.PerformanceCurveRepo.GetPerformanceCurveLibrary(libraryId);
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
            Assert.Empty(performanceCurveLibraryDto.PerformanceCurves);
            performanceCurveLibraryDto.PerformanceCurves.Add(performanceCurveDto); ;
            var controller = PerformanceCurveControllerTestSetup.SetupController(_testHelper, _testHelper.MockEsecSecurityAdmin);

            await controller.UpsertPerformanceCurveLibrary(performanceCurveLibraryDto);

            var performanceCurveLibraryDtoAfter = _testHelper.UnitOfWork.PerformanceCurveRepo.GetPerformanceCurveLibrary(libraryId);
            var performanceCurveAfter = performanceCurveLibraryDtoAfter.PerformanceCurves.Single();
            var equationAfter = performanceCurveAfter.Equation;
            Assert.Equal("3", equationAfter.Expression);
            var equationEntity = _testHelper.UnitOfWork.Context.Equation.SingleOrDefault(e => e.Id == equationAfter.Id);
            Assert.NotNull(equationEntity);
        }

        [Fact(Skip = Reason)]
        public async Task UpsertPerformanceCurveWithEquationWithEmptyId_EmptyLibraryInDb_AddsCurveToLibraryWithoutEquation()
        {
            Setup();
            var libraryId = Guid.NewGuid();
            var library = PerformanceCurveLibraryTestSetup.TestPerformanceCurveLibraryInDb(_testHelper.UnitOfWork, libraryId);
            var performanceCurveLibraryDto = _testHelper.UnitOfWork.PerformanceCurveRepo.GetPerformanceCurveLibrary(libraryId);
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
            Assert.Empty(performanceCurveLibraryDto.PerformanceCurves);
            performanceCurveLibraryDto.PerformanceCurves.Add(performanceCurveDto); ;
            var controller = PerformanceCurveControllerTestSetup.SetupController(_testHelper, _testHelper.MockEsecSecurityAdmin);

            await controller.UpsertPerformanceCurveLibrary(performanceCurveLibraryDto);

            var performanceCurveLibraryDtoAfter = _testHelper.UnitOfWork.PerformanceCurveRepo.GetPerformanceCurveLibrary(libraryId);
            var performanceCurveAfter = performanceCurveLibraryDtoAfter.PerformanceCurves.Single();
            var equationAfter = performanceCurveAfter.Equation;
            Assert.Null(equationAfter.Expression);
        }


        [Fact(Skip = Reason)]
        public async Task UpsertPerformanceCurveWithCriterionLibrary_EmptyLibraryInDb_AddsPerformanceCurveAndCriterionLibrary()
        {
            Setup();
            var libraryId = Guid.NewGuid();
            var library = PerformanceCurveLibraryTestSetup.TestPerformanceCurveLibraryInDb(_testHelper.UnitOfWork, libraryId);
            var performanceCurveLibraryDto = _testHelper.UnitOfWork.PerformanceCurveRepo.GetPerformanceCurveLibrary(libraryId);
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
            Assert.Empty(performanceCurveLibraryDto.PerformanceCurves);
            performanceCurveLibraryDto.PerformanceCurves.Add(performanceCurveDto); ;
            var controller = PerformanceCurveControllerTestSetup.SetupController(_testHelper, _testHelper.MockEsecSecurityAdmin);

            await controller.UpsertPerformanceCurveLibrary(performanceCurveLibraryDto);

            var performanceCurveLibraryDtoAfter = _testHelper.UnitOfWork.PerformanceCurveRepo.GetPerformanceCurveLibrary(libraryId);
            var performanceCurveDtoAfter = performanceCurveLibraryDtoAfter.PerformanceCurves.Single();
            var criterionLibraryAfter = performanceCurveDtoAfter.CriterionLibrary;
            Assert.Equal("MergedCriteriaExpression", criterionLibraryAfter.MergedCriteriaExpression);
        }
    }
}
