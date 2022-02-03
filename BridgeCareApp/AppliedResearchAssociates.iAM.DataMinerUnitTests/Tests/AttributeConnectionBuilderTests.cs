using Xunit;
using System;
using AppliedResearchAssociates.iAM.DataMiner.Attributes;
using Attribute = AppliedResearchAssociates.iAM.DataMiner.Attributes.Attribute;
using Moq;
using AppliedResearchAssociates.iAM.DataMiner;
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

        [Fact]
        public void BuildWithMongoDbTest()
        {
            // Arrange
            Init(ConnectionType.MONGO_DB);

            // Act, Assert
            Assert.Throws<NotImplementedException>(() => AttributeConnectionBuilder.Build(mockAttribute.Object));
        }

        private void Init(ConnectionType connectionType)
        {
            mockAttribute = new Mock<Attribute>(Guid.Empty, CommonTestParameterValues.Name, AttributeTypeNames.StringType, CommonTestParameterValues.RuleType, CommonTestParameterValues.TestCommand, connectionType, CommonTestParameterValues.ConnectionString, false, false);
        }
    }
}
