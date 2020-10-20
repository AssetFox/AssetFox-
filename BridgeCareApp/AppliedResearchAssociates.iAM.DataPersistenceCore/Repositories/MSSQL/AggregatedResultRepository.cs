using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using AppliedResearchAssociates.iAM.DataAssignment.Aggregation;
using AppliedResearchAssociates.iAM.DataPersistenceCore.MSSQL.Mappings;
using Microsoft.EntityFrameworkCore;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL
{
    public class AggregatedResultRepository : MSSQLRepository, IAggregatedResultRepository
    {
        public AggregatedResultRepository(IAMContext context) : base(context) { }

        public int CreateAggregatedResults<T>(List<AggregatedResult<T>> data)
        {
            var entities = data.Select(_ => _.ToEntity());
            Context.AggregatedResults.AddRange();
            Context.SaveChanges();
            return entities.Count();
        }

        public IEnumerable<IAggregatedResult> GetAggregatedResults(Guid networkId)
        {
            if (!Context.Networks.Any(_ => _.Id == networkId))
            {
                throw new RowNotInTableException($"No network found having id {networkId}");
            }

            var network = Context.Networks.Include(_ => _.MaintainableAssets)
                .ThenInclude(_ => _.AggregatedResults)
                .Single(_ => _.Id == networkId);

            return network == null
                ? new List<IAggregatedResult>()
                : network.MaintainableAssets.SelectMany(__ => __.AggregatedResults.ToDomain());
        }

        public int DeleteAggregatedResults(Guid networkId, List<Guid> metaDataAttributeIds, List<Guid> networkAttributeIds)
        {
            if (!Context.Networks.Any(_ => _.Id == networkId))
            {
                throw new RowNotInTableException($"No network found having id {networkId}");
            }

            var filteredAttributeIds = metaDataAttributeIds.Where(networkAttributeIds.Contains);

            var aggregatedResults = Context.MaintainableAssets
                .Include(_ => _.AggregatedResults)
                .Where(_ => _.NetworkId == networkId)
                .SelectMany(_ => _.AggregatedResults.Where(__ => filteredAttributeIds.Contains(__.AttributeId)))
                .ToList();

            if (!aggregatedResults.Any())
            {
                return 0;
            }

            aggregatedResults.ForEach(_ => Context.Entry(_).State = EntityState.Deleted);
            Context.SaveChanges();

            return aggregatedResults.Count();
        }
    }
}
