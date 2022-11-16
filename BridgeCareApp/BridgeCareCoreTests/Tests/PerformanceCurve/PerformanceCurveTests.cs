using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.DTOs.Enums;
using AppliedResearchAssociates.iAM.UnitTestsCore.Extensions;
using AppliedResearchAssociates.iAM.UnitTestsCore.TestUtils;
using BridgeCareCore.Utils;
using BridgeCareCoreTests.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Moq;
using Xunit;

namespace BridgeCareCoreTests.Tests.PerformanceCurve
{
    public class PerformanceCurveTests
    {
        private IHttpContextAccessor CreateRequestForExceptionTesting(FormFile file = null)
        {
            var httpContext = new DefaultHttpContext();

            FormFileCollection formFileCollection;
            if (file != null)
            {
                formFileCollection = new FormFileCollection { file };
            }
            else
            {
                formFileCollection = new FormFileCollection();
            }

            httpContext.Request.Form = new FormCollection(new Dictionary<string, StringValues>(), formFileCollection);
            var accessor = new Mock<IHttpContextAccessor>();
            accessor.Setup(_ => _.HttpContext).Returns(httpContext);
            return accessor.Object;
        }
        [Fact]
        public async Task DeletePerformanceCurveLibrary_OwnerButNotAdminUser_Ok()
        {
            var user = UserDtos.Dbe();
            var libraryId = Guid.NewGuid();
            var performanceCurveRepo = PerformanceCurveRepositoryMocks.New();
            performanceCurveRepo.SetupGetLibraryAccess(libraryId, user.Id, LibraryAccessLevel.Owner);
            var unitOfWork = UnitOfWorkMocks.WithCurrentUser(user);
            unitOfWork.SetupPerformanceCurveRepo(performanceCurveRepo);
            var controller = PerformanceCurveControllerTestSetup.CreateNonAdminController(unitOfWork);

            var result = await controller.DeletePerformanceCurveLibrary(libraryId);

            ActionResultAssertions.Ok(result);
            var calls = performanceCurveRepo.InvocationsWithName(nameof(IPerformanceCurveRepository.DeletePerformanceCurveLibrary));
            Assert.Single(calls);
        }
        [Fact]
        public async Task DeletePerformanceCurveLibrary_ModifyPermissionButNotAdminUser_DoesNotDelete()
        {
            var user = UserDtos.Dbe();
            var libraryId = Guid.NewGuid();
            var performanceCurveRepo = PerformanceCurveRepositoryMocks.New();
            performanceCurveRepo.SetupGetLibraryAccess(libraryId, user.Id, LibraryAccessLevel.Modify);
            var unitOfWork = UnitOfWorkMocks.WithCurrentUser(user);
            unitOfWork.SetupPerformanceCurveRepo(performanceCurveRepo);
            var hubService = HubServiceMocks.DefaultMock();
            var controller = PerformanceCurveControllerTestSetup.CreateNonAdminController(unitOfWork, hubService);

            var result = await controller.DeletePerformanceCurveLibrary(libraryId);

            ActionResultAssertions.Ok(result);
            var calls = performanceCurveRepo.InvocationsWithName(nameof(IPerformanceCurveRepository.DeletePerformanceCurveLibrary));
            Assert.Empty(calls);
            var messages = hubService.ThreeArgumentUserMessages();
            var errorMessage = messages.Single();
            Assert.Contains(ClaimHelper.LibraryDeleteUnauthorizedMessageNew, errorMessage);
        }

        [Fact]
        public async Task GetUsersOfLibrary_RequesterIsOwner_Gets()
        {
            var user1 = UserDtos.Dbe();
            var unitOfWork = UnitOfWorkMocks.WithCurrentUser(user1);
            var performanceCurveRepo = PerformanceCurveRepositoryMocks.New();
            var performanceCurveLibraryId = Guid.NewGuid();
            var ownerDto = new LibraryUserDTO
            {
                UserId = user1.Id,
                AccessLevel = LibraryAccessLevel.Owner,
            };
            var userDtos = new List<LibraryUserDTO> { ownerDto };
            performanceCurveRepo.Setup(br => br.GetLibraryUsers(performanceCurveLibraryId)).Returns(userDtos);
            var performanceCurveLibraryDto = new PerformanceCurveLibraryDTO { Id = performanceCurveLibraryId };
            var performacneCurveLibraryDtos = new List<PerformanceCurveLibraryDTO> { performanceCurveLibraryDto };
            performanceCurveRepo.SetupGetLibraryAccess(performanceCurveLibraryId, user1.Id, LibraryAccessLevel.Owner);
            var controller = PerformanceCurveControllerTestSetup.CreateNonAdminController(unitOfWork);
            unitOfWork.SetupPerformanceCurveRepo(performanceCurveRepo);

            var result = await controller.GetPerformanceCurveLibraryUsers(performanceCurveLibraryId);

            ActionResultAssertions.OkObject(result);
            var value = (result as OkObjectResult).Value;
            Assert.Equal(userDtos, value);
        }
        [Fact]
        public async Task GetUsersOfLibrary_RequesterIsAdmin_Gets()
        {
            var user1 = UserDtos.Admin();
            var user2 = UserDtos.Dbe();
            var unitOfWork = UnitOfWorkMocks.WithCurrentUser(user1);
            var performanceCurveRepo = PerformanceCurveRepositoryMocks.New();
            var performanceCurveLibraryId = Guid.NewGuid();
            var ownerDto = new LibraryUserDTO
            {
                UserId = user2.Id,
                AccessLevel = LibraryAccessLevel.Owner,
            };
            var userDtos = new List<LibraryUserDTO> { ownerDto };
            performanceCurveRepo.Setup(br => br.GetLibraryUsers(performanceCurveLibraryId)).Returns(userDtos);
            var performanceCurveLibraryDto = new PerformanceCurveLibraryDTO { Id = performanceCurveLibraryId };
            var performanceCurveLibraryDtos = new List<PerformanceCurveLibraryDTO> { performanceCurveLibraryDto };
            performanceCurveRepo.SetupGetLibraryAccess(performanceCurveLibraryId, user1.Id, LibraryAccessLevel.Read);
            var controller = PerformanceCurveControllerTestSetup.CreateAdminController(unitOfWork);
            unitOfWork.SetupPerformanceCurveRepo(performanceCurveRepo);

            var result = await controller.GetPerformanceCurveLibraryUsers(performanceCurveLibraryId);

            ActionResultAssertions.OkObject(result);
            var value = (result as OkObjectResult).Value;
            Assert.Equal(userDtos, value);
        }
        [Fact]
        public async Task GetUsersOfLibrary_RequesterIsNeitherAdminNorOwner_DoesNotGet()
        {
            var user1 = UserDtos.Dbe();
            var user2 = UserDtos.Dbe();
            var unitOfWork = UnitOfWorkMocks.WithCurrentUser(user1);
            var performanceCurveRepo = PerformanceCurveRepositoryMocks.New();
            var performanceCurveLibraryId = Guid.NewGuid();
            var ownerDto = new LibraryUserDTO
            {
                UserId = user2.Id,
                AccessLevel = LibraryAccessLevel.Owner,
            };
            var userDtos = new List<LibraryUserDTO> { ownerDto };
            performanceCurveRepo.Setup(br => br.GetLibraryUsers(performanceCurveLibraryId)).Returns(userDtos);
            var performanceCurveLibraryDto = new PerformanceCurveLibraryDTO { Id = performanceCurveLibraryId };
            var performanceCurveLibraryDtos = new List<PerformanceCurveLibraryDTO> { performanceCurveLibraryDto };
            performanceCurveRepo.SetupGetLibraryAccess(performanceCurveLibraryId, user1.Id, LibraryAccessLevel.Read);
            var hubService = HubServiceMocks.DefaultMock();
            var controller = PerformanceCurveControllerTestSetup.CreateNonAdminController(unitOfWork, hubService);
            unitOfWork.SetupPerformanceCurveRepo(performanceCurveRepo);

            var result = await controller.GetPerformanceCurveLibraryUsers(performanceCurveLibraryId);

            ActionResultAssertions.Ok(result);
            var message = hubService.SingleThreeArgumentUserMessage();
            Assert.Contains(ClaimHelper.LibraryUserListGetUnauthorizedMessageNew, message);
        }
    }
}
