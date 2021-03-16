using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.DTOs;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappings
{
    public static class UserMapper
    {
        public static UserDTO ToDto(this UserEntity entity) =>
            new UserDTO
            {
                Id = entity.Id,
                Username = entity.Username,
                HasInventoryAccess = entity.HasInventoryAccess,
                CriterionLibrary = entity.CriterionLibraryUserJoin != null
                    ? entity.CriterionLibraryUserJoin.CriterionLibrary.ToDto()
                    : new CriterionLibraryDTO()
            };
    }
}
