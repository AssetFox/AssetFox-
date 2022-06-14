using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using AppliedResearchAssociates.iAM.Data.Helpers;

namespace AppliedResearchAssociates.iAM.Data.ExcelDatabaseStorage.Visitors
{
    public class ExcelDatumJsonSerializationVisitor : IExcelCellDatumVisitor<Unit, string>
    {
        public string Visit(StringExcelCellDatum datum, Unit helper)
        {
            var returnValue = @$"""{datum.Value}""";
            return returnValue;
        }
        public string Visit(DoubleExcelCellDatum datum, Unit helper)
        {
            var stringifiedContent = datum.Value.ToString("R");
            return stringifiedContent;
        }

        public string Visit(DateTimeExcelCellDatum datum, Unit helper)
        {
            var returnValue = "D" + JsonSerializer.Serialize(datum.Value);
            return returnValue;
        }
    }
}

