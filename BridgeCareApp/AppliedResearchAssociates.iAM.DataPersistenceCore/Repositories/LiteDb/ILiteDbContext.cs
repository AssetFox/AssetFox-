namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.LiteDb
{
    public interface ILiteDbContext
    {
        LiteDB.LiteDatabase Database { get; }
    }
}
