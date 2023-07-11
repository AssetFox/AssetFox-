using System;
using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates;
using AppliedResearchAssociates.iAM.DataPersistenceCore;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DTOs;
using BridgeCareCore.Models;
using BridgeCareCore.Utils;
using static OfficeOpenXml.ExcelErrorValue;

namespace BridgeCareCore.Services
{
    public class AttributeService
    {
        private readonly IUnitOfWork _unitOfWork;
        public const string ValuesForAttribute = "Values for attribute";
        public const string IsANumberUseTextInput = "is a number; use text input";

        public AttributeService(IUnitOfWork unitOfWork) => _unitOfWork =
            unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));

        /*public List<AttributeSelectValuesResult> GetAttributeSelectValues(List<string> attributeNames)
        {
            var aggregatedResults = _unitOfWork.AggregatedResultRepo.GetAggregatedResultsForAttributeNames(attributeNames);
            if (!aggregatedResults.Any())
            {
                return new List<AttributeSelectValuesResult>();
            }
            return aggregatedResults
                .GroupBy(_ => _.Attribute.Name, _ => _)
                .ToDictionary(_ => _.Key, _ => _.ToList())
                .Select(keyValuePair =>
                {
                    var values = new List<string>();
                    var dtypes = new List<string>();
                    if (keyValuePair.Value.All(aggregatedResult =>
                        aggregatedResult.Discriminator == DataPersistenceConstants.AggregatedResultNumericDiscriminator))
                    {
                        values = keyValuePair.Value.Where(_ => _.NumericValue.HasValue)
                            .DistinctBy(_ => _.NumericValue).Select(_ => _.NumericValue!.Value.ToString()).ToList();
                        dtypes = keyValuePair.Value.Where(_ => _.Attribute.Type == "NUMBER").DistinctBy(_ => _.Attribute.Type).Select(_ => _.Attribute.Type!).ToList();
                    }
                    if (keyValuePair.Value.All(aggregatedResult =>
                        aggregatedResult.Discriminator == DataPersistenceConstants.AggregatedResultTextDiscriminator))
                    {
                        values = keyValuePair.Value.Where(_ => _.TextValue != null)
                            .DistinctBy(_ => _.TextValue).Select(_ => _.TextValue).ToList();
                        dtypes = keyValuePair.Value.Where(_ => _.Attribute.Type == "NUMBER").DistinctBy(_ => _.Attribute.Type).Select(_ => _.Attribute.Type).ToList();
                    }
                    var result = new AttributeSelectValuesResult
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
                    return result;
                }).ToList();
        }*/

        public List<AttributeSelectValuesResult> GetAttributeSelectValues(List<string> attributeNames)
        {
            var aggregatedResults = _unitOfWork.AggregatedResultRepo.GetAggregatedResultsForAttributeNames(attributeNames);
            return new List<AttributeSelectValuesResult>()
            {
                new AttributeSelectValuesResult
                {
                    // Item1 = Attribute name, Item2 = Result type, Item3 = All values associated with attribute, Item4 = Has type "NUMBER"?
                    Attribute = aggregatedResults.Item1,
                    Values = aggregatedResults.Item4 ? new List<string>() : aggregatedResults.Item3.ToSortedSet(new AlphanumericComparator()).ToList(),
                    ResultMessage = !aggregatedResults.Item3.Any() ? $"No values found for attribute {aggregatedResults.Item1}; use text input"
                                                                   : aggregatedResults.Item4 ? $"{ValuesForAttribute} {aggregatedResults.Item1} {IsANumberUseTextInput}" : "Success",
                    ResultType = aggregatedResults.Item2
                }
            };
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
