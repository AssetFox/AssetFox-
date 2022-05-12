using System;

using AppliedResearchAssociates.iAM.Reporting.Interfaces.BAMSSummaryReport.Visitors;
using AppliedResearchAssociates.iAM.Reporting.Interfaces.BAMSSummaryReport.Worksheets;

namespace AppliedResearchAssociates.iAM.Reporting.Models.BAMSSummaryReport.Worksheets
{
    public class SpecificColumnWidthChangeExcelWorksheetModel: IExcelWorksheetContentModel
    {
        /// <summary>one-based</summary>
        public int ColumnNumber { get; set; }
        public Func<double, double> WidthChange { get; set; }

        public TOutput Accept<TOutput, THelper>(IExcelWorksheetModelVisitor<THelper, TOutput> visitor, THelper helper)
            => visitor.Visit(this, helper);
    }
}
