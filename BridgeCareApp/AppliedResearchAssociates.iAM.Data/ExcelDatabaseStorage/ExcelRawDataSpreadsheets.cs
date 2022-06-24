using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AppliedResearchAssociates.iAM.Data.ExcelDatabaseStorage
{
    public static class ExcelRawDataSpreadsheets
    {
        public static ExcelRawDataSpreadsheet WithColumns(List<ExcelRawDataColumn> columns)
        {
            var returnValue = new ExcelRawDataSpreadsheet
            {
                Columns = columns,
            };
            return returnValue;
        }

        public static ExcelRawDataSpreadsheet WithColumns(params ExcelRawDataColumn[] columns)
        {
            var columnsList = columns.ToList();
            return WithColumns(columnsList);
        }
    }
}
