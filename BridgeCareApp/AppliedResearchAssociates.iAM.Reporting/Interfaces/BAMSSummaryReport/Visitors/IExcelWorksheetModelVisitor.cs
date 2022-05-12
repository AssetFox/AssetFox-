using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using AppliedResearchAssociates.iAM.Reporting.Models.BAMSSummaryReport.Worksheets;

namespace AppliedResearchAssociates.iAM.Reporting.Interfaces.BAMSSummaryReport.Visitors
{
    public interface IExcelWorksheetModelVisitor<THelper, TOutput>
    {
        TOutput Visit(AnchoredExcelRegionModel model, THelper helper);
        TOutput Visit(AutoFitColumnsExcelWorksheetContentModel model, THelper helper);
        TOutput Visit(SpecificColumnWidthChangeExcelWorksheetModel model, THelper helper);
    }
}
