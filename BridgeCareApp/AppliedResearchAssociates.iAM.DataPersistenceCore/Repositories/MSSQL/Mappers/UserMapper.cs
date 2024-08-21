using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DTOs;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers
{
    public static class UserMapper
    {
        public static UserDTO ToDto(this UserEntity entity) =>
            new UserDTO
            {
                Id = entity.Id,
                Username = entity.Username,
                ActiveStatus = entity.ActiveStatus,
                HasInventoryAccess = entity.HasInventoryAccess,
                LastNewsAccessDate = entity.LastNewsAccessDate,
                CriterionLibrary = entity.CriterionLibraryUserJoin != null
                    ? entity.CriterionLibraryUserJoin.CriterionLibrary.ToDto()
                    : new CriterionLibraryDTO()
            };
    }
}
