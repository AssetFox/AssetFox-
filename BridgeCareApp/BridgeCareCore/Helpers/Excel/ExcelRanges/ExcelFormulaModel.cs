using System;
using BridgeCareCore.Helpers.Excel.Visitors;
using OfficeOpenXml;

namespace BridgeCareCore.Helpers.Excel
{
    public class ExcelFormulaModel: IExcelModel
    {
        public Func<ExcelRange, string> Formula { get; set; }
        public T Accept<THelper, T>(IExcelModelVisitor<THelper, T> visitor, THelper helper) =>
            visitor.Visit(this, helper);
    }
}
