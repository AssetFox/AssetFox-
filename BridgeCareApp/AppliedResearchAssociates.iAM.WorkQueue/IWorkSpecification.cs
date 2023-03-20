namespace AppliedResearchAssociates.iAM.WorkQueue;

public interface IWorkSpecification
{
    string UserId { get; }

    string WorkId { get; }

    string WorkDescription { get; }

    WorkType WorkType { get; } 

    void DoWork(IServiceProvider serviceProvider, Action<string> updateStatusOnHandle, CancellationToken cancellationToken);
}
