using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BridgeCareCore.Services.SummaryReport.Models
{
    public static class ExcelRowModelExtensions
    {
        public static void AddCells(this ExcelRowModel row, params IExcelModel[] cellContent)
        {
            foreach (var model in cellContent)
            {
                row.Values.Add(RelativeExcelRangeModels.OneByOne(model));
            }
        }
    }
}
