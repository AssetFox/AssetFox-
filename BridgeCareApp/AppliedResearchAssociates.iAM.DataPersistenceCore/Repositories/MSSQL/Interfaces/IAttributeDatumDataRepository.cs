using AppliedResearchAssociates.iAM.DataMiner.Attributes;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Interfaces
{
    public interface IAttributeDatumDataRepository
    {
        void AddAttributeData<T>(IEnumerable<AttributeDatum<T>> domains, Guid segmentId);
    }
}
