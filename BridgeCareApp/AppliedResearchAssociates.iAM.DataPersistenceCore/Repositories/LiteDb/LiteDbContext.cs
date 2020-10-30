using LiteDB;
using Microsoft.Extensions.Options;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.LiteDb
{
    public class LiteDbContext : ILiteDbContext
    {
        public LiteDatabase Database { get; }

        public LiteDbContext(IOptions<LiteDbOptions> options)
        {
            var mapper = new BsonMapper();
            mapper.Entity<Entities.NetworkEntity>()
                .DbRef(_ => _.MaintainableAssetEntities, "MAINTAINABLE_ASSETS");

            mapper.Entity<Entities.MaintainableAssetEntity>()
                .DbRef(_ => _.LocationEntity, "LOCATIONS");

            Database = new LiteDatabase(options.Value.LiteDbFilePath, mapper);
        }
    }
}
