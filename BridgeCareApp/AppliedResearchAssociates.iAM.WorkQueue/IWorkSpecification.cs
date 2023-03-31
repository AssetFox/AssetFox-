namespace AppliedResearchAssociates.iAM.WorkQueue;

public interface IWorkSpecification
{
    string UserId { get; }

    string WorkId { get; }

    void DoWork(IServiceProvider serviceProvider, Action<string> updateStatusOnHandle, CancellationToken cancellationToken);
}
