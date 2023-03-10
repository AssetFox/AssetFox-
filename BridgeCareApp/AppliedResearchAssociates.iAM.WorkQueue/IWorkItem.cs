namespace AppliedResearchAssociates.iAM.WorkQueue;

public interface IWorkItem
{
    string UserId { get; }

    string WorkId { get; }

    void DoWork(IServiceProvider serviceProvider, CancellationToken cancellationToken = default);
}
