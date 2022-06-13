using System;
using System.Collections.Generic;
using System.Text;
using AppliedResearchAssociates.iAM.Data.ExcelDatabaseStorage.Visitors;
using AppliedResearchAssociates.iAM.Data.Helpers;

namespace AppliedResearchAssociates.iAM.Data.ExcelDatabaseStorage
{
    public static class ExcelCellDatumSerializer
    {
        private static ExcelDatumJsonSerializationVisitor SerializationVisitor = new ExcelDatumJsonSerializationVisitor();
        public static string Serialize(IExcelCellDatum datum)
        {
            var unit = Unit.Default;
            var returnValue = datum.Accept<string, Unit>(SerializationVisitor, unit);
            return returnValue;
        }
    }
}
