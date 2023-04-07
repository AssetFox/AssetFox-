using System;
using System.Data;
using System.Linq;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.WorkQueue;
using BridgeCareCore.Interfaces;
using BridgeCareCore.Models;

namespace BridgeCareCore.Services
{
    public class GeneralWorkQueService : IGeneralWorkQueueService
    {
        private readonly SequentialWorkQueue<WorkQueueMetadata> _sequentialWorkQueue;

        public GeneralWorkQueService(SequentialWorkQueue<WorkQueueMetadata> sequentialWorkQueue)
        {
            _sequentialWorkQueue = sequentialWorkQueue ?? throw new ArgumentNullException(nameof(sequentialWorkQueue));
        }

        public IQueuedWorkHandle<WorkQueueMetadata> CreateAndRun(IWorkSpecification<WorkQueueMetadata> workItem)
        {
            _sequentialWorkQueue.Enqueue(workItem, out var workHandle).Wait();
            return workHandle;
        }

        public bool Cancel(Guid workId)
        {
            return _sequentialWorkQueue.Cancel(workId);
        }
    }
}
