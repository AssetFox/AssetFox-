using System;
using AppliedResearchAssociates.iAM.Analysis.Engine;
using AppliedResearchAssociates.iAM.Analysis;
using AppliedResearchAssociates.iAM.Common;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories
{
    public interface ISimulationOutputRepository
    {
        void CreateSimulationOutputViaRelational(Guid simulationId, SimulationOutput simulationOutput, ILog logerForUserInfo = null, ILog loggerForTechnicalInfo = null);

        SimulationOutput GetSimulationOutput(Guid simulationId, ILog loggerForUserInfo = null, ILog loggerForTechincalInfo = null);
    }
}
