using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using AppliedResearchAssociates;
using AppliedResearchAssociates.iAM.DataPersistenceCore;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using BridgeCareCore.Models;
using BridgeCareCore.Utils;
//using MoreLinq;

namespace BridgeCareCore.Services
{
    public class AttributeService
    {
        private readonly UnitOfDataPersistenceWork _unitOfWork;

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
                        Name = aggregatedResult.Attribute.Name
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
                    if (keyValuePair.Value.All(aggregatedResultEntity =>
                        aggregatedResultEntity.Discriminator == DataPersistenceConstants.AggregatedResultNumericDiscriminator))
                    {
                        values = keyValuePair.Value.Where(_ => _.NumericValue.HasValue)
                            .DistinctBy(_ => _.NumericValue).Select(_ => _.NumericValue!.Value.ToString()).ToList();
                    }
                    if (keyValuePair.Value.All(aggregatedResultEntity =>
                        aggregatedResultEntity.Discriminator == DataPersistenceConstants.AggregatedResultTextDiscriminator))
                    {
                        values = keyValuePair.Value.Where(_ => _.TextValue != null)
                            .DistinctBy(_ => _.TextValue).Select(_ => _.TextValue).ToList();
                    }
                    return new AttributeSelectValuesResult
                    {
                        Attribute = keyValuePair.Key,
                        Values = !HasAlphaValues(values)
                            ? new List<string>()
                            : values.ToSortedSet(new AlphanumericComparator()).ToList(),
                        ResultMessage = !values.Any()
                            ? $"No values found for attribute {keyValuePair.Key}; use text input"
                            : !HasAlphaValues(values)
                                ? $"Number of values for attribute {keyValuePair.Key} exceeds 100; use text input"
                                : "Success",
                        ResultType = !values.Any() || !HasAlphaValues(values) ? "warning" : "success"
                    };
                }).ToList();
        }
        // Check list of values and determine
        // if alphabetical characters are in the
        // list.
        public bool HasAlphaValues(List<string> vals)
        {
            bool found = false;

            foreach (string val in vals)
            {
                Regex rg = new Regex(@"^[a-zA-Z\s,]*$");
                found = rg.IsMatch(val);
            }
            return found;
        }
    }
}
