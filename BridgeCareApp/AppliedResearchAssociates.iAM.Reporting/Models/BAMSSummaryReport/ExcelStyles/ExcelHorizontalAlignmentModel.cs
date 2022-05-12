using OfficeOpenXml.Style;

using AppliedResearchAssociates.iAM.Reporting.Interfaces.BAMSSummaryReport.Visitors;
using AppliedResearchAssociates.iAM.Reporting.Interfaces.BAMSSummaryReport;

namespace AppliedResearchAssociates.iAM.Reporting.Models.BAMSSummaryReport.ExcelStyles
{
    public class ExcelHorizontalAlignmentModel: IExcelModel
    {
        public ExcelHorizontalAlignment Alignment { get; set; }

        public T Accept<THelper, T>(IExcelModelVisitor<THelper, T> visitor, THelper helper) =>
            visitor.Visit(this, helper);
    }
}
