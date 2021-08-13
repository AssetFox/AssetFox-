using System;
using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.TestUtils
{
    public class TestDataForPennDOTMaintainableAssetRepo
    {
        private List<AttributeEntity> _attributeLibrary;

        public NetworkEntity TestNetwork { get; private set; }
        public IQueryable<AttributeEntity> AttributeLibrary => _attributeLibrary.AsQueryable();
        public IQueryable<MaintainableAssetEntity> MaintainableAssetsLibrary => TestNetwork.MaintainableAssets.AsQueryable();
        public IQueryable<AggregatedResultEntity> AggregatedResultsLibrary => TestNetwork.MaintainableAssets.SelectMany(_ => _.AggregatedResults).AsQueryable();

        public TestDataForPennDOTMaintainableAssetRepo()
        {
            _attributeLibrary = CreateTestAttributes();
            TestNetwork = CreateTestNetwork();
        }

        private NetworkEntity CreateTestNetwork()
        {
            var testNetwork = new NetworkEntity();
            testNetwork.Id = new Guid("c6de7a77-7d81-4e17-9a21-c6ed17f7875c");

            var FirstASection = new MaintainableAssetEntity()
            {
                Id = new Guid("799acb6e-539d-444b-b16a-6defc50b2c64"),
                FacilityName = "1",
                SectionName = "00101256",
                NetworkId = testNetwork.Id
            };
            testNetwork.MaintainableAssets.Add(FirstASection);
            AssignLength(FirstASection, 10);
            AssignName(FirstASection, "First A");
            var FirstBSection = new MaintainableAssetEntity()
            {
                Id = new Guid("8f80c690-3088-4084-b0e5-a8e070000a06"),
                FacilityName = "2",
                SectionName = "13401256",
                NetworkId = testNetwork.Id
            };
            testNetwork.MaintainableAssets.Add(FirstBSection);
            AssignLength(FirstBSection, 15.4);
            AssignName(FirstBSection, "First B");
            var FirstCSection = new MaintainableAssetEntity()
            {
                Id = new Guid("1bb0dd92-db74-45c6-a66a-72ae0c70b636"),
                FacilityName = "3",
                SectionName = "5983256",
                NetworkId = testNetwork.Id
            };
            testNetwork.MaintainableAssets.Add(FirstCSection);
            AssignLength(FirstCSection, 20);
            AssignName(FirstCSection, "First C");
            var SecondASection = new MaintainableAssetEntity()
            {
                Id = new Guid("3fb90c20-9885-48db-8e47-1c76c5040757"),
                FacilityName = "4",
                SectionName = "98451298",
                NetworkId = testNetwork.Id
            };
            testNetwork.MaintainableAssets.Add(SecondASection);
            AssignLength(SecondASection, 10);
            AssignName(SecondASection, "Second A");
            var SecondBSection = new MaintainableAssetEntity()
            {
                Id = new Guid("6d79de97-1c3c-4da5-9cc4-f5043efa047a"),
                FacilityName = "5",
                SectionName = "56451278",
                NetworkId = testNetwork.Id
            };
            testNetwork.MaintainableAssets.Add(SecondBSection);
            AssignLength(SecondBSection, 20);
            AssignName(SecondBSection, "Second B");

            return testNetwork;
        }

        private List<AttributeEntity> CreateTestAttributes()
        {
            var attributeLibrary = new List<AttributeEntity>();

            attributeLibrary.Add(new AttributeEntity()
            {
                Id = Guid.NewGuid(),
                Name = "Length"
            });
            attributeLibrary.Add(new AttributeEntity()
            {
                Id = Guid.NewGuid(),
                Name = "Name"
            });
            attributeLibrary.Add(new AttributeEntity()
            {
                Id = Guid.NewGuid(),
                Name = "NoData"
            });

            return attributeLibrary;
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
    }
}
