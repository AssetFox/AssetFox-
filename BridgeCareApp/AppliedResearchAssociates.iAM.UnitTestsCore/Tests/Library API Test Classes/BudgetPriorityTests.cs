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
            _testHelper = new TestHelper("IAMv2bp");
            _testHelper.CreateAttributes();
            _testHelper.CreateNetwork();
            _testHelper.CreateSimulation();
            _controller = new BudgetPriorityController(_testHelper.UnitOfDataPersistenceWork);
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
            _testHelper.UnitOfDataPersistenceWork.Context.BudgetPriorityLibrary.Add(TestBudgetPriorityLibrary);
            _testHelper.UnitOfDataPersistenceWork.Context.BudgetPriority.Add(TestBudgetPriority);
            _testHelper.UnitOfDataPersistenceWork.Context.SaveChanges();
        }

        private void SetupForAddOrUpdateOrDelete()
        {
            SetupForGet();
            _testHelper.UnitOfDataPersistenceWork.Context.CriterionLibrary.Add(_testHelper.TestCriterionLibrary);
            _testHelper.UnitOfDataPersistenceWork.Context.BudgetLibrary.Add(TestBudgetLibrary);
            _testHelper.UnitOfDataPersistenceWork.Context.Budget.Add(TestBudget);
            _testHelper.UnitOfDataPersistenceWork.Context.BudgetPercentagePair.Add(TestBudgetPercentagePair);
            _testHelper.UnitOfDataPersistenceWork.Context.SaveChanges();
        }

        [Fact]
        public void ShouldReturnOkResultOnGet()
        {
            try
            {
                // Act
                var result = _controller.BudgetPriorityLibraries();

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
                    .AddOrUpdateBudgetPriorityLibrary(Guid.Empty, TestBudgetPriorityLibrary.ToDto());

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
                var result = _controller.DeleteBudgetPriorityLibrary(Guid.Empty);

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
        public void ShouldGetAllBudgetPriorityLibrariesWithBudgetPriorities()
        {
            try
            {
                // Arrange
                SetupForGet();

                // Act
                var result = _controller.BudgetPriorityLibraries();

                // Assert
                var okObjResult = result.Result as OkObjectResult;
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
        public void ShouldModifyBudgetPriorityData()
        {
            try
            {
                // Arrange
                SetupForAddOrUpdateOrDelete();
                var dtos = (List<BudgetPriorityLibraryDTO>)Convert.ChangeType(
                    (_controller.BudgetPriorityLibraries().Result as OkObjectResult).Value,
                    typeof(List<BudgetPriorityLibraryDTO>));

                var budgetPriorityLibraryDTO = dtos[0];
                budgetPriorityLibraryDTO.Description = "Updated Description";
                budgetPriorityLibraryDTO.BudgetPriorities[0].PriorityLevel = 2;
                budgetPriorityLibraryDTO.BudgetPriorities[0].CriterionLibrary =
                    _testHelper.TestCriterionLibrary.ToDto();
                budgetPriorityLibraryDTO.BudgetPriorities[0].BudgetPercentagePairs[0].Percentage = 90;

                // Act
                var result =
                    _controller.AddOrUpdateBudgetPriorityLibrary(_testHelper.TestSimulation.Id,
                        budgetPriorityLibraryDTO);

                // Assert
                Assert.IsType<OkResult>(result.Result);

                var budgetPriorityLibraryEntity = _testHelper.UnitOfDataPersistenceWork.Context.BudgetPriorityLibrary
                    .Include(_ => _.BudgetPriorities)
                    .ThenInclude(_ => _.CriterionLibraryBudgetPriorityJoin)
                    .ThenInclude(_ => _.CriterionLibrary)
                    .Include(_ => _.BudgetPriorities)
                    .ThenInclude(_ => _.BudgetPercentagePairs)
                    .ThenInclude(_ => _.Budget)
                    .Include(_ => _.BudgetPriorityLibrarySimulationJoins)
                    .Single(_ => _.Id == BudgetPriorityLibraryId);

                Assert.Equal(budgetPriorityLibraryDTO.Description, budgetPriorityLibraryEntity.Description);
                Assert.Single(budgetPriorityLibraryEntity.BudgetPriorityLibrarySimulationJoins);
                var budgetPriorityLibrarySimulationJoin =
                    budgetPriorityLibraryEntity.BudgetPriorityLibrarySimulationJoins.ToList()[0];
                Assert.Equal(_testHelper.TestSimulation.Id, budgetPriorityLibrarySimulationJoin.SimulationId);
                var budgetPriorityEntity = budgetPriorityLibraryEntity.BudgetPriorities.ToList()[0];
                Assert.Equal(budgetPriorityLibraryDTO.BudgetPriorities[0].PriorityLevel, budgetPriorityEntity.PriorityLevel);
                Assert.NotNull(budgetPriorityEntity.CriterionLibraryBudgetPriorityJoin);
                Assert.Equal(budgetPriorityLibraryDTO.BudgetPriorities[0].CriterionLibrary.Id,
                    budgetPriorityEntity.CriterionLibraryBudgetPriorityJoin.CriterionLibrary.Id);
                var budgetPercentagePairEntity = budgetPriorityEntity.BudgetPercentagePairs.ToList()[0];
                Assert.Equal(budgetPriorityLibraryDTO.BudgetPriorities[0].BudgetPercentagePairs[0].Percentage,
                    budgetPercentagePairEntity.Percentage);
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
                SetupForAddOrUpdateOrDelete();
                var dtos = (List<BudgetPriorityLibraryDTO>)Convert.ChangeType(
                    (_controller.BudgetPriorityLibraries().Result as OkObjectResult).Value,
                    typeof(List<BudgetPriorityLibraryDTO>));

                var budgetPriorityLibraryDTO = dtos[0];
                budgetPriorityLibraryDTO.BudgetPriorities[0].CriterionLibrary =
                    _testHelper.TestCriterionLibrary.ToDto();

                await _controller.AddOrUpdateBudgetPriorityLibrary(_testHelper.TestSimulation.Id,
                    budgetPriorityLibraryDTO);

                // Act
                var result = _controller.DeleteBudgetPriorityLibrary(BudgetPriorityLibraryId);

                // Assert
                Assert.IsType<OkResult>(result.Result);

                Assert.True(!_testHelper.UnitOfDataPersistenceWork.Context.BudgetPriorityLibrary.Any(_ => _.Id == BudgetPriorityLibraryId));
                Assert.True(!_testHelper.UnitOfDataPersistenceWork.Context.BudgetPriority.Any(_ => _.Id == BudgetPriorityId));
                Assert.True(!_testHelper.UnitOfDataPersistenceWork.Context.BudgetPriorityLibrarySimulation.Any(_ =>
                    _.BudgetPriorityLibraryId == BudgetPriorityLibraryId));
                Assert.True(
                    !_testHelper.UnitOfDataPersistenceWork.Context.CriterionLibraryBudgetPriority.Any(_ =>
                        _.BudgetPriorityId == BudgetPriorityId));
                Assert.True(!_testHelper.UnitOfDataPersistenceWork.Context.BudgetPercentagePair.Any(_ => _.Id == BudgetPercentagePairId));
            }
            finally
            {
                // Cleanup
                _testHelper.CleanUp();
            }
        }
    }
}
