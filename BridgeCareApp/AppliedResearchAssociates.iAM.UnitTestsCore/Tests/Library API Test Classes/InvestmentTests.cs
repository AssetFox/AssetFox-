using System;
using System.Linq;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.DTOs;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers;
using AppliedResearchAssociates.iAM.UnitTestsCore.TestData;
using BridgeCareCore.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests.Library_API_Test_Classes
{
    public class InvestmentTests
    {
        private readonly TestHelper _testHelper;
        private readonly InvestmentController _controller;

        private static readonly Guid BudgetLibraryId = Guid.Parse("a7035a0c-5436-4b16-ada2-063590d94994");
        private static readonly Guid BudgetId = Guid.Parse("0d93abe9-1fd8-4304-badb-e982f8c376da");
        private static readonly Guid BudgetAmountId = Guid.Parse("40f7215a-4024-4ef2-91f9-23e583cf640b");
        private static readonly Guid InvestmentPlanId = Guid.Parse("0f9ed186-e62b-48b2-880d-85618740096b");

        public InvestmentTests()
        {
            _testHelper = new TestHelper();
            _testHelper.CreateAttributes();
            _testHelper.CreateNetwork();
            _testHelper.CreateSimulation();
            _controller = new InvestmentController(_testHelper.UnitOfWork, _testHelper.MockEsecSecurity);
        }

        public BudgetLibraryEntity TestBudgetLibrary { get; } = new BudgetLibraryEntity
        {
            Id = BudgetLibraryId,
            Name = "Test Name"
        };

        public BudgetEntity TestBudget { get; } = new BudgetEntity
        {
            Id = BudgetId,
            BudgetLibraryId = BudgetLibraryId,
            Name = "Test Name"
        };

        public BudgetAmountEntity TestBudgetAmount { get; } = new BudgetAmountEntity
        {
            Id = BudgetAmountId,
            BudgetId = BudgetId,
            Year = DateTime.Now.Year,
            Value = 500000
        };

        public InvestmentPlanEntity TestInvestmentPlan { get; } = new InvestmentPlanEntity
        {
            Id = InvestmentPlanId,
            FirstYearOfAnalysisPeriod = DateTime.Now.Year,
            InflationRatePercentage = 1,
            MinimumProjectCostLimit = 500000,
            NumberOfYearsInAnalysisPeriod = 1
        };

        private void SetupForGet()
        {
            _testHelper.UnitOfWork.Context.BudgetLibrary.Add(TestBudgetLibrary);
            _testHelper.UnitOfWork.Context.Budget.Add(TestBudget);
            _testHelper.UnitOfWork.Context.SaveChanges();
        }

        private void SetupForGetAll()
        {
            SetupForGet();
            TestInvestmentPlan.SimulationId = _testHelper.TestSimulation.Id;
            _testHelper.UnitOfWork.Context.InvestmentPlan.Add(TestInvestmentPlan);
            _testHelper.UnitOfWork.Context.SaveChanges();
        }

        private void SetupForUpsertOrDelete()
        {
            SetupForGetAll();
            _testHelper.UnitOfWork.Context.CriterionLibrary.Add(_testHelper.TestCriterionLibrary);
            _testHelper.UnitOfWork.Context.SaveChanges();
        }

        [Fact]
        public async void ShouldReturnOkResultOnGet()
        {
            try
            {
                // Act
                var result = await _controller.GetInvestment(Guid.Empty);

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
                    .UpsertInvestment(Guid.Empty,
                        new UpsertInvestmentDataDTO
                        {
                            BudgetLibrary = TestBudgetLibrary.ToDto(),
                            InvestmentPlan = new InvestmentPlanDTO()
                        });

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
                var result = await _controller.DeleteBudgetLibrary(Guid.Empty);

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
        public async void ShouldGetInvestmentData()
        {
            try
            {
                // Arrange
                SetupForGet();

                // Act
                var result = await _controller.GetInvestment(Guid.Empty);

                // Assert
                var okObjResult = result as OkObjectResult;
                Assert.NotNull(okObjResult.Value);

                var dto = (InvestmentDTO)Convert.ChangeType(okObjResult.Value, typeof(InvestmentDTO));
                Assert.Single(dto.BudgetLibraries);
                Assert.Equal(Guid.Empty, dto.InvestmentPlan.Id);

                Assert.Equal(BudgetLibraryId, dto.BudgetLibraries[0].Id);
                Assert.Single(dto.BudgetLibraries[0].Budgets);

                Assert.Equal(BudgetId, dto.BudgetLibraries[0].Budgets[0].Id);
            }
            finally
            {
                // Cleanup
                _testHelper.CleanUp();
            }
        }

        [Fact]
        public async void ShouldGetAllInvestmentData()
        {
            try
            {
                // Arrange
                SetupForGetAll();

                // Act
                var result = await _controller.GetInvestment(_testHelper.TestSimulation.Id);

                // Assert
                var okObjResult = result as OkObjectResult;
                Assert.NotNull(okObjResult.Value);

                var dto = (InvestmentDTO)Convert.ChangeType(okObjResult.Value, typeof(InvestmentDTO));
                Assert.Single(dto.BudgetLibraries);
                Assert.Equal(InvestmentPlanId, dto.InvestmentPlan.Id);

                Assert.Equal(BudgetLibraryId, dto.BudgetLibraries[0].Id);
                Assert.Single(dto.BudgetLibraries[0].Budgets);

                Assert.Equal(BudgetId, dto.BudgetLibraries[0].Budgets[0].Id);
            }
            finally
            {
                // Cleanup
                _testHelper.CleanUp();
            }
        }

        [Fact]
        public async void ShouldModifyInvestmentData()
        {
            try
            {
                // Arrange
                SetupForUpsertOrDelete();
                var getResult = await _controller.GetInvestment(_testHelper.TestSimulation.Id);
                var investmentDto = (InvestmentDTO)Convert.ChangeType((getResult as OkObjectResult).Value, typeof(InvestmentDTO));

                var upsertInvestmentDataDto = new UpsertInvestmentDataDTO
                {
                    BudgetLibrary = investmentDto.BudgetLibraries[0],
                    InvestmentPlan = investmentDto.InvestmentPlan
                };
                upsertInvestmentDataDto.BudgetLibrary.Description = "Updated Description";
                upsertInvestmentDataDto.BudgetLibrary.Budgets[0].Name = "Updated Name";
                upsertInvestmentDataDto.BudgetLibrary.Budgets[0].BudgetAmounts
                    .Add(TestBudgetAmount.ToDto(upsertInvestmentDataDto.BudgetLibrary.Budgets[0].Name));
                upsertInvestmentDataDto.BudgetLibrary.Budgets[0].BudgetAmounts[0].Value = 1000000;
                upsertInvestmentDataDto.BudgetLibrary.Budgets[0].CriterionLibrary =
                    _testHelper.TestCriterionLibrary.ToDto();
                upsertInvestmentDataDto.InvestmentPlan.MinimumProjectCostLimit = 1000000;

                // Act
                var result = await _controller.UpsertInvestment(_testHelper.TestSimulation.Id, upsertInvestmentDataDto);

                // Assert
                /*Assert.IsType<OkResult>(result);

                var budgetLibraryEntity = _testHelper.UnitOfWork.Context.BudgetLibrary
                    .Include(_ => _.Budgets)
                    .ThenInclude(_ => _.CriterionLibraryBudgetJoin)
                    .ThenInclude(_ => _.CriterionLibrary)
                    .Include(_ => _.Budgets)
                    .ThenInclude(_ => _.BudgetAmounts)
                    .Include(_ => _.BudgetLibrarySimulationJoins)
                    .Single(_ => _.Id == BudgetLibraryId);*/
                if (result is OkResult)
                {
                    getResult = await _controller.GetInvestment(_testHelper.TestSimulation.Id);
                    investmentDto = (InvestmentDTO)Convert.ChangeType((getResult as OkObjectResult).Value, typeof(InvestmentDTO));

                    Assert.Equal(upsertInvestmentDataDto.BudgetLibrary.Description,
                        investmentDto.BudgetLibraries[0].Description);
                    Assert.Single(investmentDto.BudgetLibraries[0].AppliedScenarioIds);
                    Assert.Equal(_testHelper.TestSimulation.Id, investmentDto.BudgetLibraries[0].AppliedScenarioIds[0]);

                    var budgetDto = investmentDto.BudgetLibraries[0].Budgets[0];
                    Assert.Equal(upsertInvestmentDataDto.BudgetLibrary.Budgets[0].Name, budgetDto.Name);
                    Assert.Equal(upsertInvestmentDataDto.BudgetLibrary.Budgets[0].CriterionLibrary.Id,
                        budgetDto.CriterionLibrary.Id);
                    Assert.True(budgetDto.BudgetAmounts.Any());

                var investmentPlanEntity = _testHelper.UnitOfWork.Context.InvestmentPlan
                    .Single(_ => _.Id == InvestmentPlanId);
                Assert.Equal(addOrUpdateInvestmentDTO.InvestmentPlan.MinimumProjectCostLimit,
                    investmentPlanEntity.MinimumProjectCostLimit);
                Assert.Equal(_testHelper.TestSimulation.Id, investmentPlanEntity.SimulationId);
                    var budgetAmountDto = budgetDto.BudgetAmounts[0];
                    Assert.Equal(upsertInvestmentDataDto.BudgetLibrary.Budgets[0].BudgetAmounts[0].Value,
                        budgetAmountDto.Value);

                    var investmentPlanDto = upsertInvestmentDataDto.InvestmentPlan;
                    Assert.Equal(upsertInvestmentDataDto.InvestmentPlan.MinimumProjectCostLimit,
                        investmentPlanDto.MinimumProjectCostLimit);
                }
            }
            finally
            {
                // Cleanup
                _testHelper.CleanUp();
            }
        }

        [Fact]
        public async void ShouldDeleteBudgetData()
        {
            try
            {
                // Arrange
                SetupForUpsertOrDelete();
                var getResult = await _controller.GetInvestment(_testHelper.TestSimulation.Id);
                var dto = (InvestmentDTO)Convert.ChangeType((getResult as OkObjectResult).Value, typeof(InvestmentDTO));

                var addOrUpdateInvestmentDTO = new UpsertInvestmentDataDTO
                {
                    BudgetLibrary = dto.BudgetLibraries[0],
                    InvestmentPlan = dto.InvestmentPlan
                };
                addOrUpdateInvestmentDTO.BudgetLibrary.Budgets[0].BudgetAmounts
                    .Add(TestBudgetAmount.ToDto("Test Name"));
                addOrUpdateInvestmentDTO.BudgetLibrary.Budgets[0].CriterionLibrary =
                    _testHelper.TestCriterionLibrary.ToDto();

                await _controller.UpsertInvestment(_testHelper.TestSimulation.Id, addOrUpdateInvestmentDTO);

                // Act
                var result = await _controller.DeleteBudgetLibrary(BudgetLibraryId);

                // Assert
                Assert.IsType<OkResult>(result);

                Assert.True(!_testHelper.UnitOfWork.Context.BudgetLibrary.Any(_ => _.Id == BudgetLibraryId));
                Assert.True(!_testHelper.UnitOfWork.Context.Budget.Any(_ => _.Id == BudgetId));
                Assert.True(!_testHelper.UnitOfWork.Context.BudgetLibrarySimulation.Any(_ =>
                    _.BudgetLibraryId == BudgetLibraryId));
                Assert.True(
                    !_testHelper.UnitOfWork.Context.CriterionLibraryBudget.Any(_ =>
                        _.BudgetId == BudgetId));
                Assert.True(
                    !_testHelper.UnitOfWork.Context.BudgetAmount.Any(_ => _.Id == BudgetAmountId));
            }
            finally
            {
                // Cleanup
                _testHelper.CleanUp();
            }
        }
    }
}
