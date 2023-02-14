using System;
using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.iAM.Data.Attributes;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.DTOs.Abstract;
using AppliedResearchAssociates.iAM.DTOs.Enums;
using Attribute = AppliedResearchAssociates.iAM.Data.Attributes.Attribute;

namespace AppliedResearchAssociates.iAM.Data.Mappers
{
    public static class AttributeDtoDomainMapper
    {
        private static TextAttribute ToText(AttributeDTO dto, string encryptionKey)
        {
            var datasource = dto.DataSource;
            return new TextAttribute(
                dto.DefaultValue,
                dto.Id,
                dto.Name,
                dto.AggregationRuleType,
                dto.Command,
                MapDTODataSourceTypes(datasource?.Type),
                GetConnectionString(datasource),
                dto.IsCalculated,
                dto.IsAscending,
                dto.DataSource?.Id);
        }

        private static NumericAttribute ToNumeric(AttributeDTO dto, string encryptionKey)
        {
            return double.TryParse(dto.DefaultValue, out double value)
                ? new NumericAttribute(
                    value,
                    dto.Maximum,
                    dto.Minimum,
                    dto.Id,
                    dto.Name,
                    dto.AggregationRuleType,
                    dto.Command,
                    MapDTODataSourceTypes(dto.DataSource?.Type),
                    GetConnectionString(dto.DataSource),
                    dto.IsCalculated,
                    dto.IsAscending,
                    dto.DataSource?.Id)
                : null;
        }

        public static Attribute ToDomain(AttributeDTO dto, string encryptionKey)
        {
            switch (dto.Type.ToLowerInvariant())
            {
            case "string":
                return ToText(dto, encryptionKey);
            case "number":
                return ToNumeric(dto, encryptionKey);
            }
            throw new InvalidOperationException("Unknown attribute type {dto.Type}");
        }
        private static ConnectionType MapDTODataSourceTypes(string dtoType)
        {
            if (dtoType == DataSourceTypeStrings.Excel.ToString())
            {
                return ConnectionType.EXCEL;
            }
            else if (dtoType == DataSourceTypeStrings.SQL.ToString())
            {
                return ConnectionType.MSSQL;
            }
            else
            {
                return ConnectionType.NONE;
            }
        }

        public static List<Attribute> ToDomainList(IList<AttributeDTO> attributeDTOs, string encryptionKey)
        {
            var returnValue = attributeDTOs
                .Select(dto => ToDomain(dto, encryptionKey))
                .ToList();
            return returnValue;
        }

        private static string GetConnectionString(BaseDataSourceDTO dto)
        {
            if (dto is SQLDataSourceDTO sql)
            {
                return sql.ConnectionString;
            }
            return "";
        }

        public static AttributeDTO ToDto(this Attribute domain, BaseDataSourceDTO dataSourceDTO)
        {
            double? maximum = null;
            double? minimum = null;
            string defaultValue = "";
            if (domain is NumericAttribute numericAttribute)
            {
                maximum = numericAttribute.Maximum;
                minimum = numericAttribute.Minimum;
                defaultValue = numericAttribute.DefaultValue.ToString();
            }
            if (domain is TextAttribute textAttribute)
            {
                defaultValue = textAttribute.DefaultValue;
            }

            AttributeDTO dto = new AttributeDTO
            {
                Name = domain.Name,
                Type = domain.DataType,
                Id = domain.Id,
                IsAscending = domain.IsAscending,
                IsCalculated = domain.IsCalculated,
                AggregationRuleType = domain.AggregationRuleType,
                Command = domain.Command,
                DefaultValue = defaultValue,
                Maximum = maximum,
                Minimum = minimum,
                DataSource = dataSourceDTO,
            };
            return dto;
        }

    }
}
