using System;
using AppliedResearchAssociates.iAM.WorkQueue;
using BridgeCareCore.Models;

namespace BridgeCareCore.Interfaces
{
    public interface ISimulationAnalysis
    {
        IQueuedWorkHandle CreateAndRunPermitted(Guid networkId, Guid simulationId, UserInfo userInfo, string scenarioName);

        IQueuedWorkHandle CreateAndRun(Guid networkId, Guid simulationId, UserInfo userInfo, string scenarioName);

        bool Cancel(Guid simulationId);
    }
}
