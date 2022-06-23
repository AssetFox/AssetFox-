using System;
using System.Collections.Generic;
using System.Text;

namespace AppliedResearchAssociates.iAM.Data.ExcelDatabaseStorage.Serializers
{
    public class ExcelRawDataSpreadsheetDeserializationResult
    {
        public ExcelRawDataSpreadsheet Worksheet { get; set; }
        public string Message { get; set; }
    }
}
