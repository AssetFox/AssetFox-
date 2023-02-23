using Xunit;
using AppliedResearchAssociates.iAM.Data.Attributes;
using System.Linq;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories;

namespace AppliedResearchAssociates.iAM.DataUnitTests.Tests.Attributes
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
            DatabaseResetter.EnsureDatabaseExists(unitOfWork);
            var dataSource = DataSourceTestSetup.DtoForSqlDataSourceInDb(unitOfWork, connectionString);
            var attribute = AttributeConnectionAttributes.String(connectionString, dataSource.Id);
            unitOfWork.AttributeRepo.UpsertAttributes(attribute);
            var sqlAttributeConnection = new SqlAttributeConnection(attribute, dataSource);

            // Act
            var result = sqlAttributeConnection.GetData<string>();

            // Assert
            var resultElement = result.Single();
            Assert.IsType<AttributeDatum<string>>(resultElement);
        }
    }
}
