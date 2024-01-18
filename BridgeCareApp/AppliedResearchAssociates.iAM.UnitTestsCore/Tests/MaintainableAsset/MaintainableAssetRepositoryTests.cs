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
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.TestHelpers;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests.Attributes;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests.CommittedProjects;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests.Repositories;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests.User;
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
        public void GetMaintainableAssetByKeyAttribute_AssetExists_Gets()
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

            var retreivedAsset = TestHelper.UnitOfWork.MaintainableAssetRepo.GetMaintainableAssetByKeyAttribute(network.Id, "12345");

            ObjectAssertions.Equivalent(maintainableAsset, retreivedAsset);
        }

        [Fact]
        public void GetMaintainableAssetByTextKeyAttribute_AssetExists_Gets()
        {
            AttributeTestSetup.CreateAttributes(TestHelper.UnitOfWork);
            var keyAttributeName = RandomStrings.WithPrefix("keyAttribute");
            var keyAttributeId = Guid.NewGuid();
            var keyAttributeDto = AttributeTestSetup.CreateSingleTextAttribute(
                TestHelper.UnitOfWork, keyAttributeId, keyAttributeName, ConnectionType.EXCEL, keyAttributeName);
            var assetId = Guid.NewGuid();
            var networkId = Guid.NewGuid();
            var locationIdentifier = RandomStrings.WithPrefix("Location");
            var location = Locations.Section(locationIdentifier);
            var maintainableAsset = new MaintainableAsset(assetId, networkId, location, "[Deck_Area]");
            var maintainableAssets = new List<MaintainableAsset> { maintainableAsset };
            var network = NetworkTestSetup.ModelForEntityInDbWithExistingKeyAttribute(TestHelper.UnitOfWork, maintainableAssets, keyAttributeDto.Id, networkId);
            var textAttribute = AttributeTestSetup.Text(keyAttributeDto.Id, keyAttributeDto.Name, ConnectionType.EXCEL);
            var attributeList = new List<IamAttribute> { textAttribute };
            var assetList = new List<MaintainableAsset> { maintainableAsset };
            AggregatedResultTestSetup.SetTextAggregatedResultsInDb(TestHelper.UnitOfWork, assetList, attributeList, "keyAttributeValue");

            var retreivedAsset = TestHelper.UnitOfWork.MaintainableAssetRepo.GetMaintainableAssetByKeyAttribute(network.Id, "keyAttributeValue");

            ObjectAssertions.Equivalent(maintainableAsset, retreivedAsset);
        }

        [Fact]
        public void GetMaintainableAssetByKeyAttribute_AssetDoesNotExist_ReturnsNull()
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

            var retreivedAsset = TestHelper.UnitOfWork.MaintainableAssetRepo.GetMaintainableAssetByKeyAttribute(network.Id, "54321");

            Assert.Null(retreivedAsset);
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

        [Fact]
        public async Task GetAllIdsInCommittedProjectsForSimulation_SimulationInDbWithAssetAndCommittedProject_GetsCommittedProjectId()
        {
            AttributeTestSetup.CreateAttributes(TestHelper.UnitOfWork);
            var user = await UserTestSetup.ModelForEntityInDb(TestHelper.UnitOfWork, false);
            var keyAttributeDto = AttributeDtos.BrKey;
            var assetId = Guid.NewGuid();
            var simulationId = Guid.NewGuid();
            var simulationName = RandomStrings.WithPrefix("Simulation");
            var networkId = Guid.NewGuid();
            var locationIdentifier = "2";
            var location = Locations.Section(locationIdentifier);
            var maintainableAsset = new MaintainableAsset(assetId, networkId, location, "[Deck_Area]");
            var maintainableAssets = new List<MaintainableAsset> { maintainableAsset };
            var network = NetworkTestSetup.ModelForEntityInDbWithExistingKeyAttribute(TestHelper.UnitOfWork, maintainableAssets, keyAttributeDto.Id, networkId);
            var simulation = SimulationTestSetup.ModelForEntityInDb(TestHelper.UnitOfWork, simulationId, simulationName, user.Id, networkId);
            var committedProjectId = Guid.NewGuid();
            var sectionCommittedProjectDto = SectionCommittedProjectDtos.Dto1(committedProjectId, simulation.Id);
            var sectionCommittedProjectDtos = new List<SectionCommittedProjectDTO> { sectionCommittedProjectDto };
            TestHelper.UnitOfWork.CommittedProjectRepo.UpsertCommittedProjects(sectionCommittedProjectDtos);

            var fetchedIds = TestHelper.UnitOfWork.MaintainableAssetRepo.GetAllIdsInCommittedProjectsForSimulation(simulationId, networkId);

            var fetchedId = fetchedIds.Single();
            Assert.Equal(assetId, fetchedId);
        }

        [Fact]
        public void GetMaintainableAssetAttributeIdsByNetworkId_Expected()
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

            var attributeIds = TestHelper.UnitOfWork.MaintainableAssetRepo.GetMaintainableAssetAttributeIdsByNetworkId(network.Id);

            var attributeId = attributeIds.Single();
            Assert.Equal(keyAttributeDto.Id, attributeId);
        }
    }
}
