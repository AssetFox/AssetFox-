using System.Security.Claims;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Extensions;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.TestHelpers;
using AppliedResearchAssociates.iAM.UnitTestsCore;
using AppliedResearchAssociates.iAM.UnitTestsCore.Announcement;
using AppliedResearchAssociates.iAM.UnitTestsCore.Extensions;
using AppliedResearchAssociates.iAM.UnitTestsCore.TestUtils;
using BridgeCareCore.Controllers;
using BridgeCareCore.Utils;
using BridgeCareCoreTests.Helpers;
using BridgeCareCoreTests.Tests.Announcement;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace BridgeCareCoreTests.Tests
{
    public class AnnouncementTests
    {
        public AnnouncementController CreateTestController(List<string> userClaims)
        {
            var accessor = HttpContextAccessorMocks.Default();
            var hubService = HubServiceMocks.Default();
            var testUser = ClaimsPrincipals.WithNameClaims(userClaims);
            var controller = new AnnouncementController(EsecSecurityMocks.AdminMock.Object, TestHelper.UnitOfWork,
                hubService, accessor);
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = testUser }
            };
            return controller;
        }

        private AnnouncementController CreateController(Mock<IUnitOfWork> unitOfWork)
        {
            var security = EsecSecurityMocks.AdminMock;
            var hubService = HubServiceMocks.DefaultMock();
            var contextAccessor = HttpContextAccessorMocks.DefaultMock();
            var controller = new AnnouncementController(security.Object,
                unitOfWork.Object,
                hubService.Object,
                contextAccessor.Object);
            return controller;
        }
        [Fact]
        public async Task ShouldReturnOkResultOnGet()
        {
            // Act
            var unitOfWork = UnitOfWorkMocks.New();
            var announcementRepository = AnnouncementRepositoryMocks.New(unitOfWork);
            var userRepo = UserRepositoryMocks.EveryoneExists(unitOfWork);
            var controller = CreateController(unitOfWork);
            var result = await controller.Announcements();

            // Assert
            Assert.IsType<OkObjectResult>(result);
            var _ = announcementRepository.SingleInvocationWithName(nameof(IAnnouncementRepository.Announcements));
        }

        [Fact]
        public async Task ShouldReturnOkResultOnPost()
        {
            // Act
            var dto = AnnouncementDtos.Dto();
            var unitOfWork = UnitOfWorkMocks.New();
            var announcementRepository = AnnouncementRepositoryMocks.New(unitOfWork);
            var userRepo = UserRepositoryMocks.EveryoneExists(unitOfWork);
            var controller = CreateController(unitOfWork);
            var result = await controller.UpsertAnnouncement(dto);

            // Assert
            Assert.IsType<OkResult>(result);
            var invocation = announcementRepository.SingleInvocationWithName(nameof(IAnnouncementRepository.UpsertAnnouncement));
            var argument = invocation.Arguments[0];
            ObjectAssertions.Equivalent(dto, argument);
        }

        [Fact]
        public async Task ShouldReturnOkResultOnDelete()
        {
            // Act

            var unitOfWork = UnitOfWorkMocks.New();
            var announcementRepository = AnnouncementRepositoryMocks.New(unitOfWork);
            var userRepo = UserRepositoryMocks.EveryoneExists(unitOfWork);
            var controller = CreateController(unitOfWork);
            var announcementId = Guid.NewGuid();
            var result = await controller.DeleteAnnouncement(announcementId);

            // Assert
            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async Task ShouldGetAllAnnouncements()
        {
            // Arrange
            var announcementId = Guid.NewGuid();
            var announcement = AnnouncementDtos.Dto(announcementId);
            var announcements = new List<AnnouncementDTO> { announcement };
            var unitOfWork = UnitOfWorkMocks.New();
            var users = UserRepositoryMocks.EveryoneExists(unitOfWork);
            var announcementRepository = AnnouncementRepositoryMocks.New(unitOfWork);
            announcementRepository.Setup(a => a.Announcements()).Returns(announcements);
            var controller = CreateController(unitOfWork);

            // Act
            var result = await controller.Announcements();

            // Assert
            var okObjResult = result as OkObjectResult;
            var dtos = (List<AnnouncementDTO>)Convert.ChangeType(okObjResult.Value, typeof(List<AnnouncementDTO>));
            Assert.True(dtos.Any());
            Assert.Equal(announcementId, dtos[0].Id);
        }

        [Fact]
        public async Task UserIsViewAnnouncementAuthorized()
        {
            // Admin authorized
            // Arrange
            var authorizationService = BuildAuthorizationServiceMocks.BuildAuthorizationService(services =>
            {
                services.AddAuthorization(options =>
                {
                    options.AddPolicy("ViewAnnouncementClaim",
                        policy => policy.RequireClaim(ClaimTypes.Name,
                                                      BridgeCareCore.Security.SecurityConstants.Claim.AnnouncementViewAccess));
                });
            });
            var roleClaimsMapper = new RoleClaimsMapper();
            var controller = CreateTestController(roleClaimsMapper.GetClaims(BridgeCareCore.Security.SecurityConstants.SecurityTypes.Esec, new List<string> { BridgeCareCore.Security.SecurityConstants.Role.Administrator }));
            // Act
            var allowed = await authorizationService.AuthorizeAsync(controller.User, "ViewAnnouncementClaim");
            // Assert
            Assert.True(allowed.Succeeded);
        }
        [Fact]
        public async Task UserIsModifyAnnouncementAuthorized()
        {
            // Non-admin unauthorized
            // Arrange
            var authorizationService = BuildAuthorizationServiceMocks.BuildAuthorizationService(services =>
            {
                services.AddAuthorization(options =>
                {
                    options.AddPolicy("ModifyAnnouncementClaim",
                        policy => policy.RequireClaim(ClaimTypes.Name,
                                                      BridgeCareCore.Security.SecurityConstants.Claim.AnnouncementModifyAccess));
                });
            });
            var roleClaimsMapper = new RoleClaimsMapper();
            var controller = CreateTestController(roleClaimsMapper.GetClaims(BridgeCareCore.Security.SecurityConstants.SecurityTypes.Esec, new List<string> { BridgeCareCore.Security.SecurityConstants.Role.Editor }));
            // Act
            var allowed = await authorizationService.AuthorizeAsync(controller.User, "ModifyAnnouncementClaim");
            // Assert
            Assert.False(allowed.Succeeded);
        }
        [Fact]
        public async Task UserIsViewAnnouncementAuthorized_B2C()
        {
            // Arrange
            var authorizationService = BuildAuthorizationServiceMocks.BuildAuthorizationService(services =>
            {
                services.AddAuthorization(options =>
                {
                    options.AddPolicy("ViewAnnouncementClaim",
                        policy => policy.RequireClaim(ClaimTypes.Name,
                                                      BridgeCareCore.Security.SecurityConstants.Claim.AnnouncementViewAccess));
                });
            });
            var roleClaimsMapper = new RoleClaimsMapper();
            var controller = CreateTestController(roleClaimsMapper.GetClaims(BridgeCareCore.Security.SecurityConstants.SecurityTypes.B2C, new List<string> { BridgeCareCore.Security.SecurityConstants.Role.Administrator }));
            // Act
            var allowed = await authorizationService.AuthorizeAsync(controller.User, "ViewAnnouncementClaim");
            // Assert
            Assert.True(allowed.Succeeded);
        }
    }
}
