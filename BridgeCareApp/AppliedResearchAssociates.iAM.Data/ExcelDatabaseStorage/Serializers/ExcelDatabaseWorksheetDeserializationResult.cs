using System;
using System.Collections.Generic;
using System.Text;

namespace AppliedResearchAssociates.iAM.Data.ExcelDatabaseStorage.Serializers
{
    internal class ExcelDatabaseWorksheetDeserializationResult
    {
        public ExcelDatabaseWorksheet Worksheet { get; set; }
        public string Message { get; set; }
    }
}
