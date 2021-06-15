using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BridgeCareCore.Services.SummaryReport.Models
{
    public static class ExcelRowModels
    {
        public static ExcelRowModel WithEntries(params IExcelModel[] entries)
            => new ExcelRowModel
            {
                Values = entries.Select(RelativeExcelRangeModels.OneByOne).ToList(),
            };

        public static ExcelRowModel Empty
            => WithEntries();

        public static ExcelRowModel WithCells(List<RelativeExcelRangeModel> cells)
            => new ExcelRowModel
            {
                Values = cells,
            };

        public static ExcelRowModel IndentedHeader(int indentColumns, string headerText, int headerWidth, int headerHeight)
        {
            var values = new List<RelativeExcelRangeModel>();
            for (int i=0; i<indentColumns; i++)
            {
                values.Add(RelativeExcelRangeModels.Empty());
            }
            values.Add(RelativeExcelRangeModels.Text(headerText, headerWidth, headerHeight));
            return WithCells(values);
        }
    }
}
