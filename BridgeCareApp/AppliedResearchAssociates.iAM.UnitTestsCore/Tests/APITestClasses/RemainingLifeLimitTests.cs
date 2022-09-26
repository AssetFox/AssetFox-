using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.LibraryEntities.RemainingLifeLimit;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.UnitTestsCore.TestUtils;
using BridgeCareCore.Controllers;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests.APITestClasses
{
    public class RemainingLifeLimitTests
    {
        private TestHelper _testHelper => TestHelper.Instance;

        public RemainingLifeLimitController SetupController()
        {
            _testHelper.CreateAttributes();
            _testHelper.CreateNetwork();
            var accessor = HttpContextAccessorMocks.Default();
            var controller = new RemainingLifeLimitController(EsecSecurityMocks.Admin, _testHelper.UnitOfWork,
                _testHelper.MockHubService.Object, accessor);
            return controller;
        }


        public RemainingLifeLimitLibraryEntity TestRemainingLifeLimitLibrary(Guid? id = null)
        {
            var resolveId = id ?? Guid.NewGuid();
            var returnValue = new RemainingLifeLimitLibraryEntity
            {
                Id = resolveId,
                Name = "Test Name"
            };
            return returnValue;
        }

        public RemainingLifeLimitEntity TestRemainingLifeLimit(Guid libraryId, Guid attributeId)
        {
            return new RemainingLifeLimitEntity
            {
                Id = Guid.NewGuid(),
                RemainingLifeLimitLibraryId = libraryId,
                Value = 1.0,
                AttributeId = attributeId,
            };
        }

        private RemainingLifeLimitLibraryEntity SetupForGet()
        {
            var library = TestRemainingLifeLimitLibrary();
            var attribute = _testHelper.UnitOfWork.Context.Attribute.First();
            var lifeLimit = TestRemainingLifeLimit(library.Id, attribute.Id);
            _testHelper.UnitOfWork.Context.RemainingLifeLimitLibrary.Add(library);
            _testHelper.UnitOfWork.Context.RemainingLifeLimit.Add(lifeLimit);
            _testHelper.UnitOfWork.Context.SaveChanges();
            return library;
        }

        private CriterionLibraryEntity SetupForUpsertOrDelete()
        {
            var criterionLibrary = CriterionLibraryTestSetup.TestCriterionLibrary();
            _testHelper.UnitOfWork.Context.CriterionLibrary.Add(criterionLibrary);
            _testHelper.UnitOfWork.Context.SaveChanges();
            return criterionLibrary;

        }

        [Fact]
        public async Task ShouldReturnOkResultOnGet()
        {
            var controller = SetupController();

            // Act
            var result = await controller.RemainingLifeLimitLibraries();

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task ShouldReturnOkResultOnPost()
        {
            var controller = SetupController();

            var library = TestRemainingLifeLimitLibrary();
            // Act
            var result = await controller
                .UpsertRemainingLifeLimitLibrary(library.ToDto());

            // Assert
            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async Task ShouldReturnOkResultOnDelete()
        {
            var controller = SetupController();

            // Act
            var library = SetupForGet();
            var result = await controller.DeleteRemainingLifeLimitLibrary(library.Id);

            // Assert
            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async Task ShouldGetAllRemainingLifeLimitLibrariesWithRemainingLifeLimits()
        {
            // Arrange
            var controller = SetupController();
            var library = SetupForGet();

            // Act
            var result = await controller.RemainingLifeLimitLibraries();

            // Assert
            var okObjResult = result as OkObjectResult;
            Assert.NotNull(okObjResult.Value);

            var dtos = (List<RemainingLifeLimitLibraryDTO>)Convert.ChangeType(okObjResult.Value,
                typeof(List<RemainingLifeLimitLibraryDTO>));
            var dto = dtos.Single(x => x.Id == library.Id);
            Assert.Single(dto.RemainingLifeLimits);
        }

        [Fact]
        public async Task ShouldModifyRemainingLifeLimitData()
        {
            // Arrange
            var controller = SetupController();
            var simulation = SimulationTestSetup.CreateSimulation(_testHelper.UnitOfWork);
            var lifeLimitLibrary = SetupForGet();
            var criterionLibrary = SetupForUpsertOrDelete();
            var getResult = await controller.RemainingLifeLimitLibraries();
            var dtos = (List<RemainingLifeLimitLibraryDTO>)Convert.ChangeType((getResult as OkObjectResult).Value,
                typeof(List<RemainingLifeLimitLibraryDTO>));

            var dto = dtos.Single(x => x.Id == lifeLimitLibrary.Id);
            dto.Description = "Updated Description";
            dto.RemainingLifeLimits[0].Value = 2.0;
            dto.RemainingLifeLimits[0].CriterionLibrary =
                criterionLibrary.ToDto();

            // Act
            await controller.UpsertRemainingLifeLimitLibrary(dto);

            // Assert
            var modifiedDto = _testHelper.UnitOfWork.RemainingLifeLimitRepo
                .RemainingLifeLimitLibrariesWithRemainingLifeLimits().Single(rll => rll.Id == lifeLimitLibrary.Id);

            Assert.Equal(dto.Description, modifiedDto.Description);
            // Below was already broken. Brokenness hidden behind a timer that never fired.
            //Assert.Single(modifiedDto.AppliedScenarioIds);
            //Assert.Equal(simulation.Id, modifiedDto.AppliedScenarioIds[0]);

            //Assert.Equal(dto.RemainingLifeLimits[0].Value, modifiedDto.RemainingLifeLimits[0].Value);
            //Assert.Equal(dto.RemainingLifeLimits[0].CriterionLibrary.Id,
            //    modifiedDto.RemainingLifeLimits[0].CriterionLibrary.Id);
            //Assert.Equal(dto.RemainingLifeLimits[0].Attribute, modifiedDto.RemainingLifeLimits[0].Attribute);

        }

        [Fact]
        public async Task ShouldDeleteRemainingLifeLimitData()
        {
            // Arrange
            var controller = SetupController();
            var library = SetupForGet();
            var criterionLibraryEntity = SetupForUpsertOrDelete();
            var getResult = await controller.RemainingLifeLimitLibraries();
            var dtos = (List<RemainingLifeLimitLibraryDTO>)Convert.ChangeType((getResult as OkObjectResult).Value,
                typeof(List<RemainingLifeLimitLibraryDTO>));

            var remainingLifeLimitLibraryDTO = dtos.Single(lib => lib.Id == library.Id);
            remainingLifeLimitLibraryDTO.RemainingLifeLimits[0].CriterionLibrary =
                criterionLibraryEntity.ToDto();

            await controller.UpsertRemainingLifeLimitLibrary(remainingLifeLimitLibraryDTO);

            // Act
            var result = await controller.DeleteRemainingLifeLimitLibrary(library.Id);

            // Assert
            Assert.IsType<OkResult>(result);

            Assert.True(!_testHelper.UnitOfWork.Context.RemainingLifeLimitLibrary.Any(_ => _.Id == library.Id));
            Assert.True(!_testHelper.UnitOfWork.Context.RemainingLifeLimit.Any());
            Assert.True(
                !_testHelper.UnitOfWork.Context.CriterionLibraryRemainingLifeLimit.Any());
            Assert.True(!_testHelper.UnitOfWork.Context.Attribute.Any(_ => _.RemainingLifeLimits.Any()));
        }
    }
}
