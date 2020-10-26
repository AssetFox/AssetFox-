using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using AppliedResearchAssociates.iAM.DataAssignment.Networking;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappings;
using Microsoft.EntityFrameworkCore;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL
{
    public class MaintainableAssetRepository : MSSQLRepository, IMaintainableAssetRepository
    {
        public MaintainableAssetRepository(IAMContext context) : base(context) { }

        public IEnumerable<MaintainableAsset> GetAllInNetworkWithAssignedDataAndLocations(Guid networkId)
        {
            if (!Context.Networks.Any(_ => _.Id == networkId))
            {
                throw new RowNotInTableException($"No network found having id {networkId}");
            }

            var maintainableAssets = Context.MaintainableAssets.Include(_ => _.Location)
                .Include(_ => _.AttributeData)
                .ThenInclude(_ => _.Attribute)
                .Include(_ => _.AttributeData)
                .ThenInclude(_ => _.Location)
                .Where(_ => _.NetworkId == networkId)
                .ToList();

            if (!maintainableAssets.Any())
            {
                throw new RowNotInTableException($"The network has no maintainable assets for rollup");
            }

            return maintainableAssets.Select(_ => _.ToDomain());
        }
    }
}
