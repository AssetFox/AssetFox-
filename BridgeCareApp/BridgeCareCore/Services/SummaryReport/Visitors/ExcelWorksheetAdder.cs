using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BridgeCareCore.Services.SummaryReport.Models;
using OfficeOpenXml;

namespace BridgeCareCore.Services.SummaryReport.Visitors
{
    public static class ExcelWorksheetAdder
    {
        public static ExcelWorksheet AddWorksheet(ExcelWorkbook workbook, ExcelWorksheetModel model)
        {
            var writer = new ExcelWorksheetWriter();
            var r = workbook.Worksheets.Add(model.TabName);
            foreach (var content in model.Content)
            {
                content.Accept(writer, r);
            }
            return r;
        }
    }
}
