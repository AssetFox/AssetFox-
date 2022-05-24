using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
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
        private readonly TestHelper _testHelper;
        private readonly RemainingLifeLimitController _controller;

        private static readonly Guid RemainingLifeLimitLibraryId = Guid.Parse("7f243aeb-62b7-4df8-9c15-15d6cea16ec4");

        public RemainingLifeLimitTests()
        {
            _testHelper = TestHelper.Instance;
            if (!_testHelper.DbContext.Attribute.Any())
            {
                _testHelper.CreateAttributes();
                _testHelper.CreateNetwork();
                _testHelper.CreateSimulation();
                _testHelper.SetupDefaultHttpContext();
            }
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
            Id = Guid.NewGuid(),
            RemainingLifeLimitLibraryId = RemainingLifeLimitLibraryId,
            Value = 1.0
        };

        private void SetupForGet()
        {
            if (!_testHelper.UnitOfWork.Context.RemainingLifeLimitLibrary.Any())
            {
                _testHelper.UnitOfWork.Context.RemainingLifeLimitLibrary.Add(TestRemainingLifeLimitLibrary);
                _testHelper.UnitOfWork.Context.SaveChanges();
            }
            if (!_testHelper.UnitOfWork.Context.RemainingLifeLimit.Any())
            {
                var attribute = _testHelper.UnitOfWork.Context.Attribute.First();
                TestRemainingLifeLimit.AttributeId = attribute.Id;
                _testHelper.UnitOfWork.Context.RemainingLifeLimit.Add(TestRemainingLifeLimit);
                _testHelper.UnitOfWork.Context.SaveChanges();
            }
        }

        private CriterionLibraryEntity SetupForUpsertOrDelete()
        {
            SetupForGet();
            var criterionLibrary = _testHelper.TestCriterionLibrary();
            _testHelper.UnitOfWork.Context.CriterionLibrary.Add(criterionLibrary);
            _testHelper.UnitOfWork.Context.SaveChanges();
            return criterionLibrary;

        }

        [Fact]
        public async Task ShouldReturnOkResultOnGet()
        {
            // Act
            var result = await _controller.RemainingLifeLimitLibraries();

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task ShouldReturnOkResultOnPost()
        {
            // Act
            var result = await _controller
                .UpsertRemainingLifeLimitLibrary(TestRemainingLifeLimitLibrary.ToDto());

            // Assert
            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async Task ShouldReturnOkResultOnDelete()
        {
            // Act
            var result = await _controller.DeleteRemainingLifeLimitLibrary(Guid.Empty);

            // Assert
            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async Task ShouldGetAllRemainingLifeLimitLibrariesWithRemainingLifeLimits()
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
        }

        [Fact]
        public async Task ShouldModifyRemainingLifeLimitData()
        {
            // Arrange
            var criterionLibrary = SetupForUpsertOrDelete();
            var getResult = await _controller.RemainingLifeLimitLibraries();
            var dtos = (List<RemainingLifeLimitLibraryDTO>)Convert.ChangeType((getResult as OkObjectResult).Value,
                typeof(List<RemainingLifeLimitLibraryDTO>));

            var dto = dtos[0];
            dto.Description = "Updated Description";
            dto.RemainingLifeLimits[0].Value = 2.0;
            dto.RemainingLifeLimits[0].CriterionLibrary =
                criterionLibrary.ToDto();

            // Act
            await _controller.UpsertRemainingLifeLimitLibrary(dto);

            // Assert
            var timer = new Timer { Interval = 5000 };
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

        [Fact]
        public async Task ShouldDeleteRemainingLifeLimitData()
        {
            // Arrange
            var criterionLibraryEntity = SetupForUpsertOrDelete();
            var getResult = await _controller.RemainingLifeLimitLibraries();
            var dtos = (List<RemainingLifeLimitLibraryDTO>)Convert.ChangeType((getResult as OkObjectResult).Value,
                typeof(List<RemainingLifeLimitLibraryDTO>));

            var remainingLifeLimitLibraryDTO = dtos[0];
            remainingLifeLimitLibraryDTO.RemainingLifeLimits[0].CriterionLibrary =
                criterionLibraryEntity.ToDto();

            await _controller.UpsertRemainingLifeLimitLibrary(remainingLifeLimitLibraryDTO);

            // Act
            var result = await _controller.DeleteRemainingLifeLimitLibrary(RemainingLifeLimitLibraryId);

            // Assert
            Assert.IsType<OkResult>(result);

            Assert.True(!_testHelper.UnitOfWork.Context.RemainingLifeLimitLibrary.Any(_ => _.Id == RemainingLifeLimitLibraryId));
            Assert.True(!_testHelper.UnitOfWork.Context.RemainingLifeLimit.Any());
            Assert.True(
                !_testHelper.UnitOfWork.Context.CriterionLibraryRemainingLifeLimit.Any());
            Assert.True(!_testHelper.UnitOfWork.Context.Attribute.Any(_ => _.RemainingLifeLimits.Any()));
        }
    }
}
