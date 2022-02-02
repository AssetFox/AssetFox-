using Xunit;
using System;
using AppliedResearchAssociates.iAM.DataMiner.Attributes;
using Attribute = AppliedResearchAssociates.iAM.DataMiner.Attributes.Attribute;
using Moq;
using AppliedResearchAssociates.iAM.DataMiner;

namespace AppliedResearchAssociates.iAM.DataMinerUnitTests.Tests
{
    public class AttributeConnectionBuilderTests
    {
        private Mock<Attribute> mockAttribute;

        [Fact]
        public void BuildWithMsSqlTest()
        {
            Init(ConnectionType.MSSQL);            
            var result = AttributeConnectionBuilder.Build(mockAttribute.Object);           
           
            Assert.IsType<SqlAttributeConnection>(result);
        }

        [Fact]
        public void BuildWithMongoDbTest()
        {
            Init(ConnectionType.MONGO_DB);
            
            Assert.Throws<NotImplementedException>(() => AttributeConnectionBuilder.Build(mockAttribute.Object));
        }

        private void Init(ConnectionType connectionType)
        {
            mockAttribute = new Mock<Attribute>(Guid.Empty, "Test", "STRING", "TestRuleType", "TestCommand", connectionType, "TestConnection", false, false);
        }
    }
}
