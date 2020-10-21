using System.Collections.Generic;
using AppliedResearchAssociates.iAM.DataMiner;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories
{
    public interface IAttributeMetaDataRepository
    {
        List<AttributeMetaDatum> All(string filePath);
    }
}
