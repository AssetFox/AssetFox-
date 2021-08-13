using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.LibraryEntities.Budget;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.LibraryEntities.BudgetPriority;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.ScenarioEntities.Budget;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Extensions;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.UnitTestsCore.TestUtils;
using BridgeCareCore.Controllers;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests.APITestClasses
{
    public class BudgetPriorityTests
    {
        private readonly TestHelper _testHelper;
        private readonly BudgetPriorityController _controller;

        private ScenarioBudgetEntity _testScenarioBudget;
        private BudgetPriorityLibraryEntity _testBudgetPriorityLibrary;
        private BudgetPriorityEntity _testBudgetPriority;
        private BudgetPercentagePairEntity _testBudgetPercentagePair;

        public BudgetPriorityTests()
        {
            _testHelper = new TestHelper();
            _testHelper.CreateAttributes();
            _testHelper.CreateNetwork();
            _testHelper.CreateSimulation();
            _testHelper.SetupDefaultHttpContext();
            _controller = new BudgetPriorityController(_testHelper.MockEsecSecurityAuthorized.Object,
                _testHelper.UnitOfWork,
                _testHelper.MockHubService.Object, _testHelper.MockHttpContextAccessor.Object);

            CreateBudgetPriorityTestData();
        }

        private void CreateBudgetPriorityTestData()
        {
            _testScenarioBudget = new ScenarioBudgetEntity
            {
                Id = Guid.NewGuid(),
                SimulationId = _testHelper.TestSimulation.Id,
                Name = "ScenarioBudgetEntity",
                ScenarioBudgetAmounts =
                    new List<ScenarioBudgetAmountEntity>
                    {
                        new ScenarioBudgetAmountEntity
                        {
                            Id = Guid.NewGuid(),
                            ScenarioBudget = _testScenarioBudget,
                            Year = DateTime.Now.Year,
                            Value = 500000
                        }
                    },
                CriterionLibraryScenarioBudgetJoin = new CriterionLibraryScenarioBudgetEntity
                {
                    ScenarioBudget = _testScenarioBudget,
                    CriterionLibrary = new CriterionLibraryEntity
                    {
                        Id = Guid.NewGuid(),
                        Name = "Scenario Budget Criterion",
                        IsSingleUse = true,
                        MergedCriteriaExpression = ""
                    }
                }
            };
            _testHelper.UnitOfWork.Context.AddEntity(_testScenarioBudget);


            _testBudgetPriorityLibrary = new BudgetPriorityLibraryEntity {Id = Guid.NewGuid(), Name = "BudgetPriorityLibraryEntity"};
            _testHelper.UnitOfWork.Context.AddEntity(_testBudgetPriorityLibrary);


            _testBudgetPriority = new BudgetPriorityEntity
            {
                Id = Guid.NewGuid(),
                BudgetPriorityLibraryId = _testBudgetPriorityLibrary.Id,
                PriorityLevel = 1,
                CriterionLibraryBudgetPriorityJoin = new CriterionLibraryBudgetPriorityEntity
                {
                    BudgetPriority = _testBudgetPriority,
                    CriterionLibrary = new CriterionLibraryEntity
                    {
                        Id = Guid.NewGuid(),
                        Name = "Budget Priority Criterion",
                        IsSingleUse = true,
                        MergedCriteriaExpression = ""
                    }
                }
            };
            _testHelper.UnitOfWork.Context.AddEntity(_testBudgetPriority);


            _testBudgetPercentagePair = new BudgetPercentagePairEntity
            {
                Id = Guid.NewGuid(),
                BudgetPriorityId = _testBudgetPriority.Id,
                ScenarioBudgetId = _testScenarioBudget.Id,
                Percentage = 100
            };
            _testHelper.UnitOfWork.Context.AddEntity(_testBudgetPercentagePair);


            _testHelper.UnitOfWork.Context.SaveChanges();
        }

        [Fact]
        public async void ShouldReturnOkResultOnGet()
        {
            try
            {
                // Act
                var result = await _controller.GetBudgetPriorityLibraries();

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
                    .UpsertBudgetPriorityLibrary(_testBudgetPriorityLibrary.ToDto());

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
                // Act
                var result = await _controller.GetBudgetPriorityLibraries();

                // Assert
                var okObjResult = result as OkObjectResult;
                Assert.NotNull(okObjResult.Value);

                var dtos = (List<BudgetPriorityLibraryDTO>)Convert.ChangeType(okObjResult.Value,
                    typeof(List<BudgetPriorityLibraryDTO>));
                Assert.Single(dtos);

                Assert.Equal(_testBudgetPriorityLibrary.Id, dtos[0].Id);
                Assert.Single(dtos[0].BudgetPriorities);

                Assert.Equal(_testBudgetPriority.Id, dtos[0].BudgetPriorities[0].Id);
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
                var getResult = await _controller.GetBudgetPriorityLibraries();
                var dtos = (List<BudgetPriorityLibraryDTO>)Convert.ChangeType((getResult as OkObjectResult).Value,
                    typeof(List<BudgetPriorityLibraryDTO>));

                var dto = dtos[0];
                dto.Description = "Updated Description";
                dto.BudgetPriorities[0].PriorityLevel = 2;
                dto.BudgetPriorities[0].CriterionLibrary = new CriterionLibraryDTO
                {
                    Id = Guid.NewGuid(),
                    Name = "Updated Criterion",
                    Description = "",
                    MergedCriteriaExpression = "Criterion Expression",
                    IsSingleUse = true
                };
                dto.BudgetPriorities[0].BudgetPercentagePairs[0].Percentage = 90;

                // Act
                await _controller.UpsertBudgetPriorityLibrary(_testHelper.TestSimulation.Id, dto);

                // Assert
                var timer = new Timer {Interval = 5000};
                timer.Elapsed += delegate
                {
                    var modifiedDto = _testHelper.UnitOfWork.BudgetPriorityRepo
                        .GetBudgetPriorityLibraries()[0];
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
                // Act
                var result = await _controller.DeleteBudgetPriorityLibrary(_testBudgetPriorityLibrary.Id);

                // Assert
                Assert.IsType<OkResult>(result);

                Assert.True(
                    !_testHelper.UnitOfWork.Context.BudgetPriorityLibrary.Any(_ => _.Id == _testBudgetPriorityLibrary.Id));
                Assert.True(!_testHelper.UnitOfWork.Context.BudgetPriority.Any(_ => _.Id == _testBudgetPriority.Id));
                Assert.True(
                    !_testHelper.UnitOfWork.Context.CriterionLibraryBudgetPriority.Any(_ =>
                        _.BudgetPriorityId == _testBudgetPriority.Id));
                Assert.True(
                    !_testHelper.UnitOfWork.Context.BudgetPercentagePair.Any(_ => _.Id == _testBudgetPercentagePair.Id));
            }
            finally
            {
                // Cleanup
                _testHelper.CleanUp();
            }
        }
    }
}
