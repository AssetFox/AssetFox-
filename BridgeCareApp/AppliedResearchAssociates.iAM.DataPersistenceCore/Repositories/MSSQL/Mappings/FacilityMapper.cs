using System;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.Domains;
using System.Linq;
using MoreLinq;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappings
{
    public static class FacilityMapper
    {
        public static FacilityEntity ToEntity(this Facility domain) =>
            new FacilityEntity
            {
                Id = domain.Id,
                NetworkId = domain.Network.Id,
                Name = domain.Name
            };

        public static void CreateFacility(this FacilityEntity entity, Network network)
        {
            var facility = network.AddFacility();
            facility.Id = entity.Id;
            facility.Name = entity.Name;

            if (entity.Sections.Any())
            {
                entity.Sections.ForEach(_ => _.CreateSection(facility));
            }
        }
    }
}
