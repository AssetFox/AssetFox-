using AppliedResearchAssociates.iAM.WorkQueue;
using BridgeCareCore.Models;
using System;

namespace BridgeCareCore.Interfaces
{
    public interface IWorkQueService
    {
        IQueuedWorkHandle CreateAndRun(IWorkSpecification workItem);

        bool Cancel(Guid simulationId);
    }
}
