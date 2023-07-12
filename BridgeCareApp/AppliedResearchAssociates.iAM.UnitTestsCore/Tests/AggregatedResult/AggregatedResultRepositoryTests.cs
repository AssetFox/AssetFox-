using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.Data;
using AppliedResearchAssociates.iAM.Data.Networking;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.TestHelpers;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests.Attributes;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests.Repositories;
using AppliedResearchAssociates.iAM.UnitTestsCore.TestUtils;
using Xunit;
using IamAttribute = AppliedResearchAssociates.iAM.Data.Attributes.Attribute;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests.AggregatedResult
{
    public class AggregatedResultRepositoryTests
    {
        [Fact]
        public void GetAggregatedResultsForAttributeNames_NumericAggregatedResultInDb_Empty()
        {
            var dataSource = AttributeTestSetup.CreateAttributes(TestHelper.UnitOfWork);
            var attributeNames = new List<string>
            {
                "ALANE"
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

            var aggregatedResults = TestHelper.UnitOfWork.AggregatedResultRepo.GetAggregatedResultsForAttributeNames(attributeNames);
            Assert.Null(aggregatedResults.Attribute);
            Assert.Null(aggregatedResults.ResultType);
            Assert.Null(aggregatedResults.Values);
            Assert.False(aggregatedResults.IsNumber);
        }

        [Fact]
        public void GetAggregatedResultsForAttributeNames_NumericAggregatedResultInDb_Gets()
        {
            var dataSource = AttributeTestSetup.CreateAttributes(TestHelper.UnitOfWork);
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

            var aggregatedResults = TestHelper.UnitOfWork.AggregatedResultRepo.GetAggregatedResultsForAttributeNames(attributeNames);
            var expected = new AggregatedSelectValuesResultDTO()
            {
                Attribute = attribute,
                Values = new List<string>() { "1.23" },
                ResultType = "success",
                IsNumber = true
            };
            ObjectAssertions.EquivalentExcluding(expected, aggregatedResults, x => x.Attribute);
            Assert.Equal(expected.Values[0], aggregatedResults.Values[0]);
            Assert.Equal(expected.Attribute.Id, aggregatedResults.Attribute.Id);
        }

        [Fact]
        public void GetAggregatedResultsForAttributeNames_TextAggregatedResultInDb_Gets()
        {
            var dataSource = AttributeTestSetup.CreateAttributes(TestHelper.UnitOfWork);
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
            AggregatedResultTestSetup.SetTextAggregatedResultsInDb(TestHelper.UnitOfWork, assetList, attributeList, "Result");

            var aggregatedResults = TestHelper.UnitOfWork.AggregatedResultRepo.GetAggregatedResultsForAttributeNames(attributeNames);
            var expected = new AggregatedSelectValuesResultDTO()
            {
                Attribute = attribute,
                Values = new List<string>() { "Result" },
                ResultType = "success",
                IsNumber = false
            };

            ObjectAssertions.EquivalentExcluding(expected, aggregatedResults, x => x.Attribute);
            Assert.Equal(expected.Values[0], aggregatedResults.Values[0]);
            Assert.Equal(expected.Attribute.Name, aggregatedResults.Attribute.Name);
        }
    }
}
