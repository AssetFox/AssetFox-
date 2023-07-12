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
        public const string ValuesForAttribute = "Values for attribute";
        public const string IsANumberUseTextInput = "is a number; use text input";

        public AttributeService(IUnitOfWork unitOfWork) => _unitOfWork =
            unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));

        public List<AttributeSelectValuesResult> GetAttributeSelectValues(List<string> attributeNames)
        {
            var aggregatedResults = _unitOfWork.AggregatedResultRepo.GetAggregatedResultsForAttributeNames(attributeNames);
            if (aggregatedResults.Attribute == null || aggregatedResults.ResultType == null)
                return new();

            return new List<AttributeSelectValuesResult>()
            {
                new AttributeSelectValuesResult
                {
                    Attribute = aggregatedResults.Attribute.Name,
                    Values = aggregatedResults.IsNumber ? new List<string>() : aggregatedResults.Values.ToSortedSet(new AlphanumericComparator()).ToList(),
                    ResultMessage = !aggregatedResults.Values.Any() ? $"No values found for attribute {aggregatedResults.Attribute.Name}; use text input"
                                                                   : aggregatedResults.IsNumber ? $"{ValuesForAttribute} {aggregatedResults.Attribute.Name} {IsANumberUseTextInput}" : "Success",
                    ResultType = aggregatedResults.ResultType
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
