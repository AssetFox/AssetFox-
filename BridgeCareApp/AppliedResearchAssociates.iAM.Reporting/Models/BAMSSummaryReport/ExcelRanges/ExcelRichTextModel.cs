using AppliedResearchAssociates.iAM.Reporting.Interfaces.BAMSSummaryReport;
using AppliedResearchAssociates.iAM.Reporting.Interfaces.BAMSSummaryReport.Visitors;

namespace AppliedResearchAssociates.iAM.Reporting.Models.BAMSSummaryReport.ExcelRanges
{
    public class ExcelRichTextModel: IExcelModel
    {
        public string Text { get; set; }
        public bool Bold { get; set; }
        public T Accept<THelper, T>(IExcelModelVisitor<THelper, T> visitor, THelper helper) =>
            visitor.Visit(this, helper);
    }
}
