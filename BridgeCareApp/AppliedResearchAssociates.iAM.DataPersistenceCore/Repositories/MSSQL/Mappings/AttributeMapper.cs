using System;
using AppliedResearchAssociates.iAM.DataMiner.Attributes;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using DataMiner = AppliedResearchAssociates.iAM.DataMiner;
using SimulationAnalysis = AppliedResearchAssociates.iAM.Domains;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappings
{
    public static class AttributeMapper
    {
        public static DataMiner.Attributes.Attribute ToDomain(this AttributeEntity entity)
        {
            if (entity == null)
            {
                throw new NullReferenceException("Cannot map null Attribute entity to Attribute domain");
            }

            if (entity.DataType == "NUMERIC")
            {
                return new NumericAttribute(Convert.ToDouble(entity.DefaultValue),
                    entity.Maximum ?? -1,
                    entity.Minimum ?? -1,
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
                return new DataMiner.Attributes.TextAttribute(entity.DefaultValue,
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

        public static SimulationAnalysis.Attribute ToSimulationAnalysisAttribute(this DataMiner.Attributes.Attribute domain)
        {
            if (domain.DataType == "NUMERIC" && domain is NumericAttribute numericAttribute)
            {
                var simulationAnalysisAttribute = new SimulationAnalysis.NumberAttribute(numericAttribute.Name)
                {
                    IsDecreasingWithDeterioration = numericAttribute.IsAscending,
                    DefaultValue = numericAttribute.DefaultValue
                };

                if (numericAttribute.Maximum != -1)
                {
                    simulationAnalysisAttribute.Maximum = numericAttribute.Maximum;

                }

                if (numericAttribute.Minimum != -1)
                {
                    simulationAnalysisAttribute.Minimum = numericAttribute.Minimum;
                }

                return simulationAnalysisAttribute;
            }

            if (domain.DataType == "TEXT" && domain is TextAttribute textAttribute)
            {
                return new SimulationAnalysis.TextAttribute(textAttribute.Name)
                {
                    DefaultValue = textAttribute.DefaultValue
                };
            }

            throw new InvalidOperationException("Cannot determine Attribute entity data type");
        }

        public static AttributeEntity ToEntity(this DataMiner.Attributes.Attribute domain)
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
