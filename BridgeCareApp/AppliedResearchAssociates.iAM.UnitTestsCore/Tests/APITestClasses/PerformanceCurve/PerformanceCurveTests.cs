using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.UnitTestsCore.TestUtils;
using BridgeCareCore.Models;
using BridgeCareCore.Security;
using BridgeCareCore.Security.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using MoreLinq;
using Xunit;
using Assert = Xunit.Assert;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests
{
    public class PerformanceCurveTests
    {
        private TestHelper _testHelper => TestHelper.Instance;

        private void Setup()
        {
            _testHelper.CreateAttributes();
            _testHelper.CreateNetwork();
            _testHelper.SetupDefaultHttpContext();
        }

        [Fact]
        public async Task GetPerformanceCurveLibraries_Ok()
        {
            Setup();
            // Arrange
            var controller = PerformanceCurveControllerTestSetup.SetupController(_testHelper, _testHelper.MockEsecSecurityAdmin);

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
            var controller = PerformanceCurveControllerTestSetup.SetupController(_testHelper, _testHelper.MockEsecSecurityAdmin);
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
            var controller = PerformanceCurveControllerTestSetup.SetupController(_testHelper, _testHelper.MockEsecSecurityAdmin);

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
            var controller = PerformanceCurveControllerTestSetup.SetupController(_testHelper, _testHelper.MockEsecSecurityAdmin);

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
            var controller = PerformanceCurveControllerTestSetup.SetupController(_testHelper, _testHelper.MockEsecSecurityAdmin);
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
            var controller = PerformanceCurveControllerTestSetup.SetupController(_testHelper, _testHelper.MockEsecSecurityAdmin);
            var testLibrary = PerformanceCurveLibraryTestSetup.TestPerformanceCurveLibraryInDb(_testHelper.UnitOfWork, libraryId);
            var curve = PerformanceCurveTestSetup.TestPerformanceCurveInDb(_testHelper.UnitOfWork, libraryId, curveId);


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
        public async Task Delete_PerformanceCurveLibraryExists_Deletes()
        {
            Setup();
            // Arrange
            var controller = PerformanceCurveControllerTestSetup.SetupController(_testHelper, _testHelper.MockEsecSecurityAdmin);
            var performanceCurveLibraryId = Guid.NewGuid();
            var performanceCurveId = Guid.NewGuid();
            var testLibrary = PerformanceCurveLibraryTestSetup.TestPerformanceCurveLibraryInDb(_testHelper.UnitOfWork, performanceCurveLibraryId);
            var curve = PerformanceCurveTestSetup.TestPerformanceCurveInDb(_testHelper.UnitOfWork, performanceCurveLibraryId, performanceCurveId);
            var criterionLibrary = CriterionLibraryTestSetup.TestCriterionLibraryInDb(_testHelper.UnitOfWork);
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
        }

        [Fact]
        public async Task GetScenarioPerformanceCurves_SimulationInDbWithPerformanceCurve_Gets()
        {
            Setup();
            // Arrange
            var simulation = _testHelper.CreateSimulation();
            var controller = PerformanceCurveControllerTestSetup.SetupController(_testHelper, _testHelper.MockEsecSecurityAdmin);
            var performanceCurveId = Guid.NewGuid();
            ScenarioPerformanceCurveTestSetup.SetupForScenarioCurveGet(_testHelper.UnitOfWork, simulation.Id, performanceCurveId);

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
            var controller = PerformanceCurveControllerTestSetup.SetupController(_testHelper, mockedUnauthorized);
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
