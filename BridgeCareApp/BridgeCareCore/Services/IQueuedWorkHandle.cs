using System;
using System.Threading;
using System.Threading.Tasks;
using BridgeCareCore.Models;

namespace BridgeCareCore.Services
{
    public interface IQueuedWorkHandle
    {
        DateTime QueueEntryTimestamp { get; }

        int QueueIndex { get; }

        UserInfo UserInfo { get; }

        Task WorkCompletion { get; }

        bool WorkHasStarted => WorkStartTimestamp.HasValue;

        string WorkId { get; }

        DateTime? WorkStartTimestamp { get; }

        void RemoveFromQueue(bool setCanceled = false);

        CancellationTokenSource WorkCancellationTokenSource { get; }
    }
}
