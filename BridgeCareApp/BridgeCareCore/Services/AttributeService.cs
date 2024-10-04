using System;
using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates;
using AppliedResearchAssociates.iAM.DataPersistenceCore;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DTOs;
using BridgeCareCore.Models;
using BridgeCareCore.Utils;

namespace BridgeCareCore.Services
{
    public class AttributeService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly AggregatedSelectValuesResultDtoCache _cache;
        public const string ValuesForAttribute = "Values for attribute";
        public const string IsANumberUseTextInput = "is a number; use text input";

        public AttributeService(
            IUnitOfWork unitOfWork,
            AggregatedSelectValuesResultDtoCache cache
            )
        {
            _unitOfWork =
            unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        }
        public List<AttributeSelectValuesResult> GetAttributeSelectValues(List<string> attributeNames)
        {
            List<AttributeSelectValuesResult> returnList = new();
            var dtos = new List<AggregatedSelectValuesResultDTO>();
            var uniqueAttributes = attributeNames.Distinct().ToList();
            var attributesToFetch = new List<string>();
            foreach (var attributeName in uniqueAttributes)
            {
                var cached = _cache.TryGetCachedValue(attributeName);
                if (cached == null)
                {
                    attributesToFetch.Add(attributeName);
                }
                else
                {
                    dtos.Add(cached);
                }
            }
            var aggregatedResults = _unitOfWork.AggregatedResultRepo.GetAggregatedResultsForAttributeNames(attributeNames);
            foreach (var result in aggregatedResults)
            {
                _cache.SaveToCache(result);
            }
            dtos.AddRange(aggregatedResults);
            if (dtos.Count == 0)
                return new();

            foreach (var result in dtos)
            {
                var returnValue = new AttributeSelectValuesResult
                {
                    Attribute = result.Attribute.Name,
                    Values = result.IsNumber ? new List<string>() : result.Values.ToSortedSet(new AlphanumericComparator()).ToList(),
                    ResultMessage = !result.Values.Any() ? $"No values found for attribute {result.Attribute.Name}; use text input"
                                                                   : result.IsNumber ? $"{ValuesForAttribute} {result.Attribute.Name} {IsANumberUseTextInput}" : "Success",
                    ResultType = result.ResultType
                };
                returnList.Add(returnValue);
            }
            return returnList;
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
