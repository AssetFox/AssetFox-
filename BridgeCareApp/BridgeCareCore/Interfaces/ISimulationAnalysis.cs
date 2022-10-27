using System;
using BridgeCareCore.Models;
using BridgeCareCore.Services;

namespace BridgeCareCore.Interfaces
{
    public interface ISimulationAnalysis
    {
        IQueuedWorkHandle CreateAndRunPermitted(Guid networkId, Guid simulationId, UserInfo userInfo);

        IQueuedWorkHandle CreateAndRun(Guid networkId, Guid simulationId, UserInfo userInfo);

        bool Cancel(Guid simulationId);
    }
}
