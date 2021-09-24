using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.Analysis;
using MoreLinq;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers
{
    public static class AttributeValueHistoryMapper
    {
        /*public static List<NumericAttributeValueHistoryEntity> ToEntity(this AttributeValueHistory<double> domain, Guid sectionId, Guid attributeId)
        {
            using var historyEnumerator = domain.GetEnumerator();
            historyEnumerator.Reset();

            var entities = new List<NumericAttributeValueHistoryEntity>();

            while (historyEnumerator.MoveNext())
            {
                entities.Add(new NumericAttributeValueHistoryEntity
                {
                    Id = Guid.NewGuid(),
                    SectionId = sectionId,
                    AttributeId = attributeId,
                    Year = historyEnumerator.Current.Key,
                    Value = Convert.ToDouble(historyEnumerator.Current.Value)
                });
            }

            if (!entities.Any())
            {
                entities.Add(new NumericAttributeValueHistoryEntity
                {
                    Id = Guid.NewGuid(),
                    SectionId = sectionId,
                    AttributeId = attributeId,
                    Year = 0,
                    Value = domain.MostRecentValue
                });
            }

            return entities;
        }

        public static List<TextAttributeValueHistoryEntity> ToEntity(this AttributeValueHistory<string> domain, Guid sectionId, Guid attributeId)
        {
            using var historyEnumerator = domain.GetEnumerator();
            historyEnumerator.Reset();

            var entities = new List<TextAttributeValueHistoryEntity>();

            while (historyEnumerator.MoveNext())
            {
                entities.Add(new TextAttributeValueHistoryEntity
                {
                    Id = Guid.NewGuid(),
                    SectionId = sectionId,
                    AttributeId = attributeId,
                    Year = historyEnumerator.Current.Key,
                    Value = historyEnumerator.Current.Value
                });
            }

            if (!entities.Any())
            {
                entities.Add(new TextAttributeValueHistoryEntity
                {
                    Id = Guid.NewGuid(),
                    SectionId = sectionId,
                    AttributeId = attributeId,
                    Year = 0,
                    Value = domain.MostRecentValue
                });
            }

            return entities;
        }*/

        /*public static void SetAttributeValueHistoryValues(this List<NumericAttributeValueHistoryEntity> entities,
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

                var attributeValueHistories = entitiesPerAttributeName[attributeName]
                    .Where(_ => _.Year != 0).ToList();

                if (attributeValueHistories.Any())
                {
                    attributeValueHistories.ForEach(_ => history.Add(_.Year, _.Value));
                    history.MostRecentValue = attributeValueHistories.Last().Value;
                }
                else
                {
                    history.MostRecentValue = entitiesPerAttributeName[attributeName].Single(_ => _.Year == 0).Value;
                }
            });
        }

        public static void SetAttributeValueHistoryValues(this List<TextAttributeValueHistoryEntity> entities,
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

                var attributeValueHistories = entitiesPerAttributeName[attributeName]
                    .Where(_ => _.Year != 0).ToList();

                if (attributeValueHistories.Any())
                {
                    attributeValueHistories.ForEach(_ => history.Add(_.Year, _.Value));
                    history.MostRecentValue = attributeValueHistories.Last().Value;
                }
                else
                {
                    history.MostRecentValue = entitiesPerAttributeName[attributeName].Single(_ => _.Year == 0).Value;
                }
            });
        }*/

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

                if (attributeValueHistories.Count > 1)
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
