using System.Collections.Generic;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.Domains;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories
{
    public interface IPerformanceCurveRepository
    {
        void CreatePerformanceCurveLibrary(PerformanceCurveLibraryEntity entity,
            string simulationName);
        int CreatePerformanceCurves(List<PerformanceCurve> domains, string simulationName);
        IEnumerable<PerformanceCurve> GetSimulationPerformanceCurves(string simulationName);
    }
}
