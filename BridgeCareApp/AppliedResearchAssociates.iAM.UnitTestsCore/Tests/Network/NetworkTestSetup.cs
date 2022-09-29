using System;
using System.Collections.Generic;
using AppliedResearchAssociates.iAM.Data.Attributes;
using AppliedResearchAssociates.iAM.Data.Networking;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using BridgeCareCore.Services;
using TNetwork = AppliedResearchAssociates.iAM.Data.Networking.Network;
using DataAttribute = AppliedResearchAssociates.iAM.Data.Attributes.Attribute;
using BridgeCareCore.Models;
using BridgeCareCore.Utils;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using System.Linq;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Extensions;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests
{
    public static class NetworkTestSetup
    {
        public static readonly Guid NetworkId = Guid.Parse("7f4ea3ba-6082-4e1e-91a4-b80578aeb0ed");

        public static TNetwork ModelForEntityInDb(IUnitOfWork unitOfWork, List<MaintainableAsset> maintainableAssets, Guid? networkId = null)
        {
            var resolvedNetworkId = networkId ?? Guid.NewGuid();
            var network = new TNetwork(maintainableAssets, resolvedNetworkId);
            unitOfWork.NetworkRepo.CreateNetwork(network);
            return network;
        }

        public static TNetwork ModelForEntityInDbViaFactory(IUnitOfWork unitOfWork, DataAttribute attribute, NetworkCreationParameters parameters, string networkName)
        {
            var allDataSource = parameters.NetworkDefinitionAttribute.DataSource;
            var mappedDataSource = AllDataSourceMapper.ToSpecificDto(allDataSource); 
            var attributeConnection = AttributeConnectionBuilder.Build(attribute, mappedDataSource, unitOfWork);
            var data = AttributeDataBuilder.GetData(attributeConnection);
            var network = NetworkFactory.CreateNetworkFromAttributeDataRecords(
                  data, parameters.DefaultEquation);
            network.Name = networkName;

            // insert network domain data into the data source
            unitOfWork.NetworkRepo.CreateNetwork(network);
            return network;
        }

        public static NetworkEntity TestNetwork { get; } = new NetworkEntity
        {
            Id = NetworkId,
            Name = "Test Network"
        };

        private static readonly object NetworkCreationLock = new object();

        public static void CreateNetwork(IUnitOfWork unitOfWork)
        {
            if (!unitOfWork.Context.Network.Any(_ => _.Id == NetworkId))
            {
                lock (NetworkCreationLock)  // Necessary as long as there is a chance that some tests may run in paralell. Can we eliminate that possiblity?
                {
                    if (!unitOfWork.Context.Network.Any(_ => _.Id == NetworkId))
                    {
                        unitOfWork.Context.AddEntity(TestNetwork);
                    }
                }
            }
        }
    }
}
