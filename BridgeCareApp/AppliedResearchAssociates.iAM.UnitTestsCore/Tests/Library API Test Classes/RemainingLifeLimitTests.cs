using System;
using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.DTOs;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappings;
using AppliedResearchAssociates.iAM.UnitTestsCore.TestData;
using BridgeCareCore.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests.Library_API_Test_Classes
{
    public class RemainingLifeLimitTests
    {
        private readonly TestHelper _testHelper;
        private readonly RemainingLifeLimitController _controller;

        private static readonly Guid RemainingLifeLimitLibraryId = Guid.Parse("7f243aeb-62b7-4df8-9c15-15d6cea16ec4");
        private static readonly Guid RemainingLifeLimitId = Guid.Parse("ad833cb7-13e9-43fd-bb1f-ee6f1dd39f57");

        public RemainingLifeLimitTests()
        {
            _testHelper = new TestHelper("IAMv2rll");
            _testHelper.CreateAttributes();
            _testHelper.CreateNetwork();
            _testHelper.CreateSimulation();
            _controller = new RemainingLifeLimitController(_testHelper.UnitOfDataPersistenceWork, _testHelper.MockEsecSecurity);
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
            _testHelper.UnitOfDataPersistenceWork.Context.RemainingLifeLimitLibrary.Add(TestRemainingLifeLimitLibrary);
            var attribute = _testHelper.UnitOfDataPersistenceWork.Context.Attribute.First();
            TestRemainingLifeLimit.AttributeId = attribute.Id;
            _testHelper.UnitOfDataPersistenceWork.Context.RemainingLifeLimit.Add(TestRemainingLifeLimit);
            _testHelper.UnitOfDataPersistenceWork.Context.SaveChanges();
        }

        private void SetupForUpsertOrDelete()
        {
            SetupForGet();
            _testHelper.UnitOfDataPersistenceWork.Context.CriterionLibrary.Add(_testHelper.TestCriterionLibrary);
            _testHelper.UnitOfDataPersistenceWork.Context.SaveChanges();
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

                var remainingLifeLimitLibraryDTO = dtos[0];
                remainingLifeLimitLibraryDTO.Description = "Updated Description";
                remainingLifeLimitLibraryDTO.RemainingLifeLimits[0].Value = 2.0;
                remainingLifeLimitLibraryDTO.RemainingLifeLimits[0].CriterionLibrary =
                    _testHelper.TestCriterionLibrary.ToDto();

                // Act
                var result =
                    await _controller.UpsertRemainingLifeLimitLibrary(_testHelper.TestSimulation.Id,
                        remainingLifeLimitLibraryDTO);

                // Assert
                Assert.IsType<OkResult>(result);

                var remainingLifeLimitLibraryEntity = _testHelper.UnitOfDataPersistenceWork.Context.RemainingLifeLimitLibrary
                    .Include(_ => _.RemainingLifeLimits)
                    .ThenInclude(_ => _.CriterionLibraryRemainingLifeLimitJoin)
                    .ThenInclude(_ => _.CriterionLibrary)
                    .Include(_ => _.RemainingLifeLimits)
                    .ThenInclude(_ => _.Attribute)
                    .Include(_ => _.RemainingLifeLimitLibrarySimulationJoins)
                    .Single(_ => _.Id == RemainingLifeLimitLibraryId);

                Assert.Equal(remainingLifeLimitLibraryDTO.Description, remainingLifeLimitLibraryEntity.Description);
                Assert.Single(remainingLifeLimitLibraryEntity.RemainingLifeLimitLibrarySimulationJoins);
                var remainingLifeLimitLibrarySimulationJoin =
                    remainingLifeLimitLibraryEntity.RemainingLifeLimitLibrarySimulationJoins.ToList()[0];
                Assert.Equal(_testHelper.TestSimulation.Id, remainingLifeLimitLibrarySimulationJoin.SimulationId);
                var remainingLifeLimitEntity = remainingLifeLimitLibraryEntity.RemainingLifeLimits.ToList()[0];
                Assert.Equal(remainingLifeLimitLibraryDTO.RemainingLifeLimits[0].Value, remainingLifeLimitEntity.Value);
                Assert.NotNull(remainingLifeLimitEntity.CriterionLibraryRemainingLifeLimitJoin);
                Assert.Equal(remainingLifeLimitLibraryDTO.RemainingLifeLimits[0].CriterionLibrary.Id,
                    remainingLifeLimitEntity.CriterionLibraryRemainingLifeLimitJoin.CriterionLibrary.Id);
                Assert.NotNull(remainingLifeLimitEntity.Attribute);
                Assert.Equal(remainingLifeLimitEntity.Attribute.Name, remainingLifeLimitLibraryDTO.RemainingLifeLimits[0].Attribute);
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

                Assert.True(!_testHelper.UnitOfDataPersistenceWork.Context.RemainingLifeLimitLibrary.Any(_ => _.Id == RemainingLifeLimitLibraryId));
                Assert.True(!_testHelper.UnitOfDataPersistenceWork.Context.RemainingLifeLimit.Any(_ => _.Id == RemainingLifeLimitId));
                Assert.True(!_testHelper.UnitOfDataPersistenceWork.Context.RemainingLifeLimitLibrarySimulation.Any(_ =>
                    _.RemainingLifeLimitLibraryId == RemainingLifeLimitLibraryId));
                Assert.True(
                    !_testHelper.UnitOfDataPersistenceWork.Context.CriterionLibraryRemainingLifeLimit.Any(_ =>
                        _.RemainingLifeLimitId == RemainingLifeLimitId));
                Assert.True(!_testHelper.UnitOfDataPersistenceWork.Context.Attribute.Any(_ => _.RemainingLifeLimits.Any()));
            }
            finally
            {
                // Cleanup
                _testHelper.CleanUp();
            }
        }
    }
}
