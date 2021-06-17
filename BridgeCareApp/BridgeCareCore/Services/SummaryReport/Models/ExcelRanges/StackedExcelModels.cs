using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BridgeCareCore.Services.SummaryReport.Models
{
    public static class StackedExcelModels
    {
        public static StackedExcelModel Stacked(params IExcelModel[] stack)
            => new StackedExcelModel
            {
                Content = stack.ToList(),
            };

        public static StackedExcelModel Empty => Stacked();

        public static StackedExcelModel BoldText(string text)
            => Stacked(
                ExcelValueModels.String(text),
                ExcelValueModels.Bold);
    }
}
