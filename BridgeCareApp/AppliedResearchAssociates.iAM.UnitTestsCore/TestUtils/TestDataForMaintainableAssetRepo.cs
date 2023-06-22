using System;
using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.TestUtils
{
    public class TestDataForMaintainableAssetRepo
    {
        private List<AttributeEntity> _attributeLibrary;
        private List<AdminSettingsEntity> _adminSettingsLibrary;
        private List<MaintainableAssetLocationEntity> _maintainableAssetLocationLibrary;

        public NetworkEntity TestNetwork { get; private set; }
        public IQueryable<AttributeEntity> AttributeLibrary => _attributeLibrary.AsQueryable();
        public IQueryable<MaintainableAssetEntity> MaintainableAssetsLibrary => TestNetwork.MaintainableAssets.AsQueryable();
        public IQueryable<AggregatedResultEntity> AggregatedResultsLibrary => TestNetwork.MaintainableAssets.SelectMany(_ => _.AggregatedResults).AsQueryable();
        public IQueryable<AdminSettingsEntity> AdminSettingsLibrary => _adminSettingsLibrary.AsQueryable();
        public IQueryable<MaintainableAssetLocationEntity> MaintainableAssetLocationLibrary => _maintainableAssetLocationLibrary.AsQueryable();

        public TestDataForMaintainableAssetRepo()
        {
            _attributeLibrary = CreateTestAttributes();
            _adminSettingsLibrary = CreateAdminSettingsLibrary();
            TestNetwork = CreateTestNetwork();
        }

        private List<AdminSettingsEntity> CreateAdminSettingsLibrary()
        {
            var entities = new List<AdminSettingsEntity>();
            // entities will need to be added here.
            var FirstASection = new AdminSettingsEntity()
            {
                Key = "KeyFields",
                Value = "BRKEY_,BMSID"
            };
            entities.Add(FirstASection);
            return entities;
        }

        private NetworkEntity CreateTestNetwork()
        {
            var testNetwork = new NetworkEntity();
            testNetwork.Id = new Guid("c6de7a77-7d81-4e17-9a21-c6ed17f7875c");

            var FirstASection = new MaintainableAssetEntity()
            {
                Id = new Guid("799acb6e-539d-444b-b16a-6defc50b2c64"),
                //FacilityName = "1",
                AssetName = "00101256",
                NetworkId = testNetwork.Id
            };
            testNetwork.MaintainableAssets.Add(FirstASection);
            AssignKeyAttributes(FirstASection);
            AssignLength(FirstASection, 10);
            AssignName(FirstASection, "First A");
            var FirstBSection = new MaintainableAssetEntity()
            {
                Id = new Guid("8f80c690-3088-4084-b0e5-a8e070000a06"),                
                //FacilityName = "2",
                AssetName = "13401256",
                NetworkId = testNetwork.Id
            };
            testNetwork.MaintainableAssets.Add(FirstBSection);
            AssignKeyAttributes(FirstBSection);
            AssignLength(FirstBSection, 15.4);
            AssignName(FirstBSection, "First B");
            var FirstCSection = new MaintainableAssetEntity()
            {
                Id = new Guid("1bb0dd92-db74-45c6-a66a-72ae0c70b636"),
                //FacilityName = "3",
                AssetName = "5983256",
                NetworkId = testNetwork.Id
            };
            testNetwork.MaintainableAssets.Add(FirstCSection);
            AssignKeyAttributes(FirstCSection);
            AssignLength(FirstCSection, 20);
            AssignName(FirstCSection, "First C");
            var SecondASection = new MaintainableAssetEntity()
            {
                Id = new Guid("3fb90c20-9885-48db-8e47-1c76c5040757"),
                //FacilityName = "4",
                AssetName = "98451298",
                NetworkId = testNetwork.Id
            };
            testNetwork.MaintainableAssets.Add(SecondASection);
            AssignKeyAttributes(SecondASection);
            AssignLength(SecondASection, 10);
            AssignName(SecondASection, "Second A");
            var SecondBSection = new MaintainableAssetEntity()
            {
                Id = new Guid("6d79de97-1c3c-4da5-9cc4-f5043efa047a"),
                //FacilityName = "5",
                AssetName = "56451278",
                NetworkId = testNetwork.Id
            };
            testNetwork.MaintainableAssets.Add(SecondBSection);
            AssignKeyAttributes(SecondBSection);
            AssignLength(SecondBSection, 20);
            AssignName(SecondBSection, "Second B");

            _maintainableAssetLocationLibrary = CreateTestMaintainableAssetLocations(testNetwork.MaintainableAssets.ToList());
            return testNetwork;
        }

        private List<AttributeEntity> CreateTestAttributes()
        {
            var attributeLibrary = new List<AttributeEntity>();

            attributeLibrary.Add(new AttributeEntity()
            {
                Id = Guid.NewGuid(),
                Name = "Length",
                DataType = "STRING"
            });
            attributeLibrary.Add(new AttributeEntity()
            {
                Id = Guid.NewGuid(),
                Name = "Name",
                DataType = "STRING"
            });
            attributeLibrary.Add(new AttributeEntity()
            {
                Id = Guid.NewGuid(),
                Name = "NoData",
                DataType = "STRING"
            });
            attributeLibrary.Add(new AttributeEntity
            {
                Id = new Guid("2e3ae9ac-c14c-46ab-8e4b-f93312bc8637"),
                Name = "BMSID",
                DataType = "STRING",
                AggregationRuleType = "PREDOMINANT"
            });
            attributeLibrary.Add(new AttributeEntity
            {
                Id = new Guid("104bd958-8e0a-403c-b065-07d5e91eb27b"),
                Name = "BRKEY_",
                DataType = "NUMBER",
                AggregationRuleType = "AVERAGE"
            });
            attributeLibrary.Add(new AttributeEntity
            {
                Id = new Guid("9151b85a-a980-4301-a102-a6e0c301c193"),
                Name = "Bad",
                DataType = "STRING",
                AggregationRuleType = "PREDOMINANT"
            });

            return attributeLibrary;
        }

        private List<MaintainableAssetLocationEntity> CreateTestMaintainableAssetLocations(List<MaintainableAssetEntity> maintainableAssets)
        {
            var maintainableAssetLocationLibrary = new List<MaintainableAssetLocationEntity>();

            var FirstALocation = new MaintainableAssetLocationEntity()
            {
                Id = Guid.NewGuid(),
                MaintainableAssetId = new Guid("799acb6e-539d-444b-b16a-6defc50b2c64"),
                Discriminator = "SectionLocation",
                LocationIdentifier = "1-00101256",
                MaintainableAsset = maintainableAssets.ElementAt(0),
            };
            var FirstBLocation = new MaintainableAssetLocationEntity()
            {
                Id = Guid.NewGuid(),
                MaintainableAssetId = new Guid("8f80c690-3088-4084-b0e5-a8e070000a06"),
                Discriminator = "SectionLocation",
                LocationIdentifier = "2-13401256",
                MaintainableAsset = maintainableAssets.ElementAt(1),
            };
            var FirstCLocation = new MaintainableAssetLocationEntity()
            {
                Id = Guid.NewGuid(),
                MaintainableAssetId = new Guid("1bb0dd92-db74-45c6-a66a-72ae0c70b636"),
                Discriminator = "SectionLocation",
                LocationIdentifier = "3-5983256",
                MaintainableAsset = maintainableAssets.ElementAt(2),
            };
            var SecondALocation = new MaintainableAssetLocationEntity()
            {
                Id = Guid.NewGuid(),
                MaintainableAssetId = new Guid("3fb90c20-9885-48db-8e47-1c76c5040757"),
                Discriminator = "SectionLocation",
                LocationIdentifier = "4-98451298",
                MaintainableAsset = maintainableAssets.ElementAt(3),
            };
            var SecondBLocation = new MaintainableAssetLocationEntity()
            {
                Id = Guid.NewGuid(),
                MaintainableAssetId = new Guid("6d79de97-1c3c-4da5-9cc4-f5043efa047a"),
                Discriminator = "SectionLocation",
                LocationIdentifier = "5-56451278",
                MaintainableAsset = maintainableAssets.ElementAt(4),
            };

            maintainableAssetLocationLibrary.Add(FirstALocation);
            maintainableAssetLocationLibrary.Add(FirstBLocation);
            maintainableAssetLocationLibrary.Add(FirstCLocation);
            maintainableAssetLocationLibrary.Add(SecondALocation);
            maintainableAssetLocationLibrary.Add(SecondBLocation);

            return maintainableAssetLocationLibrary;
        }

        private void AssignLength(MaintainableAssetEntity asset, double value)
        {
            var lengthAttribute = _attributeLibrary.FirstOrDefault(_ => _.Name == "Length");
            var result = new AggregatedResultEntity()
            {
                Id = Guid.NewGuid(),
                AttributeId = lengthAttribute.Id,
                Attribute = lengthAttribute,
                MaintainableAsset = asset,
                MaintainableAssetId = asset.Id,
                Discriminator = "NumericAggregatedResult",
                Year = 2020,
                NumericValue = value
            };
            asset.AggregatedResults.Add(result);
        }
        private void AssignName(MaintainableAssetEntity asset, string value)
        {
            var nameAttribute = _attributeLibrary.FirstOrDefault(_ => _.Name == "Name");
            var result = new AggregatedResultEntity()
            {
                Id = Guid.NewGuid(),
                AttributeId = nameAttribute.Id,
                Attribute = nameAttribute,
                MaintainableAsset = asset,
                MaintainableAssetId = asset.Id,
                Discriminator = "TextAggregatedResult",
                Year = 2020,
                TextValue = value
            };
            asset.AggregatedResults.Add(result);
        }

        private void AssignKeyAttributes(MaintainableAssetEntity asset)
        {
            var brkeyAttribute = _attributeLibrary.FirstOrDefault(_ => _.Name == "BRKEY_");
            var brKey = new AggregatedResultEntity
            {
                Id = Guid.NewGuid(),
                AttributeId = brkeyAttribute.Id,
                Attribute = brkeyAttribute,
                MaintainableAsset = asset,
                MaintainableAssetId = asset.Id,
                Discriminator = "NumericAggregatedResult",
                Year = 2020,
                NumericValue = int.Parse(asset.AssetName)
            };
            asset.AggregatedResults.Add(brKey);

            var bmsidAttribute = _attributeLibrary.FirstOrDefault(_ => _.Name == "BMSID");
            var bmsid = new AggregatedResultEntity
            {
                Id = Guid.NewGuid(),
                AttributeId = bmsidAttribute.Id,
                Attribute = bmsidAttribute,
                MaintainableAsset = asset,
                MaintainableAssetId = asset.Id,
                Discriminator = "TextAggregatedResult",
                Year = 2020,
                TextValue = asset.AssetName
            };
            asset.AggregatedResults.Add(bmsid);
        }
    }
}
