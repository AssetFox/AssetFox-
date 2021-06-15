using System.Collections.Generic;
using BridgeCareCore.Services.SummaryReport.Visitors;

namespace BridgeCareCore.Services.SummaryReport.Models
{
    public class StackedExcelModel: IExcelModel
    {
        public List<IExcelModel> Content { get; set; }

        public T Accept<THelper, T>(IExcelModelVisitor<THelper, T> visitor, THelper helper) =>
            visitor.Visit(this, helper);
    }
}
