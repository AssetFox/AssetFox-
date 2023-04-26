using System;
using AppliedResearchAssociates.iAM.Analysis.Engine;
using AppliedResearchAssociates.iAM.Analysis;
using AppliedResearchAssociates.iAM.Common;
using AppliedResearchAssociates.iAM.Hubs.Interfaces;
using System.Threading;
using AppliedResearchAssociates.iAM.Common.Logging;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories
{
    public interface ISimulationOutputRepository
    {
        void CreateSimulationOutputViaRelational(Guid simulationId, SimulationOutput simulationOutput,
            IWorkQueueLog logerForUserInfo = null, ILog loggerForTechnicalInfo = null, CancellationToken? cancellationToken = null);

        SimulationOutput GetSimulationOutputViaRelation(Guid simulationId, ILog loggerForUserInfo = null, ILog loggerForTechincalInfo = null);

        SimulationOutput GetSimulationOutputViaJson(Guid simulationId);
        SimulationOutput GetSimulationOutputViaJsonGraphQL(Guid simulationId);
        void CreateSimulationOutputViaJson(Guid simulationId, SimulationOutput simulationOutput);

        void ConvertSimulationOutpuFromJsonTorelational(Guid simulationId, CancellationToken? cancellationToken = null, IWorkQueueLog queueLogger = null);
    }
}
