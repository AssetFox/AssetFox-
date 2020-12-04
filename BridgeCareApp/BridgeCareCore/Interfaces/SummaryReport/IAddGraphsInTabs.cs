using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BridgeCareCore.Models.SummaryReport;
using OfficeOpenXml;

namespace BridgeCareCore.Interfaces.SummaryReport
{
    public interface IAddGraphsInTabs
    {
        public void Add(ExcelPackage excelPackage, ExcelWorksheet worksheet, ExcelWorksheet bridgeWorkSummaryWorksheet, ChartRowsModel chartRowModel,
            int simulationYearsCount);
    }
}
