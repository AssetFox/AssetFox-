using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.LibraryEntities.PerformanceCurve;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.ScenarioEntities.PerformanceCurve;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.UnitTestsCore.TestUtils;
using BridgeCareCore.Controllers;
using BridgeCareCore.Models;
using BridgeCareCore.Security;
using BridgeCareCore.Security.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using MoreLinq;
using Xunit;
using Assert = Xunit.Assert;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests.APITestClasses
{
    public class PerformanceCurveTests
    {
        private TestHelper _testHelper => TestHelper.Instance;

        private static readonly Guid EquationId = Guid.Parse("a6c65132-e45c-4a48-a0b2-72cd274c9cc2");

        private void Setup()
        {
            _testHelper.CreateAttributes();
            _testHelper.CreateNetwork();
            _testHelper.SetupDefaultHttpContext();
        }



        public EquationEntity TestEquation { get; } = new EquationEntity
        {
            Id = EquationId,
            Expression = "Test Expression"
        };

        private PerformanceCurveController SetupController(Moq.Mock<IEsecSecurity> mockedEsecSecurity)
        {
            var controller = new PerformanceCurveController(
                mockedEsecSecurity.Object,
                _testHelper.UnitOfWork,
                _testHelper.MockHubService.Object,
                _testHelper.MockHttpContextAccessor.Object,
                TestServices.PerformanceCurves);
            return controller;
        }

        private void SetupForGet(Guid performanceCurveLibraryId, Guid performanceCurveId)
        {
            var testLibrary = PerformanceCurveLibraryTestSetup.TestPerformanceCurveLibraryInDb(_testHelper.UnitOfWork, performanceCurveLibraryId);
            var curve = PerformanceCurveTestSetup.TestPerformanceCurveInDb(_testHelper.UnitOfWork, performanceCurveLibraryId, performanceCurveId);
        }

        private CriterionLibraryEntity AddTestCriterionLibrary()
        {
            var criterionLibrary = _testHelper.TestCriterionLibrary();
            _testHelper.UnitOfWork.Context.CriterionLibrary.Add(criterionLibrary);
            _testHelper.UnitOfWork.Context.SaveChanges();
            return criterionLibrary;
        }

        private CriterionLibraryEntity SetupForUpsertOrDelete(Guid performanceCurveLibaryId, Guid performanceCurveId)
        {
            SetupForGet(performanceCurveLibaryId, performanceCurveId);
            return AddTestCriterionLibrary();
        }

        private ScenarioPerformanceCurveEntity SetupForScenarioCurveGet(Guid simulationId, Guid performanceCurveId)
        {
            var performanceCurve = PerformanceCurveTestSetup.ScenarioEntity(simulationId, performanceCurveId);
            performanceCurve.AttributeId = _testHelper.UnitOfWork.Context.Attribute.First().Id;
            _testHelper.UnitOfWork.Context.ScenarioPerformanceCurve.Add(performanceCurve);
            _testHelper.UnitOfWork.Context.SaveChanges();
            return performanceCurve;
        }

        private CriterionLibraryEntity SetupForScenarioCurveUpsertOrDelete(Guid simulationId, Guid performanceCurveId)
        {
            SetupForScenarioCurveGet(simulationId, performanceCurveId);
            var criterionLibrary = AddTestCriterionLibrary();
            return criterionLibrary;
        }

        [Fact]
        public async Task GetPerformanceCurveLibraries_Ok()
        {
            Setup();
            // Arrange
            var controller = SetupController(_testHelper.MockEsecSecurityAdmin);

            // Act
            var result = await controller.GetPerformanceCurveLibraries();

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]

        public async Task UpsertPerformanceCurveLibrary_Ok()
        {
            Setup();
            // Arrange
            var controller = SetupController(_testHelper.MockEsecSecurityAdmin);
            var libraryId = Guid.NewGuid();

            // Act
            var result = await controller
                .UpsertPerformanceCurveLibrary(PerformanceCurveLibraryTestSetup.TestPerformanceCurveLibrary(libraryId).ToDto());

            // Assert
            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async Task Delete_PerformanceCurveLibraryDoesNotExist_Ok()
        {
            Setup();

            // Arrange
            var controller = SetupController(_testHelper.MockEsecSecurityAdmin);

            // Act
            var result = await controller.DeletePerformanceCurveLibrary(Guid.NewGuid());

            // Assert
            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async Task GetScenarioPerformanceCurves_SimulationExists_Ok()
        {
            Setup();

            // Arrange
            var simulation = _testHelper.CreateSimulation();
            var controller = SetupController(_testHelper.MockEsecSecurityAdmin);

            // Act
            var result = await controller.GetScenarioPerformanceCurves(simulation.Id);

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task UpsertScenarioPerformanceCurves_SimulationExists_Ok()
        {
            Setup();
            // Arrange
            var simulation = _testHelper.CreateSimulation();
            var performanceCurveId = Guid.NewGuid();
            var controller = SetupController(_testHelper.MockEsecSecurityAdmin);
            var performanceCurve = PerformanceCurveTestSetup.ScenarioEntity(simulation.Id, performanceCurveId);
            performanceCurve.Attribute = _testHelper.UnitOfWork.Context.Attribute.First();

            // Act
            var result = await controller
                .UpsertScenarioPerformanceCurves(simulation.Id,
                    new List<PerformanceCurveDTO> { performanceCurve.ToDto() });

            // Assert
            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async Task ShouldGetAllPerformanceCurveLibrariesWithPerformanceCurves()
        {
            Setup();
            var libraryId = Guid.NewGuid();
            var curveId = Guid.NewGuid();
            // Arrange
            var controller = SetupController(_testHelper.MockEsecSecurityAdmin);
            SetupForGet(libraryId, curveId);

            // Act
            var result = await controller.GetPerformanceCurveLibraries();

            // Assert
            var okObjResult = result as OkObjectResult;
            Assert.NotNull(okObjResult.Value);

            var dtos = (List<PerformanceCurveLibraryDTO>)Convert.ChangeType(okObjResult.Value,
                typeof(List<PerformanceCurveLibraryDTO>));
            var dto = dtos.Single(pc => pc.Id == libraryId);

            Assert.Single(dto.PerformanceCurves);

            Assert.Equal(curveId, dto.PerformanceCurves[0].Id);
        }

        [Fact]
        //[Fact(Skip = "Still broken 7/25/2022. Need to fix the underlying api.")]
        public async Task ShouldModifyPerformanceCurveData()
        {
            Setup();
            // Arrange
            var libraryId = Guid.NewGuid();
            var curveId = Guid.NewGuid();
            var library = PerformanceCurveLibraryTestSetup.TestPerformanceCurveLibraryInDb(_testHelper.UnitOfWork, libraryId);
            var performanceCurve = PerformanceCurveTestSetup.TestPerformanceCurveInDb(_testHelper.UnitOfWork, libraryId, curveId);
            var equation = EquationTestSetup.TwoWithJoinInDb(_testHelper.UnitOfWork, null, curveId);
            var controller = SetupController(_testHelper.MockEsecSecurityAdmin);
            var criterionLibrary = AddTestCriterionLibrary();
            var getResult = await controller.GetPerformanceCurveLibraries();
            var dtos = (List<PerformanceCurveLibraryDTO>)Convert.ChangeType((getResult as OkObjectResult).Value,
                typeof(List<PerformanceCurveLibraryDTO>));

            var performanceCurveLibraryDto = dtos.Single(dto => dto.Id == libraryId);
            performanceCurveLibraryDto.Description = "Updated Description";
            var performanceCurveDto = performanceCurveLibraryDto.PerformanceCurves[0];
            performanceCurveDto.Shift = true;
            performanceCurveDto.CriterionLibrary =
                criterionLibrary.ToDto();
            performanceCurveDto.Equation = equation.ToDto();

            // Act
            await controller.UpsertPerformanceCurveLibrary(performanceCurveLibraryDto);

            // Assert

            var performanceCurveLibraryDtosAfter = _testHelper.UnitOfWork.PerformanceCurveRepo
                .GetPerformanceCurveLibraries();
            var performanceCurveLibraryDtoAfter = performanceCurveLibraryDtosAfter.Single(pcl => pcl.Id == performanceCurveLibraryDto.Id);

            Assert.Equal(performanceCurveLibraryDto.Description, performanceCurveLibraryDtoAfter.Description);

            var performanceCurveDtoAfter = performanceCurveLibraryDtoAfter.PerformanceCurves.Single();
            Assert.Equal(performanceCurveDto.Shift, performanceCurveDtoAfter.Shift);
            Assert.Equal(criterionLibrary.Id,
                performanceCurveDtoAfter.CriterionLibrary.Id); // Wjwjwj fails here.
            Assert.Equal(performanceCurveDto.Equation.Id,
                performanceCurveDtoAfter.Equation.Id);
            Assert.Equal(performanceCurveDto.Attribute,
                performanceCurveDtoAfter.Attribute);

        }

        [Fact]
        public async Task Delete_PerformanceCurveLibraryExists_Deletes()
        {
            Setup();
            // Arrange
            var controller = SetupController(_testHelper.MockEsecSecurityAdmin);
            var performanceCurveLibraryId = Guid.NewGuid();
            var performanceCurveId = Guid.NewGuid();
            var criterionLibrary = SetupForUpsertOrDelete(performanceCurveLibraryId, performanceCurveId);
            var getResult = await controller.GetPerformanceCurveLibraries();
            var dtos = (List<PerformanceCurveLibraryDTO>)Convert.ChangeType((getResult as OkObjectResult).Value,
                typeof(List<PerformanceCurveLibraryDTO>));

            var performanceCurveLibraryDTO = dtos.Single(dto => dto.Id == performanceCurveLibraryId);
            performanceCurveLibraryDTO.PerformanceCurves[0].CriterionLibrary =
                criterionLibrary.ToDto();

            await controller.UpsertPerformanceCurveLibrary(performanceCurveLibraryDTO);

            // Act
            var result = controller.DeletePerformanceCurveLibrary(performanceCurveLibraryId);

            // Assert
            Assert.IsType<OkResult>(result.Result);

            Assert.False(_testHelper.UnitOfWork.Context.PerformanceCurveLibrary.Any(_ => _.Id == performanceCurveLibraryId));
            Assert.False(_testHelper.UnitOfWork.Context.PerformanceCurve.Any(_ => _.Id == performanceCurveId));
            Assert.False(
                _testHelper.UnitOfWork.Context.CriterionLibraryPerformanceCurve.Any(_ =>
                    _.PerformanceCurveId == performanceCurveId));
            Assert.False(
                _testHelper.UnitOfWork.Context.PerformanceCurveEquation.Any(_ =>
                    _.PerformanceCurveId == performanceCurveId));
            Assert.False(_testHelper.UnitOfWork.Context.Equation.Any(_ => _.Id == EquationId));
            Assert.False(
                _testHelper.UnitOfWork.Context.Attribute.Any(_ => _.PerformanceCurves.Any()));
        }

        [Fact]
        public async Task GetScenarioPerformanceCurves_SimulationInDbWithPerformanceCurve_Gets()
        {
            Setup();
            // Arrange
            var simulation = _testHelper.CreateSimulation();
            var controller = SetupController(_testHelper.MockEsecSecurityAdmin);
            var performanceCurveId = Guid.NewGuid();
            SetupForScenarioCurveGet(simulation.Id, performanceCurveId);

            // Act
            var result = await controller.GetScenarioPerformanceCurves(simulation.Id);

            // Assert
            var okObjResult = result as OkObjectResult;
            Assert.NotNull(okObjResult.Value);

            var dtos = (List<PerformanceCurveDTO>)Convert.ChangeType(okObjResult.Value,
                typeof(List<PerformanceCurveDTO>));
            Assert.Single(dtos);
            Assert.Equal(performanceCurveId, dtos[0].Id);
        }

        // [Fact(Skip = "Broken. Had a timer.")]
        [Fact]
        public async Task ShouldModifyScenarioPerformanceCurveData()
        {
            Setup();
            // Arrange
            var simulation = _testHelper.CreateSimulation();
            var controller = SetupController(_testHelper.MockEsecSecurityAdmin);
            var attribute = _testHelper.UnitOfWork.Context.Attribute.First();

            var deletedCurveId = Guid.NewGuid();
            _testHelper.UnitOfWork.Context.ScenarioPerformanceCurve.Add(new ScenarioPerformanceCurveEntity
            {
                Id = deletedCurveId,
                SimulationId = simulation.Id,
                AttributeId = attribute.Id,
                Shift = false,
                Name = "Deleted"
            });
            var performanceCurveId = Guid.NewGuid();

            var criterionLibrary = SetupForScenarioCurveUpsertOrDelete(simulation.Id, performanceCurveId);

            var localScenarioPerformanceCurves = _testHelper.UnitOfWork.PerformanceCurveRepo
                .GetScenarioPerformanceCurves(simulation.Id);
            Assert.Equal(2, localScenarioPerformanceCurves.Count);
            var indexToDelete = localScenarioPerformanceCurves.FindIndex(pc => pc.Id == deletedCurveId);
            var indexToUpdate = 1 - indexToDelete;
            var curveToUpdate = localScenarioPerformanceCurves[indexToUpdate];
            var idToUpdate = curveToUpdate.Id;
            curveToUpdate.Name = "Updated";
            curveToUpdate.CriterionLibrary = criterionLibrary.ToDto();
            curveToUpdate.Equation = TestEquation.ToDto();
            var curveToDelete = localScenarioPerformanceCurves.Single(curve => curve.Id == deletedCurveId);
            localScenarioPerformanceCurves.Remove(curveToDelete);
            var idToAdd = Guid.NewGuid();
            localScenarioPerformanceCurves.Add(new PerformanceCurveDTO
            {
                Id = idToAdd,
                Attribute = attribute.Name,
                Shift = false,
                Name = "New"
            });

            // Act
            await controller.UpsertScenarioPerformanceCurves(simulation.Id, localScenarioPerformanceCurves);

            // Assert
            var scenarioPerformanceCurvesAfter = _testHelper.UnitOfWork.PerformanceCurveRepo
                .GetScenarioPerformanceCurves(simulation.Id);
            Assert.Equal(localScenarioPerformanceCurves.Count, scenarioPerformanceCurvesAfter.Count);

            Assert.False(
                _testHelper.UnitOfWork.Context.ScenarioPerformanceCurve.Any(_ => _.Id == deletedCurveId));

            var localNewCurve = localScenarioPerformanceCurves.Single(_ => _.Name == "New");
            var serverNewCurve = scenarioPerformanceCurvesAfter.FirstOrDefault(_ => _.Id == localNewCurve.Id);
            Assert.NotNull(serverNewCurve);
            Assert.Equal(localNewCurve.Attribute, serverNewCurve.Attribute);

            var localUpdatedCurve = localScenarioPerformanceCurves.Single(_ => _.Id == idToUpdate);
            var serverUpdatedCurve = scenarioPerformanceCurvesAfter
                .Single(_ => _.Id == idToUpdate);
            Assert.Equal(localUpdatedCurve.Name, serverUpdatedCurve.Name);
            Assert.Equal(localUpdatedCurve.Attribute, serverUpdatedCurve.Attribute);
            //    Assert.Equal(localUpdatedCurve.CriterionLibrary.Id, serverUpdatedCurve.CriterionLibrary.Id);
            // Wjwjwj revisit the above and the below after understanding the situation with the similar error in the previous test
            Assert.Equal(localUpdatedCurve.CriterionLibrary.MergedCriteriaExpression,
                serverUpdatedCurve.CriterionLibrary.MergedCriteriaExpression);
            //    Assert.Equal(localUpdatedCurve.Equation.Id, serverUpdatedCurve.Equation.Id);

            Assert.Equal(localUpdatedCurve.Equation.Expression, serverUpdatedCurve.Equation.Expression);
        }

        [Fact]
        public async Task Post_UserIsUnauthorized_ReturnsUnauthorized()
        {
            Setup();
            // Arrange
            var simulation = _testHelper.CreateSimulation();
            _testHelper.SetupDefaultHttpContext();
            var mockedUnauthorized = new Mock<IEsecSecurity>();
            mockedUnauthorized.Setup(_ => _.GetUserInformation(It.IsAny<HttpRequest>()))
                .Returns(new UserInfo
                {
                    Name = "districtEngineer",
                    Role = Role.DistrictEngineer,
                    Email = "fake@pa.gov"
                });
            var controller = SetupController(mockedUnauthorized);
            var performanceCurveId = Guid.NewGuid();
            var performanceCurve = PerformanceCurveTestSetup.ScenarioEntity(simulation.Id, performanceCurveId);
            performanceCurve.Attribute = _testHelper.UnitOfWork.Context.Attribute.First();

            // Act
            var upsertScenarioPerformanceCurveLibraryResult = await controller
                .UpsertScenarioPerformanceCurves(simulation.Id,
                    new List<PerformanceCurveDTO> { performanceCurve.ToDto() });

            // Assert
            Assert.IsType<UnauthorizedResult>(upsertScenarioPerformanceCurveLibraryResult);
        }
    }
}
