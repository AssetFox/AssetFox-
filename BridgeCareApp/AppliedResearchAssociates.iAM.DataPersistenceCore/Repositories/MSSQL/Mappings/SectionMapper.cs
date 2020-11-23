using System;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.Domains;

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

        public static void ToSimulationAnalysisDomain(this SectionEntity entity, Facility facility)
        {
            var section = facility.AddSection();
            section.Name = entity.Name;
            section.Area = entity.Area;
            section.AreaUnit = entity.AreaUnit;
        }
    }
}
