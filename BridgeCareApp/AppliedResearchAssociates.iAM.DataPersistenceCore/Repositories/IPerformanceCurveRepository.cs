using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.DTOs;
using AppliedResearchAssociates.iAM.Domains;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories
{
    public interface IPerformanceCurveRepository
    {
        void CreatePerformanceCurveLibrary(string name, Guid simulationId);

        void CreatePerformanceCurves(List<PerformanceCurve> performanceCurves, Guid simulationId);

        void SimulationPerformanceCurves(Simulation simulation);

        Task<List<PerformanceCurveLibraryDTO>> PerformanceCurveLibrariesWithPerformanceCurves();

        void UpsertPermitted(UserInfoDTO userInfo, Guid simulationId, PerformanceCurveLibraryDTO dto);

        void UpsertPerformanceCurveLibrary(PerformanceCurveLibraryDTO dto, Guid simulationId, UserInfoDTO userInfo);

        void UpsertOrDeletePerformanceCurves(List<PerformanceCurveDTO> performanceCurves, Guid libraryId, UserInfoDTO userInfo);

        void DeletePerformanceCurveLibrary(Guid libraryId);
    }
}
