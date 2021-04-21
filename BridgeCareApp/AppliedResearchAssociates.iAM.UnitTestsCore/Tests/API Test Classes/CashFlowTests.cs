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
    public class CashFlowRuleTests
    {
        private readonly TestHelper _testHelper;
        private readonly CashFlowController _controller;

        private static readonly Guid CashFlowRuleLibraryId = Guid.Parse("6829466c-0333-4ad3-ad99-6862ea644286");
        private static readonly Guid CashFlowRuleId = Guid.Parse("87f43147-c1c9-40a6-9bad-fcbbbafb5814");
        private static readonly Guid CashFlowDistributionRuleId = Guid.Parse("917d6800-ce9a-4bcf-aa5e-91c98d9f7cac");

        public CashFlowRuleTests()
        {
            _testHelper = new TestHelper();
            _testHelper.CreateAttributes();
            _testHelper.CreateNetwork();
            _testHelper.CreateSimulation();
            _controller = new CashFlowController(_testHelper.MockEsecSecurityAuthorized.Object, _testHelper.UnitOfWork, _testHelper.MockHubService.Object);
        }

        public CashFlowRuleLibraryEntity TestCashFlowRuleLibrary { get; } = new CashFlowRuleLibraryEntity
        {
            Id = CashFlowRuleLibraryId,
            Name = "Test Name"
        };

        public CashFlowRuleEntity TestCashFlowRule { get; } = new CashFlowRuleEntity
        {
            Id = CashFlowRuleId,
            CashFlowRuleLibraryId = CashFlowRuleLibraryId,
            Name = "Test Name"
        };

        public CashFlowDistributionRuleEntity TestCashFlowDistributionRule { get; } = new CashFlowDistributionRuleEntity
        {
            Id = CashFlowDistributionRuleId,
            CashFlowRuleId = CashFlowRuleId,
            DurationInYears = 1,
            CostCeiling = 500000,
            YearlyPercentages = "100"
        };

        private void SetupForGet()
        {
            _testHelper.UnitOfWork.Context.CashFlowRuleLibrary.Add(TestCashFlowRuleLibrary);
            _testHelper.UnitOfWork.Context.CashFlowRule.Add(TestCashFlowRule);
            _testHelper.UnitOfWork.Context.SaveChanges();
        }

        private void SetupForUpsertOrDelete()
        {
            SetupForGet();
            _testHelper.UnitOfWork.Context.CriterionLibrary.Add(_testHelper.TestCriterionLibrary);
            _testHelper.UnitOfWork.Context.CashFlowDistributionRule.Add(TestCashFlowDistributionRule);
            _testHelper.UnitOfWork.Context.SaveChanges();
        }

        [Fact]
        public async void ShouldReturnOkResultOnGet()
        {
            try
            {
                // Act
                var result = await _controller.CashFlowRuleLibraries();

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
                    .UpsertCashFlowRuleLibrary(Guid.Empty, TestCashFlowRuleLibrary.ToDto());

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
                var result = await _controller.DeleteCashFlowRuleLibrary(Guid.Empty);

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
        public async void ShouldGetAllCashFlowRuleLibrariesWithCashFlowRules()
        {
            try
            {
                // Arrange
                SetupForGet();

                // Act
                var result = await _controller.CashFlowRuleLibraries();

                // Assert
                var okObjResult = result as OkObjectResult;
                Assert.NotNull(okObjResult.Value);

                var dtos = (List<CashFlowRuleLibraryDTO>)Convert.ChangeType(okObjResult.Value,
                    typeof(List<CashFlowRuleLibraryDTO>));
                Assert.Single(dtos);

                Assert.Equal(CashFlowRuleLibraryId, dtos[0].Id);
                Assert.Single(dtos[0].CashFlowRules);

                Assert.Equal(CashFlowRuleId, dtos[0].CashFlowRules[0].Id);
            }
            finally
            {
                // Cleanup
                _testHelper.CleanUp();
            }
        }

        [Fact]
        public async void ShouldModifyCashFlowRuleData()
        {
            try
            {
                // Arrange
                SetupForUpsertOrDelete();
                var getResult = await _controller.CashFlowRuleLibraries();
                var dtos = (List<CashFlowRuleLibraryDTO>)Convert.ChangeType((getResult as OkObjectResult).Value,
                    typeof(List<CashFlowRuleLibraryDTO>));

                var dto = dtos[0];
                dto.Description = "Updated Description";
                dto.CashFlowRules[0].Name = "Updated Name";
                dto.CashFlowRules[0].CriterionLibrary =
                    _testHelper.TestCriterionLibrary.ToDto();
                dto.CashFlowRules[0].CashFlowDistributionRules[0].DurationInYears = 2;

                // Act
                await _controller.UpsertCashFlowRuleLibrary(_testHelper.TestSimulation.Id, dto);

                // Assert
                var timer = new Timer {Interval = 5000};
                timer.Elapsed += delegate
                {
                    var modifiedDto =
                        _testHelper.UnitOfWork.CashFlowRuleRepo.CashFlowRuleLibrariesWithCashFlowRules()[0];
                    Assert.Equal(dto.Description, modifiedDto.Description);
                    Assert.Single(modifiedDto.AppliedScenarioIds);
                    Assert.Equal(_testHelper.TestSimulation.Id, modifiedDto.AppliedScenarioIds[0]);

                    Assert.Equal(dto.CashFlowRules[0].Name, modifiedDto.CashFlowRules[0].Name);
                    Assert.Equal(dto.CashFlowRules[0].CriterionLibrary.Id,
                        modifiedDto.CashFlowRules[0].CriterionLibrary.Id);

                    Assert.Equal(dto.CashFlowRules[0].CashFlowDistributionRules[0].DurationInYears,
                        modifiedDto.CashFlowRules[0].CashFlowDistributionRules[0].DurationInYears);
                };
            }
            finally
            {
                // Cleanup
                _testHelper.CleanUp();
            }
        }

        [Fact]
        public async void ShouldDeleteCashFlowRuleData()
        {
            try
            {
                // Arrange
                SetupForUpsertOrDelete();
                var getResult = await _controller.CashFlowRuleLibraries();
                var dtos = (List<CashFlowRuleLibraryDTO>)Convert.ChangeType((getResult as OkObjectResult).Value,
                    typeof(List<CashFlowRuleLibraryDTO>));

                var cashFlowRuleLibraryDTO = dtos[0];
                cashFlowRuleLibraryDTO.CashFlowRules[0].CriterionLibrary =
                    _testHelper.TestCriterionLibrary.ToDto();

                await _controller.UpsertCashFlowRuleLibrary(_testHelper.TestSimulation.Id,
                    cashFlowRuleLibraryDTO);

                // Act
                var result = await _controller.DeleteCashFlowRuleLibrary(CashFlowRuleLibraryId);

                // Assert
                Assert.IsType<OkResult>(result);

                Assert.True(!_testHelper.UnitOfWork.Context.CashFlowRuleLibrary.Any(_ => _.Id == CashFlowRuleLibraryId));
                Assert.True(!_testHelper.UnitOfWork.Context.CashFlowRule.Any(_ => _.Id == CashFlowRuleId));
                Assert.True(!_testHelper.UnitOfWork.Context.CashFlowRuleLibrarySimulation.Any(_ =>
                    _.CashFlowRuleLibraryId == CashFlowRuleLibraryId));
                Assert.True(
                    !_testHelper.UnitOfWork.Context.CriterionLibraryCashFlowRule.Any(_ =>
                        _.CashFlowRuleId == CashFlowRuleId));
                Assert.True(!_testHelper.UnitOfWork.Context.CashFlowDistributionRule.Any(_ => _.Id == CashFlowDistributionRuleId));
            }
            finally
            {
                // Cleanup
                _testHelper.CleanUp();
            }
        }
    }
}
