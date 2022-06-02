using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
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

        [Fact]
        public async void ShouldReturnOkResultOnGet()
        {
            // Act
            var result = await _controller.Announcements();

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async void ShouldReturnOkResultOnPost()
        {
            // Act
            var announcement = TestAnnouncement();
            var dto = announcement.ToDto();
            var result = await _controller.UpsertAnnouncement(dto);

            // Assert
            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async void ShouldReturnOkResultOnDelete()
        {
            // Act
            var result = await _controller.DeleteAnnouncement(Guid.Empty);

            // Assert
            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async void ShouldGetAllAnnouncements()
        {
            // Arrange
            var announcementId = Guid.NewGuid();
            var announcement = TestAnnouncement(announcementId);
            _testHelper.UnitOfWork.Context.AddEntity(announcement);

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
        public async void ShouldAddAnnouncementData()
        {
            // Arrange
            var announcement = TestAnnouncement();
            var dto = announcement.ToDto();

            // Act
            await _controller.UpsertAnnouncement(dto);

            // Assert
            var timer = new Timer { Interval = 5000 };
            timer.Elapsed += delegate
            {
                var newDto = _testHelper.UnitOfWork.AnnouncementRepo.Announcements()[0];
                Assert.Equal(dto.Id, newDto.Id);
                Assert.Equal(dto.Title, newDto.Title);
                Assert.Equal(dto.Content, newDto.Content);
                Assert.Equal(dto.CreatedDate, newDto.CreatedDate);
            };
        }

        [Fact]
        public async void ShouldModifyAnnouncementData()
        {
            // Arrange
            var announcement = TestAnnouncement();
            _testHelper.UnitOfWork.Context.AddEntity(announcement);
            var getResult = await _controller.Announcements();
            var dtos = (List<AnnouncementDTO>)Convert.ChangeType((getResult as OkObjectResult).Value, typeof(List<AnnouncementDTO>));

            var dto = dtos[0];
            dto.Title = "Updated Title";
            dto.Content = "Updated Content";

            // Act
            await _controller.UpsertAnnouncement(dto);

            // Assert
            var timer = new Timer { Interval = 5000 };
            timer.Elapsed += delegate
            {
                var modifiedDto = _testHelper.UnitOfWork.AnnouncementRepo.Announcements()[0];
                Assert.Equal(dto.Id, modifiedDto.Id);
                Assert.Equal(dto.Title, modifiedDto.Title);
                Assert.Equal(dto.Content, modifiedDto.Content);
                Assert.Equal(dto.CreatedDate, modifiedDto.CreatedDate);
            };
        }

        [Fact]
        public async void ShouldDeletePerformanceCurveData()
        {
            // Arrange
            var announcement = TestAnnouncement();
            _testHelper.UnitOfWork.Context.AddEntity(announcement);
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
