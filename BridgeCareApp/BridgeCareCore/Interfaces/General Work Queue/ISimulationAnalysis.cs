using System;
using AppliedResearchAssociates.iAM.WorkQueue;
using BridgeCareCore.Models;

namespace BridgeCareCore.Interfaces
{
    public interface ISimulationAnalysis
    {
        IQueuedWorkHandle<WorkQueueMetadata> CreateAndRunPermitted(Guid networkId, Guid simulationId, UserInfo userInfo, string scenarioName);

        IQueuedWorkHandle<WorkQueueMetadata> CreateAndRun(Guid networkId, Guid simulationId, UserInfo userInfo, string scenarioName);

        bool Cancel(Guid simulationId);
    }
}
