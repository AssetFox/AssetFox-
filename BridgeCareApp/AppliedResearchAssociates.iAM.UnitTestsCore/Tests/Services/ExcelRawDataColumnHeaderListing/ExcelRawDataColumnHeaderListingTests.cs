using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AppliedResearchAssociates.iAM.Data.ExcelDatabaseStorage;
using AppliedResearchAssociates.iAM.Data.ExcelDatabaseStorage.Serializers;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.TestHelpers;
using AppliedResearchAssociates.iAM.UnitTestsCore.SampleData;
using AppliedResearchAssociates.iAM.UnitTestsCore.TestUtils;
using BridgeCareCore.Services;
using OfficeOpenXml;
using Xunit;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests.Services.ExcelRawDataColumnHeaderListing
{
    public class ExcelRawDataColumnHeaderListingTests
    {
        private static string SampleAttributeDataPath()
        {
            var filename = SampleAttributeDataFilenames.Sample;
            var folder = Directory.GetCurrentDirectory();
            var returnValue = Path.Combine(folder, SampleAttributeDataFolderNames.SampleData, filename);
            return returnValue;
        }

        [Fact]
        public void RawDataSpreadsheetInDb_GetColumnNames_Does()
        {
            var path = SampleAttributeDataPath();
            var stream = File.OpenRead(path);
            var excelPackage = new ExcelPackage(stream);
            var spreadsheetService = TestServices.CreateExcelSpreadsheetImportService(TestHelper.UnitOfWork);
            var dataSourceId = Guid.NewGuid();
            var dataSourceName = RandomStrings.WithPrefix("DataSourceName");
            var dataSourceDto = new ExcelDataSourceDTO
            {
                Id = dataSourceId,
                Name = dataSourceName,
            };
            TestHelper.UnitOfWork.DataSourceRepo.UpsertDatasource(dataSourceDto);
            var importResult = spreadsheetService.ImportRawData(dataSourceDto.Id, excelPackage.Workbook.Worksheets[0]);
            var spreadsheetId = importResult.RawDataId;
            var upsertedSpreadsheet = TestHelper.UnitOfWork.ExcelWorksheetRepository.GetExcelRawData(spreadsheetId);
            var serializedWorksheetcontent = upsertedSpreadsheet.SerializedWorksheetContent;
            var deserializedWorksheetContent = ExcelRawDataSpreadsheetSerializer.Deserialize(serializedWorksheetcontent);
            var columnHeaders = deserializedWorksheetContent.Worksheet.Columns.Select(c => c.Entries.FirstOrDefault().ObjectValue().ToString()).ToList();
            var expectedColumnHeaders = new List<string> { "BRKEY", "DISTRICT", "Inspection_Date" };
            ObjectAssertions.Equivalent(expectedColumnHeaders, columnHeaders);
        }
    }
}
