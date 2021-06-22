using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BridgeCareCore.Services.SummaryReport.Visitors;
using OfficeOpenXml.Style;

namespace BridgeCareCore.Services.SummaryReport.Models
{
    public class ExcelVerticalAlignmentModel : IExcelModel
    {
        public ExcelVerticalAlignment Alignment { get; set; }

        public T Accept<THelper, T>(IExcelModelVisitor<THelper, T> visitor, THelper helper) =>
            visitor.Visit(this, helper);
    }
}
