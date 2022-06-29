using Xunit;
using System;
using AppliedResearchAssociates.iAM.Data.Attributes;
using Attribute = AppliedResearchAssociates.iAM.Data.Attributes.Attribute;
using Moq;
using AppliedResearchAssociates.iAM.Data;
using AppliedResearchAssociates.iAM.DataMinerUnitTests.TestUtils;

namespace AppliedResearchAssociates.iAM.DataMinerUnitTests.Tests
{
    public class AttributeConnectionBuilderTests
    {
        private Mock<Attribute> mockAttribute;

        [Fact]
        public void BuildWithMsSqlTest()
        {
            // Arrange
            Init(ConnectionType.MSSQL);

            // Act
            var result = AttributeConnectionBuilder.Build(mockAttribute.Object);           

            // Assert
            Assert.IsType<SqlAttributeConnection>(result);
        }

        private void Init(ConnectionType connectionType)
        {
            mockAttribute = new Mock<Attribute>(Guid.Empty, CommonTestParameterValues.Name, AttributeTypeNames.String, CommonTestParameterValues.RuleType, CommonTestParameterValues.TestCommand, connectionType, CommonTestParameterValues.ConnectionString, Guid.Empty, false, false);
        }
    }
}
