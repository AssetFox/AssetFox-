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

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests.API_Test_Classes
{
    public class BudgetPriorityTests
    {
        private readonly TestHelper _testHelper;
        private readonly BudgetPriorityController _controller;

        private static readonly Guid BudgetPriorityLibraryId = Guid.Parse("1bcee741-02a5-4375-ac61-2323d45752b4");
        private static readonly Guid BudgetPriorityId = Guid.Parse("ce1c926b-4df7-4c3c-987f-9146756111b8");
        private static readonly Guid BudgetLibraryId = Guid.Parse("a6c65132-e45c-4a48-a0b2-72cd274c9cc2");
        private static readonly Guid BudgetId = Guid.Parse("874e7a81-999e-4477-9913-68e878835344");
        private static readonly Guid BudgetPercentagePairId = Guid.Parse("ba45571b-1bb5-43c1-b0d3-3705e80b3cf3");

        public BudgetPriorityTests()
        {
            _testHelper = new TestHelper();
            _testHelper.CreateAttributes();
            _testHelper.CreateNetwork();
            _testHelper.CreateSimulation();
            _controller = new BudgetPriorityController(_testHelper.MockEsecSecurityAuthorized.Object, _testHelper.UnitOfWork,
                _testHelper.MockHubService.Object);
        }

        public BudgetPriorityLibraryEntity TestBudgetPriorityLibrary { get; } = new BudgetPriorityLibraryEntity
        {
            Id = BudgetPriorityLibraryId,
            Name = "Test Name"
        };

        public BudgetPriorityEntity TestBudgetPriority { get; } = new BudgetPriorityEntity
        {
            Id = BudgetPriorityId,
            BudgetPriorityLibraryId = BudgetPriorityLibraryId,
            PriorityLevel = 1
        };

        public BudgetLibraryEntity TestBudgetLibrary { get; } = new BudgetLibraryEntity
        {
            Id = BudgetLibraryId,
            Name = "Test Name"
        };

        public BudgetEntity TestBudget { get; } = new BudgetEntity { Id = BudgetId, BudgetLibraryId = BudgetLibraryId, Name = "Test Name", };

        public BudgetPercentagePairEntity TestBudgetPercentagePair { get; } = new BudgetPercentagePairEntity
        {
            Id = BudgetPercentagePairId,
            BudgetPriorityId = BudgetPriorityId,
            BudgetId = BudgetId,
            Percentage = 100
        };

        private void SetupForGet()
        {
            _testHelper.UnitOfWork.Context.BudgetPriorityLibrary.Add(TestBudgetPriorityLibrary);
            _testHelper.UnitOfWork.Context.BudgetPriority.Add(TestBudgetPriority);
            _testHelper.UnitOfWork.Context.SaveChanges();
        }

        private void SetupForUpsertOrDelete()
        {
            SetupForGet();
            _testHelper.UnitOfWork.Context.CriterionLibrary.Add(_testHelper.TestCriterionLibrary);
            _testHelper.UnitOfWork.Context.BudgetLibrary.Add(TestBudgetLibrary);
            _testHelper.UnitOfWork.Context.Budget.Add(TestBudget);
            _testHelper.UnitOfWork.Context.BudgetPercentagePair.Add(TestBudgetPercentagePair);
            _testHelper.UnitOfWork.Context.SaveChanges();
        }

        [Fact]
        public async void ShouldReturnOkResultOnGet()
        {
            try
            {
                // Act
                var result = await _controller.BudgetPriorityLibraries();

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
                    .UpsertBudgetPriorityLibrary(Guid.Empty, TestBudgetPriorityLibrary.ToDto());

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
                var result = await _controller.DeleteBudgetPriorityLibrary(Guid.Empty);

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
        public async void ShouldGetAllBudgetPriorityLibrariesWithBudgetPriorities()
        {
            try
            {
                // Arrange
                SetupForGet();

                // Act
                var result = await _controller.BudgetPriorityLibraries();

                // Assert
                var okObjResult = result as OkObjectResult;
                Assert.NotNull(okObjResult.Value);

                var dtos = (List<BudgetPriorityLibraryDTO>)Convert.ChangeType(okObjResult.Value,
                    typeof(List<BudgetPriorityLibraryDTO>));
                Assert.Single(dtos);

                Assert.Equal(BudgetPriorityLibraryId, dtos[0].Id);
                Assert.Single(dtos[0].BudgetPriorities);

                Assert.Equal(BudgetPriorityId, dtos[0].BudgetPriorities[0].Id);
            }
            finally
            {
                // Cleanup
                _testHelper.CleanUp();
            }
        }

        [Fact]
        public async void ShouldModifyBudgetPriorityData()
        {
            try
            {
                // Arrange
                SetupForUpsertOrDelete();
                var getResult = await _controller.BudgetPriorityLibraries();
                var dtos = (List<BudgetPriorityLibraryDTO>)Convert.ChangeType((getResult as OkObjectResult).Value,
                    typeof(List<BudgetPriorityLibraryDTO>));

                var dto = dtos[0];
                dto.Description = "Updated Description";
                dto.BudgetPriorities[0].PriorityLevel = 2;
                dto.BudgetPriorities[0].CriterionLibrary =
                    _testHelper.TestCriterionLibrary.ToDto();
                dto.BudgetPriorities[0].BudgetPercentagePairs[0].Percentage = 90;

                // Act
                await _controller.UpsertBudgetPriorityLibrary(_testHelper.TestSimulation.Id, dto);

                // Assert
                var timer = new Timer {Interval = 5000};
                timer.Elapsed += delegate
                {
                    var modifiedDto = _testHelper.UnitOfWork.BudgetPriorityRepo
                        .BudgetPriorityLibrariesWithBudgetPriorities()[0];
                    Assert.Equal(dto.Description, modifiedDto.Description);
                    Assert.Single(modifiedDto.AppliedScenarioIds);
                    Assert.Equal(_testHelper.TestSimulation.Id, modifiedDto.AppliedScenarioIds[0]);

                    Assert.Equal(dto.BudgetPriorities[0].PriorityLevel, modifiedDto.BudgetPriorities[0].PriorityLevel);
                    Assert.Equal(dto.BudgetPriorities[0].CriterionLibrary.Id,
                        modifiedDto.BudgetPriorities[0].CriterionLibrary.Id);

                    Assert.Equal(dto.BudgetPriorities[0].BudgetPercentagePairs[0].Percentage,
                        modifiedDto.BudgetPriorities[0].BudgetPercentagePairs[0].Percentage);
                };
                timer.Start();
            }
            finally
            {
                // Cleanup
                _testHelper.CleanUp();
            }
        }

        [Fact]
        public async void ShouldDeleteBudgetPriorityData()
        {
            try
            {
                // Arrange
                SetupForUpsertOrDelete();
                var getResult = await _controller.BudgetPriorityLibraries();
                var dtos = (List<BudgetPriorityLibraryDTO>)Convert.ChangeType((getResult as OkObjectResult).Value,
                    typeof(List<BudgetPriorityLibraryDTO>));

                var budgetPriorityLibraryDTO = dtos[0];
                budgetPriorityLibraryDTO.BudgetPriorities[0].CriterionLibrary =
                    _testHelper.TestCriterionLibrary.ToDto();

                await _controller.UpsertBudgetPriorityLibrary(_testHelper.TestSimulation.Id,
                    budgetPriorityLibraryDTO);

                // Act
                var result = await _controller.DeleteBudgetPriorityLibrary(BudgetPriorityLibraryId);

                // Assert
                Assert.IsType<OkResult>(result);

                Assert.True(!_testHelper.UnitOfWork.Context.BudgetPriorityLibrary.Any(_ => _.Id == BudgetPriorityLibraryId));
                Assert.True(!_testHelper.UnitOfWork.Context.BudgetPriority.Any(_ => _.Id == BudgetPriorityId));
                Assert.True(!_testHelper.UnitOfWork.Context.BudgetPriorityLibrarySimulation.Any(_ =>
                    _.BudgetPriorityLibraryId == BudgetPriorityLibraryId));
                Assert.True(
                    !_testHelper.UnitOfWork.Context.CriterionLibraryBudgetPriority.Any(_ =>
                        _.BudgetPriorityId == BudgetPriorityId));
                Assert.True(!_testHelper.UnitOfWork.Context.BudgetPercentagePair.Any(_ => _.Id == BudgetPercentagePairId));
            }
            finally
            {
                // Cleanup
                _testHelper.CleanUp();
            }
        }
    }
}
