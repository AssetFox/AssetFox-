using System;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.Domains;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers
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

        /*public static void CreateFacility(this FacilityEntity entity, Network network)
        {
            var facility = network.AddFacility();
            facility.Id = entity.Id;
            facility.Name = entity.Name;

            if (entity.Sections.Any())
            {
                entity.Sections.ForEach(_ => _.CreateSection(facility));
            }
        }*/

        public static void CreateFacility(this MaintainableAssetEntity entity, Network network)
        {
            var facilitySectionNameSplit =
                entity.MaintainableAssetLocation.LocationIdentifier.Split("-");

            var facility = network.AddFacility();
            facility.Id = Guid.NewGuid();
            facility.Name = facilitySectionNameSplit[0];

            entity.CreateSection(facility, facilitySectionNameSplit[1]);
        }
    }
}
