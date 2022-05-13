using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.UnitTestsCore.TestUtils;
using Xunit;
using DataMinerAttribute = AppliedResearchAssociates.iAM.DataMiner.Attributes.Attribute;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests.Repositories
{
    public class AttributeRepositoryTests
    {
        private readonly TestHelper _testHelper;

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
            var repo = _testHelper.UnitOfWork.AttributeRepo;
            var attributeId = Guid.NewGuid();
            var attribute = AttributeTestSetup.Numeric();
            var attributes = new List<DataMinerAttribute> { attribute };
            repo.UpsertAttributes(attributes);
            var attributesAfter = await repo.Attributes();
            var attributeInDb = attributesAfter.Single(a => a.Id == attribute.Id);
            Assert.Equal(1, attributeInDb.Minimum);
            Assert.Equal(3, attributeInDb.Maximum);
            Assert.Equal("2", attributeInDb.DefaultValue);
            Assert.Equal(attribute.Name, attributeInDb.Name);
        }

        [Fact]
        public async Task AddTextAttribute_Does()
        {
            var repo = _testHelper.UnitOfWork.AttributeRepo;
            var attributeId = Guid.NewGuid();
            var attribute = AttributeTestSetup.Text();
            var attributes = new List<DataMinerAttribute> { attribute };
            repo.UpsertAttributes(attributes);
            var attributesAfter = await repo.Attributes();
            var attributeInDb = attributesAfter.Single(a => a.Id == attribute.Id);
            Assert.Null(attributeInDb.Minimum);
            Assert.Null(attributeInDb.Maximum);
            Assert.Equal("defaultValue", attributeInDb.DefaultValue);
            Assert.Equal(attribute.Name, attributeInDb.Name);
        }

        public async Task AttributeInDb_UpdateAllowedFields_Does()
        {

        }
    }
}
