using System;
using System.Collections.Generic;
using AppliedResearchAssociates.iAM.DTOs;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories
{
    public interface IAnnouncementRepository
    {
        List<AnnouncementDTO> Announcements();

        void UpsertAnnouncement(AnnouncementDTO dto);

        void DeleteAnnouncement(Guid announcementId);
    }
}
