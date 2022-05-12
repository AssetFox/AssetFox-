using AppliedResearchAssociates.iAM.Reporting.Interfaces.BAMSSummaryReport;
using AppliedResearchAssociates.iAM.Reporting.Interfaces.BAMSSummaryReport.Visitors;

namespace AppliedResearchAssociates.iAM.Reporting.Models.BAMSSummaryReport.ExcelRanges
{
    public class ExcelDecimalValueModel: IExcelModel
    {
        public decimal Value { get; set; }
        public T Accept<THelper, T>(IExcelModelVisitor<THelper, T> visitor, THelper helper) =>
            visitor.Visit(this, helper);
    }
}
