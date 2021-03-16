using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.DTOs;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories
{
    public interface ISimulationReportDetailRepository
    {
        void UpsertSimulationReportDetail(SimulationReportDetailDTO dto, UserInfoDTO userInfo);
    }
}
