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

        public List<AggregatedSelectValuesResultDTO> GetAggregatedResultsForAttributeNames(List<string> attributeNames)
        {
            List<AggregatedSelectValuesResultDTO> returnList = new();
            var uniqueAttributeNames = attributeNames.Distinct().ToList();
            var abbreviatedAttributes = _unitOfWork.Context.Attribute.Where(a => uniqueAttributeNames.Contains(a.Name))
                .Select(AttributeMapper.ToAbbreviatedDto)
                .ToList();
            var abbreviatedAttributeDtoDictionary = abbreviatedAttributes.ToDictionary(a => a.Name, a => a);
            var allAttributeValueDtos = _unitOfWork.Context.AggregatedResult
                .Include(_ => _.Attribute)
                .Where(_ => attributeNames.Contains(_.Attribute.Name))
                .Select(e => new
                {
                    AttributeName = e.Attribute.Name,
                    e.Discriminator,
                    e.TextValue,
                    e.NumericValue,
                })
                .Distinct()
                .AsNoTracking()
                .AsSplitQuery()
                .ToList();
            foreach (var attributeName in uniqueAttributeNames)
            {
                var abbreviatedAttributeDto = abbreviatedAttributes.FirstOrDefault(a => a.Name == attributeName);
                if (abbreviatedAttributeDto == null)
                {
                    continue;
                }
                var thisAttributeValueDtos = allAttributeValueDtos.Where(_ => _.AttributeName == attributeName).ToList();

                if (!thisAttributeValueDtos.Any())
                    continue;

                var values = new List<string>();
                bool isNumber = false;
                if (thisAttributeValueDtos.All(x => x.Discriminator == DataPersistenceConstants.AggregatedResultNumericDiscriminator))
                {
                    values = thisAttributeValueDtos.Where(_ => _.NumericValue.HasValue).Select(_ => _.NumericValue.Value.ToString()).Distinct().ToList();
                    isNumber = thisAttributeValueDtos.Any(_ => abbreviatedAttributeDtoDictionary[_.AttributeName].Type == "NUMBER");
                }
                else if (thisAttributeValueDtos.All(x => x.Discriminator == DataPersistenceConstants.AggregatedResultTextDiscriminator))
                {
                    values = thisAttributeValueDtos.Where(_ => _.TextValue != null).Select(_ => _.TextValue).Distinct().ToList();
                    isNumber = thisAttributeValueDtos.Any(_ => abbreviatedAttributeDtoDictionary[_.AttributeName].Type == "NUMBER");
                }
                else
                    continue;

                AttributeDTO attr = thisAttributeValueDtos.Select(_ => abbreviatedAttributeDtoDictionary[_.AttributeName]).FirstOrDefault();
                string resultType = values.Any() ? "success" : "warning";
                AggregatedSelectValuesResultDTO returnResult = new()
                {
                    Attribute = attr,
                    Values = values,
                    ResultType = resultType,
                    IsNumber = isNumber
                };
                returnList.Add(returnResult);
            }

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
