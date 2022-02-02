using Xunit;
using Moq;
using System;
using AppliedResearchAssociates.iAM.DataMiner.Attributes;
using Attribute = AppliedResearchAssociates.iAM.DataMiner.Attributes.Attribute;
using System.Collections.Generic;

namespace AppliedResearchAssociates.iAM.DataMinerUnitTests.Tests.Attributes
{
    public class AttributeDataBuilderTests
    {
        private Mock<Attribute> mockAttribute;
        Mock<AttributeConnection> mockAttributeConnection;

        [Fact]
        public void GetDataForStringTypeTest()
        {
            Init("STRING");

            var result = AttributeDataBuilder.GetData(mockAttributeConnection.Object);
            Assert.NotNull(result);
            Assert.IsType<List<IAttributeDatum>>(result);
        }

        [Fact]
        public void GetDataForNumberTypeTest()
        {
            Init("NUMBER");

            var result = AttributeDataBuilder.GetData(mockAttributeConnection.Object);
            Assert.NotNull(result);
            Assert.IsType<List<IAttributeDatum>>(result);
        }

        [Fact]
        public void GetDataForNoTypeTest()
        {
            Init("");

            Assert.Throws<InvalidOperationException>(() => AttributeDataBuilder.GetData(mockAttributeConnection.Object));            
        }

        private void Init(string type)
        {
            mockAttribute = new Mock<Attribute>(Guid.Empty, "Test", type, "TestRuleType", "TestCommand", DataMiner.ConnectionType.MSSQL, "TestString", false, false);
            mockAttributeConnection = new Mock<AttributeConnection>(mockAttribute.Object);
            mockAttributeConnection.Setup(m => m.GetData<It.IsAnyType>()).Returns(new List<IAttributeDatum>());
        }
    }
}
