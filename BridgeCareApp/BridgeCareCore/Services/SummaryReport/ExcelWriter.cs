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
        public Unit Visit(ExcelTextFormulaModel model, ExcelRange cells)
        {
            cells.Formula = model.Formula;
            return Unit.Default;
        }
        public Unit Visit(ExcelIntegerValueModel model, ExcelRange cells)
        {
            cells.Value = model.Value;
            return Unit.Default;
        }
        public Unit Visit(ExcelRowModel model, ExcelRange cells)
        {
            var targetCells = cells;
            foreach (var entry in model.Values)
            {
                entry.Accept(this, targetCells);
                targetCells = ExcelRanges.RightNeighbor(cells);
            }
        }
    }
}
