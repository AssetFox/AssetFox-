using System;
using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.iAM.Analysis;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using MoreLinq;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers
{
    public sealed class AttributeValueHistoryMapper
    {
        public AttributeValueHistoryMapper(Network network)
        {
            NumberAttributePerName = network.Explorer.NumberAttributes.ToDictionary(a => a.Name);
            TextAttributePerName = network.Explorer.TextAttributes.ToDictionary(a => a.Name);
        }

        public void SetNumericAttributeValueHistories(
            List<AggregatedResultEntity> entities,
            AnalysisMaintainableAsset maintainableAsset)
        {
            SetAttributeValueHistories(
                entities,
                maintainableAsset,
                NumberAttributePerName,
                e => e.NumericValue.Value);
        }

        public void SetTextAttributeValueHistories(
            List<AggregatedResultEntity> entities,
            AnalysisMaintainableAsset maintainableAsset)
        {
            SetAttributeValueHistories(
                entities,
                maintainableAsset,
                TextAttributePerName,
                e => e.TextValue);
        }

        private readonly IReadOnlyDictionary<string, NumberAttribute> NumberAttributePerName;

        private readonly IReadOnlyDictionary<string, TextAttribute> TextAttributePerName;

        private static void SetAttributeValueHistories<TAttribute, TValue>
            (
            List<AggregatedResultEntity> entities,
            AnalysisMaintainableAsset maintainableAsset,
            IReadOnlyDictionary<string, TAttribute> attributePerName,
            Func<AggregatedResultEntity, TValue> getValue
            )
            where TAttribute : Attribute<TValue>
        {
            HashSet<string> attributesWithUnsetHistory = new();
            var yearsOfHistory = entities
                .GroupBy(entity =>
                {
                    _ = attributesWithUnsetHistory.Add(entity.Attribute.Name);
                    return entity.Year;
                })
                .ToList();

            yearsOfHistory.Sort(static (year1, year2) => Comparer<int>.Default.Compare(year2.Key, year1.Key));

            foreach (var yearOfHistory in yearsOfHistory)
            {
                if (attributesWithUnsetHistory.Count is 0)
                {
                    break;
                }

                foreach (var entity in yearOfHistory)
                {
                    var attribute = attributePerName[entity.Attribute.Name];
                    var history = maintainableAsset.GetHistory(attribute);
                    var value = getValue(entity);
                    history[yearOfHistory.Key] = value;

                    _ = attributesWithUnsetHistory.Remove(entity.Attribute.Name);
                }
            }
        }
    }
}
