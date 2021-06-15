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
        TOutput Visit(ExcelBoldModel excelBoldModel, THelper helper);
        TOutput Visit(ExcelTextModel excelTextModel, THelper helper);
        TOutput Visit(ExcelIntegerValueModel model, THelper helper);
        TOutput Visit(ExcelNothingModel nothing, THelper helper);
        TOutput Visit(StackedExcelModel model, THelper helper);
    }
}
