﻿using System;
using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.iAM.Data.Networking;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Extensions;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.TestHelpers;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests.Attributes;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests.Repositories;
using TNetwork = AppliedResearchAssociates.iAM.Data.Networking.Network;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests
{
    public static class NetworkTestSetup
    {
        public static readonly Guid NetworkId = Guid.Parse("7f4ea3ba-6082-4e1e-91a4-b80578aeb0ed");

        public static TNetwork ModelForEntityInDb(IUnitOfWork unitOfWork, List<MaintainableAsset> maintainableAssets, Guid? networkId = null, Guid? keyAttributeId = null, string name = "")
        {
            var resolvedNetworkId = networkId ?? Guid.NewGuid();
            var network = new TNetwork(maintainableAssets, resolvedNetworkId);
            network.Name = name;
            if (keyAttributeId != null)
            {
                network.KeyAttributeId = (Guid)keyAttributeId;
            }

            unitOfWork.NetworkRepo.CreateNetwork(network);
            return network;
        }

        public static NetworkEntity TestNetwork() => new NetworkEntity
        {
            Id = NetworkId,
            Name = "Test Network",
            KeyAttributeId = TestAttributeIds.BrKeyId,
        };

        private static readonly object NetworkCreationLock = new object();
        private static NetworkEntity CacheNetworkEntity = null;


        public static NetworkEntity CreateNetwork(UnitOfDataPersistenceWork unitOfWork)
        {
            if (CacheNetworkEntity == null)
            {
                lock (NetworkCreationLock)  // Necessary as long as there is a chance that some tests may run in paralell. Can we eliminate that possiblity?
                {
                    if (!unitOfWork.Context.Network.Any(_ => _.Id == NetworkId))
                    {
                        var network = TestNetwork();
                        unitOfWork.Context.AddEntity(network);
                        CacheNetworkEntity = network;
                        return network;
                    }
                }
            }
            return CacheNetworkEntity;
        }

        public static TNetwork ModelForEntityInDbWithNewKeyTextAttribute(
            IUnitOfWork unitOfWork,
            List<MaintainableAsset> maintainableAssets,
            Guid? networkId = null,
            Guid? keyAttributeId = null,
            string keyAttributeName = null)
        {
            var attribute = AttributeTestSetup.CreateSingleTextAttribute(unitOfWork, keyAttributeId, keyAttributeName, Data.ConnectionType.EXCEL, keyAttributeName);
            return ModelForEntityInDbWithExistingKeyAttribute(unitOfWork, maintainableAssets, attribute.Id, networkId);
        }

        public static TNetwork ModelForEntityInDbWithExistingKeyAttribute(IUnitOfWork unitOfWork, List<MaintainableAsset> maintainableAssets, Guid keyAttributeId, Guid? networkId = null)
        {
            var name = RandomStrings.WithPrefix("Network");
            var resolveNetworkId = networkId ?? Guid.NewGuid();
            var network = new TNetwork(maintainableAssets, resolveNetworkId, name);
            network.KeyAttributeId = keyAttributeId;
            unitOfWork.NetworkRepo.CreateNetwork(network);
            return network;
        }
    }
}
