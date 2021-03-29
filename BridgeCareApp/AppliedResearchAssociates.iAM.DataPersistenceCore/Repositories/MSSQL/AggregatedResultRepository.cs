﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using MoreLinq;
using AppliedResearchAssociates.iAM.DataAssignment.Aggregation;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Extensions;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers;
using AppliedResearchAssociates.iAM.Domains;
using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL
{
    public class AggregatedResultRepository : IAggregatedResultRepository
    {
        private readonly UnitOfWork.UnitOfDataPersistenceWork _unitOfWork;

        public AggregatedResultRepository(UnitOfWork.UnitOfDataPersistenceWork unitOfWork) =>
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));

        public int CreateAggregatedResults(List<IAggregatedResult> aggregatedResults)
        {
            DeleteAggregatedResults(aggregatedResults.First().MaintainableAsset.NetworkId);

            var entities = aggregatedResults.SelectMany(_ => _.ToEntity()).ToList();

            _unitOfWork.Context.BulkInsert(entities);
            _unitOfWork.Context.SaveChanges();

            return entities.Count();
        }

        public IEnumerable<IAggregatedResult> GetAggregatedResults(Guid networkId)
        {
            if (!_unitOfWork.Context.Network.Any(_ => _.Id == networkId))
            {
                throw new RowNotInTableException($"No network found having id {networkId}");
            }

            var maintainableAssets = _unitOfWork.Context.MaintainableAsset
                .Include(_ => _.AggregatedResults)
                .Where(_ => _.Id == networkId)
                .ToList();

            return !maintainableAssets.Any()
                ? throw new RowNotInTableException($"The network has no maintainable assets for rollup")
                : maintainableAssets.SelectMany(__ => __.AggregatedResults.ToList().ToDomain());
        }

        private void DeleteAggregatedResults(Guid networkId)
        {
            if (!_unitOfWork.Context.Network.Any(_ => _.Id == networkId))
            {
                throw new RowNotInTableException($"No network found having id {networkId}");
            }

            _unitOfWork.Context.Database.ExecuteSqlRaw(
                $"DELETE FROM dbo.AggregatedResult WHERE MaintainableAssetId IN (SELECT Id FROM dbo.MaintainableAsset WHERE NetworkId = '{networkId}')");
            _unitOfWork.Context.SaveChanges();
        }

        public void CreateAggregatedResults<T>(
            Dictionary<(Guid maintainableAssetId, Guid attributeId), AttributeValueHistory<T>>
                attributeValueHistoryPerMaintainableAssetIdAttributeIdTuple)
        {

            var aggregatedResultEntities = new List<AggregatedResultEntity>();

            attributeValueHistoryPerMaintainableAssetIdAttributeIdTuple.Keys.ForEach(tuple =>
            {
                var attributeValueHistory = attributeValueHistoryPerMaintainableAssetIdAttributeIdTuple[tuple];
                aggregatedResultEntities.AddRange(attributeValueHistory.ToEntity(tuple.maintainableAssetId, tuple.attributeId));
            });

            _unitOfWork.Context.AddAll(aggregatedResultEntities, _unitOfWork.UserEntity?.Id);
        }
    }
}
