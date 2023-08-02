namespace AppliedResearchAssociates.iAM.WorkQueue;

public interface IWorkSpecification<T>
{
    string UserId { get; }

    string WorkId { get; }

    string WorkDescription { get; }

    string WorkName { get; }

    T Metadata { get; } 

    void DoWork(IServiceProvider serviceProvider, Action<string> updateStatusOnHandle, CancellationToken cancellationToken);

    void OnFault(IServiceProvider serviceProvider, string errorMessage);
}
