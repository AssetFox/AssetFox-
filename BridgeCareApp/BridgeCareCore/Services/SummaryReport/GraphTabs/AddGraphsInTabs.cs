using System;
using BridgeCareCore.Interfaces.SummaryReport;
using BridgeCareCore.Models.SummaryReport;
using BridgeCareCore.Services.SummaryReport.GraphTabs.BPN;
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
        private readonly PoorBridgeDeckAreaByBPN _poorBridgeDeckAreaByBPN;

        private readonly PostedBPNCount _postedBPNCount;

        public AddGraphsInTabs(NHSConditionChart nhsConditionChart,
            NonNHSConditionBridgeCount nonNHSconditionBridgeCount,
            NonNHSConditionDeckArea nonNHSConditionDeckArea, ConditionBridgeCount conditionBridgeCount,
            ConditionDeckArea conditionDeckArea, PoorBridgeCount poorBridgeCount, PoorBridgeDeckArea poorBridgeDeckArea,
            PoorBridgeDeckAreaByBPN poorBridgeDeckAreaByBPN,

            PostedBPNCount postedBPNCount)
        {
            _nhsConditionChart = nhsConditionChart ?? throw new ArgumentNullException(nameof(nhsConditionChart));
            _nonNHSconditionBridgeCount = nonNHSconditionBridgeCount ?? throw new ArgumentNullException(nameof(nonNHSconditionBridgeCount));
            _nonNHSConditionDeckArea = nonNHSConditionDeckArea ?? throw new ArgumentNullException(nameof(nonNHSConditionDeckArea));
            _conditionBridgeCount = conditionBridgeCount ?? throw new ArgumentNullException(nameof(conditionBridgeCount));
            _conditionDeckArea = conditionDeckArea ?? throw new ArgumentNullException(nameof(conditionDeckArea));
            _poorBridgeCount = poorBridgeCount ?? throw new ArgumentNullException(nameof(poorBridgeCount));
            _poorBridgeDeckArea = poorBridgeDeckArea ?? throw new ArgumentNullException(nameof(poorBridgeDeckArea));
            _poorBridgeDeckAreaByBPN = poorBridgeDeckAreaByBPN ?? throw new ArgumentNullException(nameof(poorBridgeDeckAreaByBPN));

            _postedBPNCount = postedBPNCount ?? throw new ArgumentException(nameof(postedBPNCount));
        }

        public void Add(ExcelPackage excelPackage, ExcelWorksheet worksheet, ExcelWorksheet bridgeWorkSummaryWorksheet,
            ChartRowsModel chartRowModel, int simulationYearsCount)
        {
            // NHS Condition Bridge Cnt tab
            worksheet = excelPackage.Workbook.Worksheets.Add("NHS Condition Bridge Cnt");
            _nhsConditionChart.Fill(worksheet, bridgeWorkSummaryWorksheet, chartRowModel.NHSBridgeCountPercentSectionYearsRow, Properties.Resources.NHSConditionByBridgeCountLLCC, simulationYearsCount);

            // NHS Condition DA tab
            worksheet = excelPackage.Workbook.Worksheets.Add("NHS Condition DA");
            _nhsConditionChart.Fill(worksheet, bridgeWorkSummaryWorksheet, chartRowModel.NHSBridgeDeckAreaPercentSectionYearsRow, Properties.Resources.NHSConditionByDeckAreaLLCC, simulationYearsCount);

            // Non-NHS Condition Bridge Count
            worksheet = excelPackage.Workbook.Worksheets.Add("Non-NHS Condition Bridge Cnt");
            _nonNHSconditionBridgeCount.Fill(worksheet, bridgeWorkSummaryWorksheet, chartRowModel.NonNHSBridgeCountPercentSectionYearsRow, simulationYearsCount);

            // Non-NHS Condition DA
            worksheet = excelPackage.Workbook.Worksheets.Add("Non-NHS Condition DA");
            _nonNHSConditionDeckArea.Fill(worksheet, bridgeWorkSummaryWorksheet, chartRowModel.NonNHSDeckAreaPercentSectionYearsRow, simulationYearsCount);

            // Condition Bridge Cnt tab
            worksheet = excelPackage.Workbook.Worksheets.Add("Combined Condition Bridge Cnt");
            _conditionBridgeCount.Fill(worksheet, bridgeWorkSummaryWorksheet, chartRowModel.TotalBridgeCountPercentYearsRow, simulationYearsCount);

            // Condition DA tab
            worksheet = excelPackage.Workbook.Worksheets.Add("Combined Condition DA");
            _conditionDeckArea.Fill(worksheet, bridgeWorkSummaryWorksheet, chartRowModel.TotalDeckAreaPercentYearsRow, simulationYearsCount);

            // Poor Bridge Cnt tab
            worksheet = excelPackage.Workbook.Worksheets.Add("Poor Bridge Cnt");
            _poorBridgeCount.Fill(worksheet, bridgeWorkSummaryWorksheet, chartRowModel.TotalPoorBridgesCountSectionYearsRow, simulationYearsCount);

            // Poor Bridge DA tab
            worksheet = excelPackage.Workbook.Worksheets.Add("Poor Bridge DA");
            _poorBridgeDeckArea.Fill(worksheet, bridgeWorkSummaryWorksheet, chartRowModel.TotalPoorBridgesDeckAreaSectionYearsRow, simulationYearsCount);

            // Poor Bridge DA By BPN TAB
            worksheet = excelPackage.Workbook.Worksheets.Add("Poor Bridge DA By BPN");
            _poorBridgeDeckAreaByBPN.Fill(worksheet, bridgeWorkSummaryWorksheet, chartRowModel.TotalPoorDeckAreaByBPNSectionYearsRow, simulationYearsCount);

            // Posted BPN count TAB
            worksheet = excelPackage.Workbook.Worksheets.Add("Posted BPN Count");
            _postedBPNCount.Fill(worksheet, bridgeWorkSummaryWorksheet, chartRowModel.TotalBridgePostedCountByBPNYearsRow, simulationYearsCount);
        }
    }
}
