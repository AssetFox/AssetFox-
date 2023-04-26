using System;
using AppliedResearchAssociates.iAM.Analysis.Engine;
using AppliedResearchAssociates.iAM.Analysis;
using AppliedResearchAssociates.iAM.Common;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories
{
    public interface ISimulationOutputRepository
    {
        void CreateSimulationOutputViaRelational(Guid simulationId, SimulationOutput simulationOutput, ILog logerForUserInfo = null, ILog loggerForTechnicalInfo = null);

        SimulationOutput GetSimulationOutputViaRelation(Guid simulationId, ILog loggerForUserInfo = null, ILog loggerForTechincalInfo = null);

        SimulationOutput GetSimulationOutputViaJson(Guid simulationId);
        SimulationOutput GetSimulationOutputViaJsonGraphQL(Guid simulationId);
        void CreateSimulationOutputViaJson(Guid simulationId, SimulationOutput simulationOutput);

        void ConvertSimulationOutpuFromJsonTorelational(Guid simulationId);
    }
}
