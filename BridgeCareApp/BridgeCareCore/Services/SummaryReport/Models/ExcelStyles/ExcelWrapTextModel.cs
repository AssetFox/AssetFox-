using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BridgeCareCore.Services.SummaryReport.Visitors;

namespace BridgeCareCore.Services.SummaryReport.Models
{
    public class ExcelWrapTextModel: IExcelModel
    {
        public bool Wrap { get; set; }

        public T Accept<THelper, T>(IExcelModelVisitor<THelper, T> visitor, THelper helper) =>
            visitor.Visit(this, helper);
    }
}
