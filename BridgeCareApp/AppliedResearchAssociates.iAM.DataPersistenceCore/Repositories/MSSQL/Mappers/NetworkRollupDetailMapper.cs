using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.DTOs;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers
{
    public static class NetworkRollupDetailMapper
    {
        public static NetworkRollupDetailEntity ToEntity(this NetworkRollupDetailDTO dto) =>
            new NetworkRollupDetailEntity {NetworkId = dto.NetworkId, Status = dto.Status};
    }
}
