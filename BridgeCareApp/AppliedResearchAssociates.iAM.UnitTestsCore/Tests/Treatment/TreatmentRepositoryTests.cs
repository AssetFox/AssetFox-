using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DTOs.Enums;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests.User;
using AppliedResearchAssociates.iAM.UnitTestsCore.TestUtils;
using Xunit;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests.Treatment
{
    public class TreatmentRepositoryTests
    {
        [Fact]
        public async Task UpdateTreatmentLibraryWithUserAccessChange_Does()
        {
            var user = await UserTestSetup.ModelForEntityInDb(TestHelper.UnitOfWork);
            var library = TreatmentLibraryTestSetup.ModelForEntityInDb(TestHelper.UnitOfWork);
            TreatmentLibraryUserTestSetup.SetUsersOfTreatmentLibrary(TestHelper.UnitOfWork, library.Id, LibraryAccessLevel.Modify, user.Id);
            var libraryUsersBefore = TestHelper.UnitOfWork.TreatmentLibraryUserRepo.GetLibraryUsers(library.Id);
            var libraryUserBefore = libraryUsersBefore.Single();
            Assert.Equal(LibraryAccessLevel.Modify, libraryUserBefore.AccessLevel);
            libraryUserBefore.AccessLevel = LibraryAccessLevel.Read;

            TestHelper.UnitOfWork.TreatmentLibraryUserRepo.UpsertOrDeleteUsers(library.Id, libraryUsersBefore);

            var libraryUsersAfter = TestHelper.UnitOfWork.TreatmentLibraryUserRepo.GetLibraryUsers(library.Id);
            var libraryUserAfter = libraryUsersAfter.Single();
            Assert.Equal(LibraryAccessLevel.Read, libraryUserAfter.AccessLevel);
        }
        [Fact]
        public async Task UpdateTreatmentLibraryUsers_RequestAccessRemoval_Does()
        {
            var user = await UserTestSetup.ModelForEntityInDb(TestHelper.UnitOfWork);
            var library = TreatmentLibraryTestSetup.ModelForEntityInDb(TestHelper.UnitOfWork);
            TreatmentLibraryUserTestSetup.SetUsersOfTreatmentLibrary(TestHelper.UnitOfWork, library.Id, LibraryAccessLevel.Modify, user.Id);
            var libraryUsersBefore = TestHelper.UnitOfWork.TreatmentLibraryUserRepo.GetLibraryUsers(library.Id);
            var libraryUserBefore = libraryUsersBefore.Single();
            libraryUsersBefore.Remove(libraryUserBefore);

            TestHelper.UnitOfWork.TreatmentLibraryUserRepo.UpsertOrDeleteUsers(library.Id, libraryUsersBefore);
            TestHelper.UnitOfWork.Context.SaveChanges();

            var libraryUsersAfter = TestHelper.UnitOfWork.TreatmentLibraryUserRepo.GetLibraryUsers(library.Id);
            Assert.Empty(libraryUsersAfter);
        }


        [Fact]
        public async Task UpdateLibraryUsers_AddAccessForUser_Does()
        {
            var user1 = await UserTestSetup.ModelForEntityInDb(TestHelper.UnitOfWork);
            var user2 = await UserTestSetup.ModelForEntityInDb(TestHelper.UnitOfWork);
            var library = TreatmentLibraryTestSetup.ModelForEntityInDb(TestHelper.UnitOfWork);
            TreatmentLibraryUserTestSetup.SetUsersOfTreatmentLibrary(TestHelper.UnitOfWork, library.Id, LibraryAccessLevel.Modify, user1.Id);
            var usersBefore = TestHelper.UnitOfWork.TreatmentLibraryUserRepo.GetLibraryUsers(library.Id);
            var newUser = new LibraryUserDTO
            {
                AccessLevel = LibraryAccessLevel.Read,
                UserId = user2.Id,
            };
            usersBefore.Add(newUser);

            TestHelper.UnitOfWork.TreatmentLibraryUserRepo.UpsertOrDeleteUsers(library.Id, usersBefore);

            var libraryUsersAfter = TestHelper.UnitOfWork.TreatmentLibraryUserRepo.GetLibraryUsers(library.Id);
            var user1After = libraryUsersAfter.Single(u => u.UserId == user1.Id);
            var user2After = libraryUsersAfter.Single(u => u.UserId == user2.Id);
            Assert.Equal(LibraryAccessLevel.Modify, user1After.AccessLevel);
            Assert.Equal(LibraryAccessLevel.Read, user2After.AccessLevel);
        }
    }
}
