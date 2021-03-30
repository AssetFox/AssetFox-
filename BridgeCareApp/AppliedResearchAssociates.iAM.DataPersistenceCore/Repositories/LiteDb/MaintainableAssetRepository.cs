using System;
using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.iAM.DataAssignment.Networking;
using AppliedResearchAssociates.iAM.DataPersistenceCore.LiteDb.Mappings;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.LiteDb.Entities;
using AppliedResearchAssociates.iAM.Domains;

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

        public void CreateMaintainableAssets(List<MaintainableAsset> maintainableAssets, Guid networkId) => throw new NotImplementedException();

        public void CreateMaintainableAssets(List<Section> sections, Guid networkId) => throw new NotImplementedException();
        public void UpdateMaintainableAssetsSpatialWeighting(List<MaintainableAsset> maintainableAssets) => throw new NotImplementedException();

        List<MaintainableAsset> IMaintainableAssetRepository.GetAllInNetworkWithAssignedDataAndLocations(Guid networkId) => throw new NotImplementedException();
    }
}
