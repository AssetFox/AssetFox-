using System;
using System.Collections.Generic;
using System.Text;
using AppliedResearchAssociates.iAM.DataMiner;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories
{
    public interface IAttributeMetaDatumRepository
    {
        void UpdateAll(List<AttributeMetaDatum> attributeMetaData);
    }
}
