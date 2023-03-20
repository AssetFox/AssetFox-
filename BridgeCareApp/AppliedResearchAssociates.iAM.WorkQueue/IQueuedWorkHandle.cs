namespace AppliedResearchAssociates.iAM.WorkQueue;

public interface IQueuedWorkHandle
{
    string MostRecentStatusMessage { get; }

    string WorkDescription { get; }

    DateTime QueueEntryTimestamp { get; }

    int QueueIndex { get; }

    string UserId { get; }

    WorkType WorkType { get; }

    public string WorkName { get; }

    Task WorkCompletion { get; }

    bool WorkHasStarted => WorkStartTimestamp.HasValue;

    string WorkId { get; }

    DateTime? WorkStartTimestamp { get; }

    void RemoveFromQueue(bool setCanceled);

    CancellationTokenSource? WorkCancellationTokenSource { get; }
}
