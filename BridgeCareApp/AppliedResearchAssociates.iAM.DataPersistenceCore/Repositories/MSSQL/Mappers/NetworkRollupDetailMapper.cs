using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DTOs;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers
{
    public static class NetworkRollupDetailMapper
    {
        public static NetworkRollupDetailEntity ToEntity(this NetworkRollupDetailDTO dto) =>
            new NetworkRollupDetailEntity {NetworkId = dto.NetworkId, Status = dto.Status};
    }
}
