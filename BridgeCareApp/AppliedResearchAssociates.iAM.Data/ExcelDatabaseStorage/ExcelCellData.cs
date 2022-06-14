using System;
using System.Collections.Generic;
using System.Text;

namespace AppliedResearchAssociates.iAM.Data.ExcelDatabaseStorage
{
    public static class ExcelCellData
    {
        public static DateTimeExcelCellDatum DateTime(DateTime dateTime)
            => new DateTimeExcelCellDatum
            {
                Value = dateTime,
            };

        public static StringExcelCellDatum String(string str)
            => new StringExcelCellDatum
            {
                Value = str,
            };

        public static DoubleExcelCellDatum Double(double value)
            => new DoubleExcelCellDatum
            {
                Value = value,
            };
    }
}
