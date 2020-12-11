using System;
using System.Collections.Generic;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.Domains;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories
{
    public interface IPerformanceCurveRepository
    {
        void CreatePerformanceCurveLibrary(string name, Guid simulationId);
        void CreatePerformanceCurves(List<PerformanceCurve> performanceCurves, Guid simulationId);
        void GetSimulationPerformanceCurves(Simulation simulation);
    }
}
