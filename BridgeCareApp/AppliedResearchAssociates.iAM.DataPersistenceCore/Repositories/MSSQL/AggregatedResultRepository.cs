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

        public int CreateAggregatedResults<T>(IEnumerable<AggregatedResult<T>> data)
        {
            var entities = data.SelectMany(_ => _.ToEntity());
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

        public int DeleteAggregatedResults(Guid networkId)
        {
            if (!Context.Networks.Any(_ => _.Id == networkId))
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

            var aggregatedResults = network.MaintainableAssets.SelectMany(__ => __.AggregatedResults);

            var aggregatedResultEntities = aggregatedResults.ToList();
            aggregatedResultEntities.ForEach(_ => Context.Entry(_).State = EntityState.Deleted);
            Context.SaveChanges();

            return aggregatedResultEntities.Count();
        }
    }
}
