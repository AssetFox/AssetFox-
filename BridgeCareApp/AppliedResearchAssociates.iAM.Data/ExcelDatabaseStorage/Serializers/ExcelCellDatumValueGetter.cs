using System;
using System.Collections.Generic;
using System.Text;
using AppliedResearchAssociates.iAM.Data.ExcelDatabaseStorage.CellData;
using AppliedResearchAssociates.iAM.Data.Helpers;

namespace AppliedResearchAssociates.iAM.Data.ExcelDatabaseStorage.Serializers
{
    public class ExcelCellDatumValueGetter : IExcelCellDatumVisitor<Unit, object>
    {
        public object Visit(StringExcelCellDatum datum, Unit helper)
        {
            return datum.Value;
        }

        public object Visit(DoubleExcelCellDatum datum, Unit helper)
        {
            return datum.Value;
        }

        public object Visit(DateTimeExcelCellDatum datum, Unit helper)
        {
            return datum.Value;
        }

        public object Visit(EmptyExcelCellDatum datum, Unit helper)
        {
            return null;
        }
    }
}
