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

        List<PerformanceCurveLibraryDTO> PerformanceCurveLibrariesWithPerformanceCurves();

        void UpsertPermitted(Guid simulationId, PerformanceCurveLibraryDTO dto);

        void UpsertPerformanceCurveLibrary(PerformanceCurveLibraryDTO dto, Guid simulationId);

        void UpsertOrDeletePerformanceCurves(List<PerformanceCurveDTO> performanceCurves, Guid libraryId);

        void DeletePerformanceCurveLibrary(Guid libraryId);
    }
}
