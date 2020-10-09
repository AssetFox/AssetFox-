using System;
using AppliedResearchAssociates.iAM.DataMiner.Attributes;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using DataMinerAttribute = AppliedResearchAssociates.iAM.DataMiner.Attributes.Attribute;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.MSSQL.Mappings
{
    public static class AttributeItemMapper
    {
        public static DataMinerAttribute ToDomain(this AttributeEntity entity)
        {
            if (entity == null)
            {
                throw new NullReferenceException("Cannot map null Attribute entity to Attribute domain");
            }

            if (entity.DataType == "NUMERIC")
            {
                return new NumericAttribute(0, 0, 0,
                    entity.Id,
                    entity.Name,
                    entity.AggregationRuleType,
                    entity.Command,
                    entity.ConnectionType,
                    "");
            }

            if (entity.DataType == "TEXT")
            {
                return new TextAttribute("",
                    entity.Id,
                    entity.Name,
                    entity.AggregationRuleType,
                    entity.Command,
                    entity.ConnectionType,
                    "");
            }

            throw new InvalidOperationException("Cannot determine Attribute entity data type");
        }

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
                DataType = domain.DataType,
                AggregationRuleType = domain.AggregationRuleType,
                Command = domain.Command,
                ConnectionType = domain.ConnectionType
            };
        }
    }
}
