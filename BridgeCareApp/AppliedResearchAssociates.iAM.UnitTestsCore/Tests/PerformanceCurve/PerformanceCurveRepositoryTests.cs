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
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests.Repositories;
using System.Data;
using AppliedResearchAssociates.iAM.TestHelpers;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests.PerformanceCurve
{
    public class PerformanceCurveRepositoryTests
    {
        [Fact]
        public async Task UpdateLibraryUsers_AddAccessForUser_Does()
        {
            var user1 = await UserTestSetup.ModelForEntityInDb(TestHelper.UnitOfWork);
            var user2 = await UserTestSetup.ModelForEntityInDb(TestHelper.UnitOfWork);
            var library = PerformanceCurveLibraryTestSetup.ModelForEntityInDb(TestHelper.UnitOfWork);
            PerformanceCurveLibraryUserTestSetup.SetUsersOfPerformanceCurveLibrary(TestHelper.UnitOfWork, library.Id, LibraryAccessLevel.Modify, user1.Id);
            var usersBefore = TestHelper.UnitOfWork.PerformanceCurveRepo.GetLibraryUsers(library.Id);
            var newUser = new LibraryUserDTO
            {
                AccessLevel = LibraryAccessLevel.Read,
                UserId = user2.Id,
            };
            usersBefore.Add(newUser);

            TestHelper.UnitOfWork.PerformanceCurveRepo.UpsertOrDeleteUsers(library.Id, usersBefore);

            var libraryUsersAfter = TestHelper.UnitOfWork.PerformanceCurveRepo.GetLibraryUsers(library.Id);
            var user1After = libraryUsersAfter.Single(u => u.UserId == user1.Id);
            var user2After = libraryUsersAfter.Single(u => u.UserId == user2.Id);
            Assert.Equal(LibraryAccessLevel.Modify, user1After.AccessLevel);
            Assert.Equal(LibraryAccessLevel.Read, user2After.AccessLevel);
        }

        [Fact]
        public async Task UpdatePerformanceCurveLibraryUsers_RequestAccessRemoval_Does()
        {
            var user = await UserTestSetup.ModelForEntityInDb(TestHelper.UnitOfWork);
            var library = PerformanceCurveLibraryTestSetup.ModelForEntityInDb(TestHelper.UnitOfWork);
            PerformanceCurveLibraryUserTestSetup.SetUsersOfPerformanceCurveLibrary(TestHelper.UnitOfWork, library.Id, LibraryAccessLevel.Modify, user.Id);
            var libraryUsersBefore = TestHelper.UnitOfWork.PerformanceCurveRepo.GetLibraryUsers(library.Id);
            var libraryUserBefore = libraryUsersBefore.Single();
            libraryUsersBefore.Remove(libraryUserBefore);

            TestHelper.UnitOfWork.PerformanceCurveRepo.UpsertOrDeleteUsers(library.Id, libraryUsersBefore);
            TestHelper.UnitOfWork.Context.SaveChanges();

            var libraryUsersAfter = TestHelper.UnitOfWork.PerformanceCurveRepo.GetLibraryUsers(library.Id);
            Assert.Empty(libraryUsersAfter);
        }

        [Fact]
        public async Task UpdatePerformanceCurveLibraryWithUserAccessChange_Does()
        {
            var user = await UserTestSetup.ModelForEntityInDb(TestHelper.UnitOfWork);
            var library = PerformanceCurveLibraryTestSetup.ModelForEntityInDb(TestHelper.UnitOfWork);
            PerformanceCurveLibraryUserTestSetup.SetUsersOfPerformanceCurveLibrary(TestHelper.UnitOfWork, library.Id, LibraryAccessLevel.Modify, user.Id);
            var libraryUsersBefore = TestHelper.UnitOfWork.PerformanceCurveRepo.GetLibraryUsers(library.Id);
            var libraryUserBefore = libraryUsersBefore.Single();
            Assert.Equal(LibraryAccessLevel.Modify, libraryUserBefore.AccessLevel);
            libraryUserBefore.AccessLevel = LibraryAccessLevel.Read;

            TestHelper.UnitOfWork.PerformanceCurveRepo.UpsertOrDeleteUsers(library.Id, libraryUsersBefore);

            var libraryUsersAfter = TestHelper.UnitOfWork.PerformanceCurveRepo.GetLibraryUsers(library.Id);
            var libraryUserAfter = libraryUsersAfter.Single();
            Assert.Equal(LibraryAccessLevel.Read, libraryUserAfter.AccessLevel);
        }

        [Fact]
        public void UpsertPerformanceCurveLibrary_CurveUpsertThrows_LibraryIsNotChanged()
        {
            NetworkTestSetup.CreateNetwork(TestHelper.UnitOfWork);
            AttributeTestSetup.CreateAttributes(TestHelper.UnitOfWork);
            var attributeName = TestAttributeNames.CulvDurationN;
            var libraryId = Guid.NewGuid();
            var library = PerformanceCurveLibraryDtos.Empty(libraryId);
            TestHelper.UnitOfWork.PerformanceCurveRepo.UpsertPerformanceCurveLibrary(library);
            var curveId = Guid.NewGuid();
            var criterionLibraryId = Guid.NewGuid();
            var curve = PerformanceCurveDtos.Dto(curveId, criterionLibraryId, attributeName);
            var curves = new List<PerformanceCurveDTO> { curve };
            TestHelper.UnitOfWork.PerformanceCurveRepo.UpsertOrDeletePerformanceCurves(curves, libraryId);
            var updateLibrary = PerformanceCurveLibraryDtos.Empty(libraryId);
            updateLibrary.Description = "Updated description";
            var updateCurve = PerformanceCurveDtos.Dto(curveId, criterionLibraryId, "AttributeDoesNotExist");
            updateLibrary.PerformanceCurves = new List<PerformanceCurveDTO> { updateCurve };
            var libraryBefore = TestHelper.UnitOfWork.PerformanceCurveRepo.GetPerformanceCurveLibrary(libraryId);

            var exception = Assert.Throws<RowNotInTableException>(() => TestHelper.UnitOfWork.PerformanceCurveRepo
            .UpsertOrDeletePerformanceCurveLibraryAndCurves(updateLibrary, false, Guid.Empty));

            var libraryAfter = TestHelper.UnitOfWork.PerformanceCurveRepo.GetPerformanceCurveLibrary(libraryId);
            ObjectAssertions.Equivalent(libraryBefore, libraryAfter);
        }
    }
}
