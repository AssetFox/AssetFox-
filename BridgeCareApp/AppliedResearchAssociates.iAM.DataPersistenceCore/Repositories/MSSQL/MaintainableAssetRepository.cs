using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
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

        public void CreateMaintainableAssets(List<Facility> facilities, Guid networkId)
        {
            if (!_unitOfDataPersistenceWork.Context.Network.Any(_ => _.Id == networkId))
            {
                throw new RowNotInTableException($"No network found having id {networkId}");
            }


        }

        public void CreateMaintainableAssets(List<MaintainableAsset> maintainableAssets, Guid networkId, Guid? userId = null)
        {
            if (!_unitOfDataPersistenceWork.Context.Network.Any(_ => _.Id == networkId))
            {
                throw new RowNotInTableException($"No network found having id {networkId}");
            }

            var maintainableAssetEntities = maintainableAssets.Select(_ => _.ToEntity(networkId)).ToList();

            _unitOfDataPersistenceWork.Context.AddAll(maintainableAssetEntities, userId);

            var maintainableAssetLocationEntities = maintainableAssets
                .Select(_ => _.Location.ToEntity(_.Id, "MaintainableAssetEntity")).ToList();

            _unitOfDataPersistenceWork.Context.AddAll(maintainableAssetLocationEntities, userId);
        }
    }
}
