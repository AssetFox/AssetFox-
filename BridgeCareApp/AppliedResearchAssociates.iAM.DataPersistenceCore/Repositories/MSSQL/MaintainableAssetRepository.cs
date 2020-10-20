using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using AppliedResearchAssociates.iAM.DataAssignment.Networking;
using AppliedResearchAssociates.iAM.DataPersistenceCore.MSSQL.Mappings;
using Microsoft.EntityFrameworkCore;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL
{
    public class MaintainableAssetRepository : MSSQLRepository, IMaintainableAssetRepository
    {
        public MaintainableAssetRepository(IAMContext context) : base(context) { }

        public IEnumerable<MaintainableAsset> GetAllInNetwork(Guid networkId)
        {
            if (!Context.Networks.Any(_ => _.Id == networkId))
            {
                throw new RowNotInTableException($"No network found having id {networkId}");
            }

            var network = Context.Networks.Include(_ => _.MaintainableAssets)
                .Single(_ => _.Id == networkId);

            return network == null
                ? new List<MaintainableAsset>()
                : network.MaintainableAssets.Select(_ => _.ToDomain());
        }
    }
}
