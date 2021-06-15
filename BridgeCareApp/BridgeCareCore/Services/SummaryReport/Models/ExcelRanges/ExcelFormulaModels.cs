using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OfficeOpenXml;

namespace BridgeCareCore.Services.SummaryReport.Models
{
    public static class ExcelFormulaModels
    {
        public static ExcelFormulaModel Text(string text)
            => new ExcelFormulaModel
            {
                Formula = ExcelRangeFunctions.Constant(text),
            };
        internal static ExcelFormulaModel FromFunction(Func<ExcelRange, string> function)
            => new ExcelFormulaModel
            {
                Formula = function,
            };
    }
}
