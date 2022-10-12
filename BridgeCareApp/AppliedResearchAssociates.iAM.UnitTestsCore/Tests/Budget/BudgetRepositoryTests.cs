using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.Analysis;
using AppliedResearchAssociates.iAM.Common.PerformanceMeasurement;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests.Repositories;
using AppliedResearchAssociates.iAM.UnitTestsCore.TestUtils;
using AppliedResearchAssociates.iAM.TestHelpers;
using Xunit;
using Microsoft.EntityFrameworkCore;
using AppliedResearchAssociates.iAM.Data.Attributes;
using System.Data;
using AppliedResearchAssociates.iAM.DataUnitTests.Tests;
using Humanizer;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests
{
    public class BudgetRepositoryTests
    {
        private BudgetAmountDTO createBudgetAmountObject(string budgetName, int year, decimal value)
        {
            return new BudgetAmountDTO()
            {
                Id = Guid.NewGuid(),
                BudgetName = budgetName,
                Year = year,
                Value = value
            };
        }

        private CriterionLibraryDTO createCriterionLibraryObject(string criteriaExpression = "", bool singleUse = false)
        {
            return new CriterionLibraryDTO()
            {
                MergedCriteriaExpression = criteriaExpression,
                IsSingleUse = singleUse
            };
        }

        private BudgetDTO createBudgetObject(string budgetName)
        {
            //create budget amounts
            var budgetAmountList= new List<BudgetAmountDTO>();
            budgetAmountList.Add(createBudgetAmountObject(budgetName, 2010, 2010000));

            //create criterion library
            var criterionLibraryObject = createCriterionLibraryObject("0=0", true);

            return new BudgetDTO()
            {
                Id = Guid.NewGuid(),
                Name = budgetName,
                BudgetAmounts = budgetAmountList,
                CriterionLibrary = criterionLibraryObject
            };
        }

        private BudgetLibraryDTO createBudgetLibraryDto(string name, bool isShared = true)
        {
            //setup
            var budgetList = new List<BudgetDTO>();
            budgetList.Add(createBudgetObject("Budget 1"));

            //create budget library
            return new BudgetLibraryDTO()
            {
                Id = Guid.NewGuid(),
                Name = name,
                Budgets = budgetList?.ToList(),
                IsShared = isShared
            };
        }

        [Fact]
        public void CreateScenarioBudgets_SuccessfulWithValidInput()
        {
            //setup
            var unitOfWork = TestHelper.UnitOfWork;
            AttributeTestSetup.CreateAttributes(unitOfWork);
            NetworkTestSetup.CreateNetwork(unitOfWork);
            var simulationObject = SimulationTestSetup.DomainSimulation(unitOfWork);
            var investmentPlan = simulationObject.InvestmentPlan;
            var budgetObject = investmentPlan.AddBudget();
            var budgetName = "Test Budget";
            budgetObject.Name = budgetName;
            var budgets = investmentPlan.Budgets.ToList();

            //testing and asserts
            unitOfWork.BudgetRepo.CreateScenarioBudgets(budgets, simulationObject.Id);
            
            var budgetEntities = unitOfWork.Context.ScenarioBudget
                                    .Where(w => w.SimulationId == simulationObject.Id)
                                    .ToList();
            var budgetEntityInTest = budgetEntities.Single(b => b.Name == budgetName);
        }

        [Fact]
        public void UpsertBudgetLibrary_NullDtoThrowException()
        {
            //setup
            var unitOfWork = TestHelper.UnitOfWork;
            BudgetLibraryDTO budgetLibraryDto = null;

            //testing and asserts
            Assert.Throws<NullReferenceException>(() => unitOfWork.BudgetRepo.UpsertBudgetLibrary(budgetLibraryDto));
        }

        [Fact]
        public void BudgetLibrary_SharedButInvalidOrNoUsers()
        {
            //setup
            var unitOfWork = TestHelper.UnitOfWork;

            //create budget library
            var budgetLibraryDto = createBudgetLibraryDto("BudgetLibrary_SharedButInvalidOrNoUsers", true);
            unitOfWork.BudgetRepo.UpsertBudgetLibrary(budgetLibraryDto);

            //testing and asserts
            var budgetLibraryDtoAfter = unitOfWork.BudgetRepo.GetBudgetLibrary(budgetLibraryDto.Id);
            ObjectAssertions.EquivalentExcluding(budgetLibraryDto, budgetLibraryDtoAfter, bl => bl.Budgets);
        }

        [Fact (Skip ="Wj to figure out")]
        public async Task UpsertBudgetLibrary_SharedWithValidUsers()
        {
            //setup
            var unitOfWork = TestHelper.UnitOfWork;
            var budgetLibraryDto = createBudgetLibraryDto("UpsertBudgetLibrary_SharedWithValidUsers", true);

            //act
            unitOfWork.BudgetRepo.UpsertBudgetLibrary(budgetLibraryDto);

            //asserts
            var budgetLibraryDtoCheck = unitOfWork.BudgetRepo.GetBudgetLibrary(budgetLibraryDto.Id);
            ObjectAssertions.Equivalent(budgetLibraryDto, budgetLibraryDtoCheck);
        }
    }
}
