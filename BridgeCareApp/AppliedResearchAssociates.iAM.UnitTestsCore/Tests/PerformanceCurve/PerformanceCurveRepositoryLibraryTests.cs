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
using AppliedResearchAssociates.iAM.TestHelpers.Assertions;
using AppliedResearchAssociates.iAM.DataPersistenceCore;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests
{
    public class PerformanceCurveRepositoryLibraryTests
    {
        [Fact]
        public void GetPerformanceCurvesForLibrary_LibraryInDbWithCurves_GetsCurves()
        {
            AttributeTestSetup.CreateAttributes(TestHelper.UnitOfWork);
            var performanceCurveLibraryId = Guid.NewGuid();
            var performanceCurveId = Guid.NewGuid();
            var testLibrary = PerformanceCurveLibraryTestSetup.TestPerformanceCurveLibraryInDb(TestHelper.UnitOfWork, performanceCurveLibraryId);
            var curveDto = PerformanceCurveTestSetup.TestLibraryPerformanceCurveInDb(TestHelper.UnitOfWork, performanceCurveLibraryId, performanceCurveId, TestAttributeNames.ActionType);
            var criterionLibrary = CriterionLibraryTestSetup.TestCriterionLibraryInDb(TestHelper.UnitOfWork);
            var dtos = TestHelper.UnitOfWork.PerformanceCurveRepo.GetPerformanceCurveLibraries();

            var performanceCurveLibraryDTO = dtos.Single(dto => dto.Id == performanceCurveLibraryId);
            curveDto.CriterionLibrary = criterionLibrary;
            var equationInDb = TestHelper.UnitOfWork.Context.Equation
                .SingleOrDefault(e => e.PerformanceCurveEquationJoin.PerformanceCurve.PerformanceCurveLibraryId == performanceCurveLibraryId);
            Assert.NotNull(equationInDb);
            TestHelper.UnitOfWork.Context.ChangeTracker.Clear();

            // Act
            var curves = TestHelper.UnitOfWork.PerformanceCurveRepo.GetPerformanceCurvesForLibrary(performanceCurveLibraryId);
            var curveAfter = curves.Single();
            ObjectAssertions.EquivalentExcluding(curveDto, curveAfter, x => x.CriterionLibrary);
        }

        [Fact]
        public void GetPerformanceCurvesForLibraryOrderedById_LibraryInDbWithCurves_OrderingIsConsistent()
        {
            // sql server ordering is different from C# ordering. So a test that the guids are
            // in order according to C# will fail. Hence we don't test that.
            AttributeTestSetup.CreateAttributes(TestHelper.UnitOfWork);
            var performanceCurveLibraryId = Guid.NewGuid();
            var performanceCurveId1 = Guid.NewGuid();
            var performanceCurveId2 = Guid.NewGuid();
            var testLibrary = PerformanceCurveLibraryTestSetup.TestPerformanceCurveLibraryInDb(TestHelper.UnitOfWork, performanceCurveLibraryId);
            var curveDto1 = PerformanceCurveDtos.Dto(performanceCurveId1, attribute: TestAttributeNames.DeckDurationN);
            var curveDto2 = PerformanceCurveDtos.Dto(performanceCurveId2, attribute: TestAttributeNames.DeckDurationN);
            testLibrary.PerformanceCurves = new List<PerformanceCurveDTO> { curveDto1, curveDto2 };
            TestHelper.UnitOfWork.PerformanceCurveRepo.UpsertOrDeletePerformanceCurveLibraryAndCurves(testLibrary, false, Guid.Empty);

            var curves1 = TestHelper.UnitOfWork.PerformanceCurveRepo.GetPerformanceCurvesForLibraryOrderedById(performanceCurveLibraryId);
            var curves2 = TestHelper.UnitOfWork.PerformanceCurveRepo.GetPerformanceCurvesForLibraryOrderedById(performanceCurveLibraryId);

            ObjectAssertions.Equivalent(curves1[0], curves2[0]);
            ObjectAssertions.Equivalent(curves1[1], curves2[1]);
        }

        [Fact]
        public async Task GetPerformanceCurveLibrariesNoChildrenAccessibleToUser_LibraryAccessibleToUser_Gets()
        {
            var user = await UserTestSetup.ModelForEntityInDb(TestHelper.UnitOfWork);
            var performanceCurveLibraryId = Guid.NewGuid();
            var testLibrary = PerformanceCurveLibraryTestSetup.TestPerformanceCurveLibraryInDb(TestHelper.UnitOfWork, performanceCurveLibraryId);
            PerformanceCurveLibraryUserTestSetup.SetUsersOfPerformanceCurveLibrary(TestHelper.UnitOfWork, performanceCurveLibraryId, LibraryAccessLevel.Modify, user.Id);

            var librariesAccessibleToUser = TestHelper.UnitOfWork.PerformanceCurveRepo.GetPerformanceCurveLibrariesNoChildrenAccessibleToUser(user.Id);

            Assert.Contains(librariesAccessibleToUser, l => l.Id == testLibrary.Id);
        }

        [Fact]
        public async Task GetPerformanceCurveLibrariesNoChildrenAccessibleToUser_LibraryIsNotAccessibleToUser_DoesNotGet()
        {
            var user = await UserTestSetup.ModelForEntityInDb(TestHelper.UnitOfWork);
            var performanceCurveLibraryId = Guid.NewGuid();
            var testLibrary = PerformanceCurveLibraryTestSetup.TestPerformanceCurveLibraryInDb(TestHelper.UnitOfWork, performanceCurveLibraryId);

            var librariesAccessibleToUser = TestHelper.UnitOfWork.PerformanceCurveRepo.GetPerformanceCurveLibrariesNoChildrenAccessibleToUser(user.Id);

            Assert.DoesNotContain(librariesAccessibleToUser, l => l.Id == testLibrary.Id);
        }

        [Fact]
        public async Task GetLibraryAccess_LibraryIsNotAccessibleToUser_AccessFieldIsNull()
        {
            var user = await UserTestSetup.ModelForEntityInDb(TestHelper.UnitOfWork);
            var libraryId = Guid.NewGuid();
            var testLibrary = PerformanceCurveLibraryTestSetup.TestPerformanceCurveLibraryInDb(TestHelper.UnitOfWork, libraryId);

            var libraryAccess = TestHelper.UnitOfWork.PerformanceCurveRepo.GetLibraryAccess(libraryId, user.Id);

            var expected = new LibraryUserAccessModel
            {
                Access = null,
                LibraryExists = true,
                UserId = user.Id,
            };
            ObjectAssertions.Equivalent(expected, libraryAccess);
        }
        [Fact]
        public async Task GetLibraryAccess_LibraryIsAccessibleToUser_Expected()
        {
            var user = await UserTestSetup.ModelForEntityInDb(TestHelper.UnitOfWork);
            var libraryId = Guid.NewGuid();
            var testLibrary = PerformanceCurveLibraryTestSetup.TestPerformanceCurveLibraryInDb(TestHelper.UnitOfWork, libraryId);
            PerformanceCurveLibraryUserTestSetup.SetUsersOfPerformanceCurveLibrary(TestHelper.UnitOfWork, libraryId, LibraryAccessLevel.Modify, user.Id);

            var libraryAccess = TestHelper.UnitOfWork.PerformanceCurveRepo.GetLibraryAccess(libraryId, user.Id);

            var expected = new LibraryUserAccessModel
            {
                Access = new LibraryUserDTO
                {
                    AccessLevel = LibraryAccessLevel.Modify,
                    UserId = user.Id,
                    UserName = user.Username,
                },
                LibraryExists = true,
                UserId = user.Id,
            };
            ObjectAssertions.Equivalent(expected, libraryAccess);
        }

        [Fact]
        public void Delete_PerformanceCurveLibraryExistsWithCurveAndEquation_DeletesAll()
        {
            AttributeTestSetup.CreateAttributes(TestHelper.UnitOfWork);
            var performanceCurveLibraryId = Guid.NewGuid();
            var performanceCurveId = Guid.NewGuid();
            var testLibrary = PerformanceCurveLibraryTestSetup.TestPerformanceCurveLibraryInDb(TestHelper.UnitOfWork, performanceCurveLibraryId);
            var curveDto = PerformanceCurveTestSetup.TestLibraryPerformanceCurveInDb(TestHelper.UnitOfWork, performanceCurveLibraryId, performanceCurveId, TestAttributeNames.ActionType);
            var criterionLibrary = CriterionLibraryTestSetup.TestCriterionLibraryInDb(TestHelper.UnitOfWork);
            var dtos = TestHelper.UnitOfWork.PerformanceCurveRepo.GetPerformanceCurveLibraries();

            var performanceCurveLibraryDTO = dtos.Single(dto => dto.Id == performanceCurveLibraryId);
            curveDto.CriterionLibrary = criterionLibrary;
            var equationInDb = TestHelper.UnitOfWork.Context.Equation
                .SingleOrDefault(e => e.PerformanceCurveEquationJoin.PerformanceCurve.PerformanceCurveLibraryId == performanceCurveLibraryId);
            Assert.NotNull(equationInDb);
            TestHelper.UnitOfWork.Context.ChangeTracker.Clear();

            // Act
            TestHelper.UnitOfWork.PerformanceCurveRepo.DeletePerformanceCurveLibrary(performanceCurveLibraryId);

            Assert.False(TestHelper.UnitOfWork.Context.PerformanceCurveLibrary.Any(_ => _.Id == performanceCurveLibraryId));
            Assert.False(TestHelper.UnitOfWork.Context.PerformanceCurve.Any(_ => _.Id == performanceCurveId));
            Assert.False(
                TestHelper.UnitOfWork.Context.CriterionLibraryPerformanceCurve.Any(_ =>
                    _.PerformanceCurveId == performanceCurveId));
            Assert.False(
                TestHelper.UnitOfWork.Context.PerformanceCurveEquation.Any(_ =>
                    _.PerformanceCurveId == performanceCurveId));
            var equationInDbAfter = TestHelper.UnitOfWork.Context.Equation
                .SingleOrDefault(e => e.PerformanceCurveEquationJoin.PerformanceCurve.PerformanceCurveLibraryId == performanceCurveLibraryId);
            Assert.Null(equationInDbAfter);
        }

        [Fact]
        public void GetPerformanceCurveLibrariesNoPerformanceCurves_Does()
        {
            AttributeTestSetup.CreateAttributes(TestHelper.UnitOfWork);
            NetworkTestSetup.CreateNetwork(TestHelper.UnitOfWork);
            var libraryId = Guid.NewGuid();
            var library = PerformanceCurveLibraryTestSetup.TestPerformanceCurveLibraryInDb(TestHelper.UnitOfWork, libraryId);
            var attribute = TestAttributeNames.CulvDurationN;
            var curveId = Guid.NewGuid();
            PerformanceCurveTestSetup.TestLibraryPerformanceCurveInDb(TestHelper.UnitOfWork, libraryId, curveId, attribute);

            var libraries = TestHelper.UnitOfWork.PerformanceCurveRepo.GetPerformanceCurveLibrariesNoPerformanceCurves();

            var relevantLibrary = libraries.Single(l => l.Id == libraryId);
            Assert.Empty(relevantLibrary.PerformanceCurves);
            var relevantLibraryWithChildren = TestHelper.UnitOfWork.PerformanceCurveRepo.GetPerformanceCurveLibrary(libraryId);
            Assert.NotEmpty(relevantLibraryWithChildren.PerformanceCurves);
        }


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
        public void UpsertPerformanceCurveLibrary_Does()
        {
            NetworkTestSetup.CreateNetwork(TestHelper.UnitOfWork);
            AttributeTestSetup.CreateAttributes(TestHelper.UnitOfWork);
            var libraryId = Guid.NewGuid();
            var library = PerformanceCurveLibraryDtos.Empty(libraryId);

            TestHelper.UnitOfWork.PerformanceCurveRepo.UpsertPerformanceCurveLibrary(library);

            var libraryInDb = TestHelper.UnitOfWork.PerformanceCurveRepo.GetPerformanceCurveLibrary(libraryId);
            Assert.NotNull(libraryInDb);
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

        [Fact]
        public async Task GetLibraryModifiedDate_LibraryInDb_Gets()
        {
            var user = await UserTestSetup.ModelForEntityInDb(TestHelper.UnitOfWork);
            var performanceCurveLibraryId = Guid.NewGuid();
            var beforeDate = DateTime.Now;
            var testLibrary = PerformanceCurveLibraryTestSetup.TestPerformanceCurveLibraryInDb(TestHelper.UnitOfWork, performanceCurveLibraryId);
            var afterDate = DateTime.Now;

            var modifiedDate = TestHelper.UnitOfWork.PerformanceCurveRepo.GetLibraryModifiedDate(performanceCurveLibraryId);

            DateTimeAssertions.Between(beforeDate, afterDate, modifiedDate, TimeSpan.FromSeconds(1));
        }
    }
}
