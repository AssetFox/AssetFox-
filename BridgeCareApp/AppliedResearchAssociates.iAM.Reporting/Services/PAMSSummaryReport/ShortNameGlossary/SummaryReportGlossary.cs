using AppliedResearchAssociates.iAM.ExcelHelpers;
using OfficeOpenXml;

namespace AppliedResearchAssociates.iAM.Reporting.Services.PAMSSummaryReport.ShortNameGlossary
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
