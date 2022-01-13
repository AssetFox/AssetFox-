using System;
using System.Collections.Generic;
using System.Linq;
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
            _testHelper = new TestHelper();
            _testHelper.CreateAttributes();
            _testHelper.CreateNetwork();
            _testHelper.CreateSimulation();
            _testHelper.SetupDefaultHttpContext();
            _controller = new CriterionLibraryController(_testHelper.MockEsecSecurityAuthorized.Object, _testHelper.UnitOfWork,
                _testHelper.MockHubService.Object, _testHelper.MockHttpContextAccessor.Object);
        }

        private void Setup()
        {
            _testHelper.UnitOfWork.Context.CriterionLibrary.Add(_testHelper.TestCriterionLibrary);
            _testHelper.UnitOfWork.Context.SaveChanges();
        }

        [Fact]
        public async void ShouldReturnOkResultOnGet()
        {
            try
            {
                // Act
                var result = await _controller.CriterionLibraries();

                // Assert
                Assert.IsType<OkObjectResult>(result);
            }
            finally
            {
                // Cleanup
                _testHelper.CleanUp();
            }
        }

        [Fact]
        public async void ShouldReturnOkResultOnPost()
        {
            try
            {
                // Act
                var result = await _controller
                    .UpsertCriterionLibrary(_testHelper.TestCriterionLibrary.ToDto());

                // Assert
                Assert.IsType<OkObjectResult>(result);
            }
            finally
            {
                // Cleanup
                _testHelper.CleanUp();
            }
        }

        [Fact]
        public async void ShouldReturnOkResultOnDelete()
        {
            try
            {
                // Act
                var result = await _controller.DeleteCriterionLibrary(Guid.Empty);

                // Assert
                Assert.IsType<OkResult>(result);
            }
            finally
            {
                // Cleanup
                _testHelper.CleanUp();
            }
        }

        [Fact]
        public async void ShouldGetAllCriterionLibraries()
        {
            try
            {
                // Arrange
                Setup();

                // Act
                var result = await _controller.CriterionLibraries();

                // Assert
                var okObjResult = result as OkObjectResult;
                Assert.NotNull(okObjResult.Value);

                var dtos = (List<CriterionLibraryDTO>)Convert.ChangeType(okObjResult.Value,
                    typeof(List<CriterionLibraryDTO>));
                Assert.Single(dtos);

                Assert.Equal(_testHelper.TestCriterionLibrary.Id, dtos[0].Id);
            }
            finally
            {
                // Cleanup
                _testHelper.CleanUp();
            }
        }

        [Fact]
        public async void ShouldModifyCriterionLibraries()
        {
            try
            {
                // Arrange
                Setup();
                var getResult = await _controller.CriterionLibraries();
                var dtos = (List<CriterionLibraryDTO>)Convert.ChangeType((getResult as OkObjectResult).Value,
                    typeof(List<CriterionLibraryDTO>));

                var criterionLibraryDTO = dtos[0];
                criterionLibraryDTO.Description = "Updated Description";

                var newCriterionLibraryDTO = new CriterionLibraryEntity
                {
                    Id = Guid.NewGuid(),
                    Name = "New Name",
                    MergedCriteriaExpression = "New Expression"
                }.ToDto();

                // Act
                var updateResult = await _controller.UpsertCriterionLibrary(criterionLibraryDTO);
                var addResult = await _controller.UpsertCriterionLibrary(newCriterionLibraryDTO);

                // Assert
                Assert.IsType<OkObjectResult>(updateResult);
                Assert.IsType<OkObjectResult>(addResult);

                var updatedCriterionLibraryEntity = _testHelper.UnitOfWork.Context.CriterionLibrary
                    .Single(_ => _.Id == _testHelper.TestCriterionLibrary.Id);
                Assert.Equal(criterionLibraryDTO.Description, updatedCriterionLibraryEntity.Description);

                var newCriterionLibraryEntity =
                    _testHelper.UnitOfWork.Context.CriterionLibrary.Single(_ =>
                        _.Id == newCriterionLibraryDTO.Id);
                Assert.NotNull(newCriterionLibraryEntity);
            }
            finally
            {
                // Cleanup
                _testHelper.CleanUp();
            }
        }

        [Fact]
        public async void ShouldDeleteCriterionLibrary()
        {
            try
            {
                // Arrange
                Setup();

                // Act
                var result = await _controller.DeleteCriterionLibrary(_testHelper.TestCriterionLibrary.Id);

                // Assert
                Assert.IsType<OkResult>(result);

                Assert.True(
                    !_testHelper.UnitOfWork.Context.CriterionLibrary.Any(_ =>
                        _.Id == _testHelper.TestCriterionLibrary.Id));
            }
            finally
            {
                // Cleanup
                _testHelper.CleanUp();
            }
        }
    }
}
