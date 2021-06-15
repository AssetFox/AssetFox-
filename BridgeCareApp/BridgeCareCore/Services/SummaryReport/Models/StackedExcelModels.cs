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

        public static StackedExcelModel BoldText(string text)
            => Stacked(
                ExcelTextModels.Text(text),
                ExcelBoldModels.Bold);
    }
}
