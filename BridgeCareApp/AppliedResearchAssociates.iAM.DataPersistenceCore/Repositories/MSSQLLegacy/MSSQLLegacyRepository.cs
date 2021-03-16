namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQLLegacy
{
    public abstract class MSSQLLegacyRepository
    {
        protected LegacyDbContext Context { get; }

        protected MSSQLLegacyRepository(LegacyDbContext context) => Context = context;
    }
}
