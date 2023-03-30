using System;
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
using Org.BouncyCastle.Bcpg;
using Xunit;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests
{
    public class BudgetRepositoryTests
    {
        [Fact]
        public async Task CreateNewBudgetLibrary_Does()
        {
            var libraryId = Guid.NewGuid();
            var user = await UserTestSetup.ModelForEntityInDb(TestHelper.UnitOfWork);
            var userId = user.Id;
            var dto = BudgetLibraryDtos.New(libraryId);
            var dtoClone = BudgetLibraryDtos.New(libraryId);
            TestHelper.UnitOfWork.SetUser(user.Username);

            TestHelper.UnitOfWork.BudgetRepo.CreateNewBudgetLibrary(dto, userId);

            var libraryAfter = TestHelper.UnitOfWork.BudgetRepo.GetBudgetLibrary(libraryId);
            ObjectAssertions.EquivalentExcluding(dtoClone, libraryAfter, x => x.Owner);
        }

        [Fact]
        public void UpsertBudgetLibraryAndUpsertOrDeleteBudgets_LibraryInDb_Does()
        {
            var library = BudgetLibraryTestSetup.ModelForEntityInDb(TestHelper.UnitOfWork, "Old name");
            var libraryId = library.Id;
            library.Name = "Updated name";
            var budgetId = Guid.NewGuid();
            var budget = BudgetDtos.New(budgetId);
            library.Budgets.Add(budget);

            TestHelper.UnitOfWork.BudgetRepo.UpdateBudgetLibraryAndUpsertOrDeleteBudgets(library);

            var libraryAfter = TestHelper.UnitOfWork.BudgetRepo.GetBudgetLibrary(libraryId);
            ObjectAssertions.EquivalentExcluding(library, libraryAfter, x => x.Budgets[0].CriterionLibrary);
        }

        [Fact]
        public void UpsertOrDeleteScenarioBudgets_SimulationExistsAndBudgetInList_Inserts()
        {
            AttributeTestSetup.CreateAttributes(TestHelper.UnitOfWork);
            NetworkTestSetup.CreateNetwork(TestHelper.UnitOfWork);
            // Arrange
            var simulation = SimulationTestSetup.CreateSimulation(TestHelper.UnitOfWork);
            var budgetDto = BudgetDtos.New();
            var budgetDtos = new List<BudgetDTO> { budgetDto };

            // Act
            TestHelper.UnitOfWork.BudgetRepo.UpsertOrDeleteScenarioBudgets(budgetDtos, simulation.Id);

            var budgetsAfter = TestHelper.UnitOfWork.BudgetRepo.GetScenarioBudgets(simulation.Id);
            var budgetAfter = budgetsAfter.Single();
            ObjectAssertions.EquivalentExcluding(budgetDto, budgetAfter, x => x.CriterionLibrary);
        }

        [Fact]
        public void UpsertOrDeleteScenarioBudgets_SimulationInDbWithBudgetPriority_BudgetInList_InsertsWithPercentagePair()
        {
            AttributeTestSetup.CreateAttributes(TestHelper.UnitOfWork);
            NetworkTestSetup.CreateNetwork(TestHelper.UnitOfWork);
            // Arrange
            var simulation = SimulationTestSetup.CreateSimulation(TestHelper.UnitOfWork);
            var simulationId = simulation.Id;
            var budgetPriority = BudgetPriorityDtos.New();
            var budgetPriorities = new List<BudgetPriorityDTO> { budgetPriority };
            TestHelper.UnitOfWork.BudgetPriorityRepo.UpsertOrDeleteScenarioBudgetPriorities(budgetPriorities, simulationId);
            var budgetDto = BudgetDtos.New();
            var budgetDtos = new List<BudgetDTO> { budgetDto };

            // Act
            TestHelper.UnitOfWork.BudgetRepo.UpsertOrDeleteScenarioBudgets(budgetDtos, simulation.Id);

            var budgetsAfter = TestHelper.UnitOfWork.BudgetRepo.GetScenarioBudgets(simulation.Id);
            var budgetAfter = budgetsAfter.Single();
            ObjectAssertions.EquivalentExcluding(budgetDto, budgetAfter, x => x.CriterionLibrary);
            var budgetPrioritiesAfter = TestHelper.UnitOfWork.BudgetPriorityRepo.GetScenarioBudgetPriorities(simulationId);
            var budgetPriorityAfter = budgetPrioritiesAfter.Single();
            var percentagePairs = budgetPriorityAfter.BudgetPercentagePairs;
            var percentagePair = percentagePairs.Single();
            Assert.Equal(100, percentagePair.Percentage);
            Assert.Equal(budgetAfter.Name, percentagePair.BudgetName);
            Assert.Equal(budgetAfter.Id, percentagePair.BudgetId);
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
                UserName = user.Username,
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
        public void Delete_LibraryInDbWithBudget_DeletesBoth()
        {
            var library = BudgetLibraryTestSetup.ModelForEntityInDb(TestHelper.UnitOfWork);
            var criterionLibraryId = Guid.NewGuid();
            var budgetId = Guid.NewGuid();
            BudgetTestSetup.AddBudgetToLibrary(TestHelper.UnitOfWork, library.Id, budgetId, criterionLibraryId);
            Assert.True(TestHelper.UnitOfWork.Context.BudgetLibrary.Any(_ => _.Id == library.Id));
            Assert.True(TestHelper.UnitOfWork.Context.Budget.Any(_ => _.BudgetLibraryId == library.Id));
            Assert.True(TestHelper.UnitOfWork.Context.CriterionLibraryBudget.Any(_ => _.BudgetId == budgetId));
            Assert.True(TestHelper.UnitOfWork.Context.BudgetAmount.Any(_ => _.BudgetId == budgetId));
            TestHelper.UnitOfWork.BudgetRepo.DeleteBudgetLibrary(library.Id);

            // Assert
            Assert.False(TestHelper.UnitOfWork.Context.BudgetLibrary.Any(_ => _.Id == library.Id));
            Assert.False(TestHelper.UnitOfWork.Context.Budget.Any(_ => _.BudgetLibraryId == library.Id));
            Assert.False(TestHelper.UnitOfWork.Context.CriterionLibraryBudget.Any(_ => _.BudgetId == budgetId));
            Assert.False(TestHelper.UnitOfWork.Context.BudgetAmount.Any(_ => _.BudgetId == budgetId));
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
            var budgetId = Guid.NewGuid();
            var budgetDto = BudgetDtos.New(budgetId, budgetName);
            var budgetDtos = new List<BudgetDTO> { budgetDto };
            ScenarioBudgetTestSetup.UpsertOrDeleteScenarioBudgets(TestHelper.UnitOfWork, budgetDtos, simulation.Id);

            //testing and asserts
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
            var budgetName = RandomStrings.WithPrefix("Budget");
            var budgetId = Guid.NewGuid();
            var budgetDto = BudgetDtos.New(budgetId, budgetName);
            var budgetDtos = new List<BudgetDTO> { budgetDto };
            ScenarioBudgetTestSetup.UpsertOrDeleteScenarioBudgets(TestHelper.UnitOfWork, budgetDtos, simulation.Id);

            var actualBudgets = TestHelper.UnitOfWork.BudgetRepo.GetScenarioBudgets(simulation.Id);

            var actualBudget = actualBudgets.Single();
            Assert.Equal(budgetName, actualBudget.Name);
            Assert.Equal(budgetId, actualBudget.Id);
            Assert.Empty(actualBudget.BudgetAmounts);
            Assert.Equal(Guid.Empty, actualBudget.CriterionLibrary.Id);
        }

        [Fact]
        public void GetScenarioBudgets_SimulationInDbWithScenarioBudgetWithAmount_GetsWithAmount()
        {
            AttributeTestSetup.CreateAttributes(TestHelper.UnitOfWork);
            NetworkTestSetup.CreateNetwork(TestHelper.UnitOfWork);
            var simulation = SimulationTestSetup.DomainSimulation(TestHelper.UnitOfWork);
            var investmentPlanDto = TestHelper.UnitOfWork.InvestmentPlanRepo.GetInvestmentPlan(simulation.Id);
            investmentPlanDto.NumberOfYearsInAnalysisPeriod = 1;
            TestHelper.UnitOfWork.InvestmentPlanRepo.UpsertInvestmentPlan(investmentPlanDto, simulation.Id);
            TestHelper.UnitOfWork.InvestmentPlanRepo.GetSimulationInvestmentPlan(simulation);
            var budgetName = RandomStrings.WithPrefix("Budget");
            var budgetId = Guid.NewGuid();
            var amountId = Guid.NewGuid();
            var budgetDto = BudgetDtos.WithSingleAmount(budgetId, budgetName, 2023, 1234, amountId);
            var budgetDtos = new List<BudgetDTO> { budgetDto };
            ScenarioBudgetTestSetup.UpsertOrDeleteScenarioBudgets(TestHelper.UnitOfWork, budgetDtos, simulation.Id);

            var actualBudgets = TestHelper.UnitOfWork.BudgetRepo.GetScenarioBudgets(simulation.Id);

            var actualBudget = actualBudgets.Single();
            Assert.Equal(budgetName, actualBudget.Name);
            Assert.Equal(budgetId, actualBudget.Id);
            var budgetAmount = actualBudget.BudgetAmounts.Single();
            Assert.Equal(1234m, budgetAmount.Value);
            Assert.Equal(amountId, budgetAmount.Id);
            Assert.Equal(Guid.Empty, actualBudget.CriterionLibrary.Id);
        }

        [Fact]
        public void GetScenarioBudgets_SimulationInDbWithScenarioBudgetWithCriterionLibrary_GetsTheLibrary()
        {
            AttributeTestSetup.CreateAttributes(TestHelper.UnitOfWork);
            NetworkTestSetup.CreateNetwork(TestHelper.UnitOfWork);
            var simulation = SimulationTestSetup.DomainSimulation(TestHelper.UnitOfWork);
            var budgetName = RandomStrings.WithPrefix("Budget");
            var budgetId = Guid.NewGuid();
            var budgetDto = BudgetDtos.New(budgetId, budgetName);
            var criterionLibrary = CriterionLibraryDtos.Dto();
            budgetDto.CriterionLibrary = criterionLibrary;
            var budgetDtos = new List<BudgetDTO> { budgetDto };
            ScenarioBudgetTestSetup.UpsertOrDeleteScenarioBudgets(TestHelper.UnitOfWork, budgetDtos, simulation.Id);

            var actualBudgets = TestHelper.UnitOfWork.BudgetRepo.GetScenarioBudgets(simulation.Id);
            var actualBudget = actualBudgets.Single();
            Assert.Equal(criterionLibrary.MergedCriteriaExpression, actualBudget.CriterionLibrary.MergedCriteriaExpression);
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
