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

        public Unit Visit(ExcelNothingModel nothing, ExcelRange cells)
            => Unit.Default;

        public Unit Visit(ExcelTextModel text, ExcelRange cells)
        {
            cells.Value = text.Text;
            return Unit.Default;
        }

        public Unit Visit(ExcelBoldModel bold, ExcelRange cells)
        {
            cells.Style.Font.Bold = bold.Bold;
            return Unit.Default;
        }

        public Unit Visit(StackedExcelModel model, ExcelRange cells)
        {
            foreach (var child in model.Content)
            {
                child.Accept(this, cells);
            }
            return Unit.Default;
        }
    }
}
