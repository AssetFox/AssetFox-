using AppliedResearchAssociates.iAM.DataMiner;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.LiteDb.Entities
{
    public class RouteEntity
    {
        public RouteEntity()
        {
        }

        public string Discriminator { get; set; }

        public Direction Direction { get; set; }

        public string LocationIdentifier { get; set; }
    }
}
