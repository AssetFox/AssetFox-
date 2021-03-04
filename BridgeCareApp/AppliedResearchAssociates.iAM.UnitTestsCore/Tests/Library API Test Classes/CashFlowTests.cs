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
    public class CashFlowRuleTests
    {
        private readonly TestHelper _testHelper;
        private readonly CashFlowController _controller;

        private static readonly Guid CashFlowRuleLibraryId = Guid.Parse("6829466c-0333-4ad3-ad99-6862ea644286");
        private static readonly Guid CashFlowRuleId = Guid.Parse("87f43147-c1c9-40a6-9bad-fcbbbafb5814");
        private static readonly Guid CashFlowDistributionRuleId = Guid.Parse("917d6800-ce9a-4bcf-aa5e-91c98d9f7cac");

        public CashFlowRuleTests()
        {
            _testHelper = new TestHelper("IAMv2cf");
            _testHelper.CreateAttributes();
            _testHelper.CreateNetwork();
            _testHelper.CreateSimulation();
            _controller = new CashFlowController(_testHelper.UnitOfDataPersistenceWork);
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
            _testHelper.UnitOfDataPersistenceWork.Context.CashFlowRuleLibrary.Add(TestCashFlowRuleLibrary);
            _testHelper.UnitOfDataPersistenceWork.Context.CashFlowRule.Add(TestCashFlowRule);
            _testHelper.UnitOfDataPersistenceWork.Context.SaveChanges();
        }

        private void SetupForUpsertOrDelete()
        {
            SetupForGet();
            _testHelper.UnitOfDataPersistenceWork.Context.CriterionLibrary.Add(_testHelper.TestCriterionLibrary);
            _testHelper.UnitOfDataPersistenceWork.Context.CashFlowDistributionRule.Add(TestCashFlowDistributionRule);
            _testHelper.UnitOfDataPersistenceWork.Context.SaveChanges();
        }

        [Fact]
        public void ShouldReturnOkResultOnGet()
        {
            try
            {
                // Act
                var result = _controller.CashFlowRuleLibraries();

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
                    .UpsertCashFlowRuleLibrary(Guid.Empty, TestCashFlowRuleLibrary.ToDto());

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
                var result = _controller.DeleteCashFlowRuleLibrary(Guid.Empty);

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
        public void ShouldGetAllCashFlowRuleLibrariesWithCashFlowRules()
        {
            try
            {
                // Arrange
                SetupForGet();

                // Act
                var result = _controller.CashFlowRuleLibraries();

                // Assert
                var okObjResult = result.Result as OkObjectResult;
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
        public void ShouldModifyCashFlowRuleData()
        {
            try
            {
                // Arrange
                SetupForUpsertOrDelete();
                var dtos = (List<CashFlowRuleLibraryDTO>)Convert.ChangeType(
                    (_controller.CashFlowRuleLibraries().Result as OkObjectResult).Value,
                    typeof(List<CashFlowRuleLibraryDTO>));

                var cashFlowRuleLibraryDTO = dtos[0];
                cashFlowRuleLibraryDTO.Description = "Updated Description";
                cashFlowRuleLibraryDTO.CashFlowRules[0].Name = "Updated Name";
                cashFlowRuleLibraryDTO.CashFlowRules[0].CriterionLibrary =
                    _testHelper.TestCriterionLibrary.ToDto();
                cashFlowRuleLibraryDTO.CashFlowRules[0].CashFlowDistributionRules[0].DurationInYears = 2;

                // Act
                var result =
                    _controller.UpsertCashFlowRuleLibrary(_testHelper.TestSimulation.Id,
                        cashFlowRuleLibraryDTO);

                // Assert
                Assert.IsType<OkResult>(result.Result);

                var cashFlowRuleLibraryEntity = _testHelper.UnitOfDataPersistenceWork.Context.CashFlowRuleLibrary
                    .Include(_ => _.CashFlowRules)
                    .ThenInclude(_ => _.CriterionLibraryCashFlowRuleJoin)
                    .ThenInclude(_ => _.CriterionLibrary)
                    .Include(_ => _.CashFlowRules)
                    .ThenInclude(_ => _.CashFlowDistributionRules)
                    .Include(_ => _.CashFlowRuleLibrarySimulationJoins)
                    .Single(_ => _.Id == CashFlowRuleLibraryId);

                Assert.Equal(cashFlowRuleLibraryDTO.Description, cashFlowRuleLibraryEntity.Description);
                Assert.Single(cashFlowRuleLibraryEntity.CashFlowRuleLibrarySimulationJoins);
                var budgetPriorityLibrarySimulationJoin =
                    cashFlowRuleLibraryEntity.CashFlowRuleLibrarySimulationJoins.ToList()[0];
                Assert.Equal(_testHelper.TestSimulation.Id, budgetPriorityLibrarySimulationJoin.SimulationId);
                var cashFlowRuleEntity = cashFlowRuleLibraryEntity.CashFlowRules.ToList()[0];
                Assert.Equal(cashFlowRuleLibraryDTO.CashFlowRules[0].Name, cashFlowRuleEntity.Name);
                Assert.NotNull(cashFlowRuleEntity.CriterionLibraryCashFlowRuleJoin);
                Assert.Equal(cashFlowRuleLibraryDTO.CashFlowRules[0].CriterionLibrary.Id,
                    cashFlowRuleEntity.CriterionLibraryCashFlowRuleJoin.CriterionLibrary.Id);
                var cashFlowDistributionRuleEntity = cashFlowRuleEntity.CashFlowDistributionRules.ToList()[0];
                Assert.Equal(cashFlowRuleLibraryDTO.CashFlowRules[0].CashFlowDistributionRules[0].DurationInYears,
                    cashFlowDistributionRuleEntity.DurationInYears);
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
                var dtos = (List<CashFlowRuleLibraryDTO>)Convert.ChangeType(
                    (_controller.CashFlowRuleLibraries().Result as OkObjectResult).Value,
                    typeof(List<CashFlowRuleLibraryDTO>));

                var cashFlowRuleLibraryDTO = dtos[0];
                cashFlowRuleLibraryDTO.CashFlowRules[0].CriterionLibrary =
                    _testHelper.TestCriterionLibrary.ToDto();

                await _controller.UpsertCashFlowRuleLibrary(_testHelper.TestSimulation.Id,
                    cashFlowRuleLibraryDTO);

                // Act
                var result = _controller.DeleteCashFlowRuleLibrary(CashFlowRuleLibraryId);

                // Assert
                Assert.IsType<OkResult>(result.Result);

                Assert.True(!_testHelper.UnitOfDataPersistenceWork.Context.CashFlowRuleLibrary.Any(_ => _.Id == CashFlowRuleLibraryId));
                Assert.True(!_testHelper.UnitOfDataPersistenceWork.Context.CashFlowRule.Any(_ => _.Id == CashFlowRuleId));
                Assert.True(!_testHelper.UnitOfDataPersistenceWork.Context.CashFlowRuleLibrarySimulation.Any(_ =>
                    _.CashFlowRuleLibraryId == CashFlowRuleLibraryId));
                Assert.True(
                    !_testHelper.UnitOfDataPersistenceWork.Context.CriterionLibraryCashFlowRule.Any(_ =>
                        _.CashFlowRuleId == CashFlowRuleId));
                Assert.True(!_testHelper.UnitOfDataPersistenceWork.Context.CashFlowDistributionRule.Any(_ => _.Id == CashFlowDistributionRuleId));
            }
            finally
            {
                // Cleanup
                _testHelper.CleanUp();
            }
        }
    }
}
