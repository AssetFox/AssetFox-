using Attribute = AppliedResearchAssociates.iAM.DataMiner.Attributes.Attribute;
using Xunit;
using Moq;
using System;

namespace AppliedResearchAssociates.iAM.DataMinerUnitTests.Tests.Attributes
{
    public class AttributeTests
    {
        private readonly Mock<Attribute> mockAttribute;
        private readonly Mock<Attribute> mockAttributeToCompare;
        private readonly Guid guid = Guid.Empty;

        public AttributeTests()
        {            
            mockAttribute = new Mock<Attribute>(guid, "Test", "STRING", "TestRuleType", "TestCommand", DataMiner.ConnectionType.MSSQL, "TestString", false, false);
            mockAttributeToCompare = new Mock<Attribute>(guid, "Test", "STRING", "TestRuleType", "TestCommand", DataMiner.ConnectionType.MSSQL, "TestString", false, false);
        }

        [Fact]
        public void EqualsWithObjectTest()
        {
            mockAttribute.Setup(m => m.Equals(It.IsAny<object>())).Returns(false);
            var result = mockAttribute.Object.Equals(new object());
            Assert.False(result);
        }

        [Fact]
        public void EqualsWithAttributeTest()
        {            
            var result = mockAttribute.Object.Equals(mockAttributeToCompare.Object);
            Assert.True(result);
        }

        [Fact]
        public void GetHashCodeTest()
        {
            var result = mockAttribute.Object.GetHashCode();
            Assert.IsType<int>(result);
        }
    }
}
