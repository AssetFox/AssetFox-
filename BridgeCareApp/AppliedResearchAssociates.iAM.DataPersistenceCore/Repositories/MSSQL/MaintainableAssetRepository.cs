using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using AppliedResearchAssociates.iAM.DataAssignment.Networking;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappings;
using Microsoft.EntityFrameworkCore;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL
{
    public class MaintainableAssetRepository : IMaintainableAssetRepository
    {
        private readonly UnitOfWork.UnitOfWork _unitOfWork;

        public MaintainableAssetRepository(UnitOfWork.UnitOfWork unitOfWork) => _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));



        public IEnumerable<MaintainableAsset> GetAllInNetworkWithAssignedDataAndLocations(Guid networkId)
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

            return !maintainableAssets.Any()
                ? throw new RowNotInTableException($"The network has no maintainable assets for rollup")
                : maintainableAssets.Select(_ => _.ToDomain());
        }
    }
}
