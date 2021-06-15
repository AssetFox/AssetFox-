using BridgeCareCore.Services.SummaryReport.Visitors;

namespace BridgeCareCore.Services.SummaryReport.Models
{
    public class ExcelIntegerValueModel: IExcelModel
    {
        public int Value { get; set; }

        public T Accept<THelper, T>(IExcelModelVisitor<THelper, T> visitor, THelper helper) =>
            visitor.Visit(this, helper);
    }
}
