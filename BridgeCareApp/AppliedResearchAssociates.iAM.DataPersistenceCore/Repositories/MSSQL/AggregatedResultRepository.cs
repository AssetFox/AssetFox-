﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using AppliedResearchAssociates.iAM.DataAssignment.Aggregation;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappings;
using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL
{
    public class AggregatedResultRepository : IAggregatedResultRepository
    {
        private readonly IAMContext _context;

        public AggregatedResultRepository(IAMContext context) =>
            _context = context ?? throw new ArgumentNullException(nameof(context));

        public int CreateAggregatedResults(List<IAggregatedResult> aggregatedResults)
        {
            DeleteAggregatedResults(aggregatedResults.First().MaintainableAsset.NetworkId);

            var entities = aggregatedResults.SelectMany(_ => _.ToEntity()).ToList();

            _context.BulkInsert(entities);

            return entities.Count();
        }

        public IEnumerable<IAggregatedResult> GetAggregatedResults(Guid networkId)
        {
            if (!_context.Network.Any(_ => _.Id == networkId))
            {
                throw new RowNotInTableException($"No network found having id {networkId}");
            }

            var maintainableAssets = _context.MaintainableAsset
                .Include(_ => _.AggregatedResults)
                .Where(_ => _.Id == networkId)
                .ToList();

            return !maintainableAssets.Any()
                ? throw new RowNotInTableException($"The network has no maintainable assets for rollup")
                : maintainableAssets.SelectMany(__ => __.AggregatedResults.ToDomain());
        }

        private void DeleteAggregatedResults(Guid networkId)
        {
            if (!_context.Network.Any(_ => _.Id == networkId))
            {
                throw new RowNotInTableException($"No network found having id {networkId}");
            }

            _context.Database.ExecuteSqlRaw(
                $"DELETE FROM dbo.AggregatedResult WHERE MaintainableAssetId IN (SELECT Id FROM dbo.MaintainableAsset WHERE NetworkId = '{networkId}')");
        }
    }
}
