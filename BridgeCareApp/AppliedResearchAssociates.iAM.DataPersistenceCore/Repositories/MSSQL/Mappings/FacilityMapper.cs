using System;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.Domains;
using System.Linq;
using MoreLinq;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappings
{
    public static class FacilityMapper
    {
        public static FacilityEntity ToEntity(this Facility domain, Guid networkId) =>
            new FacilityEntity
            {
                Id = Guid.NewGuid(),
                NetworkId = networkId,
                Name = domain.Name
            };

        public static void ToSimulationAnalysisDomain(this FacilityEntity entity, Network network)
        {
            var facility = network.AddFacility();
            facility.Name = entity.Name;
            if (entity.Sections.Any())
            {
                entity.Sections.ForEach(_ => _.ToSimulationAnalysisDomain(facility));
            }
        }
    }
}
