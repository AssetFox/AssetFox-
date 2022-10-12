using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.TestHelpers;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests.User;
using AppliedResearchAssociates.iAM.UnitTestsCore.TestUtils;
using Xunit;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests
{
    public class BudgetRepositoryUserUpdateTests
    {
        [Fact]
        public async Task BudgetLibraryInDb_AddBudgetUsers_Does()
        {
            var libraryName = RandomStrings.WithPrefix("BudgetLibrary");
            var budgetLibrary = BudgetTestSetup.ModelForEntityInDb(TestHelper.UnitOfWork, libraryName, false);
            var user = await UserTestSetup.ModelForEntityInDb(TestHelper.UnitOfWork);

            var userDto = new LibraryUserDTO
            {
                AccessLevel = DTOs.Enums.LibraryAccessLevel.Modify,
                UserId = user.Id,
            };
            budgetLibrary.Users.Add(userDto);

            TestHelper.UnitOfWork.BudgetRepo.UpsertBudgetLibrary(budgetLibrary);

            var userEntitiesAfter = TestHelper.UnitOfWork.Context.BudgetLibraryUser.Where(u => u.BudgetLibraryId == budgetLibrary.Id).ToList();
            var userAfter = userEntitiesAfter.Single();
            Assert.Equal(user.Id, userAfter.UserId);
        }

        [Fact]
        public async Task BudgetLibraryInDbWithUser_GetUsers_Gets()
        {
            var libraryName = RandomStrings.WithPrefix("BudgetLibrary");
            var budgetLibrary = BudgetTestSetup.ModelForEntityInDb(TestHelper.UnitOfWork, libraryName, false);
            var user = await UserTestSetup.ModelForEntityInDb(TestHelper.UnitOfWork);
            BudgetLibraryUserTestSetup.SetUsersOfBudgetLibrary(TestHelper.UnitOfWork, budgetLibrary.Id, DTOs.Enums.LibraryAccessLevel.Read, user.Id);

            var users = TestHelper.UnitOfWork.BudgetRepo.GetLibraryUsers(budgetLibrary.Id);

            var actualUser = users.Single();
            var expected = new LibraryUserDTO
            {
                AccessLevel = DTOs.Enums.LibraryAccessLevel.Read,
                UserId = user.Id,
            };
            ObjectAssertions.Equivalent(expected, actualUser);
        }

        [Fact]
        public async Task BudgetLibraryInDbWithUser_UpsertOrDeleteUsers_UserNotInList_Removes()
        {
            var libraryName = RandomStrings.WithPrefix("BudgetLibrary");
            var budgetLibrary = BudgetTestSetup.ModelForEntityInDb(TestHelper.UnitOfWork, libraryName, false);
            var user = await UserTestSetup.ModelForEntityInDb(TestHelper.UnitOfWork);
            BudgetLibraryUserTestSetup.SetUsersOfBudgetLibrary(TestHelper.UnitOfWork, budgetLibrary.Id, DTOs.Enums.LibraryAccessLevel.Read, user.Id);
            var usersBefore = TestHelper.UnitOfWork.BudgetRepo.GetLibraryUsers(budgetLibrary.Id);
            var userBefore = usersBefore.Single();
            Assert.Equal(user.Id, userBefore.UserId);
            budgetLibrary.Users.Clear();

            TestHelper.UnitOfWork.BudgetRepo.UpsertBudgetLibrary(budgetLibrary);

            var usersAfter = TestHelper.UnitOfWork.BudgetRepo.GetLibraryUsers(budgetLibrary.Id);
            Assert.Empty(usersAfter);
        }
    }
}
