using System;
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
using AppliedResearchAssociates.iAM.DTOs;

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
            Dictionary<(Guid maintainableAssetId, Guid attributeId), IAttributeValueHistory<T>>
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

        public List<AggregatedResultDTO> GetAggregatedResultsForAttributeNames(Guid networkId, List<string> attributeNames)
        {
            return _unitOfWork.Context.AggregatedResult
                .Include(_ => _.MaintainableAsset)
                .Include(_ => _.Attribute)
                .Where(_ => attributeNames.Contains(_.Attribute.Name) && _.MaintainableAsset.NetworkId == networkId)
                .Select(e => AggregatedResultMapper.ToDto(e))
                .AsNoTracking().AsSplitQuery().ToList();
        }

        /*public List<AggregatedResultDTO> GetAggregatedResultsForAttributeNames(List<string> attributeNames)
        {
            return _unitOfWork.Context.AggregatedResult
                .Include(_ => _.MaintainableAsset)
                .Include(_ => _.Attribute)
                .Where(_ => attributeNames.Contains(_.Attribute.Name))
                .Select(e => AggregatedResultMapper.ToDto(e))
                .AsNoTracking().AsSplitQuery().ToList();
        }*/

        public (string, string, List<string>, bool) GetAggregatedResultsForAttributeNames(List<string> attributeNames)
        {
            var allOfAttributeDTOs = _unitOfWork.Context.AggregatedResult
                .Include(_ => _.Attribute)
                .Where(_ => attributeNames.Contains(_.Attribute.Name))
                //.Where(_ => _.Attribute.Name == attributeNames[0])
                .Select(e => AggregatedResultMapper.ToDto(e))
                .AsNoTracking().AsSplitQuery().ToList();

            var values = new List<string>();
            bool isNumber = false;
            if (allOfAttributeDTOs.All(x => x.Discriminator == DataPersistenceConstants.AggregatedResultNumericDiscriminator))
            {
                values = allOfAttributeDTOs.Where(_ => _.NumericValue.HasValue).Select(_ => _.NumericValue.Value.ToString()).Distinct().ToList();
                isNumber = allOfAttributeDTOs.Any(_ => _.Attribute.Type == "NUMBER");
            }
            else if (allOfAttributeDTOs.All(x => x.Discriminator == DataPersistenceConstants.AggregatedResultTextDiscriminator))
            {
                values = allOfAttributeDTOs.Where(_ => _.TextValue != null).Select(_ => _.TextValue).Distinct().ToList();
                isNumber = allOfAttributeDTOs.Any(_ => _.Attribute.Type == "NUMBER");
            }

            string name = allOfAttributeDTOs.Select(_ => _.Attribute.Name).FirstOrDefault();
            string resultType = !values.Any() ? "warning" : "success";
            return new (name, resultType, values, isNumber);
        }

        public List<AggregatedResultDTO> GetAggregatedResultsForMaintainableAsset(Guid assetId, List<Guid> attributeIds)
        {
            var entities = _unitOfWork.Context.AggregatedResult.AsSplitQuery().AsNoTracking().Include(_ => _.Attribute)
                    .Where(_ => _.MaintainableAssetId == assetId).ToList().Where(_ => attributeIds.Contains(_.AttributeId)).ToList();
            return entities.Select(AggregatedResultMapper.ToDto).ToList();
        }
    }
}
