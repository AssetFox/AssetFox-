using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Antlr4.Runtime;
using AppliedResearchAssociates.CalculateEvaluate;
using AppliedResearchAssociates.iAM.Data.Networking;
using AppliedResearchAssociates.iAM.TestHelpers;
using AppliedResearchAssociates.iAM.UnitTestsCore;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests.Repositories;
using AppliedResearchAssociates.iAM.UnitTestsCore.TestUtils;
using BridgeCareCore.Services;
using Xunit;
using IamAttribute = AppliedResearchAssociates.iAM.Data.Attributes.Attribute;


namespace BridgeCareCoreTests.Tests.Integration
{
    public class CommittedProjectServiceIntegrationTests
    {
        private CommittedProjectService CreateCommittedProjectService()
        {
            var service = new CommittedProjectService(TestHelper.UnitOfWork);
            return service;
        }

        [Fact]
        public void GetTreatmentCost_CriteriaFailToEvaluate_Throws()
        {
            var networkId = Guid.NewGuid();
            var service = CreateCommittedProjectService();
            var treatmentLibraryId = Guid.NewGuid();
            var treatmentLibrary = TreatmentLibraryTestSetup.ModelForEntityInDb(TestHelper.UnitOfWork, treatmentLibraryId);
            var assetKeyData = "key";
            var treatmentName = "treatment";
            var keyAttributeId = Guid.NewGuid();
            var maintainableAssets = new List<MaintainableAsset>();
            var assetId = Guid.NewGuid();
            var locationIdentifier = RandomStrings.WithPrefix("Location");
            var location = Locations.Section(locationIdentifier);
            var maintainableAsset = new MaintainableAsset(assetId, networkId, location, "[Deck_Area]");
            var attributeName = RandomStrings.WithPrefix("attribute");
            var attribute = AttributeTestSetup.Text(keyAttributeId, attributeName);
            maintainableAssets.Add(maintainableAsset);
            var network = NetworkTestSetup.ModelForEntityInDbWithKeyAttribute(
                TestHelper.UnitOfWork, maintainableAssets, networkId, keyAttributeId, attributeName);
            var attributes = new List<IamAttribute> { attribute };
            AggregatedResultTestSetup.SetTextAggregatedResultsInDb(TestHelper.UnitOfWork,
                maintainableAssets, attributes, assetKeyData);
            var treatmentId = Guid.NewGuid();
            var treatment = TreatmentTestSetup.ModelForSingleTreatmentOfLibraryInDb(
                TestHelper.UnitOfWork, treatmentLibraryId, treatmentId, "treatment");
            var treatmentCost = TreatmentCostTestSetup.ModelForEntityInDb(
                TestHelper.UnitOfWork, treatmentId, treatmentLibraryId, mergedCriteriaExpression: "ThrowingCriteria");

            var exception = Assert.Throws<CalculateEvaluateCompilationException>(() => service.GetTreatmentCost(
                treatmentLibraryId,
                assetKeyData,
                treatmentName,
                networkId));
            var expectedMessage = @"Unknown reference ""ThrowingCriteria"".";
            Assert.Equal(expectedMessage, exception.Message);
        }


        [Fact]
        public void GetTreatmentCost_CriteriaEvaluateToTrue_FindsCost()
        {
            var networkId = Guid.NewGuid();
            var service = CreateCommittedProjectService();
            var treatmentLibraryId = Guid.NewGuid();
            var treatmentLibrary = TreatmentLibraryTestSetup.ModelForEntityInDb(TestHelper.UnitOfWork, treatmentLibraryId);
            var assetKeyData = "key";
            var treatmentName = "treatment";
            var keyAttributeId = Guid.NewGuid();
            var maintainableAssets = new List<MaintainableAsset>();
            var assetId = Guid.NewGuid();
            var locationIdentifier = RandomStrings.WithPrefix("Location");
            var location = Locations.Section(locationIdentifier);
            var maintainableAsset = new MaintainableAsset(assetId, networkId, location, "[Deck_Area]");
            var keyAttributeName = RandomStrings.WithPrefix("attribute");
            var keyAttribute = AttributeTestSetup.Text(keyAttributeId, keyAttributeName);
            var resultAttributeName = RandomStrings.WithPrefix("result");
            var resultAttributeId = Guid.NewGuid();
            var resultAttribute = AttributeTestSetup.Text(resultAttributeId, resultAttributeName); ;
            AttributeTestSetup.CreateSingleTextAttribute(TestHelper.UnitOfWork,
                resultAttributeId, resultAttributeName);
            maintainableAssets.Add(maintainableAsset);
            var network = NetworkTestSetup.ModelForEntityInDbWithKeyAttribute(
                TestHelper.UnitOfWork, maintainableAssets, networkId, keyAttributeId, keyAttributeName);
            var attributes = new List<IamAttribute> { keyAttribute, resultAttribute };
            AggregatedResultTestSetup.SetTextAggregatedResultsInDb(TestHelper.UnitOfWork,
                maintainableAssets, attributes, assetKeyData);
            var treatmentId = Guid.NewGuid();
            var treatment = TreatmentTestSetup.ModelForSingleTreatmentOfLibraryInDb(
                TestHelper.UnitOfWork, treatmentLibraryId, treatmentId, "treatment");
            var treatmentCost = TreatmentCostTestSetup.ModelForEntityInDb(
                TestHelper.UnitOfWork, treatmentId, treatmentLibraryId, mergedCriteriaExpression: $"[{resultAttributeName}]='ok'");
            var keyAttributes = new List<IamAttribute> { keyAttribute };
            var resultAttributes = new List<IamAttribute> { resultAttribute };
            var resultDictionary = new Dictionary<string, List<IamAttribute>>();
            resultDictionary["ok"] = resultAttributes;
            resultDictionary["key"] = keyAttributes;
            AggregatedResultTestSetup.SetTextAggregatedResultsInDb(TestHelper.UnitOfWork,
                maintainableAssets, resultAttributes, "ok");
            AggregatedResultTestSetup.SetTextAggregatedResultsInDb(TestHelper.UnitOfWork,
                maintainableAssets, resultDictionary);

            var cost = service.GetTreatmentCost(
                treatmentLibraryId,
                assetKeyData,
                treatmentName,
                networkId);
            Assert.Equal(12345, cost);
        }
    }
}
