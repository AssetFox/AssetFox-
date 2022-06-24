using System;
using System.Collections.Generic;
using System.Text;

namespace AppliedResearchAssociates.iAM.Data.ExcelDatabaseStorage.WorksheetContentExtractors
{
    internal static class ExcelCellContentExtractor
    {
        public static IExcelCellDatum ExtractContent(object cellValue)
        {
            if (cellValue is double d)
            {
                return new DoubleExcelCellDatum
                {
                    Value = d
                };
            }
            if (cellValue is string str)
            {
                return new StringExcelCellDatum
                {
                    Value = str
                };
            }
            if (cellValue is DateTime dt)
            {
                return ExcelCellData.DateTime(dt);
            }
            return new StringExcelCellDatum
            {
                Value = ""
            };
        }
    }
}
