using Xunit;
using System;
using AppliedResearchAssociates.iAM.Data.Attributes;
using AppliedResearchAssociates.iAM.DTOs;
using Attribute = AppliedResearchAssociates.iAM.Data.Attributes.Attribute;
using Moq;
using System.Linq;
using AppliedResearchAssociates.iAM.DataMinerUnitTests.TestUtils;
using Microsoft.Extensions.Configuration;
using System.IO;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL;
using Microsoft.EntityFrameworkCore;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories;
using AppliedResearchAssociates.iAM.DTOs.Abstract;

namespace AppliedResearchAssociates.iAM.DataMinerUnitTests.Tests.Attributes
{
    public class SqlAttributeConnectionTests// also create tests for ExcelAttributeConnection
    {
        [Fact]
        public void GetData_StringAttributeInDatabase_Gets()
        {
            // Arrange
            var connectionString = GetConnectionString();
            var dbContext = new IAMContext(new DbContextOptionsBuilder<IAMContext>()
                .UseSqlServer(connectionString)
                .Options);
            var config = TestConfiguration.Get();
            var unitOfWork = new UnitOfDataPersistenceWork(config, dbContext);
            DatabaseResetter.ResetDatabase(unitOfWork);
            var dataSource = GetDataSource();
            var mockAttribute = GetAttribute(dataSource, AttributeTypeNames.String, CommonTestParameterValues.NameColumn);
            unitOfWork.DataSourceRepo.UpsertDatasource(dataSource);
            unitOfWork.AttributeRepo.UpsertAttributes(mockAttribute);
            var sqlAttributeConnection = new SqlAttributeConnection(mockAttribute, dataSource);

            // Act
            var result = sqlAttributeConnection.GetData<string>();

            // Assert
            Assert.NotNull(result);
            var resultElements = result.ToList();
            var resultElement = resultElements.Single();
            Assert.IsType<AttributeDatum<string>>(resultElement);
        }

        private static SQLDataSourceDTO GetDataSource()
        {
            var connectionString = GetConnectionString();
            var sqlDataSource = new SQLDataSourceDTO
            {
                ConnectionString = connectionString,
                Id = Guid.NewGuid(),
                Name = "TestDataSource",
            };
            return sqlDataSource;
        }


        private static string GetConnectionString()
        {
            var config = TestConfiguration.Get();
            var returnValue = config.GetConnectionString("BridgeCareConnex");
            return returnValue;
        }

        private TextAttribute GetAttribute(BaseDataSourceDTO dataSource, string type, string column)
        {
            var dataSourceId = dataSource.Id;
            var testCommand = "SELECT Top 1 Id AS ID_, Name AS FACILITY, Name AS SECTION, Name AS LOCATION_IDENTIFIER, CreatedDate AS DATE_, " + column + " AS DATA_ FROM dbo.Attribute";
            var connectionString = GetConnectionString();
            return new TextAttribute(
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
