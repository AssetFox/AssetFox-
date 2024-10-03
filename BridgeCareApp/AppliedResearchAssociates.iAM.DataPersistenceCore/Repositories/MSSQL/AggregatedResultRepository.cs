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
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.Cache;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL
{
    public class AggregatedResultRepository : IAggregatedResultRepository
    {
        private readonly UnitOfDataPersistenceWork _unitOfWork;
        private static AggregatedSelectValuesResultDtoCache _aggregatedResultCache = new();

        public AggregatedResultRepository(
            UnitOfDataPersistenceWork unitOfWork
            )
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

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

        public List<AggregatedSelectValuesResultDTO> GetAggregatedResultsForAttributeNames(List<string> attributeNames)
        {
            List<AggregatedSelectValuesResultDTO> returnList = new();
            var uniqueAttributes = attributeNames.Distinct().ToList();
            var attributesToFetch = new List<string>();
            foreach (var attributeName in uniqueAttributes)
            {
                var cached = _aggregatedResultCache.TryGetCachedValue(attributeName);
                if (cached == null)
                {
                    attributesToFetch.Add(attributeName);
                } else
                {
                    returnList.Add(cached);
                }
            }
            if (attributesToFetch.Count > 0)
            {
                var allOfAttributeDTOs = _unitOfWork.Context.AggregatedResult
                    .Include(_ => _.Attribute)
                    .Where(_ => attributesToFetch.Contains(_.Attribute.Name))
                    .Select(e => AggregatedResultMapper.ToDto(e))
                    .AsNoTracking().AsSplitQuery().ToList();

                foreach (var attributeName in attributesToFetch)
                {
                    var attributeDTO = allOfAttributeDTOs.Where(_ => _.Attribute.Name == attributeName).ToList();

                    if (!attributeDTO.Any())
                        break;

                    var values = new List<string>();
                    bool isNumber = false;
                    if (attributeDTO.All(x => x.Discriminator == DataPersistenceConstants.AggregatedResultNumericDiscriminator))
                    {
                        values = attributeDTO.Where(_ => _.NumericValue.HasValue).Select(_ => _.NumericValue.Value.ToString()).Distinct().ToList();
                        isNumber = attributeDTO.Any(_ => _.Attribute.Type == "NUMBER");
                    }
                    else if (attributeDTO.All(x => x.Discriminator == DataPersistenceConstants.AggregatedResultTextDiscriminator))
                    {
                        values = attributeDTO.Where(_ => _.TextValue != null).Select(_ => _.TextValue).Distinct().ToList();
                        isNumber = attributeDTO.Any(_ => _.Attribute.Type == "NUMBER");
                    }
                    else
                        break;

                    AttributeDTO attr = attributeDTO.Select(_ => _.Attribute).FirstOrDefault();
                    string resultType = values.Any() ? "success" : "warning";
                    AggregatedSelectValuesResultDTO returnResult = new()
                    {
                        Attribute = attr,
                        Values = values,
                        ResultType = resultType,
                        IsNumber = isNumber
                    };
                    returnList.Add(returnResult);
                    _aggregatedResultCache.SaveToCache(returnResult);
                }
            }
            _aggregatedResultCache.ClearInvalid();
            return returnList;
        }

        public List<AggregatedResultDTO> GetAggregatedResultsForMaintainableAsset(Guid assetId, List<Guid> attributeIds)
        {
            var entities = _unitOfWork.Context.AggregatedResult.AsSplitQuery().AsNoTracking().Include(_ => _.Attribute)
                    .Where(_ => _.MaintainableAssetId == assetId).ToList().Where(_ => attributeIds.Contains(_.AttributeId)).ToList();
            return entities.Select(AggregatedResultMapper.ToDto).ToList();
        }

        public List<AggregatedResultDTO> GetAllAggregatedResultsForMaintainableAsset(Guid assetId)
        {
            var entities = _unitOfWork.Context.AggregatedResult.AsSplitQuery().AsNoTracking().Include(_ => _.Attribute)
                    .Where(_ => _.MaintainableAssetId == assetId).ToList();
            return entities.Select(AggregatedResultMapper.ToDto).ToList();
        }

        public List<AggregatedResultDTO> GetAllAggregatedResultsForNetwork(Guid networkId)
        {
            return _unitOfWork.Context.AggregatedResult
                .Include(_ => _.MaintainableAsset)
                .Include(_ => _.Attribute)
                .Where(_ => _.MaintainableAsset.NetworkId == networkId)
                .Select(e => AggregatedResultMapper.ToDto(e))
                .AsNoTracking().AsSplitQuery().ToList();
        }

        /// <summary>
        /// Gets a dictionary of AssetAttributeValuePair lists for each MaintainableAsset in a given network
        /// </summary>
        /// <paramref name="networkId">
        /// GUID for Network
        /// </paramref>
        /// <returns>
        /// List of AttributeName, AttributeValue for each MaintainableAsset
        /// </returns>
        /// <seealso cref="AssetAttributeValuePair"/>
        public Dictionary<Guid, List<AssetAttributeValuePair>> GetAssetAttributeValuePairDictionary(Guid networkId)
        {
            return _unitOfWork.Context.MaintainableAsset
                    .AsSplitQuery()
                    .AsNoTracking()
                    .Where(_ => _.NetworkId == networkId)
                    .Join(_unitOfWork.Context.AggregatedResult,
                          maintainableAsset => maintainableAsset.Id,
                          aggregatedResult => aggregatedResult.MaintainableAssetId,
                          (maintainableAsset, aggregatedResult) => new { maintainableAsset, aggregatedResult })
                    .Join(_unitOfWork.Context.Attribute,
                          combined => combined.aggregatedResult.AttributeId,
                          attribute => attribute.Id,
                          (combined, attribute) => new { combined, attribute })
                    .Select(_ => new
                    {
                        MaintainableAssetId = _.combined.maintainableAsset.Id,
                        AttributeName = _.attribute.Name,
                        AttributeValue = _.combined.aggregatedResult.TextValue != null ? _.combined.aggregatedResult.TextValue : (_.combined.aggregatedResult.NumericValue != null ? _.combined.aggregatedResult.NumericValue.ToString() : null)
                    })
                    .AsEnumerable()
                    .OrderBy(_ => _.AttributeName)
                    .GroupBy(_ => _.MaintainableAssetId)
                    .ToDictionary(_ => _.Key, _ => _.Select(__ => new AssetAttributeValuePair { AttributeName = __.AttributeName, AttributeValue = __.AttributeValue }).ToList());
        }
    }
}
