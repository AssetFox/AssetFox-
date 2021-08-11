using System;
using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Extensions;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DTOs;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL
{
    public class AnnouncementRepository : IAnnouncementRepository
    {
        private readonly UnitOfDataPersistenceWork _unitOfWork;

        public AnnouncementRepository(UnitOfDataPersistenceWork unitOfWork) =>
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));

        public List<AnnouncementDTO> Announcements()
        {
            if (!_unitOfWork.Context.Announcement.Any())
            {
                return new List<AnnouncementDTO>();
            }

            return _unitOfWork.Context.Announcement.Select(_ => _.ToDto()).ToList();
        }

        public void UpsertAnnouncement(AnnouncementDTO dto)
        {
            var announcementEntity = dto.ToEntity();

            _unitOfWork.Context.Upsert(announcementEntity, dto.Id, _unitOfWork.UserEntity?.Id);
        }

        public void DeleteAnnouncement(Guid announcementId)
        {
            if (!_unitOfWork.Context.Announcement.Any(_ => _.Id == announcementId))
            {
                return;
            }

            _unitOfWork.Context.DeleteEntity<AnnouncementEntity>(_ => _.Id == announcementId);
        }
    }
}
