using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using AppliedResearchAssociates.iAM.Data;
using AppliedResearchAssociates.iAM.Data.Networking;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Extensions;
using AppliedResearchAssociates.iAM.DataUnitTests.Tests;
using AppliedResearchAssociates.iAM.TestHelpers;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests.Repositories;
using AppliedResearchAssociates.iAM.UnitTestsCore.TestUtils;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests
{
    public class OrphanCleanupSprocTests
    {
        private string RunOrphanCleanupSproc()
        {
            var retMessageParam = new SqlParameter("@RetMessage", SqlDbType.VarChar, 250);
            retMessageParam.Direction = ParameterDirection.Output;
            TestHelper.UnitOfWork.Context.Database.ExecuteSqlRaw("EXEC usp_orphan_cleanup @RetMessage", retMessageParam);
            var retMessageValue = retMessageParam.Value;
            return retMessageValue.ToString();
        }

        [Fact]
        public void OrphanCleanup_Runs()
        {
            var retMessage = RunOrphanCleanupSproc();
            Assert.Empty(retMessage);
        }

        [Fact]
        public void RunOrphanCleanup_OrphanInDb_Deletes()
        {
            var orphanId = Guid.NewGuid();
            var nonexistentNetworkId = Guid.NewGuid();
            var orphan = new MaintainableAssetEntity
            {
                Id = orphanId,
                AssetName = "orphan",
                NetworkId = nonexistentNetworkId,
            };
            var entities = new List<MaintainableAssetEntity> { orphan };
            TestHelper.UnitOfWork.Context.AddAll(entities);
            var orphanInDbBefore = TestHelper.UnitOfWork.Context.MaintainableAsset
                .SingleOrDefault(a => a.Id == orphanId);
            Assert.NotNull(orphanInDbBefore);
            RunOrphanCleanupSproc();
            var orphanInDbAfter = TestHelper.UnitOfWork.Context.MaintainableAsset
             .SingleOrDefault(a => a.Id == orphanId);
            Assert.Null(orphanInDbAfter);
        }

        [Fact]
        public void RunOrphanCleanup_NonOrphanInDb_DoesNotDelete()
        {
            AttributeTestSetup.CreateAttributes(TestHelper.UnitOfWork);
            NetworkTestSetup.CreateNetwork(TestHelper.UnitOfWork);
            var networkId = Guid.NewGuid();
            var assetId = Guid.NewGuid();
            var keyAttributeId = Guid.NewGuid();
            var keyAttributeName = RandomStrings.WithPrefix("KeyAttribute");
            var attributeDto = AttributeTestSetup.CreateSingleTextAttribute(TestHelper.UnitOfWork, keyAttributeId, keyAttributeName, ConnectionType.EXCEL, "location");
            var asset = MaintainableAssets.InNetwork(networkId, keyAttributeName, assetId);
            var assets = new List<MaintainableAsset> { asset };
            var network = NetworkTestSetup.ModelForEntityInDb(TestHelper.UnitOfWork, assets, networkId, keyAttributeId);
            var entityInDbBefore = TestHelper.UnitOfWork.Context.MaintainableAsset
                .SingleOrDefault(a => a.Id == assetId);
            Assert.NotNull(entityInDbBefore);
            RunOrphanCleanupSproc();
            var entityInDbAfter = TestHelper.UnitOfWork.Context.MaintainableAsset
             .SingleOrDefault(a => a.Id == assetId);
            Assert.NotNull(entityInDbAfter);
        }
    }
}
