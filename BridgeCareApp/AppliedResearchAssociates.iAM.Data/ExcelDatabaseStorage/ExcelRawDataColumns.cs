using System;
using System.Collections.Generic;
using System.Text;

namespace AppliedResearchAssociates.iAM.Data.ExcelDatabaseStorage
{
    public static class ExcelRawDataColumns
    {
        public static ExcelRawDataColumn WithEntries(List<IExcelCellDatum> entries)
        {
            var returnValue = new ExcelRawDataColumn
            {
                Entries = entries,
            };
            return returnValue;
        }
    }
}
