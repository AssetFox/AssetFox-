using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using AppliedResearchAssociates.iAM.DataAssignment.Segmentation;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Mappings;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL
{
    public class MaintainableAssetRepository : MSSQLRepository<MaintainableAsset>
    {
        public MaintainableAssetRepository(IAMContext context) : base(context) { }

        /*public override void AddAll(IEnumerable<MaintainableAsset> maintainableAssets, params object[] args)
        {
            if (!args.Any())
            {
                throw new NullReferenceException("No network was provided for given maintainable assets");
            }

            context.MaintainableAssets.AddRange(maintainableAssets.Select(d => d.ToEntity((Guid)args[0])));
        }*/

        public override IEnumerable<MaintainableAsset> Find(params object[] args)
        {
            if (!args.Any())
            {
                throw new NullReferenceException("No arguments found for maintainable assets query");
            }

            return context.MaintainableAssets
                .Where(m => m.NetworkId == (Guid)args[0])
                .Include(m => m.Location)
                .Include(m => m.AttributeData)
                .ThenInclude(a => a.Attribute)
                .Select(m => m.ToDomain());
        }
    }
}
