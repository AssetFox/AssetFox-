using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.UnitTestsCore.TestData;
using BridgeCareCore.Controllers;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests.APITestClasses
{
    public class RemainingLifeLimitTests
    {
        private readonly TestHelper _testHelper;
        private readonly RemainingLifeLimitController _controller;

        private static readonly Guid RemainingLifeLimitLibraryId = Guid.Parse("7f243aeb-62b7-4df8-9c15-15d6cea16ec4");
        private static readonly Guid RemainingLifeLimitId = Guid.Parse("ad833cb7-13e9-43fd-bb1f-ee6f1dd39f57");

        public RemainingLifeLimitTests()
        {
            _testHelper = new TestHelper();
            _testHelper.CreateAttributes();
            _testHelper.CreateNetwork();
            _testHelper.CreateSimulation();
            _testHelper.SetupDefaultHttpContext();
            _controller = new RemainingLifeLimitController(_testHelper.MockEsecSecurityAuthorized.Object, _testHelper.UnitOfWork,
                _testHelper.MockHubService.Object, _testHelper.MockHttpContextAccessor.Object);
        }

        public RemainingLifeLimitLibraryEntity TestRemainingLifeLimitLibrary { get; } = new RemainingLifeLimitLibraryEntity
        {
            Id = RemainingLifeLimitLibraryId,
            Name = "Test Name"
        };

        public RemainingLifeLimitEntity TestRemainingLifeLimit { get; } = new RemainingLifeLimitEntity
        {
            Id = RemainingLifeLimitId,
            RemainingLifeLimitLibraryId = RemainingLifeLimitLibraryId,
            Value = 1.0
        };

        private void SetupForGet()
        {
            _testHelper.UnitOfWork.Context.RemainingLifeLimitLibrary.Add(TestRemainingLifeLimitLibrary);
            var attribute = _testHelper.UnitOfWork.Context.Attribute.First();
            TestRemainingLifeLimit.AttributeId = attribute.Id;
            _testHelper.UnitOfWork.Context.RemainingLifeLimit.Add(TestRemainingLifeLimit);
            _testHelper.UnitOfWork.Context.SaveChanges();
        }

        private void SetupForUpsertOrDelete()
        {
            SetupForGet();
            _testHelper.UnitOfWork.Context.CriterionLibrary.Add(_testHelper.TestCriterionLibrary);
            _testHelper.UnitOfWork.Context.SaveChanges();
        }

        [Fact]
        public async void ShouldReturnOkResultOnGet()
        {
            try
            {
                // Act
                var result = await _controller.RemainingLifeLimitLibraries();

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
                    .UpsertRemainingLifeLimitLibrary(Guid.Empty, TestRemainingLifeLimitLibrary.ToDto());

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
        public async void ShouldReturnOkResultOnDelete()
        {
            try
            {
                // Act
                var result = await _controller.DeleteRemainingLifeLimitLibrary(Guid.Empty);

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
        public async void ShouldGetAllRemainingLifeLimitLibrariesWithRemainingLifeLimits()
        {
            try
            {
                // Arrange
                SetupForGet();

                // Act
                var result = await _controller.RemainingLifeLimitLibraries();

                // Assert
                var okObjResult = result as OkObjectResult;
                Assert.NotNull(okObjResult.Value);

                var dtos = (List<RemainingLifeLimitLibraryDTO>)Convert.ChangeType(okObjResult.Value,
                    typeof(List<RemainingLifeLimitLibraryDTO>));
                Assert.Single(dtos);

                Assert.Equal(RemainingLifeLimitLibraryId, dtos[0].Id);
                Assert.Single(dtos[0].RemainingLifeLimits);

                Assert.Equal(RemainingLifeLimitId, dtos[0].RemainingLifeLimits[0].Id);
            }
            finally
            {
                // Cleanup
                _testHelper.CleanUp();
            }
        }

        [Fact]
        public async void ShouldModifyRemainingLifeLimitData()
        {
            try
            {
                // Arrange
                SetupForUpsertOrDelete();
                var getResult = await _controller.RemainingLifeLimitLibraries();
                var dtos = (List<RemainingLifeLimitLibraryDTO>)Convert.ChangeType((getResult as OkObjectResult).Value,
                    typeof(List<RemainingLifeLimitLibraryDTO>));

                var dto = dtos[0];
                dto.Description = "Updated Description";
                dto.RemainingLifeLimits[0].Value = 2.0;
                dto.RemainingLifeLimits[0].CriterionLibrary =
                    _testHelper.TestCriterionLibrary.ToDto();

                // Act
                await _controller.UpsertRemainingLifeLimitLibrary(_testHelper.TestSimulation.Id,
                        dto);

                // Assert
                var timer = new Timer {Interval = 5000};
                timer.Elapsed += delegate
                {
                    var modifiedDto = _testHelper.UnitOfWork.RemainingLifeLimitRepo
                        .RemainingLifeLimitLibrariesWithRemainingLifeLimits()[0];
                    Assert.Equal(dto.Description, modifiedDto.Description);
                    Assert.Single(modifiedDto.AppliedScenarioIds);
                    Assert.Equal(_testHelper.TestSimulation.Id, modifiedDto.AppliedScenarioIds[0]);

                    Assert.Equal(dto.RemainingLifeLimits[0].Value, modifiedDto.RemainingLifeLimits[0].Value);
                    Assert.Equal(dto.RemainingLifeLimits[0].CriterionLibrary.Id,
                        modifiedDto.RemainingLifeLimits[0].CriterionLibrary.Id);
                    Assert.Equal(dto.RemainingLifeLimits[0].Attribute, modifiedDto.RemainingLifeLimits[0].Attribute);
                };
            }
            finally
            {
                // Cleanup
                _testHelper.CleanUp();
            }
        }

        [Fact]
        public async void ShouldDeleteRemainingLifeLimitData()
        {
            try
            {
                // Arrange
                SetupForUpsertOrDelete();
                var getResult = await _controller.RemainingLifeLimitLibraries();
                var dtos = (List<RemainingLifeLimitLibraryDTO>)Convert.ChangeType((getResult as OkObjectResult).Value,
                    typeof(List<RemainingLifeLimitLibraryDTO>));

                var remainingLifeLimitLibraryDTO = dtos[0];
                remainingLifeLimitLibraryDTO.RemainingLifeLimits[0].CriterionLibrary =
                    _testHelper.TestCriterionLibrary.ToDto();

                await _controller.UpsertRemainingLifeLimitLibrary(_testHelper.TestSimulation.Id,
                    remainingLifeLimitLibraryDTO);

                // Act
                var result = await _controller.DeleteRemainingLifeLimitLibrary(RemainingLifeLimitLibraryId);

                // Assert
                Assert.IsType<OkResult>(result);

                Assert.True(!_testHelper.UnitOfWork.Context.RemainingLifeLimitLibrary.Any(_ => _.Id == RemainingLifeLimitLibraryId));
                Assert.True(!_testHelper.UnitOfWork.Context.RemainingLifeLimit.Any(_ => _.Id == RemainingLifeLimitId));
                Assert.True(!_testHelper.UnitOfWork.Context.RemainingLifeLimitLibrarySimulation.Any(_ =>
                    _.RemainingLifeLimitLibraryId == RemainingLifeLimitLibraryId));
                Assert.True(
                    !_testHelper.UnitOfWork.Context.CriterionLibraryRemainingLifeLimit.Any(_ =>
                        _.RemainingLifeLimitId == RemainingLifeLimitId));
                Assert.True(!_testHelper.UnitOfWork.Context.Attribute.Any(_ => _.RemainingLifeLimits.Any()));
            }
            finally
            {
                // Cleanup
                _testHelper.CleanUp();
            }
        }
    }
}
