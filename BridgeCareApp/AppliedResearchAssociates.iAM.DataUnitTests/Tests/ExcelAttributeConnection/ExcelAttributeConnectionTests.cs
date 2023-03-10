using System.Linq;
using AppliedResearchAssociates.iAM.Data.Attributes;
using AppliedResearchAssociates.iAM.Data.ExcelDatabaseStorage.Serializers;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories;
using Xunit;

namespace AppliedResearchAssociates.iAM.DataUnitTests.Tests.Attributes
{
    public class ExcelAttributeConnectionTests
    {
        [Fact]
        public void GetData_StringAttributeInDatabase_Gets()
        {
            // Arrange
            var config = TestConfiguration.Get();
            var unitOfWork = UnitOfWorkSetup.New(config);
            DatabaseResetter.ResetDatabase(unitOfWork);
            var dataSource = DataSourceTestSetup.DtoForExcelDataSourceInDb(unitOfWork);
            var attribute = AttributeConnectionAttributes.ForExcelTestData(dataSource.Id);
            var importedSpreadsheet = ExcelRawDataSetup.RawData(dataSource.Id);
            var deserializationResult = ExcelRawDataSpreadsheetSerializer.Deserialize(importedSpreadsheet.SerializedWorksheetContent);
            var rawDataSpreadsheet = deserializationResult.Worksheet;

            unitOfWork.AttributeRepo.UpsertAttributes(attribute);

            var excelAttributeConnection = new ExcelAttributeConnection(attribute, dataSource, rawDataSpreadsheet);

            // Act
            var result = excelAttributeConnection.GetData<string>();

            // Assert
            var resultList = result.ToList();
            Assert.Equal(4, resultList.Count);
            Assert.IsType<AttributeDatum<string>>(resultList[0]);
        }
    }
}
