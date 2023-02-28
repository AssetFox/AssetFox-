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
        public async void GetTreatmentCost_Behaves()
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
            AggregatedResultTestSetup.AddTextAggregatedResultsToDb(TestHelper.UnitOfWork,
                maintainableAssets, attributes, assetKeyData);
            var treatmentId = Guid.NewGuid();
            var treatment = TreatmentTestSetup.ModelForSingleTreatmentOfLibraryInDb(
                TestHelper.UnitOfWork, treatmentLibraryId, treatmentId, "treatment");
            var treatmentCost = TreatmentCostTestSetup.ModelForEntityInDb(
                TestHelper.UnitOfWork, treatmentId, treatmentLibraryId);

            var exception = Assert.Throws<CalculateEvaluateCompilationException>(() => service.GetTreatmentCost(
                treatmentLibraryId,
                assetKeyData,
                treatmentName,
                networkId));
            var expectedMessage = @"Unknown reference ""True"".";
            Assert.Equal(expectedMessage, exception.Message);
        }
    }
}
