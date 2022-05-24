using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.LibraryEntities.CashFlow;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.ScenarioEntities.CashFlow;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Extensions;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.UnitTestsCore.TestUtils;
using BridgeCareCore.Controllers;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests.APITestClasses
{
    public class CashFlowRuleTests
    {
        private readonly TestHelper _testHelper;
        private CashFlowController _controller;

        private CashFlowRuleLibraryEntity _testCashFlowRuleLibrary;
        private CashFlowRuleEntity _testCashFlowRule;
        private CashFlowDistributionRuleEntity _testCashFlowDistributionRule;
        private ScenarioCashFlowRuleEntity _testScenarioCashFlowRule;
        private ScenarioCashFlowDistributionRuleEntity _testScenarioCashFlowDistributionRule;

        public CashFlowRuleTests()
        {
            _testHelper = TestHelper.Instance;
            if (!_testHelper.DbContext.Attribute.Any())
            {
                _testHelper.CreateSingletons();
                _testHelper.CreateSimulation();
            }
        }

        private void CreateAuthorizedController() =>
            _controller = new CashFlowController(_testHelper.MockEsecSecurityAuthorized.Object,
                _testHelper.UnitOfWork,
                _testHelper.MockHubService.Object, _testHelper.MockHttpContextAccessor.Object);

        private void CreateUnauthorizedController() =>
            _controller = new CashFlowController(_testHelper.MockEsecSecurityNotAuthorized.Object,
                _testHelper.UnitOfWork,
                _testHelper.MockHubService.Object, _testHelper.MockHttpContextAccessor.Object);

        private void CreateLibraryTestData()
        {
            _testCashFlowRuleLibrary = new CashFlowRuleLibraryEntity
            {
                Id = Guid.NewGuid(),
                Name = "TestCashFlowRuleLibrary",
                Description = ""
            };
            _testHelper.UnitOfWork.Context.AddEntity(_testCashFlowRuleLibrary);


            _testCashFlowRule = new CashFlowRuleEntity
            {
                Id = Guid.NewGuid(),
                CashFlowRuleLibraryId = _testCashFlowRuleLibrary.Id,
                Name = "TestCashFlowRule",
                CriterionLibraryCashFlowRuleJoin = new CriterionLibraryCashFlowRuleEntity
                {
                    CriterionLibrary = new CriterionLibraryEntity
                    {
                        Id = Guid.NewGuid(),
                        Name = "TestCriterionLibrary",
                        Description = "",
                        MergedCriteriaExpression = "",
                        IsSingleUse = true
                    }
                }
            };
            _testHelper.UnitOfWork.Context.AddEntity(_testCashFlowRule);


            _testCashFlowDistributionRule = new CashFlowDistributionRuleEntity
            {
                Id = Guid.NewGuid(),
                CashFlowRuleId = _testCashFlowRule.Id,
                DurationInYears = 1,
                CostCeiling = 500000,
                YearlyPercentages = "100"
            };
            _testHelper.UnitOfWork.Context.AddEntity(_testCashFlowDistributionRule);
        }

        private void CreateScenarioTestData(Guid simulationId)
        {
            _testScenarioCashFlowRule = new ScenarioCashFlowRuleEntity
            {
                Id = Guid.NewGuid(),
                SimulationId = simulationId,
                Name = "TestCashFlowRule",
                CriterionLibraryScenarioCashFlowRuleJoin = new CriterionLibraryScenarioCashFlowRuleEntity
                {
                    CriterionLibrary = new CriterionLibraryEntity
                    {
                        Id = Guid.NewGuid(),
                        Name = "TestCriterionLibrary",
                        Description = "",
                        MergedCriteriaExpression = "",
                        IsSingleUse = true
                    }
                }
            };
            _testHelper.UnitOfWork.Context.AddEntity(_testScenarioCashFlowRule);


            _testScenarioCashFlowDistributionRule = new ScenarioCashFlowDistributionRuleEntity
            {
                Id = Guid.NewGuid(),
                ScenarioCashFlowRuleId = _testScenarioCashFlowRule.Id,
                DurationInYears = 1,
                CostCeiling = 500000,
                YearlyPercentages = "100"
            };
            _testHelper.UnitOfWork.Context.AddEntity(_testScenarioCashFlowDistributionRule);
        }

        [Fact]
        public async Task ShouldReturnOkResultOnLibraryGet()
        {
            // Arrange
            CreateAuthorizedController();

            // Act
            var result = await _controller.GetCashFlowRuleLibraries();

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task ShouldReturnOkResultOnScenarioGet()
        {
            // Arrange
            CreateAuthorizedController();
            var simulation = _testHelper.CreateSimulation();

            // Act
            var result = await _controller.GetScenarioCashFlowRules(simulation.Id);

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task ShouldReturnOkResultOnLibraryPost()
        {
            // Arrange
            CreateAuthorizedController();
            var dto = new CashFlowRuleLibraryDTO
            {
                Id = Guid.NewGuid(),
                Name = "",
                CashFlowRules = new List<CashFlowRuleDTO>()
            };

            // Act
            var result = await _controller.UpsertCashFlowRuleLibrary(dto);

            // Assert
            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async Task ShouldReturnOkResultOnScenarioPost()
        {
            // Arrange
            CreateAuthorizedController();
            var simulation = _testHelper.CreateSimulation();
            var dtos = new List<CashFlowRuleDTO>();

            // Act
            var result = await _controller.UpsertScenarioCashFlowRules(simulation.Id, dtos);

            // Assert
            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async Task ShouldReturnOkResultOnDelete()
        {
            // Arrange
            CreateAuthorizedController();

            // Act
            var result = await _controller.DeleteCashFlowRuleLibrary(Guid.Empty);

            // Assert
            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async Task ShouldGetLibraryData()
        {
            // Arrange
            CreateAuthorizedController();
            CreateLibraryTestData();

            // Act
            var result = await _controller.GetCashFlowRuleLibraries();

            // Assert
            var okObjResult = result as OkObjectResult;
            Assert.NotNull(okObjResult.Value);

            var dtos = (List<CashFlowRuleLibraryDTO>)Convert.ChangeType(okObjResult.Value,
                typeof(List<CashFlowRuleLibraryDTO>));
            Assert.Single(dtos);

            Assert.Equal(_testCashFlowRuleLibrary.Id, dtos[0].Id);
            Assert.Single(dtos[0].CashFlowRules);

            Assert.Equal(_testCashFlowRule.Id, dtos[0].CashFlowRules[0].Id);
            Assert.Equal(_testCashFlowRule.CriterionLibraryCashFlowRuleJoin.CriterionLibrary.Id,
                dtos[0].CashFlowRules[0].CriterionLibrary.Id);
            Assert.Single(dtos[0].CashFlowRules[0].CashFlowDistributionRules);

            Assert.Equal(_testCashFlowDistributionRule.Id,
                dtos[0].CashFlowRules[0].CashFlowDistributionRules[0].Id);
        }

        [Fact]
        public async Task ShouldGetScenarioData()
        {
            // Arrange
            var simulation = _testHelper.CreateSimulation();
            CreateAuthorizedController();
            CreateScenarioTestData(simulation.Id);

            // Act
            var result = await _controller.GetScenarioCashFlowRules(simulation.Id);

            // Assert
            var okObjResult = result as OkObjectResult;
            Assert.NotNull(okObjResult.Value);

            var dtos = (List<CashFlowRuleDTO>)Convert.ChangeType(okObjResult.Value,
                typeof(List<CashFlowRuleDTO>));
            Assert.Single(dtos);

            Assert.Equal(_testScenarioCashFlowRule.Id, dtos[0].Id);
            Assert.Equal(_testScenarioCashFlowRule.CriterionLibraryScenarioCashFlowRuleJoin.CriterionLibrary.Id,
                dtos[0].CriterionLibrary.Id);
            Assert.Single(dtos[0].CashFlowDistributionRules);

            Assert.Equal(_testScenarioCashFlowDistributionRule.Id,
                dtos[0].CashFlowDistributionRules[0].Id);
        }

        [Fact]
        public async Task ShouldModifyLibraryData()
        {
            // Arrange
            CreateAuthorizedController();
            CreateLibraryTestData();

            _testCashFlowRule.CashFlowDistributionRules.Add(_testCashFlowDistributionRule);
            _testCashFlowRuleLibrary.CashFlowRules.Add(_testCashFlowRule);

            var dto = _testCashFlowRuleLibrary.ToDto();
            dto.Description = "Updated Description";
            dto.CashFlowRules[0].Name = "Updated Name";
            dto.CashFlowRules[0].CriterionLibrary.MergedCriteriaExpression = "Updated Expression";
            dto.CashFlowRules[0].CashFlowDistributionRules[0].DurationInYears = 2;

            // Act
            await _controller.UpsertCashFlowRuleLibrary(dto);

            // Assert
            var timer = new Timer { Interval = 5000 };
            timer.Elapsed += delegate
            {
                var modifiedDto = _testHelper.UnitOfWork.CashFlowRuleRepo.GetCashFlowRuleLibraries()[0];
                Assert.Equal(dto.Description, modifiedDto.Description);

                Assert.Equal(dto.CashFlowRules[0].Name, modifiedDto.CashFlowRules[0].Name);
                Assert.Equal(dto.CashFlowRules[0].CriterionLibrary.MergedCriteriaExpression,
                    modifiedDto.CashFlowRules[0].CriterionLibrary.MergedCriteriaExpression);

                Assert.Equal(dto.CashFlowRules[0].CashFlowDistributionRules[0].DurationInYears,
                    modifiedDto.CashFlowRules[0].CashFlowDistributionRules[0].DurationInYears);
            };
        }

        [Fact]
        // Wjwjwj deleted timer. Could be broken.
        public async Task ShouldModifyScenarioData()
        {
            // Arrange
            var simulation = _testHelper.CreateSimulation();
            CreateAuthorizedController();
            CreateScenarioTestData(simulation.Id);

            _testScenarioCashFlowRule.ScenarioCashFlowDistributionRules.Add(_testScenarioCashFlowDistributionRule);

            var dtos = new List<CashFlowRuleDTO> { _testScenarioCashFlowRule.ToDto() };
            dtos[0].Name = "Updated Name";
            dtos[0].CriterionLibrary.MergedCriteriaExpression = "Updated Expression";
            dtos[0].CashFlowDistributionRules[0].DurationInYears = 2;

            // Act
            await _controller.UpsertScenarioCashFlowRules(simulation.Id, dtos);

            // Assert
            await Task.Delay(5000);
            var modifiedDtos = _testHelper.UnitOfWork.CashFlowRuleRepo
                .GetScenarioCashFlowRules(simulation.Id);
            Assert.Single(modifiedDtos);

            Assert.Equal(dtos[0].Name, modifiedDtos[0].Name);
            Assert.Equal(dtos[0].CriterionLibrary.MergedCriteriaExpression,
                modifiedDtos[0].CriterionLibrary.MergedCriteriaExpression);

            Assert.Equal(dtos[0].CashFlowDistributionRules[0].DurationInYears,
                modifiedDtos[0].CashFlowDistributionRules[0].DurationInYears);
        }

        [Fact]
        public async Task ShouldDeleteLibraryData()
        {
            // Arrange
            CreateAuthorizedController();
            CreateLibraryTestData();

            // Act
            var result = await _controller.DeleteCashFlowRuleLibrary(_testCashFlowRuleLibrary.Id);

            // Assert
            Assert.IsType<OkResult>(result);

            Assert.True(
                !_testHelper.UnitOfWork.Context.CashFlowRuleLibrary.Any(_ => _.Id == _testCashFlowRuleLibrary.Id));
            Assert.True(!_testHelper.UnitOfWork.Context.CashFlowRule.Any(_ => _.Id == _testCashFlowRule.Id));
            Assert.True(
                !_testHelper.UnitOfWork.Context.CriterionLibraryCashFlowRule.Any(_ =>
                    _.CashFlowRuleId == _testCashFlowRule.Id));
            Assert.True(
                !_testHelper.UnitOfWork.Context.CashFlowDistributionRule.Any(_ =>
                    _.Id == _testCashFlowDistributionRule.Id));
        }

        [Fact]
        public async Task ShouldThrowUnauthorizedOnInvestmentPost()
        {
            // Arrange
            var simulation = _testHelper.CreateSimulation();
            CreateUnauthorizedController();
            CreateScenarioTestData(simulation.Id);

            var dtos = new List<CashFlowRuleDTO>();

            // Act
            var result = await _controller.UpsertScenarioCashFlowRules(simulation.Id, dtos);

            // Assert
            Assert.IsType<UnauthorizedResult>(result);
        }
    }
}
