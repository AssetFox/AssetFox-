using System;
using System.Data;
using System.Linq;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.WorkQueue;
using BridgeCareCore.Interfaces;
using BridgeCareCore.Models;

namespace BridgeCareCore.Services
{
    public class GeneralWorkQueService : IWorkQueService
    {
        private readonly UnitOfDataPersistenceWork _unitOfWork;
        private readonly SequentialWorkQueue _sequentialWorkQueue;

        public GeneralWorkQueService(UnitOfDataPersistenceWork unitOfWork, SequentialWorkQueue sequentialWorkQueue)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _sequentialWorkQueue = sequentialWorkQueue ?? throw new ArgumentNullException(nameof(sequentialWorkQueue));
        }

        public IQueuedWorkHandle CreateAndRun(IWorkSpecification workItem)
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
