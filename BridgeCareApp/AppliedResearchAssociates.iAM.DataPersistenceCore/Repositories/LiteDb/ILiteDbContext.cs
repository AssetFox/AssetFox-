using LiteDB;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.LiteDb
{
    public interface ILiteDbContext
    {
        LiteDatabase Database { get; }
    }
}
