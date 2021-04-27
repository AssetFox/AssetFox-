using System.Linq;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.Domains;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers
{
    public static class SectionMapper
    {
        public static SectionEntity ToEntity(this Section domain) =>
            new SectionEntity
            {
                Id = domain.Id,
                FacilityId = domain.Facility.Id,
                Name = domain.Name,
                SpatialWeighting = domain.SpatialWeighting.ToEntity()
                //Area = domain.Area,
                //AreaUnit = domain.AreaUnit
            };

        /*public static void CreateSection(this SectionEntity entity, Facility facility)
        {
            var section = facility.AddSection();
            section.Id = entity.Id;
            section.Name = entity.Name;
            section.Area = entity.Area;
            section.AreaUnit = entity.AreaUnit;

            if (entity.NumericAttributeValueHistories.Any())
            {
                entity.NumericAttributeValueHistories.ToList().SetAttributeValueHistoryValues(section);
            }

            /*if (entity.NumericAttributeValueHistoryMostRecentValues.Any())
            {
                entity.NumericAttributeValueHistoryMostRecentValues.ToList().SetAttributeValueHistoryMostRecentValue(section);
            }#1#

            if (entity.TextAttributeValueHistories.Any())
            {
                entity.TextAttributeValueHistories.ToList().SetAttributeValueHistoryValues(section);
            }

            /*if (entity.TextAttributeValueHistoryMostRecentValues.Any())
            {
                entity.TextAttributeValueHistoryMostRecentValues.ToList().SetAttributeValueHistoryMostRecentValue(section);
            }#1#
        }*/

        public static void CreateSection(this MaintainableAssetEntity entity, Facility facility, string sectionName)
        {
            var section = facility.AddSection();
            section.Id = entity.Id;
            section.Name = sectionName;
            section.SpatialWeighting.Expression = entity.SpatialWeighting;

            if (entity.AggregatedResults.Any(_ => _.Discriminator == DataPersistenceConstants.AggregatedResultNumericDiscriminator))
            {
                entity.AggregatedResults
                    .Where(_ => _.Discriminator == DataPersistenceConstants.AggregatedResultNumericDiscriminator)
                    .ToList()
                    .SetNumericAttributeValueHistories(section);
            }

            if (entity.AggregatedResults.Any(_ => _.Discriminator == DataPersistenceConstants.AggregatedResultTextDiscriminator))
            {
                entity.AggregatedResults
                    .Where(_ => _.Discriminator == DataPersistenceConstants.AggregatedResultTextDiscriminator).ToList()
                    .SetTextAttributeValueHistories(section);
            }
        }
    }
}
