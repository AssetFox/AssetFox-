using DataMinerAttribute = AppliedResearchAssociates.iAM.DataMiner.Attributes.Attribute;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Interfaces
{
    public interface IAttributeDataRepository
    {
        void AddAttribute(DataMinerAttribute domain);
    }
}
