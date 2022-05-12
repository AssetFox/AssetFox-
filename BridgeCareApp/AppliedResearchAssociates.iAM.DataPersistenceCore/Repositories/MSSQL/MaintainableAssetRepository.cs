using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using AppliedResearchAssociates.iAM.DataAssignment.Networking;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Extensions;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.Analysis;
using AppliedResearchAssociates.iAM.DTOs;
using EFCore.BulkExtensions;
using MoreLinq;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL
{
    public class MaintainableAssetRepository : IMaintainableAssetRepository
    {
        private readonly UnitOfDataPersistenceWork _unitOfWork;

        public MaintainableAssetRepository(UnitOfDataPersistenceWork unitOfWork) => _unitOfWork = unitOfWork ??
                                         throw new ArgumentNullException(nameof(unitOfWork));

        public List<MaintainableAsset> GetAllInNetworkWithAssignedDataAndLocations(Guid networkId)
        {
            if (!_unitOfWork.Context.Network.Any(_ => _.Id == networkId))
            {
                throw new RowNotInTableException("The specified network was not found.");
            }

            if (!_unitOfWork.Context.MaintainableAsset.Any(_ => _.NetworkId == networkId))
            {
                throw new RowNotInTableException("The network has no maintainable assets for rollup.");
            }

            var assets = _unitOfWork.Context.MaintainableAsset
                .Where(_ => _.NetworkId == networkId)
                .Select(asset => new MaintainableAssetEntity
                {
                    Id = asset.Id,
                    NetworkId = networkId,
                    SpatialWeighting = asset.SpatialWeighting,
                    MaintainableAssetLocation = new MaintainableAssetLocationEntity(
                        asset.MaintainableAssetLocation.Id, asset.MaintainableAssetLocation.Discriminator,
                        asset.MaintainableAssetLocation.LocationIdentifier),
                    AssignedData = asset.AssignedData.Select(datum => new AttributeDatumEntity
                    {
                        Id = datum.Id,
                        NumericValue = datum.NumericValue,
                        TextValue = datum.TextValue,
                        Discriminator = datum.Discriminator,
                        TimeStamp = datum.TimeStamp,
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
    }
}
