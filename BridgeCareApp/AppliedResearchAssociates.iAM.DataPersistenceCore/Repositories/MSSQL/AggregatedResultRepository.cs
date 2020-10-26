using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using AppliedResearchAssociates.iAM.DataAssignment.Aggregation;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappings;
using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL
{
    public class AggregatedResultRepository : MSSQLRepository, IAggregatedResultRepository
    {
        public AggregatedResultRepository(IAMContext context) : base(context) { }

        public int CreateAggregatedResults(List<IAggregatedResult> aggregatedResults)
        {
            DeleteAggregatedResults(aggregatedResults.First().MaintainableAsset.NetworkId);

            var entities = aggregatedResults.SelectMany(_ => _.ToEntity()).ToList();

            Context.BulkInsert(entities);
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

        private void DeleteAggregatedResults(Guid networkId)
        {
            if (!Context.Networks.Any(_ => _.Id == networkId))
            {
                throw new RowNotInTableException($"No network found having id {networkId}");
            }

            Context.Database.ExecuteSqlRaw(
                $"DELETE FROM dbo.AggregatedResults WHERE MaintainableAssetId IN (SELECT Id FROM dbo.MaintainableAssets WHERE NetworkId = '{networkId}')");
        }
    }
}
