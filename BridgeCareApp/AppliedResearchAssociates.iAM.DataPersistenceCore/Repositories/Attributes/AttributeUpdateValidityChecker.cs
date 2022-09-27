using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.Attributes
{
    public static class AttributeUpdateValidityChecker
    {
        public const string NameChangeNotAllowed = "Changing attribute name is not allowed.";
        public const string ConnectionTypeChangeNotAllowed = "Changing attribute connection type is not allowed.";
        public static AttributeUpdateValidityCheckResult CheckUpdateValidity(List<AttributeEntity> oldEntities, AttributeEntity proposedNewEntity)
        {
            var compare = oldEntities.Single(e => e.Id == proposedNewEntity.Id);
            if (proposedNewEntity.Name != compare.Name)
            {
                return AttributeUpdateValidityCheckResults.NotOk($"{NameChangeNotAllowed} The proposed update changes {compare.Name} to {proposedNewEntity.Name}");
            }
           /* if (proposedNewEntity.ConnectionType != compare.ConnectionType)
            {
                return AttributeUpdateValidityCheckResults.NotOk($"{ConnectionTypeChangeNotAllowed} For the attribute named {proposedNewEntity.Name}, the update changes {compare.ConnectionType} to {proposedNewEntity.ConnectionType}");
            }*/
            return AttributeUpdateValidityCheckResults.Ok();
        }
    }
}
