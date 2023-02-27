using System;
using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.iAM.Data.Networking;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Extensions;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.TestHelpers;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests.Repositories;
using TNetwork = AppliedResearchAssociates.iAM.Data.Networking.Network;

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

        public static NetworkEntity TestNetwork() => new NetworkEntity
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
                        var network = TestNetwork();
                        unitOfWork.Context.AddEntity(network);
                    }
                }
            }
        }

        public static void CreateNetworkWithKeyAttribute(
            IUnitOfWork unitOfWork,
            Guid? networkId = null,
            Guid? keyAttributeId = null,
            string keyAttributeName = null)
        {
            var name = RandomStrings.WithPrefix("Network");
            var resolveNetworkId = networkId ?? Guid.NewGuid();
            var attribute = AttributeTestSetup.CreateSingleTextAttribute(unitOfWork, keyAttributeId, keyAttributeName);
            var network = new NetworkEntity
            {
                Id = resolveNetworkId,
                Name = name,
                KeyAttributeId = attribute.Id,
            };
            unitOfWork.Context.AddEntity(network);
        }
    }
}
