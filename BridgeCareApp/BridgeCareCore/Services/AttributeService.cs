using System;
using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates;
using AppliedResearchAssociates.iAM.DataPersistenceCore;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DTOs;
using BridgeCareCore.Models;
using BridgeCareCore.Utils;

namespace BridgeCareCore.Services
{
    public class AttributeService
    {
        private readonly UnitOfDataPersistenceWork _unitOfWork;
        public const string ValuesForAttribute = "Values for attribute";
        public const string IsANumberUseTextInput = "is a number; use text input";

        public AttributeService(UnitOfDataPersistenceWork unitOfDataPersistenceWork) => _unitOfWork =
            unitOfDataPersistenceWork ?? throw new ArgumentNullException(nameof(unitOfDataPersistenceWork));

        public List<AttributeSelectValuesResult> GetAttributeSelectValues(List<string> attributeNames)
        {
            if (!_unitOfWork.Context.AggregatedResult.Any(_ => attributeNames.Contains(_.Attribute.Name)))
            {
                return new List<AttributeSelectValuesResult>();
            }
            return _unitOfWork.Context.AggregatedResult
                .Where(_ => attributeNames.Contains(_.Attribute.Name))
                .Select(aggregatedResult => new AggregatedResultEntity
                {
                    Attribute = new AttributeEntity
                    {
                        Name = aggregatedResult.Attribute.Name, DataType = aggregatedResult.Attribute.DataType
                    },
                    NumericValue = aggregatedResult.NumericValue,
                    TextValue = aggregatedResult.TextValue,
                    Discriminator = aggregatedResult.Discriminator
                }).AsEnumerable()
                .GroupBy(_ => _.Attribute.Name, _ => _)
                .ToDictionary(_ => _.Key, _ => _.ToList())
                .Select(keyValuePair =>
                {
                    var values = new List<string>();
                    var dtypes = new List<string>();
                    if (keyValuePair.Value.All(aggregatedResultEntity =>
                        aggregatedResultEntity.Discriminator == DataPersistenceConstants.AggregatedResultNumericDiscriminator))
                    {
                        values = keyValuePair.Value.Where(_ => _.NumericValue.HasValue)
                            .DistinctBy(_ => _.NumericValue).Select(_ => _.NumericValue!.Value.ToString()).ToList();
                        dtypes = keyValuePair.Value.Where(_ => _.Attribute.DataType == "NUMBER").DistinctBy(_ => _.Attribute.DataType).Select(_ => _.Attribute.DataType!).ToList();
                    }
                    if (keyValuePair.Value.All(aggregatedResultEntity =>
                        aggregatedResultEntity.Discriminator == DataPersistenceConstants.AggregatedResultTextDiscriminator))
                    {
                        values = keyValuePair.Value.Where(_ => _.TextValue != null)
                            .DistinctBy(_ => _.TextValue).Select(_ => _.TextValue).ToList();
                        dtypes = keyValuePair.Value.Where(_ => _.Attribute.DataType == "NUMBER").DistinctBy(_ => _.Attribute.DataType).Select(_ => _.Attribute.DataType).ToList();
                    }
                    return new AttributeSelectValuesResult
                    {
                        Attribute = keyValuePair.Key,
                        //Values = values.Count > 100
                        Values = dtypes.Count > 0
                            ? new List<string>()
                            : values.ToSortedSet(new AlphanumericComparator()).ToList(),
                        ResultMessage = !values.Any()
                            ? $"No values found for attribute {keyValuePair.Key}; use text input"
 //                           : values.Count > 100
                            : dtypes.Count > 0
                                ? $"{ValuesForAttribute} {keyValuePair.Key} {IsANumberUseTextInput}"
                                : "Success",
                        ResultType = !values.Any() ? "warning" : "success"
                    };
                }).ToList();
        }
        public static AttributeDTO ConvertAllAttribute(AllAttributeDTO allAttribute)
        {
            var result = new AttributeDTO
            {
                Id = allAttribute.Id,
                Name = allAttribute.Name,
                AggregationRuleType = allAttribute.AggregationRuleType,
                Command = allAttribute.Command,
                DefaultValue = allAttribute.DefaultValue,
                IsAscending = allAttribute.IsAscending,
                IsCalculated = allAttribute.IsCalculated,
                Maximum = allAttribute.Maximum,
                Minimum = allAttribute.Minimum,
                Type = allAttribute.Type
            };
            var dataSourceType = allAttribute.DataSource.Type;

            switch (dataSourceType)
            {
            case "SQL":
                var sqlSource = new SQLDataSourceDTO
                {
                    Id = allAttribute.DataSource.Id,
                    Name = allAttribute.DataSource.Name,
                    ConnectionString = allAttribute.DataSource.ConnectionString
                };
                result.DataSource = sqlSource;
                return result;
            case "Excel":
                var excelSource = new ExcelDataSourceDTO
                {
                    Id = allAttribute.DataSource.Id,
                    Name = allAttribute.DataSource.Name,
                    LocationColumn = allAttribute.DataSource.LocationColumn,
                    DateColumn = allAttribute.DataSource.DateColumn
                };
                result.DataSource = excelSource;
                return result;
            case "None":
                return result;
            default:
                throw new ArgumentException($"Unable to convert All Attribute Data with a type of {allAttribute.Type}");
            }
        }

        public static List<AttributeDTO> ConvertAllAttributeList(List<AllAttributeDTO> allAttributeDTOs)
        {
            var attributeList = new List<AttributeDTO>();
            if (allAttributeDTOs != null)
            {
                foreach (var all in allAttributeDTOs)
                {
                    var attribute = ConvertAllAttribute(all);
                    attributeList.Add(attribute);
                }
            }
            return attributeList;
        }
    }
}
