﻿
using BridgeCareCore.Helpers.Excel.Visitors;
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
