using System;
using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.iAM.DataAssignment.Segmentation;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Mappings;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Interfaces;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL
{
    public class MaintainableAssetRepository : MSSQLRepository<MaintainableAsset, MaintainableAssetEntity>, IMaintainableAssetRepository
    {
        public MaintainableAssetRepository(IAMContext context) : base(context)
        {
        }

        public void AddNetworkSegments(IEnumerable<MaintainableAsset> maintainableAssets, Guid networkId) => context.MaintainableAssets.AddRange(maintainableAssets.Select(d => d.ToEntity(networkId)));
    }
}
