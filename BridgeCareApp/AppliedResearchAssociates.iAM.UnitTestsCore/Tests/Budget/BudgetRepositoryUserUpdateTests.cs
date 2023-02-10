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
using AppliedResearchAssociates.iAM.DTOs.Enums;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests
{
    public class BudgetRepositoryUserUpdateTests
    {
        [Fact]
        public async Task BudgetLibraryInDb_AddUsers_Does()
        {
            var libraryName = RandomStrings.WithPrefix("BudgetLibrary");
            var budgetLibrary = BudgetLibraryTestSetup.ModelForEntityInDb(TestHelper.UnitOfWork, libraryName);
            var user = await UserTestSetup.ModelForEntityInDb(TestHelper.UnitOfWork);
            var userDto = new LibraryUserDTO
            {
                AccessLevel = LibraryAccessLevel.Modify,
                UserId = user.Id,
            };
            var userDtos = new List<LibraryUserDTO> { userDto };

            TestHelper.UnitOfWork.BudgetRepo.UpsertOrDeleteUsers(budgetLibrary.Id, userDtos);

            var userEntitiesAfter = TestHelper.UnitOfWork.Context.BudgetLibraryUser.Where(u => u.BudgetLibraryId == budgetLibrary.Id).ToList();
            var userAfter = userEntitiesAfter.Single();
            Assert.Equal(user.Id, userAfter.UserId);
        }

        [Fact]
        public async Task BudgetLibraryInDbWithUser_GetUsers_Gets()
        {
            var libraryName = RandomStrings.WithPrefix("BudgetLibrary");
            var budgetLibrary = BudgetLibraryTestSetup.ModelForEntityInDb(TestHelper.UnitOfWork, libraryName);
            var user = await UserTestSetup.ModelForEntityInDb(TestHelper.UnitOfWork);
            BudgetLibraryUserTestSetup.SetUsersOfBudgetLibrary(TestHelper.UnitOfWork, budgetLibrary.Id, DTOs.Enums.LibraryAccessLevel.Read, user.Id);

            var users = TestHelper.UnitOfWork.BudgetRepo.GetLibraryAccess(budgetLibrary.Id, user.Id);

            var actualUser = users.Access;
            var expected = new LibraryUserDTO
            {
                AccessLevel = LibraryAccessLevel.Read,
                UserId = user.Id,
                UserName = user.Username,
            };
            ObjectAssertions.Equivalent(expected, actualUser);
        }

        [Fact]
        public async Task BudgetLibraryInDbWithUser_UpsertOrDeleteUsers_UserNotInList_Removes()
        {
            var libraryName = RandomStrings.WithPrefix("BudgetLibrary");
            var budgetLibrary = BudgetLibraryTestSetup.ModelForEntityInDb(TestHelper.UnitOfWork, libraryName);
            var user = await UserTestSetup.ModelForEntityInDb(TestHelper.UnitOfWork);
            BudgetLibraryUserTestSetup.SetUsersOfBudgetLibrary(TestHelper.UnitOfWork, budgetLibrary.Id, DTOs.Enums.LibraryAccessLevel.Read, user.Id);
            var usersBefore = TestHelper.UnitOfWork.BudgetRepo.GetLibraryAccess(budgetLibrary.Id, user.Id);
            var userBefore = usersBefore.Access;
            Assert.Equal(user.Id, userBefore.UserId);

            TestHelper.UnitOfWork.BudgetRepo.UpsertOrDeleteUsers(budgetLibrary.Id, new List<LibraryUserDTO>());

            var usersAfter = TestHelper.UnitOfWork.BudgetRepo.GetLibraryAccess(budgetLibrary.Id, user.Id);
            Assert.Null(usersAfter.Access);
        }

        [Fact]
        public async Task CreateBudgetLibraryWithUser_Does()
        {
            var libraryName = RandomStrings.WithPrefix("BudgetLibrary");
            var user = await UserTestSetup.ModelForEntityInDb(TestHelper.UnitOfWork);
            var libraryDto = BudgetLibraryTestSetup.ModelForEntityInDb(TestHelper.UnitOfWork, libraryName);
            var userDto = BudgetLibraryUserTestSetup.CreateLibraryUserDto(user.Id);
            var userDtos = new List<LibraryUserDTO> { userDto };

            TestHelper.UnitOfWork.BudgetRepo.UpsertOrDeleteUsers(libraryDto.Id, userDtos);

            var usersAfter = TestHelper.UnitOfWork.BudgetRepo.GetLibraryAccess(libraryDto.Id, user.Id);
            var accessAfter = usersAfter.Access;
            Assert.Equal(user.Id, accessAfter.UserId);
        }
    }
}
