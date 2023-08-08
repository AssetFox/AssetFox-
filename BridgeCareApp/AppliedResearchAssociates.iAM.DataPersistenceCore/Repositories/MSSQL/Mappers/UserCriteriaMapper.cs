using System;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DTOs;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers
{
    public static class UserCriteriaMapper
    {
        public static UserCriteriaDTO ToDto(this UserCriteriaFilterEntity entity) =>
            new UserCriteriaDTO
            {
                CriteriaId = entity.UserCriteriaId,
                UserId = entity.UserId,
                UserName = entity.User.Username,
                Criteria = entity.Criteria,
                HasCriteria = entity.HasCriteria,
                HasAccess = entity.User.HasInventoryAccess,
                Name = entity.User.Name,
                Description = entity.User.Description,
            };

        public static UserCriteriaFilterEntity ToEntity(this UserCriteriaDTO dto) =>
            new UserCriteriaFilterEntity
            {
                UserCriteriaId = dto.CriteriaId,
                UserId = dto.UserId,
                Criteria = dto.Criteria,
                HasCriteria = dto.HasCriteria
            };

        public static UserCriteriaFilterEntity GenerateDefaultCriteriaForAdmin(this UserEntity entity) =>
            new UserCriteriaFilterEntity
            {
                UserCriteriaId = Guid.NewGuid(),
                UserId = entity.Id,
                HasCriteria = false
            };
    }
}
