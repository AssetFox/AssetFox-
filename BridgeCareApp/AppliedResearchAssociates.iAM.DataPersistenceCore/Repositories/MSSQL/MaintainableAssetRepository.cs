using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Extensions;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.Analysis;
using EFCore.BulkExtensions;
using MoreLinq;
using AppliedResearchAssociates.iAM.Data.Networking;
using AppliedResearchAssociates.iAM.Data;
using Microsoft.EntityFrameworkCore;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL
{
    public class MaintainableAssetRepository : IMaintainableAssetRepository
    {
        private readonly UnitOfDataPersistenceWork _unitOfWork;

        public MaintainableAssetRepository(UnitOfDataPersistenceWork unitOfWork) => _unitOfWork = unitOfWork ??
                                         throw new ArgumentNullException(nameof(unitOfWork));

        public List<Data.Networking.MaintainableAsset> GetAllInNetworkWithAssignedDataAndLocations(Guid networkId)
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

            return assets.Select(_ => _.ToDomain(_unitOfWork.EncryptionKey)).ToList();
        }

        public List<Data.Networking.MaintainableAsset> GetAllInNetworkWithLocations(Guid networkId)
        {
            if (!_unitOfWork.Context.Network.Any(_ => _.Id == networkId))
            {
                throw new RowNotInTableException("The specified network was not found.");
            }

            if (!_unitOfWork.Context.MaintainableAsset.Any(_ => _.NetworkId == networkId))
            {
                throw new RowNotInTableException("The network has no maintainable assets for rollup.");
            }

            var assets = _unitOfWork.Context.MaintainableAsset.AsNoTracking()
                .Where(_ => _.NetworkId == networkId)
                .Select(asset => new MaintainableAssetEntity
                {
                    Id = asset.Id,
                    NetworkId = networkId,
                    SpatialWeighting = asset.SpatialWeighting,
                    MaintainableAssetLocation = new MaintainableAssetLocationEntity(
                        asset.MaintainableAssetLocation.Id, asset.MaintainableAssetLocation.Discriminator,
                        asset.MaintainableAssetLocation.LocationIdentifier)                  
                }).ToList();

            return assets.Select(_ => _.ToDomain(_unitOfWork.EncryptionKey)).ToList();
        }

        public MaintainableAsset GetAssetAtLocation(Location location)
        {
            var asset = _unitOfWork.Context.MaintainableAsset.FirstOrDefault(_ => location.MatchOn(_.MaintainableAssetLocation.ToDomain()));
            return asset.ToDomain(_unitOfWork.EncryptionKey);
        }

        public void UpdateMaintainableAssetsSpatialWeighting(List<Data.Networking.MaintainableAsset> maintainableAssets)
        {
            var networkId = maintainableAssets.First().NetworkId;
            var maintainableAssetEntities = maintainableAssets.Select(_ => _.ToEntity(networkId)).ToList();

            var propsToExclude = new List<string> { "CreatedDate", "CreatedBy", "AssetName" }; var config = new BulkConfig { PropertiesToExclude = propsToExclude };

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

        public bool CheckIfKeyAttributeValueExists(Guid networkId, string attributeValue)
        {
            var network = _unitOfWork.Context.Network.AsNoTracking().Include(_ => _.Simulations).FirstOrDefault(_ => _.Id == networkId);
            if (network == null)
                return false;
            var attrEntity = _unitOfWork.Context.Attribute.AsNoTracking().FirstOrDefault(_ => _.Id == network.KeyAttributeId);
            if (attrEntity == null)
                return false;
            

            if (attrEntity.DataType == "NUMBER")
                return  _unitOfWork.Context.MaintainableAsset.AsNoTracking().Include(_ => _.AggregatedResults)
                    .Where(_ => _.NetworkId == network.Id).SelectMany(_ => _.AggregatedResults)
                    .Any(_ => _.AttributeId == attrEntity.Id && _.NumericValue.ToString() == attributeValue);
            else
                return _unitOfWork.Context.MaintainableAsset.AsNoTracking().Include(_ => _.AggregatedResults)
                    .Where(_ => _.NetworkId == network.Id).SelectMany(_ => _.AggregatedResults)
                    .Any(_ => _.AttributeId == attrEntity.Id && _.TextValue == attributeValue);
        }

        public Dictionary<string, bool> CheckIfKeyAttributeValuesExists(Guid networkId, List<string> attributeValues)
        {

            var network = _unitOfWork.Context.Network.AsNoTracking().Include(_ => _.Simulations).FirstOrDefault(_ => _.Id == networkId);
            if (network == null)
                return new Dictionary<string, bool>();
            var attrEntity = _unitOfWork.Context.Attribute.AsNoTracking().FirstOrDefault(_ => _.Id == network.KeyAttributeId);
            if (attrEntity == null)
                return new Dictionary<string, bool>();

            var aggResults = _unitOfWork.Context.MaintainableAsset.AsNoTracking().Include(_ => _.AggregatedResults)
                    .Where(_ => _.NetworkId == network.Id)
                    .SelectMany(_ => _.AggregatedResults)
                    .Where(_ => _.AttributeId == attrEntity.Id)
                    .Select(_ => attrEntity.DataType == "NUMBER" ? _.NumericValue.ToString() : _.TextValue)
                    .Where(_ => attributeValues.Contains(_)).ToList();

            return attributeValues.Distinct().ToDictionary(_ => _, _ => aggResults.Contains(_));
        }

        public MaintainableAsset GetMaintainableAssetByKeyAttribute(Guid networkId, string attributeValue)
        {
            var network = _unitOfWork.Context.Network.AsNoTracking().Include(_ => _.Simulations).FirstOrDefault(_ => _.Id == networkId);
            if (network == null)
                return null;
            var attrEntity = _unitOfWork.Context.Attribute.AsNoTracking().FirstOrDefault(_ => _.Id == network.KeyAttributeId);
            if (attrEntity == null)
                return null;
            MaintainableAsset asset;

            if (attrEntity.DataType == "NUMBER")
            {
                asset = _unitOfWork.Context.MaintainableAsset.AsNoTracking().Include(_ => _.AggregatedResults).Include(_ => _.MaintainableAssetLocation)
                .FirstOrDefault(_ => _.NetworkId == network.Id
                && _.AggregatedResults.Any(ar => ar.AttributeId == attrEntity.Id && ar.NumericValue.ToString() == attributeValue))
                ?.ToDomain(_unitOfWork.EncryptionKey);
            }
            else
            {
                asset = _unitOfWork.Context.MaintainableAsset.AsNoTracking().Include(_ => _.AggregatedResults).Include(_ => _.MaintainableAssetLocation)
                 .FirstOrDefault(_ => _.NetworkId == network.Id
                 && _.AggregatedResults.Any(ar => ar.AttributeId == attrEntity.Id && ar.TextValue == attributeValue))
                 ?.ToDomain(_unitOfWork.EncryptionKey);

            }
            return asset;
        }

        public string GetPredominantAssetSpatialWeighting(Guid networkId)
        {
            return _unitOfWork.Context.MaintainableAsset
                .Where(_ => _.NetworkId == networkId)
                .Select(_ => _.SpatialWeighting)
                .GroupBy(_ => _)
                .OrderByDescending(_ => _.Count())
                .Select(_ => _.Key)
                .FirstOrDefault();
        }

        public List<Guid> GetAllIdsInCommittedProjectsForSimulation(Guid simulationId, Guid networkId)
        {
            var assetIds = new List<Guid>();
            var committedProjects = _unitOfWork.Context.CommittedProject.Include(c => c.CommittedProjectLocation).Where(c => c.SimulationId == simulationId);
            var committedProjectLocations = committedProjects.Select(c => c.CommittedProjectLocation.ToDomain()).ToList();

            var assets = _unitOfWork.Context.MaintainableAsset
               .Where(_ => _.NetworkId == networkId)
               .Include(_ => _.MaintainableAssetLocation)
               .ToList();

            foreach (var committedProjectLocation in committedProjectLocations)
            {
                var assetId = assets.FirstOrDefault(a => committedProjectLocation.MatchOn(a.MaintainableAssetLocation.ToDomain())).Id;
                if (!assetIds.Contains(assetId))
                {
                    assetIds.Add(assetId);
                }
            }

            return assetIds;
        }

        public List<Guid> GetMaintainableAssetAttributeIdsByNetworkId(Guid networkId)
        {
            var attributeIds = _unitOfWork.Context.AggregatedResult
                .Include(_ => _.MaintainableAsset)
                .Where(ar => ar.MaintainableAsset.NetworkId == networkId)
                .Select(ar => ar.AttributeId)
                .Distinct()
                .ToList();

            return attributeIds;
        }
    }
}
