using System;
using System.Collections.Generic;
using System.Text;
using AppliedResearchAssociates.iAM.Data.ExcelDatabaseStorage.CellData;

namespace AppliedResearchAssociates.iAM.Data.ExcelDatabaseStorage
{
    public static class ExcelCellData
    {
        private static EmptyExcelCellDatum _Empty { get; } = new EmptyExcelCellDatum();
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

        public static EmptyExcelCellDatum Empty
            => _Empty;
    }
}
