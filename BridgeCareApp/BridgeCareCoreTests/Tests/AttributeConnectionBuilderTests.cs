using Xunit;
using System;
using AppliedResearchAssociates.iAM.Data.Attributes;
using Attribute = AppliedResearchAssociates.iAM.Data.Attributes.Attribute;
using Moq;
using AppliedResearchAssociates.iAM.Data;
using AppliedResearchAssociates.iAM.DataUnitTests.TestUtils;
using AppliedResearchAssociates.iAM.DTOs;
using BridgeCareCore.Services;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM;

namespace BridgeCareCoreTests.Tests
{
    public class AttributeConnectionBuilderTests
    {
        private Mock<Attribute> mockAttribute;
        private Mock<SQLDataSourceDTO> mockSQLDataSourceDTO = new Mock<SQLDataSourceDTO>();
        private Mock<IUnitOfWork> mockUnitOfWork = new Mock<IUnitOfWork>();

        [Fact]
        public void BuildWithMsSqlTest()
        {
            // Arrange
            Init(ConnectionType.MSSQL);

            // Act
            var result = AttributeConnectionBuilder.Build(mockAttribute.Object, mockSQLDataSourceDTO.Object, mockUnitOfWork.Object);           

            // Assert
            Assert.IsType<SqlAttributeConnection>(result);
        }

        private void Init(ConnectionType connectionType)
        {
            mockAttribute = new Mock<Attribute>(Guid.Empty, CommonTestParameterValues.Name, AttributeTypeNames.String, CommonTestParameterValues.RuleType, CommonTestParameterValues.TestCommand, connectionType, CommonTestParameterValues.ConnectionString, Guid.Empty, false, false);
        }
    }
}
