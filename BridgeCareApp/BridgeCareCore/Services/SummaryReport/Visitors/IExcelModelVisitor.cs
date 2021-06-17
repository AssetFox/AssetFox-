using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BridgeCareCore.Services.SummaryReport.Models;

namespace BridgeCareCore.Services.SummaryReport.Visitors
{
    public interface IExcelModelVisitor<THelper, TOutput>
    {
        TOutput Visit(ExcelStringValueModel model, THelper helper);
        TOutput Visit(ExcelMoneyValueModel model, THelper helper);
        TOutput Visit(ExcelFormulaModel model, THelper helper);
        TOutput Visit(ExcelBoldModel excelBoldModel, THelper helper);
        TOutput Visit(ExcelIntegerValueModel model, THelper helper);
        TOutput Visit(ExcelNothingModel nothing, THelper helper);
        TOutput Visit(StackedExcelModel model, THelper helper);
        TOutput Visit(ExcelBorderModel model, THelper helper);
    }
}
