using System;
using AppliedResearchAssociates.iAM.DTOs;
using OfficeOpenXml;

namespace BridgeCareCore.Services
{
    public interface IExcelSpreadsheetImportService
    {
        ExcelSpreadsheetImportResultDTO ImportSpreadsheet(Guid dataSourceId, ExcelWorksheet worksheet, bool includeColumnsWithoutTitles = false);
    }
}