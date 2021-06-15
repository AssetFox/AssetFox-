using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BridgeCareCore.Services.SummaryReport.Models
{
    public interface IExcelModelVisitor<THelper, TOutput>
    {
        TOutput Visit(ExcelStringValueModel model, THelper helper);
        TOutput Visit(ExcelMoneyValueModel model, THelper helper);
        TOutput Visit(ExcelFormulaModel model, THelper helper);
        TOutput Visit(ExcelIntegerValueModel model, THelper helper);
    }
}
