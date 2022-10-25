﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.DTOs.Enums;
using AppliedResearchAssociates.iAM.TestHelpers;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests.Repositories;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests.User;
using AppliedResearchAssociates.iAM.UnitTestsCore.TestUtils;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using Xunit;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests
{
    public class BudgetRepositoryTests
    {
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
        public void UpsertBudgetLibrary_NullDto_Throws()
        {
            //setup
            var unitOfWork = TestHelper.UnitOfWork;
            BudgetLibraryDTO budgetLibraryDto = null;

            //testing and asserts
            Assert.ThrowsAny<Exception>(() => unitOfWork.BudgetRepo.UpsertBudgetLibrary(budgetLibraryDto));
        }

        [Fact]
        public async Task GetLibraryUsers_BudgetLibraryInDbWithUser_GetsWithUser()
        {
            var user = await UserTestSetup.ModelForEntityInDb(TestHelper.UnitOfWork);
            var library = BudgetLibraryTestSetup.ModelForEntityInDb(TestHelper.UnitOfWork);
            BudgetLibraryUserTestSetup.SetUsersOfBudgetLibrary(TestHelper.UnitOfWork, library.Id, LibraryAccessLevel.Modify, user.Id);

            var users = TestHelper.UnitOfWork.BudgetRepo.GetLibraryUsers(library.Id);

            var actualUser = users.Single();
            var expectedUser = new LibraryUserDTO
            {
                AccessLevel = LibraryAccessLevel.Modify,
                UserId = user.Id,
            };
            ObjectAssertions.Equivalent(expectedUser, actualUser);
        }

        [Fact]
        public async Task GetAllBudgetLibraries_BudgetLibraryInDb_Gets()
        {
            // wjwjwj is this now duplicative?
            var user = await UserTestSetup.ModelForEntityInDb(TestHelper.UnitOfWork);
            var library = BudgetLibraryTestSetup.ModelForEntityInDb(TestHelper.UnitOfWork);
            BudgetLibraryUserTestSetup.SetUsersOfBudgetLibrary(TestHelper.UnitOfWork, library.Id, LibraryAccessLevel.Modify, user.Id);

            var libraries = TestHelper.UnitOfWork.BudgetRepo.GetBudgetLibraries();
            var actualLibrary = libraries.Single(l => l.Id == library.Id);
            Assert.Equal(library.Name, actualLibrary.Name);
        }

        [Fact]
        public async Task GetBudgetLibrariesNoChildren_BudgetLibraryInDbWithBudget_GetsWithoutBudget()
        {
            var user = await UserTestSetup.ModelForEntityInDb(TestHelper.UnitOfWork);
            var library = BudgetLibraryTestSetup.ModelForEntityInDb(TestHelper.UnitOfWork);

            var libraries = TestHelper.UnitOfWork.BudgetRepo.GetBudgetLibrariesNoChildren();

            var libraryAfter = libraries.Single(l => l.Id == library.Id);
            var budgets = libraryAfter.Budgets;
            Assert.Empty(budgets);
        }

        [Fact]
        public async Task GetBudgetLibrariesNoChildrenAccessibleToUser_BudgetLibraryInDbWithAccessForUser_Gets()
        {
            var user = await UserTestSetup.ModelForEntityInDb(TestHelper.UnitOfWork);
            var library = BudgetLibraryTestSetup.ModelForEntityInDb(TestHelper.UnitOfWork);
            BudgetLibraryUserTestSetup.SetUsersOfBudgetLibrary(TestHelper.UnitOfWork, library.Id, LibraryAccessLevel.Modify, user.Id);
            TestHelper.UnitOfWork.Context.ChangeTracker.Clear();

            var libraries = TestHelper.UnitOfWork.BudgetRepo.GetBudgetLibrariesNoChildrenAccessibleToUser(user.Id);

            var actual = libraries.Single();
            library.Budgets.Clear();
            ObjectAssertions.Equivalent(library, actual);
        }

        [Fact]
        public async Task GetBudgetLibrariesNoChildrenAccessibleToUser_BudgetLibraryInDbWithoutAccessForUser_DoesNotGet() { 
            var user = await UserTestSetup.ModelForEntityInDb(TestHelper.UnitOfWork);
            var user2 = await UserTestSetup.ModelForEntityInDb(TestHelper.UnitOfWork);
            var library = BudgetLibraryTestSetup.ModelForEntityInDb(TestHelper.UnitOfWork);
            BudgetLibraryUserTestSetup.SetUsersOfBudgetLibrary(TestHelper.UnitOfWork, library.Id, LibraryAccessLevel.Modify, user2.Id);
            TestHelper.UnitOfWork.Context.ChangeTracker.Clear();

            var libraries = TestHelper.UnitOfWork.BudgetRepo.GetBudgetLibrariesNoChildrenAccessibleToUser(user.Id);

            Assert.Empty(libraries);
        }

        [Fact]
        public async Task Delete_BudgetLibraryInDbWithUser_Deletes()
        {
            var user = await UserTestSetup.ModelForEntityInDb(TestHelper.UnitOfWork);
            var library = BudgetLibraryTestSetup.ModelForEntityInDb(TestHelper.UnitOfWork);
            BudgetLibraryUserTestSetup.SetUsersOfBudgetLibrary(TestHelper.UnitOfWork, library.Id, LibraryAccessLevel.Modify, user.Id);

            TestHelper.UnitOfWork.BudgetRepo.DeleteBudgetLibrary(library.Id);

            var librariesAfter = TestHelper.UnitOfWork.BudgetRepo.GetBudgetLibraries();
            var libraryAfter = librariesAfter.SingleOrDefault(l => l.Id == library.Id);
            Assert.Null(libraryAfter);
            var libraryUsersAfter = TestHelper.UnitOfWork.Context.BudgetLibraryUser.Where(u => u.UserId == user.Id).ToList();
            Assert.Empty(libraryUsersAfter);
        }

        [Fact]
        public void Delete_LibraryInDbWithCriterionLibrary_DeletesBoth()
        {

        }

        [Fact]
        public void Delete_LibraryInDbWithBudget_DeletesBoth()
        {
            var library = BudgetLibraryTestSetup.ModelForEntityInDb(TestHelper.UnitOfWork);
            BudgetTestSetup.AddBudgetToLibrary(TestHelper.UnitOfWork, library.Id);
            Assert.True(TestHelper.UnitOfWork.Context.BudgetLibrary.Any(_ => _.Id == library.Id));
            Assert.True(TestHelper.UnitOfWork.Context.Budget.Any(_ => _.BudgetLibraryId == library.Id));

            TestHelper.UnitOfWork.BudgetRepo.DeleteBudgetLibrary(library.Id);

            // Assert
            Assert.False(TestHelper.UnitOfWork.Context.BudgetLibrary.Any(_ => _.Id == library.Id));
            Assert.False(TestHelper.UnitOfWork.Context.Budget.Any(_ => _.BudgetLibraryId == library.Id));
            //Assert.True(
            //    !TestHelper.UnitOfWork.Context.CriterionLibraryBudget.Any(_ =>
            //        _.BudgetId == _testBudget.Id));
            //Assert.True(
            //    !TestHelper.UnitOfWork.Context.BudgetAmount.Any(_ =>
            //        _.Id == _testBudget.BudgetAmounts.ToList()[0].Id));
        }


        [Fact]
        public void GetScenarioSimpleBudgetDetails_SimulationInDb_EmptyList()
        {
            NetworkTestSetup.CreateNetwork(TestHelper.UnitOfWork);
            var simulation = SimulationTestSetup.CreateSimulation(TestHelper.UnitOfWork);

            var simpleDetails = TestHelper.UnitOfWork.BudgetRepo.GetScenarioSimpleBudgetDetails(simulation.Id);

            Assert.Empty(simpleDetails);
        }

        [Fact]
        public void GetScenarioSimpleBudgetDetails_SimulationInDbWithScenarioBudgets_Gets()
        {
            AttributeTestSetup.CreateAttributes(TestHelper.UnitOfWork);
            NetworkTestSetup.CreateNetwork(TestHelper.UnitOfWork);
            var simulation = SimulationTestSetup.DomainSimulation(TestHelper.UnitOfWork);
            var investmentPlan = simulation.InvestmentPlan;
            var budgetName = RandomStrings.WithPrefix("Budget");
            var budget = investmentPlan.AddBudget();
            budget.Name = budgetName;
            var budgets = investmentPlan.Budgets.ToList();
            ScenarioBudgetTestSetup.CreateScenarioBudgets(TestHelper.UnitOfWork, budgets, simulation.Id);

            var details = TestHelper.UnitOfWork.BudgetRepo.GetScenarioSimpleBudgetDetails(simulation.Id);

            var detail = details.Single();
            Assert.Equal(budgetName, detail.Name);
        }

        [Fact]
        public void GetScenarioBudgets_SimulationInDbWithScenarioBudget_Gets()
        {
            AttributeTestSetup.CreateAttributes(TestHelper.UnitOfWork);
            NetworkTestSetup.CreateNetwork(TestHelper.UnitOfWork);
            var simulation = SimulationTestSetup.DomainSimulation(TestHelper.UnitOfWork);
            var investmentPlan = simulation.InvestmentPlan;
            var budgetName = RandomStrings.WithPrefix("Budget");
            var budget = investmentPlan.AddBudget();
            budget.Name = budgetName;
            var budgets = investmentPlan.Budgets.ToList();
            ScenarioBudgetTestSetup.CreateScenarioBudgets(TestHelper.UnitOfWork, budgets, simulation.Id);

            var actualBudgets = TestHelper.UnitOfWork.BudgetRepo.GetScenarioBudgets(simulation.Id);

            var actualBudget = actualBudgets.Single();
            Assert.Equal(budgetName, actualBudget.Name);
            Assert.Equal(budget.Id, actualBudget.Id);
            Assert.Empty(actualBudget.BudgetAmounts);
            Assert.Equal(Guid.Empty, actualBudget.CriterionLibrary.Id);
        }

        [Fact]
        public void GetScenarioBudgets_SimulationInDbWithScenarioBudgetWithAmount_GetsWithAmount()
        {
            AttributeTestSetup.CreateAttributes(TestHelper.UnitOfWork);
            NetworkTestSetup.CreateNetwork(TestHelper.UnitOfWork);
            var simulation = SimulationTestSetup.DomainSimulation(TestHelper.UnitOfWork);
            var investmentPlan = simulation.InvestmentPlan;
            TestHelper.UnitOfWork.InvestmentPlanRepo.CreateInvestmentPlan(investmentPlan, simulation.Id);
            investmentPlan.NumberOfYearsInAnalysisPeriod = 1;
            var budgetName = RandomStrings.WithPrefix("Budget");
            var budget = investmentPlan.AddBudget();
            budget.YearlyAmounts[0].Value = 1234;
            budget.Name = budgetName;
            var budgets = investmentPlan.Budgets.ToList();
            ScenarioBudgetTestSetup.CreateScenarioBudgets(TestHelper.UnitOfWork, budgets, simulation.Id);

            var actualBudgets = TestHelper.UnitOfWork.BudgetRepo.GetScenarioBudgets(simulation.Id);

            var actualBudget = actualBudgets.Single();
            Assert.Equal(budgetName, actualBudget.Name);
            Assert.Equal(budget.Id, actualBudget.Id);
            var budgetAmount = actualBudget.BudgetAmounts.Single();
            Assert.Equal(1234m, budgetAmount.Value);
            Assert.Equal(Guid.Empty, actualBudget.CriterionLibrary.Id);
        }

        [Fact]
        public void GetScenarioBudgets_SimulationInDbWithScenarioBudgetWithCriterionLibrary_GetsTheLibrary()
        {

            AttributeTestSetup.CreateAttributes(TestHelper.UnitOfWork);
            NetworkTestSetup.CreateNetwork(TestHelper.UnitOfWork);
            var simulation = SimulationTestSetup.DomainSimulation(TestHelper.UnitOfWork);
            var investmentPlan = simulation.InvestmentPlan;
            var budgetName = RandomStrings.WithPrefix("Budget");
            var budget = investmentPlan.AddBudget();
            budget.Name = budgetName;
            var budgets = investmentPlan.Budgets.ToList();
            ScenarioBudgetTestSetup.CreateScenarioBudgets(TestHelper.UnitOfWork, budgets, simulation.Id);
        }

        [Fact]
        public void GetScenarioBudgets_CriterionLibraryInDb_GetsBudgetWithCriterionLibraryId()
        {
            var criterionLibraryName = RandomStrings.WithPrefix("Budget");
            var criterionLibrary = CriterionLibraryTestSetup.TestCriterionLibraryInDb(TestHelper.UnitOfWork, criterionLibraryName);
            AttributeTestSetup.CreateAttributes(TestHelper.UnitOfWork);
            NetworkTestSetup.CreateNetwork(TestHelper.UnitOfWork);
            var simulation = SimulationTestSetup.DomainSimulation(TestHelper.UnitOfWork);
            var investmentPlan = simulation.InvestmentPlan;
            var budgetName = RandomStrings.WithPrefix("Budget");
            var budget = investmentPlan.AddBudget();
            budget.Name = budgetName;
            var budgets = investmentPlan.Budgets.ToList();
            var guids = new List<Guid> { budget.Id };
            var dictionary = new Dictionary<string, List<Guid>>
            {
                {
                    criterionLibrary.MergedCriteriaExpression,
                    guids
                }
            };
            TestHelper.UnitOfWork.CriterionLibraryRepo.JoinEntitiesWithCriteria(
                dictionary, "BudgetEntity", simulation.Name
                );
            ScenarioBudgetTestSetup.CreateScenarioBudgets(TestHelper.UnitOfWork, budgets, simulation.Id);

            var scenarioBudgets = TestHelper.UnitOfWork.BudgetRepo.GetScenarioBudgets(simulation.Id);

            var scenarioBudget = scenarioBudgets.Single();
            Assert.Equal(budgetName, scenarioBudget.Name);
            var scenarioBudgetCriterionLibrary = scenarioBudget.CriterionLibrary;
        }

        [Fact]
        public void UpsertOrDeleteScenarioBudgets_SimulationExistsAndBudgetListIsEmpty_InsertsNothing()
        {
            AttributeTestSetup.CreateAttributes(TestHelper.UnitOfWork);
            NetworkTestSetup.CreateNetwork(TestHelper.UnitOfWork);
            // Arrange
            var simulation = SimulationTestSetup.CreateSimulation(TestHelper.UnitOfWork);
            var budgetDtos = new List<BudgetDTO>();

            // Act
            TestHelper.UnitOfWork.BudgetRepo.UpsertOrDeleteScenarioBudgets(budgetDtos, simulation.Id);

            var budgetsAfter = TestHelper.UnitOfWork.BudgetRepo.GetScenarioBudgets(simulation.Id);
            Assert.Empty(budgetsAfter);
        }

        [Fact]
        public async Task UpdateBudgetLibraryWithUserAccessChange_Does()
        {
            var user = await UserTestSetup.ModelForEntityInDb(TestHelper.UnitOfWork);
            var library = BudgetLibraryTestSetup.ModelForEntityInDb(TestHelper.UnitOfWork);
            BudgetLibraryUserTestSetup.SetUsersOfBudgetLibrary(TestHelper.UnitOfWork, library.Id, LibraryAccessLevel.Modify, user.Id);
            var libraryUsersBefore = TestHelper.UnitOfWork.BudgetRepo.GetLibraryUsers(library.Id);
            var libraryUserBefore = libraryUsersBefore.Single();
            Assert.Equal(LibraryAccessLevel.Modify, libraryUserBefore.AccessLevel);
            libraryUserBefore.AccessLevel = LibraryAccessLevel.Read;

            TestHelper.UnitOfWork.BudgetRepo.UpsertOrDeleteUsers(library.Id, libraryUsersBefore);

            var libraryUsersAfter = TestHelper.UnitOfWork.BudgetRepo.GetLibraryUsers(library.Id);
            var libraryUserAfter = libraryUsersAfter.Single();
            Assert.Equal(LibraryAccessLevel.Read, libraryUserAfter.AccessLevel);
        }

        [Fact]
        public async Task UpdateBudgetLibraryUsers_RequestAccessRemoval_Does()
        {
            var user = await UserTestSetup.ModelForEntityInDb(TestHelper.UnitOfWork);
            var library = BudgetLibraryTestSetup.ModelForEntityInDb(TestHelper.UnitOfWork);
            BudgetLibraryUserTestSetup.SetUsersOfBudgetLibrary(TestHelper.UnitOfWork, library.Id, LibraryAccessLevel.Modify, user.Id);
            var libraryUsersBefore = TestHelper.UnitOfWork.BudgetRepo.GetLibraryUsers(library.Id);
            var libraryUserBefore = libraryUsersBefore.Single();
            libraryUsersBefore.Remove(libraryUserBefore);

            TestHelper.UnitOfWork.BudgetRepo.UpsertOrDeleteUsers(library.Id, libraryUsersBefore);
            TestHelper.UnitOfWork.Context.SaveChanges();
            
            var libraryUsersAfter = TestHelper.UnitOfWork.BudgetRepo.GetLibraryUsers(library.Id);
            Assert.Empty(libraryUsersAfter);
        }

        [Fact]
        public async Task UpdateLibraryUsers_AddAccessForUser_Does()
        {
            var user1 = await UserTestSetup.ModelForEntityInDb(TestHelper.UnitOfWork);
            var user2 = await UserTestSetup.ModelForEntityInDb(TestHelper.UnitOfWork);
            var library = BudgetLibraryTestSetup.ModelForEntityInDb(TestHelper.UnitOfWork);
            BudgetLibraryUserTestSetup.SetUsersOfBudgetLibrary(TestHelper.UnitOfWork, library.Id, LibraryAccessLevel.Modify, user1.Id);
            var usersBefore = TestHelper.UnitOfWork.BudgetRepo.GetLibraryUsers(library.Id);
            var newUser = new LibraryUserDTO
            {
                AccessLevel = LibraryAccessLevel.Read,
                UserId = user2.Id,
            };
            usersBefore.Add(newUser);

            TestHelper.UnitOfWork.BudgetRepo.UpsertOrDeleteUsers(library.Id, usersBefore);

            var libraryUsersAfter = TestHelper.UnitOfWork.BudgetRepo.GetLibraryUsers(library.Id);
            var user1After = libraryUsersAfter.Single(u => u.UserId == user1.Id);
            var user2After = libraryUsersAfter.Single(u => u.UserId == user2.Id);
            Assert.Equal(LibraryAccessLevel.Modify, user1After.AccessLevel);
            Assert.Equal(LibraryAccessLevel.Read, user2After.AccessLevel);
        }
    }
}
