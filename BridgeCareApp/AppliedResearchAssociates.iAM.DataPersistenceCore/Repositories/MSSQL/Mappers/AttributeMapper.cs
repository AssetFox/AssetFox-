using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AppliedResearchAssociates.iAM.Data;
using AppliedResearchAssociates.iAM.Data.Attributes;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.DTOs.Abstract;
using AppliedResearchAssociates.iAM.DTOs.Enums;
using Attribute = AppliedResearchAssociates.iAM.Data.Attributes.Attribute;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers
{
    public static class AttributeMapper
    {
        public static Attribute ToDomain(this AttributeEntity entity, string encryptionKey)
        {
            if (entity == null)
            {
                throw new NullReferenceException("Cannot map null Attribute entity to Attribute domain");
            }

            var connectionString = GetConnectionString(entity.DataSource, encryptionKey);

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
                    connectionString,
                    entity.IsCalculated,
                    entity.IsAscending,
                    entity.DataSourceId);
            }

            if (entity.DataType == "STRING")
            {
                return new TextAttribute(entity.DefaultValue,
                    entity.Id,
                    entity.Name,
                    entity.AggregationRuleType,
                    entity.Command,
                    entity.ConnectionType,
                    connectionString,
                    entity.IsCalculated,
                    entity.IsAscending,
                    entity.DataSourceId);
            }

            throw new InvalidOperationException("Cannot determine Attribute entity data type");
        }




        

        public static AttributeEntity ToEntity(this Attribute domain, IDataSourceRepository dataSources, string encryptionKey)
        {
            if (domain == null)
            {
                throw new NullReferenceException("Cannot map null Attribute domain to Attribute entity");
            }

            var dataSource = new DataSourceEntity()
            {
                Id = Guid.Empty,
                Type = "None"
            };

            if (domain.DataSourceId != null && domain.DataSourceId != Guid.Empty)
            {
                var dataSourceFromDb = dataSources?.GetDataSource(domain.DataSourceId.Value);
                var possibleDataSource = dataSourceFromDb.ToEntity(encryptionKey);
                if (possibleDataSource != null) dataSource = possibleDataSource;
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
                IsAscending = domain.IsAscending,
            };

            if (dataSource.Type != "None")
            {
                entity.DataSource = dataSource;
                entity.DataSourceId = dataSource.Id;
            }

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

        public static Analysis.Attribute GetAttributesFromDomain(this AttributeEntity entity, IEnumerable<Analysis.Attribute> attributes)
        {
            var filteredAttribute = attributes.Where(_ => _.Name == entity.Name).FirstOrDefault();

            if (filteredAttribute == null)
            {
                throw new ArgumentNullException($"The attribute present in `Attribute entity` {entity.Name}, is not present in `Explorer.Network.Attribute` object");
            }

            return filteredAttribute;
        }

        public static AttributeDTO ToAbbreviatedDto(this AttributeEntity entity)
            => new AttributeDTO
            {
                Name = entity.Name,
                Type = entity.DataType,
                Id = entity.Id,
            };

        public static AttributeDTO ToDto(this AttributeEntity entity, string encryptionKey) =>
            new()
            {
                Name = entity.Name,
                Type = entity.DataType,
                Id = entity.Id,
                IsAscending = entity.IsAscending,
                IsCalculated = entity.IsCalculated,
                AggregationRuleType = entity.AggregationRuleType,
                Command = entity.Command,
                DefaultValue = entity.DefaultValue,
                Maximum = entity.Maximum,
                Minimum = entity.Minimum,
                DataSource = entity.DataSource?.ToDTO(encryptionKey)
            };
        /// <summary>Safe to call if the entity might be null. If it is
        /// in fact null, the returned DTO will also be null.</summary>
        public static AttributeDTO ToDtoNullPropagating(this AttributeEntity entity, string encryptionKey)
        {
            if (entity == null)
            {
                return null;
            }
            return ToDto(entity, encryptionKey);
        }


        private static string GetConnectionString(DataSourceEntity entity, string encryptionKey)
        {
            string connectionString = "";
            if (entity == null)
            {
                return connectionString;
            }

            if (entity.Type == "SQL")
            {
                // ToDTO decrypts the connection
                var dsDto = (SQLDataSourceDTO)entity.ToDTO(encryptionKey);
                connectionString = dsDto.ConnectionString;
            }

            return connectionString;
        }
    }
}
