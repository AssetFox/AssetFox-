using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using AppliedResearchAssociates.iAM.DataAssignment.Networking;
using AppliedResearchAssociates.iAM.DataMiner.Attributes;
using AppliedResearchAssociates.iAM.DataPersistenceCore.MSSQL.Mappings;
using Microsoft.EntityFrameworkCore;
using MoreLinq.Extensions;
using Attribute = AppliedResearchAssociates.iAM.DataMiner.Attributes.Attribute;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL
{
    public class AttributeDatumRepository : MSSQLRepository, IAttributeDatumRepository
    {
        public AttributeDatumRepository(IAMContext context) : base(context) { }

        public int UpdateMaintainableAssetAssignedData(IEnumerable<MaintainableAsset> maintainableAssets)
        {
            //var filteredAttributeIds = metaDataAttributeIds.Where(networkAttributeIds.Contains);

            //var assignedData = Context.MaintainableAssets
            //    .Include(_ => _.AttributeData)
            //    .Where(_ => _.Id == network.Id)
            //    .SelectMany(_ => _.AttributeData.Where(__ => filteredAttributeIds.Contains(__.AttributeId)))
            //    .ToList();

            //if (!assignedData.Any())
            //{
            //    return 0;
            //}

            //assignedData.ForEach(_ => Context.Entry(_).State = EntityState.Deleted);
            //Context.SaveChanges();
            //return assignedData.Count();
            return 1;
        }
    }
}
