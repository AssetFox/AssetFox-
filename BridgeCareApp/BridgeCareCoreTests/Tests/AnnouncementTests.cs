using System.Security.Claims;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Extensions;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.UnitTestsCore.TestUtils;
using BridgeCareCore.Controllers;
using BridgeCareCore.Utils;
using BridgeCareCoreTests.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace BridgeCareCoreTests.Tests
{
    public class AnnouncementTests
    {
        private readonly AnnouncementController _controller;

        public AnnouncementTests()
        {
            var accessor = HttpContextAccessorMocks.Default();
            var hubService = HubServiceMocks.Default();
            _controller = new AnnouncementController(EsecSecurityMocks.Admin, TestHelper.UnitOfWork,
                hubService, accessor);
        }

        public AnnouncementEntity TestAnnouncement(Guid? id = null)
        {
            var resolvedId = id ?? Guid.NewGuid();
            var returnValue = new AnnouncementEntity
            {
                Id = resolvedId,
                Title = "Test Title",
                Content = "Test Content"
            };
            return returnValue;
        }
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
        [Fact]
        public async Task ShouldReturnOkResultOnGet()
        {
            // Act
            var result = await _controller.Announcements();

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task ShouldReturnOkResultOnPost()
        {
            // Act
            var announcement = TestAnnouncement();
            var dto = announcement.ToDto();
            var result = await _controller.UpsertAnnouncement(dto);

            // Assert
            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async Task ShouldReturnOkResultOnDelete()
        {
            // Act
            var result = await _controller.DeleteAnnouncement(Guid.Empty);

            // Assert
            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async Task ShouldGetAllAnnouncements()
        {
            // Arrange
            var announcementId = Guid.NewGuid();
            var announcement = TestAnnouncement(announcementId);
            TestHelper.UnitOfWork.Context.AddEntity(announcement);

            // Act
            var result = await _controller.Announcements();

            // Assert
            var okObjResult = result as OkObjectResult;
            Assert.NotNull(okObjResult.Value);

            var dtos = (List<AnnouncementDTO>)Convert.ChangeType(okObjResult.Value, typeof(List<AnnouncementDTO>));
            Assert.True(dtos.Any());
            Assert.Equal(announcementId, dtos[0].Id);
        }

        [Fact]
        public async Task ShouldAddAnnouncementData()
        {
            // Arrange
            var announcement = TestAnnouncement();
            var dto = announcement.ToDto();

            // Act
            await _controller.UpsertAnnouncement(dto);

            // Assert
            var newDto = TestHelper.UnitOfWork.AnnouncementRepo.Announcements()[0];
            Assert.Equal(dto.Id, newDto.Id);
            Assert.Equal(dto.Title, newDto.Title);
            Assert.Equal(dto.Content, newDto.Content);
            Assert.Equal(dto.CreatedDate, newDto.CreatedDate);
        }

        [Fact]
        public async Task ShouldModifyAnnouncementData()
        {
            // Arrange
            var announcement = TestAnnouncement();
            TestHelper.UnitOfWork.Context.AddEntity(announcement);
            var getResult = await _controller.Announcements();
            var dtos = (List<AnnouncementDTO>)Convert.ChangeType((getResult as OkObjectResult).Value, typeof(List<AnnouncementDTO>));

            var dto = dtos[0];
            dto.Title = "Updated Title";
            dto.Content = "Updated Content";

            // Act
            await _controller.UpsertAnnouncement(dto);

            // Assert
            var modifiedDto = TestHelper.UnitOfWork.AnnouncementRepo.Announcements()[0];
            Assert.Equal(dto.Id, modifiedDto.Id);
            Assert.Equal(dto.Title, modifiedDto.Title);
            Assert.Equal(dto.Content, modifiedDto.Content);
            Assert.Equal(dto.CreatedDate, modifiedDto.CreatedDate);
        }

        [Fact]
        public async Task ShouldDeletePerformanceCurveData()
        {
            // Arrange
            var announcement = TestAnnouncement();
            TestHelper.UnitOfWork.Context.AddEntity(announcement);
            var getResult = await _controller.Announcements();
            var dtos = (List<AnnouncementDTO>)Convert.ChangeType((getResult as OkObjectResult).Value, typeof(List<AnnouncementDTO>));

            // Act
            var result = _controller.DeleteAnnouncement(dtos[0].Id);

            // Assert
            Assert.IsType<OkResult>(result.Result);

            Assert.True(!TestHelper.UnitOfWork.Context.Announcement.Any(_ => _.Id == dtos[0].Id));
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
