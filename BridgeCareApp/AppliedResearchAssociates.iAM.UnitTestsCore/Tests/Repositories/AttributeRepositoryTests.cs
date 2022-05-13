using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataMiner.Attributes;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories;
using AppliedResearchAssociates.iAM.UnitTestsCore.TestUtils;
using Xunit;
using DataMinerAttribute = AppliedResearchAssociates.iAM.DataMiner.Attributes.Attribute;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests.Repositories
{
    public class AttributeRepositoryTests
    {
        private readonly TestHelper _testHelper;
        private IAttributeRepository attributeRepository => _testHelper.UnitOfWork.AttributeRepo;

        public AttributeRepositoryTests()
        {
            _testHelper = TestHelper.Instance;
            if (!_testHelper.DbContext.Attribute.Any())
            {
                _testHelper.CreateAttributes();
                _testHelper.CreateNetwork();
                _testHelper.CreateSimulation();
                _testHelper.CreateCalculatedAttributeLibrary();
            }
        }

        [Fact]
        public async Task AddNumericAttribute_Does()
        {
            var repo = attributeRepository;
            var attribute = AttributeTestSetup.Numeric();
            var attributes = new List<DataMinerAttribute> { attribute };
            repo.UpsertAttributes(attributes);
            var attributesAfter = await repo.Attributes();
            var attributeAfter = attributesAfter.Single(a => a.Id == attribute.Id);
            Assert.Equal(1, attributeAfter.Minimum);
            Assert.Equal(3, attributeAfter.Maximum);
            Assert.Equal("2", attributeAfter.DefaultValue);
            Assert.Equal(attribute.Name, attributeAfter.Name);
        }

        [Fact]
        public async Task AddTextAttribute_Does()
        {
            var repo = attributeRepository;
            var attribute = AttributeTestSetup.Text();
            var attributes = new List<DataMinerAttribute> { attribute };
            repo.UpsertAttributes(attributes);
            var attributesAfter = await repo.Attributes();
            var attributeAfter = attributesAfter.Single(a => a.Id == attribute.Id);
            Assert.Null(attributeAfter.Minimum);
            Assert.Null(attributeAfter.Maximum);
            Assert.Equal("defaultValue", attributeAfter.DefaultValue);
            Assert.Equal(attribute.Name, attributeAfter.Name);
        }

        [Fact]
        public async Task AttributeInDb_UpdateAllowedFields_Does()
        {
            var repo = attributeRepository;
            var attribute = AttributeTestSetup.Numeric();
            var attributes = new List<DataMinerAttribute> { attribute };
            repo.UpsertAttributes(attributes);
            var updateAttribute = new NumericAttribute(
                20, 100, 10, attribute.Id, attribute.Name, "updatedRuleType",
                "updatedCommand", DataMiner.ConnectionType.MONGO_DB, "connectionString",
                !attribute.IsCalculated, !attribute.IsAscending);
            var updateAttributes = new List<DataMinerAttribute> { updateAttribute };
            repo.UpsertAttributes(updateAttributes);
            var attributesAfter = await repo.Attributes();
            var attributeAfter = attributesAfter.Single(a => a.Id == attribute.Id);
            Assert.Equal(10, attributeAfter.Minimum);
            Assert.Equal(100, attributeAfter.Maximum);
            Assert.Equal("20", attributeAfter.DefaultValue);
            Assert.Equal(attribute.Name, attributeAfter.Name);
        }

        [Fact]
        public async Task NumericAttributeInDb_UpdateNameOnly_NameIsNotChanged()
        {
            var repo = attributeRepository;
            var attribute = AttributeTestSetup.Numeric();
            var attributes = new List<DataMinerAttribute> { attribute };
            repo.UpsertAttributes(attributes);
            var updateAttribute = AttributeTestSetup.Numeric(attribute.Id, "updated name should fail");
            var updateAttributes = new List<DataMinerAttribute> { updateAttribute };
            repo.UpsertAttributes(updateAttributes);
            var attributesAfter = await repo.Attributes();
            var attributeAfter = attributesAfter.Single(a => a.Id == attribute.Id);
            Assert.Equal(attribute.Name, attributeAfter.Name);
        }

        [Fact]
        public async Task NumericAttributeInDb_UpdateManyFields_NotChanged()
        {
            var repo = attributeRepository;
            var attribute = AttributeTestSetup.Numeric();
            var attributes = new List<DataMinerAttribute> { attribute };
            repo.UpsertAttributes(attributes);
            var updateAttribute = new NumericAttribute(222, 1000, 123, attribute.Id, "this should kill the update", "update rule type", "update command", DataMiner.ConnectionType.MSSQL, "connectionString", !attribute.IsCalculated, !attribute.IsAscending);
            var updateAttributes = new List<DataMinerAttribute> { updateAttribute };
            repo.UpsertAttributes(updateAttributes);
            var attributesAfter = await repo.Attributes();
            var attributeAfter = attributesAfter.Single(a => a.Id == attribute.Id);
            Assert.Equal(attribute.Name, attributeAfter.Name);
        }
    }
}
