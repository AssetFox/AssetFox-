
using AppliedResearchAssociates.iAM.Reporting.Interfaces.BAMSSummaryReport.Visitors;
using AppliedResearchAssociates.iAM.Reporting.Interfaces.BAMSSummaryReport.Worksheets;

using AppliedResearchAssociates.iAM.Reporting.Models.BAMSSummaryReport.Regions;

namespace AppliedResearchAssociates.iAM.Reporting.Models.BAMSSummaryReport.Worksheets
{
    /// <summary>A region model, together with its location in the worksheet.</summary>
    public class AnchoredExcelRegionModel: IExcelWorksheetContentModel
    {
        public RowBasedExcelRegionModel Region { get; set; }
        public int StartRow { get; set; } = 1;
        public int StartColumn { get; set; } = 1;
        public TOutput Accept<TOutput, THelper>(IExcelWorksheetModelVisitor<THelper, TOutput> visitor, THelper helper)
            => visitor.Visit(this, helper);
    }
}
