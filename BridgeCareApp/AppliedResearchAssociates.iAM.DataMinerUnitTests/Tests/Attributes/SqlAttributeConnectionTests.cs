using Xunit;
using System;
using AppliedResearchAssociates.iAM.Data.Attributes;
using Attribute = AppliedResearchAssociates.iAM.Data.Attributes.Attribute;
using Moq;
using System.Linq;
using AppliedResearchAssociates.iAM.DataMinerUnitTests.TestUtils;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace AppliedResearchAssociates.iAM.DataMinerUnitTests.Tests.Attributes
{
    public class SqlAttributeConnectionTests
    {
        private Mock<Attribute> mockAttribute;
        private string testCommand = string.Empty;         

        [Fact (Skip ="This is accessing the real db. It shouldn't. WjTodo fix that.")]
        public void GetData_StringAttributeInDatabase_Gets()
        {
            // Arrange
            Init(AttributeTypeNames.String, CommonTestParameterValues.NameColumn);
            var sqlAttributeConnection = new SqlAttributeConnection(mockAttribute.Object);

            // Act
            var result = sqlAttributeConnection.GetData<string>();

            // Assert
            Assert.NotNull(result);
            var resultElements = result.ToList();  // WjTodo seeing a failure here when the "real" db does not exist.
            var resultElement = resultElements.Single();
            Assert.IsType<AttributeDatum<string>>(resultElement);
        }

        private static string GetConnectionString()
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("testConnections.json")
                .Build();
            var returnValue = config.GetConnectionString("BridgeCareConnexRealDb");
            return returnValue;
        }

        private void Init(string type, string dataColumn)
        {
            testCommand = "SELECT Top 1 Id AS ID_, Name AS FACILITY, Name AS SECTION, Name AS LOCATION_IDENTIFIER, CreatedDate AS DATE_, " + dataColumn + " AS DATA_ FROM dbo.Attribute";
            var connectionString = GetConnectionString();
            mockAttribute = new Mock<Attribute>(Guid.Empty, CommonTestParameterValues.Name, type, CommonTestParameterValues.RuleType, testCommand, Data.ConnectionType.MSSQL, connectionString, false, false);
        }
    }
}
