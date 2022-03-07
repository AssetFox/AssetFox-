using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.LibraryEntities.Budget;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.LibraryEntities.BudgetPriority;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.ScenarioEntities.Budget;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.ScenarioEntities.BudgetPriority;
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
        private BudgetPriorityController _controller;

        private ScenarioBudgetEntity _testScenarioBudget;
        private ScenarioBudgetPriorityEntity _testScenarioBudgetPriority;
        private BudgetPercentagePairEntity _testBudgetPercentagePair;
        private BudgetPriorityLibraryEntity _testBudgetPriorityLibrary;
        private BudgetPriorityEntity _testBudgetPriority;

        public BudgetPriorityTests()
        {
            _testHelper = TestHelper.Instance;
            if (!_testHelper.DbContext.Attribute.Any())
            {
                _testHelper.CreateAttributes();
                _testHelper.CreateNetwork();
                _testHelper.CreateSimulation();
                _testHelper.SetupDefaultHttpContext();
            }
        }

        private void CreateAuthorizedController() =>
            _controller = new BudgetPriorityController(_testHelper.MockEsecSecurityAuthorized.Object,
                _testHelper.UnitOfWork,
                _testHelper.MockHubService.Object, _testHelper.MockHttpContextAccessor.Object);

        private void CreateUnauthorizedController() =>
            _controller = new BudgetPriorityController(_testHelper.MockEsecSecurityNotAuthorized.Object,
                _testHelper.UnitOfWork,
                _testHelper.MockHubService.Object, _testHelper.MockHttpContextAccessor.Object);

        private void CreateLibraryTestData()
        {
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


            _testHelper.UnitOfWork.Context.SaveChanges();
        }

        private void CreateScenarioTestData()
        {
            _testScenarioBudget = new ScenarioBudgetEntity
            {
                Id = Guid.NewGuid(),
                SimulationId = _testHelper.TestSimulation.Id,
                Name = "ScenarioBudgetEntity"
            };
            _testHelper.UnitOfWork.Context.AddEntity(_testScenarioBudget);


            _testScenarioBudgetPriority = new ScenarioBudgetPriorityEntity
            {
                Id = Guid.NewGuid(),
                SimulationId = _testHelper.TestSimulation.Id,
                PriorityLevel = 1,
                CriterionLibraryScenarioBudgetPriorityJoin = new CriterionLibraryScenarioBudgetPriorityEntity
                {
                    CriterionLibrary = new CriterionLibraryEntity
                    {
                        Id = Guid.NewGuid(),
                        Name = "Budget Priority Criterion",
                        IsSingleUse = true,
                        MergedCriteriaExpression = ""
                    }
                }
            };
            _testHelper.UnitOfWork.Context.AddEntity(_testScenarioBudgetPriority);


            _testBudgetPercentagePair = new BudgetPercentagePairEntity
            {
                Id = Guid.NewGuid(),
                ScenarioBudgetPriorityId = _testScenarioBudgetPriority.Id,
                ScenarioBudgetId = _testScenarioBudget.Id,
                Percentage = 100
            };
            _testHelper.UnitOfWork.Context.AddEntity(_testBudgetPercentagePair);
        }

        [Fact]
        public async void ShouldReturnOkResultOnLibraryGet()
        {
            try
            {
                // Arrange
                CreateAuthorizedController();

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
        public async void ShouldReturnOkResultOnScenarioGet()
        {
            try
            {
                // Arrange
                CreateAuthorizedController();

                // Act
                var result = await _controller.GetScenarioBudgetPriorities(_testHelper.TestSimulation.Id);

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
        public async void ShouldReturnOkResultOnLibraryPost()
        {
            try
            {
                // Arrange
                CreateAuthorizedController();
                var dto = new BudgetPriorityLibraryDTO
                {
                    Id = Guid.NewGuid(), Name = "", BudgetPriorities = new List<BudgetPriorityDTO>()
                };

                // Act
                var result = await _controller
                    .UpsertBudgetPriorityLibrary(dto);

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
        public async void ShouldReturnOkResultOnScenarioPost()
        {
            try
            {
                // Arrange
                CreateAuthorizedController();
                var dtos = new List<BudgetPriorityDTO>();

                // Act
                var result = await _controller
                    .UpsertScenarioBudgetPriorities(_testHelper.TestSimulation.Id, dtos);

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
                CreateAuthorizedController();
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
        public async void ShouldGetLibraryData()
        {
            try
            {
                // Arrange
                CreateAuthorizedController();
                CreateLibraryTestData();

                // Act
                var result = await _controller.GetBudgetPriorityLibraries();

                // Assert
                var okObjResult = result as OkObjectResult;
                Assert.NotNull(okObjResult.Value);

                var dtos = (List<BudgetPriorityLibraryDTO>)Convert.ChangeType(okObjResult.Value,
                    typeof(List<BudgetPriorityLibraryDTO>));
                Assert.True(dtos.Count() > 0);                                
                Assert.True(dtos[0].BudgetPriorities.Count()>0);                                
                Assert.Equal(_testBudgetPriority.PriorityLevel, dtos[0].BudgetPriorities[0].PriorityLevel);
                Assert.Equal(_testBudgetPriority.Year, dtos[0].BudgetPriorities[0].Year);
                Assert.Equal(_testBudgetPriority.CriterionLibraryBudgetPriorityJoin.CriterionLibraryId, dtos[0].BudgetPriorities[0].CriterionLibrary.Id);
            }
            finally
            {
                // Cleanup
                _testHelper.CleanUp();
            }
        }

        [Fact]
        public async void ShouldGetScenarioData()
        {
            try
            {
                // Arrange
                CreateAuthorizedController();
                CreateScenarioTestData();

                // Act
                var result = await _controller.GetScenarioBudgetPriorities(_testHelper.TestSimulation.Id);

                // Assert
                var okObjResult = result as OkObjectResult;
                Assert.NotNull(okObjResult.Value);

                var dtos = (List<BudgetPriorityDTO>)Convert.ChangeType(okObjResult.Value,
                    typeof(List<BudgetPriorityDTO>));
                Assert.Single(dtos);
                Assert.Equal(_testScenarioBudgetPriority.Id, dtos[0].Id);
                Assert.Equal(_testScenarioBudgetPriority.PriorityLevel, dtos[0].PriorityLevel);
                Assert.Equal(_testScenarioBudgetPriority.Year, dtos[0].Year);

                Assert.Single(dtos[0].BudgetPercentagePairs);
                Assert.Equal(_testBudgetPercentagePair.Id, dtos[0].BudgetPercentagePairs[0].Id);
                Assert.Equal(_testBudgetPercentagePair.Percentage, dtos[0].BudgetPercentagePairs[0].Percentage);
                Assert.Equal(_testBudgetPercentagePair.ScenarioBudgetId, dtos[0].BudgetPercentagePairs[0].BudgetId);
                Assert.Equal(_testScenarioBudget.Name, dtos[0].BudgetPercentagePairs[0].BudgetName);

                Assert.Equal(_testScenarioBudgetPriority.CriterionLibraryScenarioBudgetPriorityJoin.CriterionLibraryId, dtos[0].CriterionLibrary.Id);
            }
            finally
            {
                // Cleanup
                _testHelper.CleanUp();
            }
        }

        [Fact]
        public async void ShouldModifyLibraryData()
        {
            try
            {
                // Arrange
                CreateAuthorizedController();
                CreateLibraryTestData();

                // Arrange
                _testBudgetPriorityLibrary.BudgetPriorities = new List<BudgetPriorityEntity> {_testBudgetPriority};

                var dto = _testBudgetPriorityLibrary.ToDto();
                dto.Description = "Updated Description";
                dto.BudgetPriorities[0].PriorityLevel = 2;
                dto.BudgetPriorities[0].Year = DateTime.Now.Year + 1;
                dto.BudgetPriorities[0].CriterionLibrary = new CriterionLibraryDTO();

                // Act
                await _controller.UpsertBudgetPriorityLibrary(dto);

                // Assert
                var timer = new Timer {Interval = 5000};
                timer.Elapsed += delegate
                {
                    var modifiedDto = _testHelper.UnitOfWork.BudgetPriorityRepo.GetBudgetPriorityLibraries()[0];
                    Assert.Equal(dto.Description, modifiedDto.Description);

                    Assert.Equal(dto.BudgetPriorities[0].PriorityLevel, modifiedDto.BudgetPriorities[0].PriorityLevel);
                    Assert.Equal(dto.BudgetPriorities[0].Year, modifiedDto.BudgetPriorities[0].Year);
                    Assert.Equal(dto.BudgetPriorities[0].CriterionLibrary.Id,
                        modifiedDto.BudgetPriorities[0].CriterionLibrary.Id);
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
        public async void ShouldModifyScenarioData()
        {
            try
            {
                // Arrange
                CreateAuthorizedController();
                CreateScenarioTestData();

                // Arrange
                _testScenarioBudgetPriority.BudgetPercentagePairs =
                    new List<BudgetPercentagePairEntity> {_testBudgetPercentagePair};
                var dtos = new List<BudgetPriorityDTO> {_testScenarioBudgetPriority.ToDto()};

                dtos[0].PriorityLevel = 2;
                dtos[0].Year = DateTime.Now.Year + 1;
                dtos[0].CriterionLibrary = new CriterionLibraryDTO();
                dtos[0].BudgetPercentagePairs[0].Percentage = 90;

                // Act
                await _controller.UpsertScenarioBudgetPriorities(_testHelper.TestSimulation.Id, dtos);

                // Assert
                var timer = new Timer {Interval = 5000};
                timer.Elapsed += delegate
                {
                    var modifiedDto = _testHelper.UnitOfWork.BudgetPriorityRepo.GetScenarioBudgetPriorities(_testHelper.TestSimulation.Id)[0];
                    Assert.Equal(dtos[0].PriorityLevel, modifiedDto.PriorityLevel);
                    Assert.Equal(dtos[0].Year, modifiedDto.Year);
                    Assert.Equal(dtos[0].CriterionLibrary.Id, modifiedDto.CriterionLibrary.Id);
                    Assert.Equal(dtos[0].BudgetPercentagePairs[0].Percentage, modifiedDto.BudgetPercentagePairs[0].Percentage);
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
        public async void ShouldDeleteLibraryData()
        {
            try
            {
                // Arrange
                CreateAuthorizedController();
                CreateLibraryTestData();

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
            }
            finally
            {
                // Cleanup
                _testHelper.CleanUp();
            }
        }

        [Fact]
        public async void ShouldThrowUnauthorizedOnInvestmentPost()
        {
            try
            {
                // Arrange
                CreateUnauthorizedController();
                CreateScenarioTestData();

                var dtos = new List<BudgetPriorityDTO>();

                // Act
                var result = await _controller.UpsertScenarioBudgetPriorities(_testHelper.TestSimulation.Id, dtos);

                // Assert
                Assert.IsType<UnauthorizedResult>(result);
            }
            finally
            {
                // Cleanup
                _testHelper.CleanUp();
            }
        }
    }
}
