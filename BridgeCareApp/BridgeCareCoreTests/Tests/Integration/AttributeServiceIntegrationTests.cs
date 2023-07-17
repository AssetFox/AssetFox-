using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.Analysis;
using AppliedResearchAssociates.iAM.Data;
using AppliedResearchAssociates.iAM.Data.Networking;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests.Attributes;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests.Repositories;
using AppliedResearchAssociates.iAM.UnitTestsCore.TestUtils;
using IamAttribute = AppliedResearchAssociates.iAM.Data.Attributes.Attribute;
using BridgeCareCore.Services;
using Xunit;
using BridgeCareCore.Models;
using AppliedResearchAssociates.iAM.TestHelpers;

namespace BridgeCareCoreTests.Tests.Integration
{
    public class AttributeServiceIntegrationTests
    {
        private AttributeService CreateAttributeService(UnitOfDataPersistenceWork unitOfWork)
        {
            return new AttributeService(unitOfWork);
        }

        [Fact]
        public void GetAttributeSelectValues_NoAggregatedResultsInDatabaseForAttributes_Empty()
        {
            AttributeTestSetup.CreateAttributes(TestHelper.UnitOfWork);
            var service = CreateAttributeService(TestHelper.UnitOfWork);
            List<string> attributeNames = new() { "NONEXISTANT" };
            var values = service.GetAttributeSelectValues(attributeNames);
            Assert.Empty(values);
        }

        [Fact]
        public void GetAttributeSelectValues_NumericAggregatedResultInDatabase_Warns()
        {
            var dataSource = AttributeTestSetup.CreateAttributes(TestHelper.UnitOfWork);
            var service = CreateAttributeService(TestHelper.UnitOfWork);
            var attributeNames = new List<string>
            {
                TestAttributeNames.DeckDurationN,
            };
            NetworkTestSetup.CreateNetwork(TestHelper.UnitOfWork);
            var attribute = AttributeDtos.DeckDurationN;
            var networkId = NetworkTestSetup.NetworkId;
            var assetName = "AssetName";
            var location = new SectionLocation(Guid.NewGuid(), assetName);
            var maintainableAssetId = Guid.NewGuid();
            var spatialWeightingValue = "[Deck_Area]";
            var newAsset = new MaintainableAsset(maintainableAssetId, networkId, location, spatialWeightingValue);
            var assetList = new List<MaintainableAsset> { newAsset };
            TestHelper.UnitOfWork.MaintainableAssetRepo.CreateMaintainableAssets(assetList, networkId);
            var numericAttribute = AttributeTestSetup.Numeric(attribute.Id, attribute.Name, dataSource.Id);
            var attributeList = new List<IamAttribute> { numericAttribute };
            AggregatedResultTestSetup.AddNumericAggregatedResultsToDb(TestHelper.UnitOfWork, assetList, attributeList);

            var values = service.GetAttributeSelectValues(attributeNames);

            var theValue = values.Single();
            var expectedResultMessage = $"{AttributeService.ValuesForAttribute} {attribute.Name} {AttributeService.IsANumberUseTextInput}";
            var expectedValue = new AttributeSelectValuesResult
            {
                Attribute = attribute.Name,
                ResultMessage = expectedResultMessage,
                ResultType = "success",
                Values = new List<string>(),
            };
            ObjectAssertions.Equivalent(expectedValue, theValue);
        }

        [Fact]
        public void GetAttributeSelectValues_TextAggregatedResultInDatabase_Gets()
        {
            var dataSource = AttributeTestSetup.CreateAttributes(TestHelper.UnitOfWork);
            var service = CreateAttributeService(TestHelper.UnitOfWork);
            var attributeNames = new List<string>
            {
                TestAttributeNames.Interstate,
            };
            NetworkTestSetup.CreateNetwork(TestHelper.UnitOfWork);
            var attribute = AttributeDtos.Interstate;
            var networkId = NetworkTestSetup.NetworkId;
            var assetName = "AssetName";
            var location = new SectionLocation(Guid.NewGuid(), assetName);
            var maintainableAssetId = Guid.NewGuid();
            var spatialWeightingValue = "[Deck_Area]";
            var newAsset = new MaintainableAsset(maintainableAssetId, networkId, location, spatialWeightingValue);
            var assetList = new List<MaintainableAsset> { newAsset };
            TestHelper.UnitOfWork.MaintainableAssetRepo.CreateMaintainableAssets(assetList, networkId);
            var numericAttribute = AttributeTestSetup.Numeric(attribute.Id, attribute.Name, dataSource.Id);
            var attributeList = new List<IamAttribute> { numericAttribute };
            AggregatedResultTestSetup.SetTextAggregatedResultsInDb(TestHelper.UnitOfWork, assetList, attributeList);

            var values = service.GetAttributeSelectValues(attributeNames);

            var theValue = values.Single();
            var expectedResultMessage = $"{AttributeService.ValuesForAttribute} {attribute.Name} {AttributeService.IsANumberUseTextInput}";
            var expectedValue = new AttributeSelectValuesResult
            {
                Attribute = attribute.Name,
                ResultMessage = "Success",
                ResultType = "success",
                Values = new List<string> { "AggregatedResult"},
            };
            ObjectAssertions.Equivalent(expectedValue, theValue);
        }
    }
}
