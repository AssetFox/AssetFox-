using System;
using BridgeCareCore.Services;

namespace BridgeCareCore.Interfaces
{
    public interface ISimulationAnalysis
    {
        IQueuedWorkHandle CreateAndRunPermitted(Guid networkId, Guid simulationId);

        IQueuedWorkHandle CreateAndRun(Guid networkId, Guid simulationId);
    }
}
