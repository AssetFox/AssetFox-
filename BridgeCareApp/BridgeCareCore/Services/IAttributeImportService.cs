using System;
using AppliedResearchAssociates.iAM.Data.ExcelDatabaseStorage;
using AppliedResearchAssociates.iAM.DTOs;

namespace BridgeCareCore.Services
{
    public interface IAttributeImportService
    {
        AttributesImportResultDTO ImportExcelAttributes(string keyColumnName, string inspectionDateColumnName, string spatialWeightingValue, ExcelRawDataSpreadsheet worksheet);
    }
}
