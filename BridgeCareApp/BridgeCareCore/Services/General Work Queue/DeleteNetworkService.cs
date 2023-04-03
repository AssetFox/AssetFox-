using System;
using System.Data;
using System.Linq;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.WorkQueue;
using BridgeCareCore.Interfaces;
using BridgeCareCore.Models;

namespace BridgeCareCore.Services
{
    public class WorkQueService : IWorkQueService
    {
        private readonly UnitOfDataPersistenceWork _unitOfWork;
        private readonly SequentialWorkQueue<WorkQueueMetadata> _sequentialWorkQueue;

        public WorkQueService(UnitOfDataPersistenceWork unitOfWork, SequentialWorkQueue<WorkQueueMetadata> sequentialWorkQueue)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _sequentialWorkQueue = sequentialWorkQueue ?? throw new ArgumentNullException(nameof(sequentialWorkQueue));
        }

        public IQueuedWorkHandle<WorkQueueMetadata> CreateAndRun(IWorkSpecification<WorkQueueMetadata> workItem)
        {
            _sequentialWorkQueue.Enqueue(workItem, out var workHandle).Wait();
            return workHandle;
        }

        public bool Cancel(Guid simulationId)
        {
            return _sequentialWorkQueue.Cancel(simulationId);
        }
    }
}
