using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.UnitTestsCore.TestUtils;
using BridgeCareCore.Controllers;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests.APITestClasses
{
    public class CriterionTests
    {
        private readonly TestHelper _testHelper;
        private readonly CriterionLibraryController _controller;

        public CriterionTests()
        {
            _testHelper = TestHelper.Instance;
            if (!_testHelper.DbContext.Attribute.Any())
            {
                _testHelper.CreateAttributes();
                _testHelper.CreateNetwork();
                _testHelper.CreateSimulation();
                _testHelper.SetupDefaultHttpContext();
            }
            _controller = new CriterionLibraryController(_testHelper.MockEsecSecurityAuthorized.Object, _testHelper.UnitOfWork,
                _testHelper.MockHubService.Object, _testHelper.MockHttpContextAccessor.Object);
        }

        private CriterionLibraryEntity Setup()
        {
            var criterionLibrary = _testHelper.TestCriterionLibrary();
            _testHelper.UnitOfWork.Context.CriterionLibrary.Add(criterionLibrary);
            _testHelper.UnitOfWork.Context.SaveChanges();
            return criterionLibrary;
        }

        [Fact]
        public async Task ShouldReturnOkResultOnGet()
        {
            // Act
            var result = await _controller.CriterionLibraries();

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task ShouldReturnOkResultOnPost()
        {
            // Act
            var result = await _controller
                .UpsertCriterionLibrary(_testHelper.TestCriterionLibrary().ToDto());

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task ShouldReturnOkResultOnDelete()
        {
            // Act
            var result = await _controller.DeleteCriterionLibrary(Guid.Empty);

            // Assert
            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async Task ShouldGetAllCriterionLibraries()
        {
            // Arrange
            var criterionLibrary = Setup();

            // Act
            var result = await _controller.CriterionLibraries();

            // Assert
            var okObjResult = result as OkObjectResult;
            Assert.NotNull(okObjResult.Value);

            var dtos = (List<CriterionLibraryDTO>)Convert.ChangeType(okObjResult.Value,
                typeof(List<CriterionLibraryDTO>));
            Assert.True(dtos.Any(cl => cl.Id == criterionLibrary.Id));
        }

        [Fact]
        public async Task ShouldModifyCriterionLibraries()
        {
            // Arrange
            var criterionLibrary = Setup();
            var getResult = await _controller.CriterionLibraries();
            var dtos = (List<CriterionLibraryDTO>)Convert.ChangeType((getResult as OkObjectResult).Value,
                typeof(List<CriterionLibraryDTO>));

            var criterionLibraryDTO = dtos.Single(dto => dto.Id == criterionLibrary.Id);
            criterionLibraryDTO.Description = "Updated Description";

            var newName = RandomStrings.WithPrefix("New Name");
            var newDescription = RandomStrings.WithPrefix("Updated description");
            var newCriterionLibraryDTO = new CriterionLibraryEntity
            {
                Id = criterionLibrary.Id,
                Name = newName,
                MergedCriteriaExpression = "New Expression",
                Description = newDescription,
            }.ToDto();

            // Act
            var updateResult = await _controller.UpsertCriterionLibrary(criterionLibraryDTO);
            var addResult = await _controller.UpsertCriterionLibrary(newCriterionLibraryDTO);

            // Assert
            Assert.IsType<OkObjectResult>(updateResult);
            Assert.IsType<OkObjectResult>(addResult);

            var updatedCriterionLibraryEntity = _testHelper.UnitOfWork.Context.CriterionLibrary
                .Single(_ => _.Id == criterionLibrary.Id);
            Assert.Equal(newDescription, updatedCriterionLibraryEntity.Description);

            var newCriterionLibraryEntity =
                _testHelper.UnitOfWork.Context.CriterionLibrary.Single(_ =>
                    _.Id == newCriterionLibraryDTO.Id);
            Assert.NotNull(newCriterionLibraryEntity);
        }

        [Fact]
        public async Task ShouldDeleteCriterionLibrary()
        {
            // Arrange
            var criterionLibrary = Setup();

            // Act
            var result = await _controller.DeleteCriterionLibrary(criterionLibrary.Id);

            // Assert
            Assert.IsType<OkResult>(result);

            Assert.True(
                !_testHelper.UnitOfWork.Context.CriterionLibrary.Any(_ =>
                    _.Id == criterionLibrary.Id));
        }
    }
}
