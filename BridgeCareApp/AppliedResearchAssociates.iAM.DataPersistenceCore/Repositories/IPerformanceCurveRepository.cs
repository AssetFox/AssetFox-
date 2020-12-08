using System.Collections.Generic;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.Domains;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories
{
    public interface IPerformanceCurveRepository
    {
        void CreatePerformanceCurveLibrary(string name, string simulationName);
        void CreatePerformanceCurves(List<PerformanceCurve> performanceCurves, string simulationName);
        void GetSimulationPerformanceCurves(Simulation simulation);
    }
}
