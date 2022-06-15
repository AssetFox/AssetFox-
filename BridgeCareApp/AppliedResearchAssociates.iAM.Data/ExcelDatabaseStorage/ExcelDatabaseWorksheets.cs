using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AppliedResearchAssociates.iAM.Data.ExcelDatabaseStorage
{
    public static class ExcelDatabaseWorksheets
    {
        public static ExcelDatabaseWorksheet WithColumns(List<ExcelDatabaseColumn> columns)
        {
            var returnValue = new ExcelDatabaseWorksheet
            {
                Columns = columns,
            };
            return returnValue;
        }

        public static ExcelDatabaseWorksheet WithColumns(params ExcelDatabaseColumn[] columns)
        {
            var columnsList = columns.ToList();
            return WithColumns(columnsList);
        }
    }
}
