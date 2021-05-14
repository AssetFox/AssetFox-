using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DTOs;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers
{
    public static class AnnouncementMapper
    {
        public static AnnouncementEntity ToEntity(this AnnouncementDTO dto) =>
            new AnnouncementEntity
            {
                Id = dto.Id, Title = dto.Title, Content = dto.Content, CreatedDate = dto.CreatedDate
            };

        public static AnnouncementDTO ToDto(this AnnouncementEntity entity) =>
            new AnnouncementDTO
            {
                Id = entity.Id, Title = entity.Title, Content = entity.Content, CreatedDate = entity.CreatedDate
            };
    }
}
