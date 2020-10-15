using System.Collections.Generic;
using AppliedResearchAssociates.iAM.DataMiner;
using AppliedResearchAssociates.iAM.DataMiner.Attributes;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories
{
    public interface IAttributeRepository : IRepository<Attribute>
    {
        void AddAttributes(List<Attribute> domains);
        void AddAttribute(Attribute domain);
    }
}
