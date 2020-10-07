using System;
using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.iAM.DataAssignment.Segmentation;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Mappings;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL
{
    public class MaintainableAssetRepository : MSSQLRepository<MaintainableAsset>, IMaintainableAssetRepository
    {
        public MaintainableAssetRepository(IAMContext context) : base(context)
        {
        }

        public void AddNetworkMaintainableAssets(IEnumerable<MaintainableAsset> maintainableAssets, Guid networkId) => context.MaintainableAssets.AddRange(maintainableAssets.Select(d => d.ToEntity(networkId)));

        public IEnumerable<Segment> GetNetworkSegmentsWithAssignedData(Guid networkId) => context.Segments.Where(s => s.NetworkId == networkId)
            .Include(s => s.Location)
            .Include(s => s.AttributeData)
            .ThenInclude(a => a.Attribute)
            .Select(s => s.ToDomain());
    }
}
