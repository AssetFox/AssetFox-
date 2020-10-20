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

        public IEnumerable<Attribute> GetAttributesFromNetwork(Guid networkId) => throw new NotImplementedException();

        public int UpdateAssignedData(Network network)
        {
            DeleteAssignedDataFromNetwork(network.Id);

            var attributeDatumEntities = network.MaintainableAssets
                    .SelectMany(_ => _.AssignedData.Select(__ => __.ToEntity(_.Id)))
                    .ToList();

            Context.AttributeData.AddRange(attributeDatumEntities);
            Context.SaveChanges();

            return attributeDatumEntities.Count();
        }

        public int DeleteAssignedDataFromNetwork(Guid networkId)
        {
            if (Context.Networks.Any(_ => _.Id == networkId))
            {
                throw new RowNotInTableException($"No network found having id {networkId}");
            }

            var network = Context.Networks.Include(_ => _.MaintainableAssets)
                .ThenInclude(_ => _.AggregatedResults)
                .Single(_ => _.Id == networkId);

            if (network == null)
            {
                return 0;
            }

            var assignedData = network.MaintainableAssets.SelectMany(_ => _.AggregatedResults).ToList();

            assignedData.ForEach(_ => Context.Entry(_).State = EntityState.Deleted);
            Context.SaveChanges();

            return assignedData.Count();
        }
    }
}
