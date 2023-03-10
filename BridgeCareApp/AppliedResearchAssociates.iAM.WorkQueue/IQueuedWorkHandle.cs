namespace AppliedResearchAssociates.iAM.WorkQueue;

public interface IQueuedWorkHandle
{
    DateTime QueueEntryTimestamp { get; }

    int QueueIndex { get; }

    string UserId { get; }

    Task WorkCompletion { get; }

    bool WorkHasStarted => WorkStartTimestamp.HasValue;

    string WorkId { get; }

    DateTime? WorkStartTimestamp { get; }

    void RemoveFromQueue(bool setCanceled = false);

    CancellationTokenSource? WorkCancellationTokenSource { get; }
}
