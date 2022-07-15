﻿using Xunit;
using System;
using AppliedResearchAssociates.iAM.Data.Attributes;
using AppliedResearchAssociates.iAM.DTOs;
using Attribute = AppliedResearchAssociates.iAM.Data.Attributes.Attribute;
using Moq;
using System.Linq;
using AppliedResearchAssociates.iAM.DataMinerUnitTests.TestUtils;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace AppliedResearchAssociates.iAM.DataMinerUnitTests.Tests.Attributes
{
    public class SqlAttributeConnectionTests// also create tests for ExcelAttributeConnection
    {
        private Attribute? mockAttribute;
        private Mock<SQLDataSourceDTO> mockSQLDataSource = new Mock<SQLDataSourceDTO>();
        private string testCommand = string.Empty;         

        [Fact]
        public void GetData_StringAttributeInDatabase_Gets()
        {
            // Arrange
            Init(AttributeTypeNames.String, CommonTestParameterValues.NameColumn);
            var sqlAttributeConnection = new SqlAttributeConnection(mockAttribute, mockSQLDataSource.Object);

            // Act
            var result = sqlAttributeConnection.GetData<string>();

            // Assert
            Assert.NotNull(result);
            var resultElements = result.ToList(); 
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
            var dataSourceId = Guid.NewGuid();
            mockAttribute = new TextAttribute(
                "TextAttribute",
                Guid.Empty,
                CommonTestParameterValues.Name,
                type,
                testCommand,
                Data.ConnectionType.MSSQL,
                connectionString,
                false,
                false,
                dataSourceId);
        }
    }
}
