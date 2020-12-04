using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BridgeCareCore.Interfaces.SummaryReport;
using BridgeCareCore.Models.SummaryReport;
using BridgeCareCore.Services.SummaryReport.GraphTabs.NHSConditionCharts;
using OfficeOpenXml;

namespace BridgeCareCore.Services.SummaryReport.GraphTabs
{
    public class AddGraphsInTabs : IAddGraphsInTabs
    {
        private readonly NHSConditionChart _nhsConditionChart;
        public void Add(ExcelPackage excelPackage, ExcelWorksheet worksheet, ExcelWorksheet bridgeWorkSummaryWorksheet,
            ChartRowsModel chartRowModel, int simulationYearsCount)
        {
            // NHS Condition Bridge Cnt tab
            worksheet = excelPackage.Workbook.Worksheets.Add("NHS Condition Bridge Cnt");
            _nhsConditionChart.Fill(worksheet, bridgeWorkSummaryWorksheet, chartRowModel.NHSBridgeCountPercentSectionYearsRow, Properties.Resources.NHSConditionByBridgeCountLLCC, simulationYearsCount);

        }
    }
}
