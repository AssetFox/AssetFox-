﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using AppliedResearchAssociates.iAM.DataAssignment.Networking;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Extensions;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers;
using AppliedResearchAssociates.iAM.Domains;
using AppliedResearchAssociates.iAM.DTOs;
using EFCore.BulkExtensions;
using MoreLinq;

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
                throw new RowNotInTableException($"No network found having id {networkId}.");
            }

            if (!_unitOfWork.Context.MaintainableAsset.Any(_ => _.NetworkId == networkId))
            {
                throw new RowNotInTableException($"The network has no maintainable assets for rollup.");
            }

            var assets = _unitOfWork.Context.MaintainableAsset
                .Where(_ => _.NetworkId == networkId)
                .Select(asset => new MaintainableAssetEntity
                {
                    Id = asset.Id,
                    NetworkId = networkId,
                    SpatialWeighting = asset.SpatialWeighting,
                    //Area = asset.Area,
                    //AreaUnit = asset.AreaUnit,
                    MaintainableAssetLocation = new MaintainableAssetLocationEntity(
                        asset.MaintainableAssetLocation.Id, asset.MaintainableAssetLocation.Discriminator,
                        asset.MaintainableAssetLocation.LocationIdentifier),
                    AssignedData = asset.AssignedData.Select(datum => new AttributeDatumEntity
                    {
                        Id = datum.Id,
                        NumericValue = datum.NumericValue,
                        TextValue = datum.TextValue,
                        Discriminator = datum.Discriminator,
                        AttributeDatumLocation = new AttributeDatumLocationEntity(datum.AttributeDatumLocation.Id,
                            datum.AttributeDatumLocation.Discriminator,
                            datum.AttributeDatumLocation.LocationIdentifier),
                        Attribute = new AttributeEntity
                        {
                            Id = datum.Attribute.Id,
                            Name = datum.Attribute.Name,
                            Minimum = datum.Attribute.Minimum,
                            Maximum = datum.Attribute.Maximum,
                            AggregationRuleType = datum.Attribute.AggregationRuleType,
                            Command = datum.Attribute.Command,
                            ConnectionType = datum.Attribute.ConnectionType,
                            IsCalculated = datum.Attribute.IsCalculated,
                            IsAscending = datum.Attribute.IsAscending,
                            DataType = datum.Attribute.DataType
                        }
                    }).ToList()
                })
                .ToList();

            return assets.Select(_ => _.ToDomain()).ToList();
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

        public void UpdateMaintainableAssetsSpatialWeighting(List<MaintainableAsset> maintainableAssets)
        {
            var networkId = maintainableAssets.First().NetworkId;
            var maintainableAssetEntities = maintainableAssets.Select(_ => _.ToEntity(networkId)).ToList();

            var propsToExclude = new List<string> { "CreatedDate", "CreatedBy", "FacilityName", "SectionName" };
            var config = new BulkConfig { PropertiesToExclude = propsToExclude };

            _unitOfWork.Context.UpdateAll(maintainableAssetEntities, _unitOfWork.UserEntity?.Id, config);
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

        public List<BMSIDAndBRKeyDTO> GetBMSIDAndBRKey()
        {
            var valuesFromDb = _unitOfWork.Context.MaintainableAsset.Select(_ => new { bmsId = _.SectionName, brKey = _.FacilityName });

            var result = new List<BMSIDAndBRKeyDTO>();
            foreach (var item in valuesFromDb)
            {
                result.Add(new BMSIDAndBRKeyDTO { BmsId = item.bmsId, BrKey = item.brKey });
            }
            return result;
        }
    }
}
