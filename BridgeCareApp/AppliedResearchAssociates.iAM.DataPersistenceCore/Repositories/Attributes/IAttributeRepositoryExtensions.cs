using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories
{
    public static class IAttributeRepositoryExtensions
    {
        public static Dictionary<Guid, string> GetAttributeNameLookupDictionary(this IAttributeRepository repository)
        {

            var allAttributes = repository.GetAttributes();
            var attributeNameLookup = new Dictionary<Guid, string>();
            foreach (var attribute in allAttributes)
            {
                attributeNameLookup[attribute.Id] = attribute.Name;
            }
            return attributeNameLookup;
        }
    }
}
