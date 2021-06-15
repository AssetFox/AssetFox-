using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OfficeOpenXml;

namespace BridgeCareCore.Services.SummaryReport.Models
{
    public class ExcelFormulaModel: IExcelModel
    {
        public Func<ExcelRange, string> Formula { get; set; }
        public T Accept<THelper, T>(IExcelModelVisitor<THelper, T> visitor, THelper helper) =>
            visitor.Visit(this, helper);
    }
}
