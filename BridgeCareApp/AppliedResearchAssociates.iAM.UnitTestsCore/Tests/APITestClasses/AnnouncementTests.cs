using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Extensions;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.UnitTestsCore.TestData;
using BridgeCareCore.Controllers;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests.APITestClasses
{
    public class AnnouncementTests
    {
        private readonly TestHelper _testHelper;
        private readonly AnnouncementController _controller;

        private static readonly Guid AnnouncementId = Guid.Parse("afe4ebe4-b5fa-4d94-aaec-70a3f3979e4b");

        public AnnouncementTests()
        {
            _testHelper = new TestHelper();
            _testHelper.SetupDefaultHttpContext();
            _controller = new AnnouncementController(_testHelper.MockEsecSecurityAuthorized.Object, _testHelper.UnitOfWork,
                _testHelper.MockHubService.Object, _testHelper.MockHttpContextAccessor.Object);
        }

        public AnnouncementEntity TestAnnouncement { get; } = new AnnouncementEntity
        {
            Id = AnnouncementId,
            Title = "Test Title",
            Content = "Test Content"
        };

        [Fact]
        public async void ShouldReturnOkResultOnGet()
        {
            try
            {
                // Act
                var result = await _controller.Announcements();

                // Assert
                Assert.IsType<OkObjectResult>(result);
            }
            finally
            {
                // Cleanup
                _testHelper.CleanUp();
            }
        }

        [Fact]
        public async void ShouldReturnOkResultOnPost()
        {
            try
            {
                // Act
                var result = await _controller.UpsertAnnouncement(TestAnnouncement.ToDto());

                // Assert
                Assert.IsType<OkResult>(result);
            }
            finally
            {
                // Cleanup
                _testHelper.CleanUp();
            }
        }

        [Fact]
        public async void ShouldReturnOkResultOnDelete()
        {
            try
            {
                // Act
                var result = await _controller.DeleteAnnouncement(Guid.Empty);

                // Assert
                Assert.IsType<OkResult>(result);
            }
            finally
            {
                // Cleanup
                _testHelper.CleanUp();
            }
        }

        [Fact]
        public async void ShouldGetAllAnnouncements()
        {
            try
            {
                // Arrange
                _testHelper.UnitOfWork.Context.AddEntity(TestAnnouncement);

                // Act
                var result = await _controller.Announcements();

                // Assert
                var okObjResult = result as OkObjectResult;
                Assert.NotNull(okObjResult.Value);

                var dtos = (List<AnnouncementDTO>)Convert.ChangeType(okObjResult.Value, typeof(List<AnnouncementDTO>));
                Assert.Single(dtos);

                Assert.Equal(AnnouncementId, dtos[0].Id);
            }
            finally
            {
                // Cleanup
                _testHelper.CleanUp();
            }
        }

        [Fact]
        public async void ShouldAddAnnouncementData()
        {
            try
            {
                // Arrange
                var dto = TestAnnouncement.ToDto();

                // Act
                await _controller.UpsertAnnouncement(dto);

                // Assert
                var timer = new Timer {Interval = 5000};
                timer.Elapsed += delegate
                {
                    var newDto = _testHelper.UnitOfWork.AnnouncementRepo.Announcements()[0];
                    Assert.Equal(dto.Id, newDto.Id);
                    Assert.Equal(dto.Title, newDto.Title);
                    Assert.Equal(dto.Content, newDto.Content);
                    Assert.Equal(dto.CreatedDate, newDto.CreatedDate);
                };
            }
            finally
            {
                // Cleanup
                _testHelper.CleanUp();
            }
        }

        [Fact]
        public async void ShouldModifyAnnouncementData()
        {
            try
            {
                // Arrange
                _testHelper.UnitOfWork.Context.AddEntity(TestAnnouncement);
                var getResult = await _controller.Announcements();
                var dtos = (List<AnnouncementDTO>)Convert.ChangeType((getResult as OkObjectResult).Value, typeof(List<AnnouncementDTO>));

                var dto = dtos[0];
                dto.Title = "Updated Title";
                dto.Content = "Updated Content";

                // Act
                await _controller.UpsertAnnouncement(dto);

                // Assert
                var timer = new Timer {Interval = 5000};
                timer.Elapsed += delegate
                {
                    var modifiedDto = _testHelper.UnitOfWork.AnnouncementRepo.Announcements()[0];
                    Assert.Equal(dto.Id, modifiedDto.Id);
                    Assert.Equal(dto.Title, modifiedDto.Title);
                    Assert.Equal(dto.Content, modifiedDto.Content);
                    Assert.Equal(dto.CreatedDate, modifiedDto.CreatedDate);
                };
            }
            finally
            {
                // Cleanup
                _testHelper.CleanUp();
            }
        }

        [Fact]
        public async void ShouldDeletePerformanceCurveData()
        {
            try
            {
                // Arrange
                _testHelper.UnitOfWork.Context.AddEntity(TestAnnouncement);
                var getResult = await _controller.Announcements();
                var dtos = (List<AnnouncementDTO>)Convert.ChangeType((getResult as OkObjectResult).Value, typeof(List<AnnouncementDTO>));

                // Act
                var result = _controller.DeleteAnnouncement(dtos[0].Id);

                // Assert
                Assert.IsType<OkResult>(result.Result);

                var timer = new Timer {Interval = 5000};
                timer.Elapsed += delegate
                {
                    Assert.True(!_testHelper.UnitOfWork.Context.Announcement.Any(_ => _.Id == dtos[0].Id));
                };
            }
            finally
            {
                // Cleanup
                _testHelper.CleanUp();
            }
        }
    }
}