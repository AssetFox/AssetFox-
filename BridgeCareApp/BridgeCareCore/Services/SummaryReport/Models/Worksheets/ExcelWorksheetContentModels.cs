using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BridgeCareCore.Services.SummaryReport.Models.Worksheets
{
    public static class ExcelWorksheetContentModels
    {
        public static AutoFitColumnsExcelWorksheetContentModel AutoFitColumns(double minWidth)
            => new AutoFitColumnsExcelWorksheetContentModel
            {
                MinWidth = minWidth,
            };
    }
}
