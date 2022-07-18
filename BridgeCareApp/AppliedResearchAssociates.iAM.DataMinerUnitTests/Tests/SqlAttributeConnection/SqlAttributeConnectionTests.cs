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
            var config = TestConfiguration.Get();
            var connectionString = TestConnectionStrings.BridgeCare(config);
            var unitOfWork = UnitOfWorkSetup.New(config);
            DatabaseResetter.ResetDatabase(unitOfWork);
            var dataSource = SqlDataSourceDtos.WithConnectionString(connectionString);
            var attribute = AttributeConnectionAttributes.String(connectionString, dataSource.Id);
            unitOfWork.DataSourceRepo.UpsertDatasource(dataSource);
            unitOfWork.AttributeRepo.UpsertAttributes(attribute);
            var sqlAttributeConnection = new SqlAttributeConnection(attribute, dataSource);

            // Act
            var result = sqlAttributeConnection.GetData<string>();

            // Assert
            Assert.NotNull(result);
            var resultElement = result.Single();
            Assert.IsType<AttributeDatum<string>>(resultElement);
        }
    }
}
