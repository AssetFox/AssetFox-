using AppliedResearchAssociates.iAM.Reporting.Interfaces.BAMSSummaryReport;
using AppliedResearchAssociates.iAM.Reporting.Interfaces.BAMSSummaryReport.Visitors;
using OfficeOpenXml.Style;

namespace AppliedResearchAssociates.iAM.Reporting.Models.BAMSSummaryReport.ExcelStyles
{
    public class ExcelBorderModel: IExcelModel
    {
        public ExcelBorderStyle BorderStyle { get; set; }
        public T Accept<THelper, T>(IExcelModelVisitor<THelper, T> visitor, THelper helper) =>
            visitor.Visit(this, helper);
    }
}
