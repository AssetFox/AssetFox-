using Attribute = AppliedResearchAssociates.iAM.Data.Attributes.Attribute;
using Xunit;
using Moq;
using System;
using AppliedResearchAssociates.iAM.DataMinerUnitTests.TestUtils;

namespace AppliedResearchAssociates.iAM.DataMinerUnitTests.Tests.Attributes
{
    public class AttributeTests
    {
        private readonly Mock<Attribute> mockAttribute;
        private readonly Mock<Attribute> mockAttributeToCompare;
        private readonly Guid guid = Guid.Empty;

        public AttributeTests()
        {            
            mockAttribute = new Mock<Attribute>(guid, CommonTestParameterValues.Name, AttributeTypeNames.String, CommonTestParameterValues.RuleType, CommonTestParameterValues.TestCommand, Data.ConnectionType.MSSQL, CommonTestParameterValues.ConnectionString, Guid.Empty, false, false);
            mockAttributeToCompare = new Mock<Attribute>(guid, CommonTestParameterValues.Name, AttributeTypeNames.String, CommonTestParameterValues.RuleType, CommonTestParameterValues.TestCommand, Data.ConnectionType.MSSQL, CommonTestParameterValues.ConnectionString, Guid.Empty, false, false);
        }

        [Fact]
        public void EqualsWithObjectTest()
        {
            // Arrange
            mockAttribute.Setup(m => m.Equals(It.IsAny<object>())).Returns(false);

            // Act
            var result = mockAttribute.Object.Equals(new object());

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void EqualsWithAttributeTest()
        {
            // Act
            var result = mockAttribute.Object.Equals(mockAttributeToCompare.Object);

            // Assert
            Assert.True(result);
        }
    }
}
