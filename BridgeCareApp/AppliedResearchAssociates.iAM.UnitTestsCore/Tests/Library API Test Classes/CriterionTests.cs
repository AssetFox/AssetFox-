using System;
using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.DTOs;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappings;
using AppliedResearchAssociates.iAM.UnitTestsCore.TestData;
using BridgeCareCore.Controllers;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests.Library_API_Test_Classes
{
    public class CriterionTests
    {
        private readonly TestHelper _testHelper;
        private readonly CriterionLibraryController _controller;

        public CriterionTests()
        {
            _testHelper = new TestHelper("IAMvv2c");
            _testHelper.CreateAttributes();
            _testHelper.CreateNetwork();
            _testHelper.CreateSimulation();
            _controller = new CriterionLibraryController(_testHelper.UnitOfDataPersistenceWork);
        }

        private void Setup()
        {
            _testHelper.UnitOfDataPersistenceWork.Context.CriterionLibrary.Add(_testHelper.TestCriterionLibrary);
            _testHelper.UnitOfDataPersistenceWork.Context.SaveChanges();
        }

        [Fact]
        public void ShouldReturnOkResultOnGet()
        {
            try
            {
                // Act
                var result = _controller.CriterionLibraries();

                // Assert
                Assert.IsType<OkObjectResult>(result.Result);
            }
            finally
            {
                // Cleanup
                _testHelper.CleanUp();
            }
        }

        [Fact]
        public void ShouldReturnOkResultOnPost()
        {
            try
            {
                // Act
                var result = _controller
                    .AddOrUpdateCriterionLibrary(_testHelper.TestCriterionLibrary.ToDto());

                // Assert
                Assert.IsType<OkResult>(result.Result);
            }
            finally
            {
                // Cleanup
                _testHelper.CleanUp();
            }
        }

        [Fact]
        public void ShouldReturnOkResultOnDelete()
        {
            try
            {
                // Act
                var result = _controller.DeleteCriterionLibrary(Guid.Empty);

                // Assert
                Assert.IsType<OkResult>(result.Result);
            }
            finally
            {
                // Cleanup
                _testHelper.CleanUp();
            }
        }

        [Fact]
        public void ShouldGetAllCriterionLibraries()
        {
            try
            {
                // Arrange
                Setup();

                // Act
                var result = _controller.CriterionLibraries();

                // Assert
                var okObjResult = result.Result as OkObjectResult;
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
        public void ShouldModifyCriterionLibraries()
        {
            try
            {
                // Arrange
                Setup();
                var dtos = (List<CriterionLibraryDTO>)Convert.ChangeType(
                    (_controller.CriterionLibraries().Result as OkObjectResult).Value,
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
                var updateResult = _controller.AddOrUpdateCriterionLibrary(criterionLibraryDTO);
                var addResult = _controller.AddOrUpdateCriterionLibrary(newCriterionLibraryDTO);

                // Assert
                Assert.IsType<OkResult>(updateResult.Result);
                Assert.IsType<OkResult>(addResult.Result);

                var updatedCriterionLibraryEntity = _testHelper.UnitOfDataPersistenceWork.Context.CriterionLibrary
                    .Single(_ => _.Id == _testHelper.TestCriterionLibrary.Id);
                Assert.Equal(criterionLibraryDTO.Description, updatedCriterionLibraryEntity.Description);

                var newCriterionLibraryEntity =
                    _testHelper.UnitOfDataPersistenceWork.Context.CriterionLibrary.Single(_ =>
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
                var result = _controller.DeleteCriterionLibrary(_testHelper.TestCriterionLibrary.Id);

                // Assert
                Assert.IsType<OkResult>(result.Result);

                Assert.True(
                    !_testHelper.UnitOfDataPersistenceWork.Context.CriterionLibrary.Any(_ =>
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
