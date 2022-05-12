using AppliedResearchAssociates.iAM.Reporting.Interfaces.BAMSSummaryReport;
using AppliedResearchAssociates.iAM.Reporting.Interfaces.BAMSSummaryReport.Visitors;

namespace AppliedResearchAssociates.iAM.Reporting.Models.BAMSSummaryReport.ExcelStyles
{
    public class ExcelItalicModel: IExcelModel
    {
        public bool Italic { get; set; }

        public T Accept<THelper, T>(IExcelModelVisitor<THelper, T> visitor, THelper helper) =>
            visitor.Visit(this, helper);
    }
}
