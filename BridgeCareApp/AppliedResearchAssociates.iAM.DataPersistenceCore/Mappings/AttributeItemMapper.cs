using System;
using System.Collections.Generic;
using System.Text;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using DataMinerAttribute = AppliedResearchAssociates.iAM.DataMiner.Attributes.Attribute;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Mappings
{
    public static class AttributeItemMapper
    {
        public static AttributeEntity ToEntity(this DataMinerAttribute domain)
        {
            if (domain == null)
            {
                throw new NullReferenceException("Cannot map null Attribute domain to Attribute entity");
            }

            return new AttributeEntity
            {
                Id = domain.Id,
                Name = domain.Name,
                Command = domain.Command,
                ConnectionType = domain.ConnectionType
            };
        }
    }
}
