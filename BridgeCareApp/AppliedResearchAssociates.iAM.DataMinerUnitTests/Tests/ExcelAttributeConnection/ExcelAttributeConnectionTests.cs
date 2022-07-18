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
using AppliedResearchAssociates.iAM.Data.ExcelDatabaseStorage;
using AppliedResearchAssociates.iAM.Data.ExcelDatabaseStorage.Serializers;

namespace AppliedResearchAssociates.iAM.DataMinerUnitTests.Tests.Attributes
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
            var dataSource = ExcelDataSourceDtos.WithColumnNames("Inspection_Date", "BRKEY");
            var attribute = AttributeConnectionAttributes.ForExcelTestData(dataSource.Id);
            var importedSpreadsheet = ExcelRawDataSetup.RawData(dataSource.Id);
            var deserializationResult = ExcelRawDataSpreadsheetSerializer.Deserialize(importedSpreadsheet.SerializedWorksheetContent);
            var rawDataSpreadsheet = deserializationResult.Worksheet;

            unitOfWork.DataSourceRepo.UpsertDatasource(dataSource);
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
