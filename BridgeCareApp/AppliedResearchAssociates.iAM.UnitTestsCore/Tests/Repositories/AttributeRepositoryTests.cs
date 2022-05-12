using System;
using System.Collections.Generic;
using System.Linq;
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
        }

        [Fact]
        public void AddAttribute_Does()
        {
            var repo = _testHelper.UnitOfWork.AttributeRepo;
            var attributeId = Guid.NewGuid();
            var attribute = AttributeTestSetup.Numeric();
            var attributes = new List<DataMinerAttribute> { attribute };
            repo.UpsertAttributes(attributes);
            var attributeInDb = _testHelper.DbContext.Attribute.Single(a => a.Id == attributeId);
            Assert.Equal(1, attribute.Minimum);
            Assert.Equal(3, attribute.Maximum);
            Assert.Equal(2, attribute.DefaultValue);
            Assert.Equal(attribute.Name, attributeInDb.Name);
        }
    }
}
