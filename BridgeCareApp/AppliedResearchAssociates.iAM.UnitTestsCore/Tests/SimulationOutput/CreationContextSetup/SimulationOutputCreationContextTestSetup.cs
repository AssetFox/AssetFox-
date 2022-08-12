using System;
using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.iAM.Data.Networking;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.TestHelpers;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests.Repositories;
using Attribute = AppliedResearchAssociates.iAM.Data.Attributes.Attribute;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests
{
    public static class SimulationOutputCreationContextTestSetup
    {
        public static SimulationOutputSetupContext SingleAssetContextWithObjectsInDatabase(UnitOfDataPersistenceWork unitOfWork, int numberOfYears = 1)
        {
            var assetPair = AssetNameIdPairs.Random();
            var assetPairs = new List<AssetNameIdPair> { assetPair };
            var context = ContextWithObjectsInDatabase(unitOfWork, assetPairs);
            return context;
        }

        public static SimulationOutputSetupContext ContextWithObjectsInDatabase(UnitOfDataPersistenceWork unitOfWork, List<AssetNameIdPair> assetNameIdPairs, int numberOfYears = 1)
        {
            var networkId = Guid.NewGuid();
            var maintainableAssets = new List<MaintainableAsset>();
            foreach (var assetPair in assetNameIdPairs)
            {
                var locationIdentifier = RandomStrings.WithPrefix("Location");
                var location = Locations.Section(locationIdentifier);
                var maintainableAsset = new MaintainableAsset(assetPair.Id, networkId, location, "[Deck_Area]");
                maintainableAssets.Add(maintainableAsset);
            }
            var network = NetworkTestSetup.ModelForEntityInDb(unitOfWork, maintainableAssets, networkId);
            // WjJake -- this manual setting of the asset name has to be wrong, but it is the only way I could see to get the Name into our asset.
            var assetIds = assetNameIdPairs.Select(pair => pair.Id).ToList();
            var assetEntities = unitOfWork.Context.MaintainableAsset.Where(ma => assetIds.Contains(ma.Id)).ToList();
            var assetDictionary = new Dictionary<Guid, string>();
            foreach (var assetPair in assetNameIdPairs)
            {
                assetDictionary[assetPair.Id] = assetPair.Name;
            }
            foreach (var entity in assetEntities)
            {
                entity.AssetName = assetDictionary[entity.Id];
            }
            unitOfWork.Context.UpdateRange(assetEntities);
            unitOfWork.Context.SaveChanges();
            var simulation = SimulationTestSetup.EntityInDb(unitOfWork, networkId);
            var numericAttributeId = Guid.NewGuid();
            var textAttributeId = Guid.NewGuid();
            var numericAttribute = AttributeTestSetup.Numeric(numericAttributeId);
            var textAttribute = AttributeTestSetup.Text(textAttributeId);
            var attributes = new List<Attribute> { numericAttribute, textAttribute };
            unitOfWork.AttributeRepo.UpsertAttributes(attributes);
            var years = Enumerable.Range(2022, numberOfYears).ToList();
            var context = new SimulationOutputSetupContext
            {
                BudgetName = "Budget",
                AssetNameIdPairs = assetNameIdPairs,
                TreatmentName = "Treatment",
                Years = years,
                NumericAttributeName = numericAttribute.Name,
                TextAttributeName = textAttribute.Name,
                SimulationId = simulation.Id,
            };
            return context;
        }
    }
}
