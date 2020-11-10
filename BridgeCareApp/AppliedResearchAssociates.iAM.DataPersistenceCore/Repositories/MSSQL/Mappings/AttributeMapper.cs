using System;
using AppliedResearchAssociates.iAM.DataMiner.Attributes;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using Attribute = AppliedResearchAssociates.iAM.DataMiner.Attributes.Attribute;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappings
{
    public static class AttributeMapper
    {
        public static Attribute ToDomain(this AttributeEntity entity)
        {
            if (entity == null)
            {
                throw new NullReferenceException("Cannot map null Attribute entity to Attribute domain");
            }

            if (entity.DataType == "NUMERIC")
            {
                return new NumericAttribute(Convert.ToDouble(entity.DefaultValue),
                    entity.Maximum ?? 0,
                    entity.Minimum ?? 0,
                    entity.Id,
                    entity.Name,
                    entity.AggregationRuleType,
                    entity.Command,
                    entity.ConnectionType,
                    "",
                    entity.IsCalculated,
                    entity.IsAscending);
            }

            if (entity.DataType == "TEXT")
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
    }
}
