using System;
using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.iAM.DataMiner.Attributes;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.DTOs;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.Domains;
using MoreLinq;
using Attribute = AppliedResearchAssociates.iAM.DataMiner.Attributes.Attribute;
using TextAttribute = AppliedResearchAssociates.iAM.DataMiner.Attributes.TextAttribute;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers
{
    public static class AttributeMapper
    {
        public static Attribute ToDomain(this AttributeEntity entity)
        {
            if (entity == null)
            {
                throw new NullReferenceException("Cannot map null Attribute entity to Attribute domain");
            }

            if (entity.DataType == "NUMBER")
            {
                return new NumericAttribute(Convert.ToDouble(entity.DefaultValue),
                    entity.Maximum,
                    entity.Minimum,
                    entity.Id,
                    entity.Name,
                    entity.AggregationRuleType,
                    entity.Command,
                    entity.ConnectionType,
                    "",
                    entity.IsCalculated,
                    entity.IsAscending);
            }

            if (entity.DataType == "STRING")
            {
                return new TextAttribute(entity.DefaultValue,
                    entity.Id,
                    entity.Name,
                    entity.AggregationRuleType,
                    entity.Command,
                    entity.ConnectionType,
                    "",
                    entity.IsCalculated,
                    entity.IsAscending);
            }

            throw new InvalidOperationException("Cannot determine Attribute entity data type");
        }

        public static AttributeEntity ToEntity(this Attribute domain)
        {
            if (domain == null)
            {
                throw new NullReferenceException("Cannot map null Attribute domain to Attribute entity");
            }

            var entity = new AttributeEntity
            {
                Id = domain.Id,
                Name = domain.Name,
                DataType = domain.DataType,
                AggregationRuleType = domain.AggregationRuleType,
                Command = domain.Command,
                ConnectionType = domain.ConnectionType,
                IsCalculated = domain.IsCalculated,
                IsAscending = domain.IsAscending
            };

            if (domain is NumericAttribute numericAttribute)
            {
                entity.DefaultValue = numericAttribute.DefaultValue.ToString();
                entity.Maximum = numericAttribute.Maximum;
                entity.Minimum = numericAttribute.Minimum;
            }
            else if (domain is TextAttribute textAttribute)
            {
                entity.DefaultValue = textAttribute.DefaultValue;
            }

            return entity;
        }

        public static Domains.Attribute GetAttributesFromDomain(this AttributeEntity entity, IEnumerable<Domains.Attribute> attributes)
        {
            var filteredAttribute = attributes.Where(_ => _.Name == entity.Name).FirstOrDefault();

            if(filteredAttribute == null)
            {
                throw new ArgumentNullException($"The attribute present in `Attribute entity` {entity.Name}, is not present in `Explorer.Network.Attribute` object");
            }

            return filteredAttribute;
        }

        public static AttributeDTO ToDto(this AttributeEntity entity) =>
            new AttributeDTO { Name = entity.Name, Type = entity.DataType };
    }
}
