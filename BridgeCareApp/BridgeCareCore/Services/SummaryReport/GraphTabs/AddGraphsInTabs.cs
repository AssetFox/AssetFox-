using System;
using BridgeCareCore.Interfaces.SummaryReport;
using BridgeCareCore.Models.SummaryReport;
using BridgeCareCore.Services.SummaryReport.GraphTabs.NHSConditionCharts;
using OfficeOpenXml;

namespace BridgeCareCore.Services.SummaryReport.GraphTabs
{
    public class AddGraphsInTabs : IAddGraphsInTabs
    {
        private readonly NHSConditionChart _nhsConditionChart;
        private readonly NonNHSConditionBridgeCount _nonNHSconditionBridgeCount;
        private readonly NonNHSConditionDeckArea _nonNHSConditionDeckArea;
        private readonly ConditionBridgeCount _conditionBridgeCount;
        private readonly ConditionDeckArea _conditionDeckArea;
        private readonly PoorBridgeCount _poorBridgeCount;
        private readonly PoorBridgeDeckArea _poorBridgeDeckArea;

        private readonly IAddBPNGraphTab _addBPNGraphTab;

        public AddGraphsInTabs(NHSConditionChart nhsConditionChart,
            NonNHSConditionBridgeCount nonNHSconditionBridgeCount,
            NonNHSConditionDeckArea nonNHSConditionDeckArea, ConditionBridgeCount conditionBridgeCount,
            ConditionDeckArea conditionDeckArea, PoorBridgeCount poorBridgeCount, PoorBridgeDeckArea poorBridgeDeckArea,
             IAddBPNGraphTab addBPNGraphTab)
        {
            _nhsConditionChart = nhsConditionChart ?? throw new ArgumentNullException(nameof(nhsConditionChart));
            _nonNHSconditionBridgeCount = nonNHSconditionBridgeCount ?? throw new ArgumentNullException(nameof(nonNHSconditionBridgeCount));
            _nonNHSConditionDeckArea = nonNHSConditionDeckArea ?? throw new ArgumentNullException(nameof(nonNHSConditionDeckArea));
            _conditionBridgeCount = conditionBridgeCount ?? throw new ArgumentNullException(nameof(conditionBridgeCount));
            _conditionDeckArea = conditionDeckArea ?? throw new ArgumentNullException(nameof(conditionDeckArea));
            _poorBridgeCount = poorBridgeCount ?? throw new ArgumentNullException(nameof(poorBridgeCount));
            _poorBridgeDeckArea = poorBridgeDeckArea ?? throw new ArgumentNullException(nameof(poorBridgeDeckArea));

            _addBPNGraphTab = addBPNGraphTab ?? throw new ArgumentNullException(nameof(addBPNGraphTab));
        }

        public void Add(ExcelPackage excelPackage, ExcelWorksheet worksheet, ExcelWorksheet bridgeWorkSummaryWorksheet,
            ChartRowsModel chartRowModel, int simulationYearsCount)
        {
            // NHS Condition Bridge Cnt tab
            worksheet = excelPackage.Workbook.Worksheets.Add("NHS Count");
            _nhsConditionChart.Fill(worksheet, bridgeWorkSummaryWorksheet, chartRowModel.NHSBridgeCountPercentSectionYearsRow, Properties.Resources.NHSConditionByBridgeCountLLCC, simulationYearsCount);

            // NHS Condition DA tab
            worksheet = excelPackage.Workbook.Worksheets.Add("NHS DA");
            _nhsConditionChart.Fill(worksheet, bridgeWorkSummaryWorksheet, chartRowModel.NHSBridgeDeckAreaPercentSectionYearsRow, Properties.Resources.NHSConditionByDeckAreaLLCC, simulationYearsCount);

            // Non-NHS Condition Bridge Count
            worksheet = excelPackage.Workbook.Worksheets.Add("Non NHS Count");
            _nonNHSconditionBridgeCount.Fill(worksheet, bridgeWorkSummaryWorksheet, chartRowModel.NonNHSBridgeCountPercentSectionYearsRow, simulationYearsCount);

            // Non-NHS Condition DA
            worksheet = excelPackage.Workbook.Worksheets.Add("Non NHS DA");
            _nonNHSConditionDeckArea.Fill(worksheet, bridgeWorkSummaryWorksheet, chartRowModel.NonNHSDeckAreaPercentSectionYearsRow, simulationYearsCount);

            // Condition Bridge Cnt tab
            worksheet = excelPackage.Workbook.Worksheets.Add("Combined Count");
            _conditionBridgeCount.Fill(worksheet, bridgeWorkSummaryWorksheet, chartRowModel.TotalBridgeCountPercentYearsRow, simulationYearsCount);

            // Condition DA tab
            worksheet = excelPackage.Workbook.Worksheets.Add("Combined DA");
            _conditionDeckArea.Fill(worksheet, bridgeWorkSummaryWorksheet, chartRowModel.TotalDeckAreaPercentYearsRow, simulationYearsCount);

            // Poor Bridge Cnt tab
            worksheet = excelPackage.Workbook.Worksheets.Add("Poor Count");
            _poorBridgeCount.Fill(worksheet, bridgeWorkSummaryWorksheet, chartRowModel.TotalPoorBridgesCountSectionYearsRow, simulationYearsCount);

            // Poor Bridge DA tab
            worksheet = excelPackage.Workbook.Worksheets.Add("Poor DA");
            _poorBridgeDeckArea.Fill(worksheet, bridgeWorkSummaryWorksheet, chartRowModel.TotalPoorBridgesDeckAreaSectionYearsRow, simulationYearsCount);

            _addBPNGraphTab.AddBPNTab(excelPackage, worksheet, bridgeWorkSummaryWorksheet, chartRowModel, simulationYearsCount);
        }
    }
}
