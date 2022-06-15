using System;
using System.Collections.Generic;
using System.Text;

namespace AppliedResearchAssociates.iAM.Data.ExcelDatabaseStorage
{
    public static class ExcelDatabaseColumns
    {
        public static ExcelDatabaseColumn WithEntries(List<IExcelCellDatum> entries)
        {
            var returnValue = new ExcelDatabaseColumn
            {
                Entries = entries,
            };
            return returnValue;
        }
    }
}
