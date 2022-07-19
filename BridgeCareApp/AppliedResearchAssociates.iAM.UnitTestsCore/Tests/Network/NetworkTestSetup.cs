using System;
using System.Collections.Generic;
using AppliedResearchAssociates.iAM.Data.Attributes;
using AppliedResearchAssociates.iAM.Data.Networking;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using BridgeCareCore.Services;
using TNetwork = AppliedResearchAssociates.iAM.Data.Networking.Network;
using DataAttribute = AppliedResearchAssociates.iAM.Data.Attributes.Attribute;
using BridgeCareCore.Models;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests
{
    public static class NetworkTestSetup
    {
        public static TNetwork ModelForEntityInDb(IUnitOfWork unitOfWork, List<MaintainableAsset> maintainableAssets, Guid? networkId = null)
        {
            var resolvedNetworkId = networkId ?? Guid.NewGuid();
            var network = new TNetwork(maintainableAssets, resolvedNetworkId);
            unitOfWork.NetworkRepo.CreateNetwork(network);
            return network;
        }

        public static TNetwork ModelForEntityInDbViaFactory(IUnitOfWork unitOfWork, DataAttribute attribute, NetworkCreationParameters parameters, string networkName)
        {
            var attributeConnection = AttributeConnectionBuilder.Build(attribute, parameters.NetworkDefinitionAttribute.DataSource, unitOfWork);
            var data = AttributeDataBuilder.GetData(attributeConnection);
            var network = NetworkFactory.CreateNetworkFromAttributeDataRecords(
                  data, parameters.DefaultEquation);
            network.Name = networkName;

            // insert network domain data into the data source
            unitOfWork.NetworkRepo.CreateNetwork(network);
            return network;
        }
    }
}
