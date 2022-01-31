using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.Analysis;
using MoreLinq;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers
{
    public static class AttributeValueHistoryMapper
    {
        public static void SetNumericAttributeValueHistories(this List<AggregatedResultEntity> entities,
            Section section)
        {
            var entitiesPerAttributeName = entities
                .GroupBy(_ => _.Attribute.Name, _ => _)
                .ToDictionary(_ => _.Key, _ => _.OrderBy(__ => __.Year).ToList());

            entitiesPerAttributeName.Keys.ForEach(attributeName =>
            {
                var numberAttribute = section.Facility.Network.Explorer.NumberAttributes
                    .Single(_ => _.Name == attributeName);

                var history = section.GetHistory(numberAttribute);

                var attributeValueHistories = entitiesPerAttributeName[attributeName].ToList();

                if (attributeValueHistories.Any())
                {
                    attributeValueHistories.ForEach(_ => history.Add(_.Year, _.NumericValue ?? 0));
                    history.MostRecentValue = attributeValueHistories.Last().NumericValue ?? 0;
                }
                else
                {
                    history.MostRecentValue = entitiesPerAttributeName[attributeName].FirstOrDefault()?.NumericValue ?? 0;
                }
            });
        }

        public static void SetTextAttributeValueHistories(this List<AggregatedResultEntity> entities,
            Section section)
        {
            var entitiesPerAttributeName = entities
                .GroupBy(_ => _.Attribute.Name, _ => _)
                .ToDictionary(_ => _.Key, _ => _.OrderBy(__ => __.Year).ToList());

            entitiesPerAttributeName.Keys.ForEach(attributeName =>
            {
                var textAttribute = section.Facility.Network.Explorer.TextAttributes
                    .Single(_ => _.Name == attributeName);

                var history = section.GetHistory(textAttribute);

                var attributeValueHistories = entitiesPerAttributeName[attributeName].ToList();

                if (attributeValueHistories.Any())
                {
                    attributeValueHistories.ForEach(_ => history.Add(_.Year, _.TextValue));
                    history.MostRecentValue = attributeValueHistories.Last().TextValue;
                }
                else
                {
                    history.MostRecentValue = entitiesPerAttributeName[attributeName].FirstOrDefault()?.TextValue;
                }
            });
        }
    }
}
