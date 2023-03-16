using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.TestHelpers;
using AppliedResearchAssociates.iAM.UnitTestsCore.TestUtils;
using Xunit;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests.User
{
    public class UserCriteriaRepositoryTests
    {
        [Fact]
        public async Task GetOwnUserCriteria_UserExists_Expected()
        {
            var user = await UserTestSetup.ModelForEntityInDb(TestHelper.UnitOfWork, true);
            var userInfo = new UserInfoDTO
            {
                Sub = user.Username,
                HasAdminAccess = true,
            };

            var userCriteria = TestHelper.UnitOfWork.UserCriteriaRepo.GetOwnUserCriteria(userInfo);
            var expected = new UserCriteriaDTO
            {
                HasAccess = true,
                HasCriteria = false,
                UserId = user.Id,
                UserName = user.Username,
            };
            ObjectAssertions.EquivalentExcluding(expected, userCriteria, x => x.CriteriaId);
        }

        [Fact]
        public async Task GetOwnUserCriteria_UserDoesNotExist_Expected()
        {
            var username = RandomStrings.WithPrefix("User");
            var userInfo = new UserInfoDTO
            {
                Sub = username,
                HasAdminAccess = true,
            };

            var userCriteria = TestHelper.UnitOfWork.UserCriteriaRepo.GetOwnUserCriteria(userInfo);

            var expected = new UserCriteriaDTO
            {
                HasAccess = true,
                HasCriteria = false,
                UserName = username,
            };
            ObjectAssertions.EquivalentExcluding(expected, userCriteria, x => x.CriteriaId, x => x.UserId);
            var userInDb = await TestHelper.UnitOfWork.UserRepo.GetUserById(userCriteria.UserId);
            Assert.NotNull(userInDb);
        }

        [Fact]
        public void UpsertUserCriteria_UserDoesNotExist_Throws()
        {
            var userId = Guid.NewGuid();
            var dto = new UserCriteriaDTO
            {
                UserId = userId,
            };

            var exception = Assert.Throws<RowNotInTableException>(() => TestHelper.UnitOfWork.UserCriteriaRepo
                .UpsertUserCriteria(dto));

            Assert.Equal(UserCriteriaRepository.TheUserWasNotFound, exception.Message);
        }


        [Fact]
        public async void UpsertUserCriteria_ExistsButCriteriaDont_Adds()
        {
            var user = await UserTestSetup.ModelForEntityInDb(TestHelper.UnitOfWork, false);
            var userId = user.Id;
            var dto = new UserCriteriaDTO
            {
                UserId = userId,
            };
            var filterBefore = TestHelper.UnitOfWork.Context.UserCriteria.SingleOrDefault(uc => uc.UserId == userId);
            Assert.Null(filterBefore);

            TestHelper.UnitOfWork.UserCriteriaRepo
                .UpsertUserCriteria(dto);

            var filter = TestHelper.UnitOfWork.Context.UserCriteria.Single(uc => uc.UserId == userId);
        }
    }
}
