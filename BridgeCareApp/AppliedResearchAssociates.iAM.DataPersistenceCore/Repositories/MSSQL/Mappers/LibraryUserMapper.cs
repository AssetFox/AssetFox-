using System;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.LibraryEntities.RemainingLifeLimit;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.DTOs.Enums;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers
{
    public static class LibraryUserMapper
    {
        public static RemainingLifeLimitLibraryUserEntity ToRemainingLifeLimitLibraryUserEntity(this LibraryUserDTO dto, Guid remainingLifeLimitLibraryId) =>
            new RemainingLifeLimitLibraryUserEntity
            {
                RemainingLifeLimitLibraryId = remainingLifeLimitLibraryId,
                UserId = dto.UserId,
                AccessLevel = (int)dto.AccessLevel,
            };

        public static LibraryUserDTO ToDto(this RemainingLifeLimitLibraryUserEntity entity) =>
            new LibraryUserDTO
            {
                UserId = entity.UserId,
                UserName = entity.User?.Username,
                AccessLevel = (LibraryAccessLevel)entity.AccessLevel,
            };
    }
}
