using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using MoreLinq;
using AppliedResearchAssociates.iAM.DataAssignment.Networking;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Extensions;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers;
using AppliedResearchAssociates.iAM.Domains;
using Microsoft.EntityFrameworkCore;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL
{
    public class MaintainableAssetRepository : IMaintainableAssetRepository
    {
        private readonly UnitOfWork.UnitOfDataPersistenceWork _unitOfWork;

        public MaintainableAssetRepository(UnitOfWork.UnitOfDataPersistenceWork unitOfWork) => _unitOfWork = unitOfWork ??
                                         throw new ArgumentNullException(nameof(unitOfWork));

        public List<MaintainableAsset> GetAllInNetworkWithAssignedDataAndLocations(Guid networkId)
        {
            if (!_unitOfWork.Context.Network.Any(_ => _.Id == networkId))
            {
                throw new RowNotInTableException($"No network found having id {networkId}");
            }

            var maintainableAssets = _unitOfWork.Context.MaintainableAsset
                .Include(_ => _.MaintainableAssetLocation)
                .Include(_ => _.AssignedData)
                .ThenInclude(_ => _.Attribute)
                .Include(_ => _.AssignedData)
                .ThenInclude(_ => _.AttributeDatumLocation)
                .Where(_ => _.NetworkId == networkId)
                .ToList();

            var attributeId = Guid.Empty;

            var assets = (from ma in _unitOfWork.Context.MaintainableAsset
                          join mal in _unitOfWork.Context.MaintainableAssetLocation on ma.Id equals mal.MaintainableAssetId
                          join ad in _unitOfWork.Context.AttributeDatum on ma.Id equals ad.MaintainableAssetId
                          where ad.AttributeId == attributeId
                          select new MaintainableAssetEntity
                          {
                              Id = ma.Id,
                              NetworkId = ma.NetworkId,
                              MaintainableAssetLocation =
                                  new MaintainableAssetLocationEntity(mal.Id, mal.Discriminator, mal.LocationIdentifier)
                                  {
                                      MaintainableAssetId = mal.MaintainableAssetId
                                  },
                              AssignedData = (from adSub in _unitOfWork.Context.AttributeDatum
                                              where adSub.MaintainableAssetId == ma.Id
                                              select new AttributeDatumEntity
                                              {
                                                  Id = adSub.Id,
                                                  TimeStamp = adSub.TimeStamp,
                                                  NumericValue = adSub.NumericValue,
                                                  TextValue = adSub.TextValue,
                                                  Discriminator = adSub.Discriminator,
                                                  AttributeId = adSub.AttributeId,
                                                  MaintainableAssetId = adSub.MaintainableAssetId
                                              }).ToList()
                          });

            return !maintainableAssets.Any()
                ? throw new RowNotInTableException($"The network has no maintainable assets for rollup")
                : maintainableAssets.Select(_ => _.ToDomain()).ToList();
        }

        public void CreateMaintainableAssets(List<Section> sections, Guid networkId)
        {
            if (!_unitOfWork.Context.Network.Any(_ => _.Id == networkId))
            {
                throw new RowNotInTableException($"No network found having id {networkId}");
            }

            var attributeEntities = _unitOfWork.Context.Attribute.ToList();
            var attributeNames = attributeEntities.Select(_ => _.Name).ToList();
            var sectionAttributeNames = sections
                .SelectMany(_ => _.HistoricalAttributes.Select(__ => __.Name))
                .Distinct().ToList();
            if (sectionAttributeNames.Any() && !sectionAttributeNames.All(sectionAttributeName => attributeNames.Contains(sectionAttributeName)))
            {
                var missingAttributes = sectionAttributeNames.Except(attributeNames).ToList();
                if (missingAttributes.Count == 1)
                {
                    throw new RowNotInTableException($"No attribute found having name {missingAttributes[0]}.");
                }

                throw new RowNotInTableException(
                    $"No attributes found having names: {string.Join(", ", missingAttributes)}.");
            }

            var attributeIdPerName = attributeEntities.ToDictionary(_ => _.Name, _ => _.Id);

            var numericAttributeValueHistoryPerMaintainableAssetIdAttributeIdTuple =
                new Dictionary<(Guid sectionId, Guid attributeId), AttributeValueHistory<double>>();
            var textAttributeValueHistoryPerMaintainableAssetIdAttributeIdTuple =
                new Dictionary<(Guid sectionId, Guid attributeId), AttributeValueHistory<string>>();

            var maintainableAssetEntities = sections.Select(_ =>
            {
                var maintainableAssetEntity = _.ToEntity(networkId);

                if (_.HistoricalAttributes.Any())
                {
                    _.HistoricalAttributes.ForEach(attribute =>
                    {
                        if (attribute is NumberAttribute numberAttribute)
                        {
                            numericAttributeValueHistoryPerMaintainableAssetIdAttributeIdTuple.Add(
                                (_.Id, attributeIdPerName[numberAttribute.Name]), _.GetHistory(numberAttribute)
                            );
                        }

                        if (attribute is TextAttribute textAttribute)
                        {
                            textAttributeValueHistoryPerMaintainableAssetIdAttributeIdTuple.Add(
                                (_.Id, attributeIdPerName[textAttribute.Name]), _.GetHistory(textAttribute)
                            );
                        }
                    });
                }

                return maintainableAssetEntity;
            }).ToList();

            _unitOfWork.Context.AddAll(maintainableAssetEntities, _unitOfWork.UserEntity?.Id);

            var maintainableAssetLocationEntities =
                maintainableAssetEntities.Select(_ => _.CreateMaintainableAssetLocation()).ToList();

            _unitOfWork.Context.AddAll(maintainableAssetLocationEntities, _unitOfWork.UserEntity?.Id);

            if (numericAttributeValueHistoryPerMaintainableAssetIdAttributeIdTuple.Any())
            {
                _unitOfWork.AggregatedResultRepo.CreateAggregatedResults(
                    numericAttributeValueHistoryPerMaintainableAssetIdAttributeIdTuple);
            }

            if (textAttributeValueHistoryPerMaintainableAssetIdAttributeIdTuple.Any())
            {
                _unitOfWork.AggregatedResultRepo.CreateAggregatedResults(
                    textAttributeValueHistoryPerMaintainableAssetIdAttributeIdTuple);
            }
        }

        public void CreateMaintainableAssets(List<MaintainableAsset> maintainableAssets, Guid networkId)
        {
            if (!_unitOfWork.Context.Network.Any(_ => _.Id == networkId))
            {
                throw new RowNotInTableException($"No network found having id {networkId}");
            }

            var maintainableAssetEntities = maintainableAssets.Select(_ => _.ToEntity(networkId)).ToList();

            _unitOfWork.Context.AddAll(maintainableAssetEntities, _unitOfWork.UserEntity?.Id);

            var maintainableAssetLocationEntities = maintainableAssets
                .Select(_ => _.Location.ToEntity(_.Id, typeof(MaintainableAssetEntity))).ToList();

            _unitOfWork.Context.AddAll(maintainableAssetLocationEntities, _unitOfWork.UserEntity?.Id);
        }
    }
}
