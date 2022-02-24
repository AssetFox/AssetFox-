using System;
using System.Threading.Tasks;

namespace BridgeCareCore.Services
{
    public interface IQueuedWorkHandle
    {
        public DateTime QueueEntryTimestamp { get; }

        public int QueueIndex { get; }

        public Task WorkCompletion { get; }

        public string WorkId { get; }

        public void RemoveFromQueue();
    }
}
