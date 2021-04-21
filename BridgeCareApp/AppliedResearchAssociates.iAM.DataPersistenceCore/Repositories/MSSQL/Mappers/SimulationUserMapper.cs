using System;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DTOs;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers
{
    public static class SimulationUserMapper
    {
        public static SimulationUserEntity ToEntity(this SimulationUserDTO dto, Guid simulationId) =>
            new SimulationUserEntity
            {
                SimulationId = simulationId,
                UserId = dto.UserId,
                CanModify = dto.CanModify,
                IsOwner = dto.IsOwner
            };

        public static SimulationUserDTO ToDto(this SimulationUserEntity entity) =>
            new SimulationUserDTO
            {
                UserId = entity.User.Id,
                CanModify = entity.CanModify,
                IsOwner = entity.IsOwner,
                Username = entity.User.Username
            };
    }
}
