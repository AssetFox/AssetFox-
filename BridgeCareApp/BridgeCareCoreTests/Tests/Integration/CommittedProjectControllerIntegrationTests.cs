using AppliedResearchAssociates.iAM.Data.Networking;
using AppliedResearchAssociates.iAM.TestHelpers;
using AppliedResearchAssociates.iAM.UnitTestsCore;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests.Repositories;
using AppliedResearchAssociates.iAM.UnitTestsCore.TestUtils;
using BridgeCareCore.Controllers;
using BridgeCareCore.Models;
using BridgeCareCore.Services;
using BridgeCareCoreTests.Helpers;
using BridgeCareCoreTests.Tests.General_Work_Queue;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using Xunit;
using IamAttribute = AppliedResearchAssociates.iAM.Data.Attributes.Attribute;


namespace BridgeCareCoreTests.Tests.Integration
{
    public class CommittedProjectControllerIntegrationTests
    {
        private CommittedProjectController CreateController()
        {
            var service = new CommittedProjectService(TestHelper.UnitOfWork);
            var pagingService = new CommittedProjectPagingService(TestHelper.UnitOfWork);
            var security = EsecSecurityMocks.Admin;
            var hubService = HubServiceMocks.Default();
            var contextAccessor = HttpContextAccessorMocks.Default();
            var claimHelper = ClaimHelperMocks.New();
            var generalWorkQueue = GeneralWorkQueueServiceMocks.New();
            return new CommittedProjectController(
                service,
                pagingService,
                security,
                TestHelper.UnitOfWork,
                hubService,
                contextAccessor,
                claimHelper.Object,
                generalWorkQueue.Object
                );
        }

        [Fact]
        public async Task FillTreatmentValues_Does()
        {
            var networkId = Guid.NewGuid();
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
                TestHelper.UnitOfWork, treatmentLibraryId, treatmentId, treatmentName);
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

            var controller = CreateController();
            var fillModel = new CommittedProjectFillTreatmentValuesModel
            {
                TreatmentLibraryId = treatmentLibraryId,
                TreatmentName = treatmentName,
                NetworkId = networkId,
                KeyAttributeValue = assetKeyData,
            };

            var result = await controller.FillTreatmentValues(fillModel);

            var value = ActionResultAssertions.OkObject(result);
            var castValue = value as CommittedProjectFillTreatmentReturnValuesModel;
            Assert.Equal(12345, castValue.TreatmentCost);
        }
    }
}
