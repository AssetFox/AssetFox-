using System.Collections.Generic;
using AppliedResearchAssociates.iAM.DataMiner.Attributes;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Interfaces
{
    public interface IAttributeDataRepository
    {
        void AddAttributes(List<Attribute> domains);
        void AddAttribute(Attribute domain);
    }
}
