using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.Reporting;
using AppliedResearchAssociates.iAM.Reporting.Models.PAMSSummaryReport;
using OfficeOpenXml;

namespace AppliedResearchAssociates.iAM.Reporting.Interfaces.PAMSSummaryReport
{
    public interface IAddBPNGraphTab
    {
        public void AddBPNTab(ExcelPackage excelPackage, ExcelWorksheet worksheet, ExcelWorksheet pamsWorkSummaryWorksheet, ChartRowsModel chartRowModel, int simulationYearsCount);
    }
}
