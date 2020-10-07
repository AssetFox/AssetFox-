using AppliedResearchAssociates.iAM.DataMiner.Attributes;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Interfaces
{
    public interface IAttributeDatumDataRepository
    {
        void AddAttributeDatum<T>(AttributeDatum<T> domain, string locationUniqueIdentifier);
    }
}
