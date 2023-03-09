using System;
using AppliedResearchAssociates.iAM.Data.ExcelDatabaseStorage.Serializers;
using AppliedResearchAssociates.iAM.DTOs;

namespace AppliedResearchAssociates.iAM.Data.ExcelDatabaseStorage
{
    public static class ExcelRawDataSpreadsheetSerializationMapper
    {
        public static ExcelRawDataDTO ToDTO(this ExcelRawDataSpreadsheet worksheet, Guid dataSourceID, Guid worksheetId)
        {
            var serializedContent = ExcelRawDataSpreadsheetSerializer.Serialize(worksheet);
            var returnValue = new ExcelRawDataDTO
            {
                Id = worksheetId,
                DataSourceId = dataSourceID,
                SerializedWorksheetContent = serializedContent,
            };
            return returnValue;
        }
    }
}
