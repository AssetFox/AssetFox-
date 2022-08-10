using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.Data.Networking;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.TestHelpers;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests.Repositories;
using Attribute = AppliedResearchAssociates.iAM.Data.Attributes.Attribute;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests
{
    public static class SimulationOutputCreationContextTestSetup
    {
        public static SimulationOutputSetupContext ContextWithObjectsInDatabase(UnitOfDataPersistenceWork unitOfWork)
        {
            var networkId = Guid.NewGuid();
            var assetId = Guid.NewGuid();
            var locationIdentifier = RandomStrings.WithPrefix("Location");
            var location = Locations.Section(locationIdentifier);
            var maintainableAsset = new MaintainableAsset(assetId, networkId, location, "[Deck_Area]");
            var maintainableAssets = new List<MaintainableAsset> { maintainableAsset };
            var network = NetworkTestSetup.ModelForEntityInDb(unitOfWork, maintainableAssets, networkId);
            var simulation = SimulationTestSetup.EntityInDb(unitOfWork, networkId);
            var numericAttributeId = Guid.NewGuid();
            var textAttributeId = Guid.NewGuid();
            var numericAttribute = AttributeTestSetup.Numeric(numericAttributeId);
            var textAttribute = AttributeTestSetup.Text(textAttributeId);
            var attributes = new List<Attribute> { numericAttribute, textAttribute };
            unitOfWork.AttributeRepo.UpsertAttributes(attributes);
            var context = new SimulationOutputSetupContext
            {
                BudgetName = "Budget",
                ManagedAssetId = assetId,
                TreatmentName = "Treatment",
                Years = new List<int> { 2022 },
                NumericAttributeName = numericAttribute.Name,
                TextAttributeName = textAttribute.Name,
                SimulationId = simulation.Id,
            };
            return context;
        }
    }
}
