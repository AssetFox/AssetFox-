using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BridgeCareCore.Services.SummaryReport.Models;
using OfficeOpenXml;

namespace BridgeCareCore.Services.SummaryReport
{
    public class ExcelWriter : IExcelModelVisitor<ExcelRange, Unit>
    {
        public Unit Visit(ExcelStringValueModel model, ExcelRange cells)
        {
            cells.Value = model.Value;
            return Unit.Default;
        }
        public Unit Visit(ExcelMoneyValueModel model, ExcelRange cells)
        {
            cells.Value = model.Value;
            return Unit.Default;
        }
        public Unit Visit(ExcelFormulaModel model, ExcelRange cells)
        {
            var formula = model.Formula(cells);
            cells.Formula = formula;
            return Unit.Default;
        }
        public Unit Visit(ExcelIntegerValueModel model, ExcelRange cells)
        {
            cells.Value = model.Value;
            return Unit.Default;
        }
    }
}
