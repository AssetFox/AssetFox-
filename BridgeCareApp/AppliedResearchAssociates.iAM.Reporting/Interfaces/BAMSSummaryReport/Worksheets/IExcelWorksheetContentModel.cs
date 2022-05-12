using AppliedResearchAssociates.iAM.Reporting.Interfaces.BAMSSummaryReport.Visitors;

namespace AppliedResearchAssociates.iAM.Reporting.Interfaces.BAMSSummaryReport.Worksheets
{
    public interface IExcelWorksheetContentModel
    {
        public TOutput Accept<TOutput, THelper>(IExcelWorksheetModelVisitor<THelper, TOutput> visitor, THelper helper);
    }
}
