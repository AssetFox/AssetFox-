using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.Data;
using AppliedResearchAssociates.iAM.Data.Attributes;
using AppliedResearchAssociates.iAM.Data.Mappers;
using AppliedResearchAssociates.iAM.Data.Networking;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Migrations;
using AppliedResearchAssociates.iAM.DataUnitTests.Tests;
using AppliedResearchAssociates.iAM.DataUnitTests.TestUtils;
using AppliedResearchAssociates.iAM.TestHelpers;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests.Attributes;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests.Repositories;
using AppliedResearchAssociates.iAM.UnitTestsCore.TestUtils;
using Xunit;
using IamAttribute = AppliedResearchAssociates.iAM.Data.Attributes.Attribute;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests
{
    public class MaintainableAssetRepositoryTests
    {
        [Fact]
        public void CheckIfKeyAttributeValueExists_ValueDoesExist_True()
        {
            var dataSource = AttributeTestSetup.CreateAttributes(TestHelper.UnitOfWork);
            var keyAttributeDto = AttributeDtos.BrKey;
            var assetId = Guid.NewGuid();
            var networkId = Guid.NewGuid();
            var locationIdentifier = RandomStrings.WithPrefix("Location");
            var location = Locations.Section(locationIdentifier);
            var maintainableAsset = new MaintainableAsset(assetId, networkId, location, "[Deck_Area]");
            var maintainableAssets = new List<MaintainableAsset> { maintainableAsset };
            var network = NetworkTestSetup.ModelForEntityInDbWithExistingKeyAttribute(TestHelper.UnitOfWork, maintainableAssets, keyAttributeDto.Id, networkId);
            var numberAttribute = AttributeTestSetup.Numeric(keyAttributeDto.Id, keyAttributeDto.Name, dataSource.Id, ConnectionType.EXCEL);
            var attributeList = new List<IamAttribute> { numberAttribute };
            var assetList = new List<MaintainableAsset> { maintainableAsset };
            AggregatedResultTestSetup.AddNumericAggregatedResultsToDb(TestHelper.UnitOfWork, assetList, attributeList, 12345);

            var exists = TestHelper.UnitOfWork.MaintainableAssetRepo.CheckIfKeyAttributeValueExists(network.Id, "12345");

            Assert.True(exists);
        }
        [Fact]
        public void CheckIfKeyAttributeValuesExists_ValueDoesExist_True()
        {
            var dataSource = AttributeTestSetup.CreateAttributes(TestHelper.UnitOfWork);
            var keyAttributeDto = AttributeDtos.BrKey;
            var assetId = Guid.NewGuid();
            var networkId = Guid.NewGuid();
            var locationIdentifier = RandomStrings.WithPrefix("Location");
            var location = Locations.Section(locationIdentifier);
            var maintainableAsset = new MaintainableAsset(assetId, networkId, location, "[Deck_Area]");
            var maintainableAssets = new List<MaintainableAsset> { maintainableAsset };
            var network = NetworkTestSetup.ModelForEntityInDbWithExistingKeyAttribute(TestHelper.UnitOfWork, maintainableAssets, keyAttributeDto.Id, networkId);
            var numberAttribute = AttributeTestSetup.Numeric(keyAttributeDto.Id, keyAttributeDto.Name, dataSource.Id, ConnectionType.EXCEL);
            var attributeList = new List<IamAttribute> { numberAttribute };
            var assetList = new List<MaintainableAsset> { maintainableAsset };
            AggregatedResultTestSetup.AddNumericAggregatedResultsToDb(TestHelper.UnitOfWork, assetList, attributeList, 12345);
            var maxIntAsString = int.MaxValue.ToString();
            var attributeValues = new List<string> { "12345", maxIntAsString };

            var exists = TestHelper.UnitOfWork.MaintainableAssetRepo.CheckIfKeyAttributeValuesExists(network.Id, attributeValues);

            Assert.True(exists["12345"]);
            Assert.False(exists[maxIntAsString]);
        }

        [Fact]
        public void UpdateMaintainableAssetsSpatialWeighting_AssetInDb_Updates()
        {
            AttributeTestSetup.CreateAttributes(TestHelper.UnitOfWork);
            var keyAttributeDto = AttributeDtos.BrKey;
            var assetId = Guid.NewGuid();
            var networkId = Guid.NewGuid();
            var locationIdentifier = RandomStrings.WithPrefix("Location");
            var location = Locations.Section(locationIdentifier);
            var maintainableAsset = new MaintainableAsset(assetId, networkId, location, "[Deck_Area]");
            var maintainableAssets = new List<MaintainableAsset> { maintainableAsset };
            var network = NetworkTestSetup.ModelForEntityInDbWithExistingKeyAttribute(TestHelper.UnitOfWork, maintainableAssets, keyAttributeDto.Id, networkId);
            var modifiedMaintainableAsset = new MaintainableAsset(maintainableAsset.Id, networkId, location, "Nonexistent attribute name");
            var modifiedMaintainableAssets = new List<MaintainableAsset> { modifiedMaintainableAsset };

            TestHelper.UnitOfWork.MaintainableAssetRepo.UpdateMaintainableAssetsSpatialWeighting(
                modifiedMaintainableAssets);

            var maintainableAssetsAfter = TestHelper.UnitOfWork.MaintainableAssetRepo.GetAllInNetworkWithLocations(networkId);
            Assert.Equal("Nonexistent attribute name", maintainableAssetsAfter.Single().SpatialWeighting);
        }

        [Fact]
        public void GetPredominantSpatialWeighting_Does()
        {
            AttributeTestSetup.CreateAttributes(TestHelper.UnitOfWork);
            var keyAttributeDto = AttributeDtos.BrKey;
            var assetId = Guid.NewGuid();
            var networkId = Guid.NewGuid();
            var locationIdentifier = RandomStrings.WithPrefix("Location");
            var location = Locations.Section(locationIdentifier);
            var maintainableAsset = new MaintainableAsset(assetId, networkId, location, "[Deck_Area]");
            var maintainableAssets = new List<MaintainableAsset> { maintainableAsset };
            var network = NetworkTestSetup.ModelForEntityInDbWithExistingKeyAttribute(TestHelper.UnitOfWork, maintainableAssets, keyAttributeDto.Id, networkId);

            var predominantSpacialWeighting = TestHelper.UnitOfWork.MaintainableAssetRepo.GetPredominantAssetSpatialWeighting(networkId);

            Assert.Equal("[Deck_Area]", predominantSpacialWeighting);
        }
    }
}
