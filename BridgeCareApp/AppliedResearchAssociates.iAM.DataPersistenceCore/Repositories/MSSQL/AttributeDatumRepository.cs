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

        public int UpdateAssignedData(Network network)
        {
            var attributeDatumEntities = network.MaintainableAssets
                    .SelectMany(_ => _.AssignedData.Select(__ => __.ToEntity(_.Id)))
                    .ToList();

            Context.AttributeData.AddRange(attributeDatumEntities);
            Context.SaveChanges();

            return attributeDatumEntities.Count();
        }

        public int DeleteAssignedDataFromNetwork(Guid networkId, List<Guid> existingAttributeIds)
        {
            if (Context.Networks.Any(_ => _.Id == networkId))
            {
                throw new RowNotInTableException($"No network found having id {networkId}");
            }

            var assignedData = Context.MaintainableAssets
                .Include(_ => _.AttributeData)
                .Where(_ => _.Id == networkId)
                .SelectMany(_ => _.AttributeData.Where(__ => existingAttributeIds.Contains(__.AttributeId)))
                .ToList();

            if (!assignedData.Any())
            {
                return 0;
            }

            assignedData.ForEach(_ => Context.Entry(_).State = EntityState.Deleted);
            Context.SaveChanges();

            return assignedData.Count();
        }
    }
}
