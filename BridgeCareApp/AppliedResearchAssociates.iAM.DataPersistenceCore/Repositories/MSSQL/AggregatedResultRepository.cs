﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Extensions;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.Analysis;
using Microsoft.EntityFrameworkCore;
using MoreLinq;
using AppliedResearchAssociates.iAM.Data.Aggregation;
using System.Text;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL
{
    public class AggregatedResultRepository : IAggregatedResultRepository
    {
        private readonly UnitOfDataPersistenceWork _unitOfWork;

        public AggregatedResultRepository(UnitOfDataPersistenceWork unitOfWork) =>
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));

        public void AddAggregatedResults(List<IAggregatedResult> aggregatedResults)
        {
            DeleteAggregatedResults(aggregatedResults.First().MaintainableAsset.NetworkId);

            var entities = aggregatedResults.SelectMany(_ => _.ToEntity()).ToList();

            _unitOfWork.Context.AddAll(entities);
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
                ? throw new RowNotInTableException("The network has no maintainable assets for rollup")
                : maintainableAssets.SelectMany(__ => __.AggregatedResults.ToList().ToDomain(_unitOfWork.EncryptionKey));            
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
