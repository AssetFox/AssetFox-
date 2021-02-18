using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.DTOs;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.Domains;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories
{
    public interface IPerformanceCurveRepository
    {
        void CreatePerformanceCurveLibrary(string name, Guid simulationId);
        void AddOrUpdatePerformanceCurveLibrary(PerformanceCurveLibraryDTO dto, Guid? simulationId);
        void CreatePerformanceCurves(List<PerformanceCurve> performanceCurves, Guid simulationId);
        void AddOrUpdateOrDeletePerformanceCurves(List<PerformanceCurveDTO> performanceCurves, Guid libraryId);
        void GetSimulationPerformanceCurves(Simulation simulation);
        Task<List<PerformanceCurveLibraryDTO>> GetPerformanceCurveLibrariesWithPerformanceCurves();
        Task<PerformanceCurveLibraryDTO> GetSimulationPerformanceCurveLibraryWithPerformanceCurves(Guid simulationId);
        void DeletePerformanceCurveLibrary(Guid libraryId);
    }
}
