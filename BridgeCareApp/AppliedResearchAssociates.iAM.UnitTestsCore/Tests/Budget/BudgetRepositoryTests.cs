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
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests.User;
using AppliedResearchAssociates.iAM.DTOs.Enums;

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
            var budgetLibraryDto = BudgetTestSetup.CreateBudgetLibraryDto("BudgetLibrary_SharedButInvalidOrNoUsers", true);
            unitOfWork.BudgetRepo.UpsertBudgetLibrary(budgetLibraryDto);

            //testing and asserts
            var budgetLibraryDtoAfter = unitOfWork.BudgetRepo.GetBudgetLibrary(budgetLibraryDto.Id);
            ObjectAssertions.EquivalentExcluding(budgetLibraryDto, budgetLibraryDtoAfter, bl => bl.Budgets);
        }

        [Fact]
        public async Task BudgetInDbWithUser_Get_GetsWithUser()
        {
            var user = await UserTestSetup.ModelForEntityInDb(TestHelper.UnitOfWork);
            var library = BudgetTestSetup.ModelForEntityInDb(TestHelper.UnitOfWork);
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
        public async Task BudgetInDbWithUser_GetAllBudgetLibraries_GetsWithUser()
        {
            var user = await UserTestSetup.ModelForEntityInDb(TestHelper.UnitOfWork);
            var library = BudgetTestSetup.ModelForEntityInDb(TestHelper.UnitOfWork);
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
    }
}
