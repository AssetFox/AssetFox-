using System;
using System.Collections.Generic;
using System.Text;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.DTOs;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappings
{
    public static class UserCriteriaMapper
    {
        public static UserCriteriaDTO ToDto(this UserCriteriaFilterEntity entity) =>
            new UserCriteriaDTO
            {
                UserName = entity.UserEntityJoin.Username,
                Criteria = entity.Criteria,
                HasAccess = entity.UserEntityJoin.HasInventoryAccess,
                CriteriaId = entity.UserCriteriaId,
                UserId = entity.UserId,
                HasCriteria = entity.HasCriteria
            };
    }
}
