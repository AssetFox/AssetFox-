using Xunit;
using Moq;
using System;
using AppliedResearchAssociates.iAM.DataMiner.Attributes;
using Attribute = AppliedResearchAssociates.iAM.DataMiner.Attributes.Attribute;
using System.Collections.Generic;
using AppliedResearchAssociates.iAM.DataMinerUnitTests.TestUtils;

namespace AppliedResearchAssociates.iAM.DataMinerUnitTests.Tests.Attributes
{
    public class AttributeDataBuilderTests
    {
        private Mock<Attribute> mockAttribute;
        Mock<AttributeConnection> mockAttributeConnection;

        [Fact]
        public void GetDataForStringTypeTest()
        {
            // Arrange
            Init(AttributeTypeNames.StringType);

            // Act
            var result = AttributeDataBuilder.GetData(mockAttributeConnection.Object);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<List<IAttributeDatum>>(result);
        }

        [Fact]
        public void GetDataForNumberTypeTest()
        {
            // Arrange
            Init(AttributeTypeNames.NumberType);

            var result = AttributeDataBuilder.GetData(mockAttributeConnection.Object);
            Assert.NotNull(result);
            Assert.IsType<List<IAttributeDatum>>(result);
        }

        [Fact]
        public void GetDataForNoTypeTest()
        {
            // Arrange
            Init(string.Empty);

            Assert.Throws<InvalidOperationException>(() => AttributeDataBuilder.GetData(mockAttributeConnection.Object));            
        }

        private void Init(string type)
        {
            mockAttribute = new Mock<Attribute>(Guid.Empty, CommonTestParameterValues.Name, type, CommonTestParameterValues.RuleType, CommonTestParameterValues.TestCommand, DataMiner.ConnectionType.MSSQL, CommonTestParameterValues.ConnectionString, false, false);
            mockAttributeConnection = new Mock<AttributeConnection>(mockAttribute.Object);
            mockAttributeConnection.Setup(m => m.GetData<It.IsAnyType>()).Returns(new List<IAttributeDatum>());
        }
    }
}
