using AppliedResearchAssociates.iAM.ExcelHelpers.Visitors;
using OfficeOpenXml;

namespace AppliedResearchAssociates.iAM.Reporting.Services.BAMSSummaryReport.ShortNameGlossary
{
    public class SummaryReportGlossary
    {
        public void Fill(ExcelWorksheet worksheet)
        {
            var regions = ShortNameGlossaryModels.Content;
            ExcelWorksheetWriter.VisitList(worksheet, regions);
        }
    }
}
