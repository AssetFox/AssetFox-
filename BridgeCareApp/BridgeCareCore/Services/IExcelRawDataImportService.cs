using System;
using AppliedResearchAssociates.iAM.DTOs;
using OfficeOpenXml;

namespace BridgeCareCore.Services
{
    public interface IExcelRawDataImportService
    {
        ExcelRawDataImportResultDTO ImportRawData(Guid dataSourceId, ExcelWorksheet worksheet, bool includeColumnsWithoutTitles = false);
    }
}
