using System;
using System.Collections.Generic;
using System.Text;

namespace AppliedResearchAssociates.iAM.Data.ExcelDatabaseStorage.Serializers
{
    public class ExcelCellDatumDeserializationResult
    {
        public IExcelCellDatum Datum { get; set; }
        public string Message { get; set; }
    }
}
