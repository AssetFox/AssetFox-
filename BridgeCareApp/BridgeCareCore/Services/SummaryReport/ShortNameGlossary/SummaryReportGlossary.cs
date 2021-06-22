using System.Drawing;
using BridgeCareCore.Interfaces.SummaryReport;
using BridgeCareCore.Services.SummaryReport.BridgeData;
using BridgeCareCore.Services.SummaryReport.Models;
using BridgeCareCore.Services.SummaryReport.Visitors;
using OfficeOpenXml;

namespace BridgeCareCore.Services.SummaryReport.ShortNameGlossary
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
