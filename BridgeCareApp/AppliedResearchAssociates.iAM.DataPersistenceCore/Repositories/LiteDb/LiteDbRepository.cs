namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.LiteDb
{
    public abstract class LiteDbRepository
    {
        protected ILiteDbContext Context { get; }

        protected LiteDbRepository(ILiteDbContext context) => Context = context;
    }
}
