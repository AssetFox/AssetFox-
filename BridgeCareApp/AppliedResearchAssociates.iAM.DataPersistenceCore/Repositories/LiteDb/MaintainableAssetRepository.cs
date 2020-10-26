using System;
using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.iAM.DataAssignment.Networking;
using AppliedResearchAssociates.iAM.DataPersistenceCore.LiteDb.Mappings;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.LiteDb.Entities;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.LiteDb
{
    public class MaintainableAssetRepository : LiteDbRepository, IMaintainableAssetRepository
    {
        public MaintainableAssetRepository(ILiteDbContext context) : base(context) { }

        public IEnumerable<MaintainableAsset> GetAllInNetworkWithAssignedDataAndLocations(Guid networkId) =>
            Context.Database.GetCollection<MaintainableAssetEntity>("MAINTAINABLE_ASSETS")
                .Include(_ => _.LocationEntity)
                .Find(_ => _.NetworkId == networkId)
                .Select(_ => _.ToDomain());
    }
}
