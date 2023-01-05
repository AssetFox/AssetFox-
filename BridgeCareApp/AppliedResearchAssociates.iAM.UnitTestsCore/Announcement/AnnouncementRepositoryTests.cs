using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Extensions;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.UnitTestsCore.TestUtils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Announcement
{
    public class AnnouncementRepositoryTests
    {
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
        public void ShouldReturnOkResultOnGet()
        {
            // Please just don't throw
            TestHelper.UnitOfWork.AnnouncementRepo.Announcements();
        }

        [Fact]
        public void UpsertAnnouncement_NotInDb_Adds()
        {
            // Act
            var dto = AnnouncementDtos.Dto();
            TestHelper.UnitOfWork.AnnouncementRepo.UpsertAnnouncement(dto);

            // Assert
            var announcementsAfter = TestHelper.UnitOfWork.AnnouncementRepo.Announcements();
            var newDto = announcementsAfter.Single(a => a.Id == dto.Id);
            Assert.Equal(dto.Id, newDto.Id);
            Assert.Equal(dto.Title, newDto.Title);
            Assert.Equal(dto.Content, newDto.Content);
            Assert.Equal(dto.CreatedDate, newDto.CreatedDate);
        }

        [Fact]
        public void DeleteNonexistentAnnouncement_DoesNotThrow()
        {
            // Act
            TestHelper.UnitOfWork.AnnouncementRepo.DeleteAnnouncement(Guid.NewGuid());
        }

        [Fact]
        public void GetAllAnnouncements_AnnouncementInDb_Gets()
        {
            // Arrange
            var announcementId = Guid.NewGuid();
            var announcement = TestAnnouncement(announcementId);
            TestHelper.UnitOfWork.Context.AddEntity(announcement);

            // Act
            var dtos = TestHelper.UnitOfWork.AnnouncementRepo.Announcements();

            Assert.True(dtos.Any());
            Assert.Single(dtos.Where(dto => dto.Id == announcementId));
        }

        [Fact]
        public async Task ShouldModifyAnnouncementData()
        {
            // Arrange
            var announcement = TestAnnouncement();
            TestHelper.UnitOfWork.Context.AddEntity(announcement);
            var dtos = TestHelper.UnitOfWork.AnnouncementRepo.Announcements();

            var dto = dtos.Single(a => a.Id == announcement.Id);
            dto.Title = "Updated Title";
            dto.Content = "Updated Content";

            // Act
            TestHelper.UnitOfWork.AnnouncementRepo.UpsertAnnouncement(dto);

            // Assert
            var modifiedDto = TestHelper.UnitOfWork.AnnouncementRepo.Announcements().Single(a => a.Id == announcement.Id);
            Assert.Equal(dto.Title, modifiedDto.Title);
            Assert.Equal(dto.Content, modifiedDto.Content);
            Assert.Equal(dto.CreatedDate, modifiedDto.CreatedDate);
        }

        [Fact]
        public void DeleteAnnouncement_AnnouncementInDb_Deletes()
        {
            // Arrange
            var announcement = TestAnnouncement();
            TestHelper.UnitOfWork.Context.AddEntity(announcement);
            TestHelper.UnitOfWork.Context.SaveChanges();
            // Act
            TestHelper.UnitOfWork.AnnouncementRepo.DeleteAnnouncement(announcement.Id);

            Assert.False(TestHelper.UnitOfWork.Context.Announcement.Any(_ => _.Id == announcement.Id));
        }
    }
}
