using System;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.Domains;
using Microsoft.EntityFrameworkCore.Internal;
using System.Linq;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappings
{
    public static class SectionMapper
    {
        public static SectionEntity ToEntity(this Section domain, Guid facilityId) =>
            new SectionEntity
            {
                Id = Guid.NewGuid(),
                FacilityId = facilityId,
                Name = domain.Name,
                Area = domain.Area,
                AreaUnit = domain.AreaUnit
            };

        public static void CreateSection(this SectionEntity entity, Facility facility)
        {
            var section = facility.AddSection();
            section.Name = entity.Name;
            section.Area = entity.Area;
            section.AreaUnit = entity.AreaUnit;

            if (entity.NumericAttributeValueHistories.Any())
            {
                entity.NumericAttributeValueHistories.ToList().SetAttributeValueHistoryValues(section);
            }

            if (entity.NumericAttributeValueHistoryMostRecentValues.Any())
            {
                entity.NumericAttributeValueHistoryMostRecentValues.ToList().SetAttributeValueHistoryMostRecentValue(section);
            }

            if (entity.TextAttributeValueHistories.Any())
            {
                entity.TextAttributeValueHistories.ToList().SetAttributeValueHistoryValues(section);
            }

            if (entity.TextAttributeValueHistoryMostRecentValues.Any())
            {
                entity.TextAttributeValueHistoryMostRecentValues.ToList().SetAttributeValueHistoryMostRecentValue(section);
            }
        }
    }
}
