using AppliedResearchAssociates.iAM.DTOs;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories
{
    public interface ISimulationReportDetailRepository
    {
        void UpsertSimulationReportDetail(SimulationReportDetailDTO dto);
    }
}
