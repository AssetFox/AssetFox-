using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.DTOs.Enums;
using AppliedResearchAssociates.iAM.TestHelpers;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests.Repositories;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests.ScenarioBudget;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests.User;
using AppliedResearchAssociates.iAM.UnitTestsCore.TestUtils;
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
            Assert.Throws<NullReferenceException>(() => unitOfWork.BudgetRepo.UpsertBudgetLibrary(budgetLibraryDto));
        }

        [Fact]
        public void BudgetLibrary_SharedButInvalidOrNoUsers()
        {
            //setup
            var unitOfWork = TestHelper.UnitOfWork;

            //create budget library
            var budgetLibraryDto = BudgetLibraryTestSetup.CreateBudgetLibraryDto("BudgetLibrary_SharedButInvalidOrNoUsers", true);
            unitOfWork.BudgetRepo.UpsertBudgetLibrary(budgetLibraryDto);

            //testing and asserts
            var budgetLibraryDtoAfter = unitOfWork.BudgetRepo.GetBudgetLibrary(budgetLibraryDto.Id);
            ObjectAssertions.EquivalentExcluding(budgetLibraryDto, budgetLibraryDtoAfter, bl => bl.Budgets);
        }

        [Fact]
        public async Task GetBudgetLibrary_BudgetLibraryInDbWithUser_GetsWithUser()
        {
            var user = await UserTestSetup.ModelForEntityInDb(TestHelper.UnitOfWork);
            var library = BudgetLibraryTestSetup.ModelForEntityInDb(TestHelper.UnitOfWork);
            BudgetLibraryUserTestSetup.SetUsersOfBudgetLibrary(TestHelper.UnitOfWork, library.Id, LibraryAccessLevel.Modify, user.Id);

            var libraryWithUsers = TestHelper.UnitOfWork.BudgetRepo.GetBudgetLibrary(library.Id);

            var users = libraryWithUsers.Users;
            var actualUser = users.Single();
            var expectedUser = new LibraryUserDTO
            {
                AccessLevel = LibraryAccessLevel.Modify,
                UserId = user.Id,
            };
            ObjectAssertions.Equivalent(expectedUser, actualUser);
        }

        [Fact]
        public async Task GetAllBudgetLibraries_BudgetLibraryInDbWithUser_GetsWithUser()
        {
            var user = await UserTestSetup.ModelForEntityInDb(TestHelper.UnitOfWork);
            var library = BudgetLibraryTestSetup.ModelForEntityInDb(TestHelper.UnitOfWork);
            BudgetLibraryUserTestSetup.SetUsersOfBudgetLibrary(TestHelper.UnitOfWork, library.Id, LibraryAccessLevel.Modify, user.Id);

            var libraries = TestHelper.UnitOfWork.BudgetRepo.GetBudgetLibraries();

            var libraryAfter = libraries.Single(l => l.Id == library.Id);
            var users = libraryAfter.Users;
            var actualUser = users.Single();
            var expectedUser = new LibraryUserDTO
            {
                AccessLevel = LibraryAccessLevel.Modify,
                UserId = user.Id,
            };
            ObjectAssertions.Equivalent(expectedUser, actualUser);
        }


        [Fact]
        public async Task GetBudgetLibrariesNoChildren_BudgetLibraryInDbWithUser_GetsWithoutUser()
        {
            var user = await UserTestSetup.ModelForEntityInDb(TestHelper.UnitOfWork);
            var library = BudgetLibraryTestSetup.ModelForEntityInDb(TestHelper.UnitOfWork);
            BudgetLibraryUserTestSetup.SetUsersOfBudgetLibrary(TestHelper.UnitOfWork, library.Id, LibraryAccessLevel.Modify, user.Id);

            var libraries = TestHelper.UnitOfWork.BudgetRepo.GetBudgetLibrariesNoChildren();

            var libraryAfter = libraries.Single(l => l.Id == library.Id);
            var users = libraryAfter.Users;
            Assert.Empty(users);
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
        public void CriterionLibraryInDb_IdkWut()
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
    }
}
