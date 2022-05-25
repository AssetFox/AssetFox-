using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Extensions;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.UnitTestsCore.TestUtils;
using BridgeCareCore.Controllers;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests.APITestClasses
{
    public class AnnouncementTests
    {
        private readonly TestHelper _testHelper;
        private readonly AnnouncementController _controller;

        public AnnouncementTests()
        {
            _testHelper = TestHelper.Instance;
            _testHelper.SetupDefaultHttpContext();
            _controller = new AnnouncementController(_testHelper.MockEsecSecurityAuthorized.Object, _testHelper.UnitOfWork,
                _testHelper.MockHubService.Object, _testHelper.MockHttpContextAccessor.Object);
        }

        public AnnouncementEntity TestAnnouncement { get; } = new AnnouncementEntity
        {
            Id = Guid.Empty,
            Title = "Test Title",
            Content = "Test Content"
        };

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
            var result = await _controller.UpsertAnnouncement(TestAnnouncement.ToDto());

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
            var AnnouncementId = Guid.NewGuid();
            TestAnnouncement.Id = AnnouncementId;
            _testHelper.UnitOfWork.Context.AddEntity(TestAnnouncement);

            // Act
            var result = await _controller.Announcements();

            // Assert
            var okObjResult = result as OkObjectResult;
            Assert.NotNull(okObjResult.Value);

            var dtos = (List<AnnouncementDTO>)Convert.ChangeType(okObjResult.Value, typeof(List<AnnouncementDTO>));
            Assert.True(dtos.Any());
            Assert.Equal(AnnouncementId, dtos[0].Id);
        }

        [Fact]
        public async Task ShouldAddAnnouncementData()
        {
            // Arrange
            TestAnnouncement.Id = Guid.NewGuid();
            var dto = TestAnnouncement.ToDto();

            // Act
            await _controller.UpsertAnnouncement(dto);

            // Assert
            var newDto = _testHelper.UnitOfWork.AnnouncementRepo.Announcements()[0];
            Assert.Equal(dto.Id, newDto.Id);
            Assert.Equal(dto.Title, newDto.Title);
            Assert.Equal(dto.Content, newDto.Content);
            Assert.Equal(dto.CreatedDate, newDto.CreatedDate);
        }

        [Fact]
        public async Task ShouldModifyAnnouncementData()
        {
            // Arrange                
            TestAnnouncement.Id = Guid.NewGuid();
            _testHelper.UnitOfWork.Context.AddEntity(TestAnnouncement);
            var getResult = await _controller.Announcements();
            var dtos = (List<AnnouncementDTO>)Convert.ChangeType((getResult as OkObjectResult).Value, typeof(List<AnnouncementDTO>));

            var dto = dtos[0];
            dto.Title = "Updated Title";
            dto.Content = "Updated Content";

            // Act
            await _controller.UpsertAnnouncement(dto);

            // Assert
            var modifiedDto = _testHelper.UnitOfWork.AnnouncementRepo.Announcements()[0];
            Assert.Equal(dto.Id, modifiedDto.Id);
            Assert.Equal(dto.Title, modifiedDto.Title);
            Assert.Equal(dto.Content, modifiedDto.Content);
            Assert.Equal(dto.CreatedDate, modifiedDto.CreatedDate);
        }

        [Fact]
        public async Task ShouldDeletePerformanceCurveData()
        {
            // Arrange
            TestAnnouncement.Id = Guid.NewGuid();
            _testHelper.UnitOfWork.Context.AddEntity(TestAnnouncement);
            var getResult = await _controller.Announcements();
            var dtos = (List<AnnouncementDTO>)Convert.ChangeType((getResult as OkObjectResult).Value, typeof(List<AnnouncementDTO>));

            // Act
            var result = _controller.DeleteAnnouncement(dtos[0].Id);

            // Assert
            Assert.IsType<OkResult>(result.Result);

            Assert.True(!_testHelper.UnitOfWork.Context.Announcement.Any(_ => _.Id == dtos[0].Id));
        }
    }
}
