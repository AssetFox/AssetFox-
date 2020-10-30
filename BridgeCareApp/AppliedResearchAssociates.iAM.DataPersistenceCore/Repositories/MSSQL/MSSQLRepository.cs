namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL
{
    public abstract class MSSQLRepository
    {
        protected IAMContext Context { get; }

        protected MSSQLRepository(IAMContext context) => Context = context;
    }
}
