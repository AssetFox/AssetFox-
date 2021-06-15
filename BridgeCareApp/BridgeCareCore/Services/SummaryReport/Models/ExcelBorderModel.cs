using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OfficeOpenXml.Style;

namespace BridgeCareCore.Services.SummaryReport.Models
{
    public class ExcelBorderModel: IExcelModel
    {
        public ExcelBorderStyle BorderStyle { get; set; }
        public T Accept<THelper, T>(IExcelModelVisitor<THelper, T> visitor, THelper helper) =>
            visitor.Visit(this, helper);
    }
}
