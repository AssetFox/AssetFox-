using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.TestData
{
    public class TestDataForPennDOTAssetDataRepo
    {
        private List<AttributeEntity> _attributeLibrary;

        public NetworkEntity TestNetwork { get; private set; }
        public IQueryable<AttributeEntity> AttributeLibrary => _attributeLibrary.AsQueryable();
        public IQueryable<SectionEntity> SectionLibrary => TestNetwork.Facilities.SelectMany(_ => _.Sections).AsQueryable();

        public TestDataForPennDOTAssetDataRepo()
        {
            _attributeLibrary = CreateTestAttributes();
            TestNetwork = CreateTestNetwork();
        }

        public NetworkEntity CreateTestNetwork()
        {
            var testNetwork = new NetworkEntity();
            testNetwork.Id = new Guid("c6de7a77-7d81-4e17-9a21-c6ed17f7875c");

            var FirstFacility = new FacilityEntity()
            {
                Name = "First",
                Id = new Guid("8f908a9c-7ee9-40ea-bc42-f88d76d47cb2"),
                NetworkId = testNetwork.Id,
                Network = testNetwork
            };
            testNetwork.Facilities.Add(FirstFacility);
            var SecondFacility = new FacilityEntity()
            {
                Name = "Second",
                Id = new Guid("0279fd78-4116-4796-8a0a-f27df88f4307"),
                NetworkId = testNetwork.Id,
                Network = testNetwork
            };
            testNetwork.Facilities.Add(SecondFacility);

            var FirstASection = new SectionEntity()
            {
                Name = "FirstA",
                Id = new Guid("799acb6e-539d-444b-b16a-6defc50b2c64"),
                FacilityId = FirstFacility.Id,
                Area = 100,
                AreaUnit = "ft2",
                Facility = FirstFacility,
            };
            FirstFacility.Sections.Add(FirstASection);
            AssignBRKey(FirstASection, 1);
            AssignBMSID(FirstASection, "00101256");
            AssignLength(FirstASection, 10);
            AssignName(FirstASection, "First A");

            var FirstBSection = new SectionEntity()
            {
                Name = "FirstB",
                Id = new Guid("8f80c690-3088-4084-b0e5-a8e070000a06"),
                FacilityId = FirstFacility.Id,
                Area = 251.45,
                AreaUnit = "ft2",
                Facility = FirstFacility,
            };
            FirstFacility.Sections.Add(FirstBSection);
            AssignBRKey(FirstBSection, 2);
            AssignBMSID(FirstBSection, "13401256");
            AssignLength(FirstBSection, 15.4);
            AssignName(FirstBSection, "First B");

            var FirstCSection = new SectionEntity()
            {
                Name = "First C",
                Id = new Guid("1bb0dd92-db74-45c6-a66a-72ae0c70b636"),
                FacilityId = FirstFacility.Id,
                Area = 200,
                AreaUnit = "ft2",
                Facility = FirstFacility,
            };
            FirstFacility.Sections.Add(FirstCSection);
            AssignBRKey(FirstCSection, 3);
            AssignBMSID(FirstCSection, "5983256");
            AssignLength(FirstCSection, 20);
            AssignName(FirstCSection, "First C");

            var SecondASection = new SectionEntity()
            {
                Name = "SecondA",
                Id = new Guid("3fb90c20-9885-48db-8e47-1c76c5040757"),
                FacilityId = SecondFacility.Id,
                Area = 100,
                AreaUnit = "ft2",
                Facility = SecondFacility,
            };
            SecondFacility.Sections.Add(SecondASection);
            AssignBRKey(SecondASection, 4);
            AssignBMSID(SecondASection, "98451298");
            AssignLength(SecondASection, 10);
            AssignName(SecondASection, "Second A");

            var SecondBSection = new SectionEntity()
            {
                Name = "SecondB",
                Id = new Guid("6d79de97-1c3c-4da5-9cc4-f5043efa047a"),
                FacilityId = SecondFacility.Id,
                Area = 200,
                AreaUnit = "ft2",
                Facility = SecondFacility,
            };
            SecondFacility.Sections.Add(SecondBSection);
            AssignBRKey(SecondBSection, 5);
            AssignBMSID(SecondBSection, "56451278");
            AssignLength(SecondBSection, 20);
            AssignName(SecondBSection, "Second B");

            return testNetwork;
        }

        public List<AttributeEntity> CreateTestAttributes()
        {
            var attributeLibrary = new List<AttributeEntity>();

            attributeLibrary.Add(new AttributeEntity()
            {
                Id = new Guid(),
                Name = "BRKey"
            });
            attributeLibrary.Add(new AttributeEntity()
            {
                Id = new Guid(),
                Name = "BMSID"
            });
            attributeLibrary.Add(new AttributeEntity()
            {
                Id = new Guid(),
                Name = "Length"
            });
            attributeLibrary.Add(new AttributeEntity()
            {
                Id = new Guid(),
                Name = "Name"
            });

            return attributeLibrary;
        }

        private void AssignBRKey(SectionEntity section, double value)
        {
            var brkeyAttribute = _attributeLibrary.FirstOrDefault(_ => _.Name == "BRKey");
            var newNumericAttribute = new NumericAttributeValueHistoryEntity()
            {
                Year = 2020,
                Value = value,
                Id = new Guid(),
                SectionId = section.Id,
                AttributeId = brkeyAttribute.Id,
                Attribute = brkeyAttribute,
                Section = section
            };
            section.NumericAttributeValueHistories.Add(newNumericAttribute);
        }

        private void AssignBMSID(SectionEntity section, string value)
        {
            var bmsidAttribute = _attributeLibrary.FirstOrDefault(_ => _.Name == "BMSID");
            var newtextAttribute = new TextAttributeValueHistoryEntity()
            {
                Year = 2020,
                Value = value,
                Id = new Guid(),
                SectionId = section.Id,
                AttributeId = bmsidAttribute.Id,
                Attribute = bmsidAttribute,
                Section = section
            };
            section.TextAttributeValueHistories.Add(newtextAttribute);
        }

        private void AssignLength(SectionEntity section, double value)
        {
            var lengthAttribute = _attributeLibrary.FirstOrDefault(_ => _.Name == "Length");
            var newNumericAttribute = new NumericAttributeValueHistoryEntity()
            {
                Year = 2020,
                Value = value,
                Id = new Guid(),
                SectionId = section.Id,
                AttributeId = lengthAttribute.Id,
                Attribute = lengthAttribute,
                Section = section
            };
            section.NumericAttributeValueHistories.Add(newNumericAttribute);
        }

        private void AssignName(SectionEntity section, string value)
        {
            var nameAttribute = _attributeLibrary.FirstOrDefault(_ => _.Name == "Name");
            var newtextAttribute = new TextAttributeValueHistoryEntity()
            {
                Year = 2020,
                Value = value,
                Id = new Guid(),
                SectionId = section.Id,
                AttributeId = nameAttribute.Id,
                Attribute = nameAttribute,
                Section = section
            };
            section.TextAttributeValueHistories.Add(newtextAttribute);
        }
    }
}
