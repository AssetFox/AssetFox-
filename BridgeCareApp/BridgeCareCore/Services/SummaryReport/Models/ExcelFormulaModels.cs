using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BridgeCareCore.Services.SummaryReport.Models
{
    public static class ExcelFormulaModels
    {
        public static ExcelTextFormulaModel Text(string text)
            => new ExcelTextFormulaModel
            {
                Formula = text,
            };
    }
}
