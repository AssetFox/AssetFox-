using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BridgeCareCore.Services.SummaryReport.Models
{
    public static class ExcelRowModelExtensions
    {
        public static void AddCell(this ExcelRowModel row, IExcelModel cellContent)
        {
            row.Values.Add(RelativeExcelRangeModels.OneByOne(cellContent));
        }
    }
}
