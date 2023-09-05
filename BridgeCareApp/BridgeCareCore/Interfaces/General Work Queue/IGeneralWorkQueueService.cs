using AppliedResearchAssociates.iAM.WorkQueue;
using BridgeCareCore.Models;
using System;

namespace BridgeCareCore.Interfaces
{
    public interface IGeneralWorkQueueService
    {
        IQueuedWorkHandle<WorkQueueMetadata> CreateAndRun(IWorkSpecification<WorkQueueMetadata> workItem);
        IQueuedWorkHandle<WorkQueueMetadata> CreateAndRunInFastQueue(IWorkSpecification<WorkQueueMetadata> workItem);

        bool Cancel(string simulationId);
        bool CancelInFastQueue(string workId);
    }
}
