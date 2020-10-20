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

        public IEnumerable<MaintainableAsset> GetAllInNetworkWithAssignedData(Guid networkId)
        {
            if (!Context.Networks.Any(_ => _.Id == networkId))
            {
                throw new RowNotInTableException($"No network found having id {networkId}");
            }

            var maintainableAssets = Context.MaintainableAssets
                .Include(_ => _.AttributeData)
                .ThenInclude(_ => _.Attribute)
                .Where(_ => _.Id == networkId);

            if (!maintainableAssets.Any())
            {
                throw new RowNotInTableException($"Could not find maintainable assets with assigned data");
            }

            return maintainableAssets.Select(_ => _.ToDomain());
        }
    }
}
