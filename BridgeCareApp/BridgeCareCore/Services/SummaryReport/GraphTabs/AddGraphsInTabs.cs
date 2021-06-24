using System;
using System.Collections.Generic;
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
        private readonly GraphData _graphData;

        private readonly PostedBPNCount _postedBPNCount;

        public AddGraphsInTabs(GraphData graphData,
            NHSConditionChart nhsConditionChart,
            NonNHSConditionBridgeCount nonNHSconditionBridgeCount,
            NonNHSConditionDeckArea nonNHSConditionDeckArea,
            ConditionBridgeCount conditionBridgeCount,
            ConditionDeckArea conditionDeckArea,
            PoorBridgeCount poorBridgeCount,
            PoorBridgeDeckArea poorBridgeDeckArea,
            PoorBridgeDeckAreaByBPN poorBridgeDeckAreaByBPN,

            PostedBPNCount postedBPNCount)
        {
            _graphData = graphData ?? throw new ArgumentNullException(nameof(graphData));
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
            var GraphDataDependentTabs = new Dictionary<string, ExcelWorksheet>();
            void AddGraphDataDependentTab(string tabTitle) => GraphDataDependentTabs.Add(tabTitle, excelPackage.Workbook.Worksheets.Add(tabTitle));

            // "Graph Data" tab is required for these, but we don't want "Graph Data" showing up until the end since it isn't really a report.
            // Create the tabs and cache until "Graph Data" tab is available.
            AddGraphDataDependentTab("NHS Condition Bridge Cnt");
            AddGraphDataDependentTab("NHS Condition DA");
            AddGraphDataDependentTab("Non-NHS Condition Bridge Cnt");
            AddGraphDataDependentTab("Non-NHS Condition DA");
            AddGraphDataDependentTab("Combined Condition Bridge Cnt");
            AddGraphDataDependentTab("Combined Condition DA");

            // Create the tabs that are NOT dependent on "Graph Data" tab
            // (these will precede the "Graph Data" tab)

            // Poor Bridge Cnt tab
            worksheet = excelPackage.Workbook.Worksheets.Add("Poor Bridge Cnt");
            _poorBridgeCount.Fill(worksheet, bridgeWorkSummaryWorksheet, chartRowModel.TotalPoorBridgesCountSectionYearsRow, simulationYearsCount);

            // Poor Bridge DA tab
            worksheet = excelPackage.Workbook.Worksheets.Add("Poor Bridge DA");
            _poorBridgeDeckArea.Fill(worksheet, bridgeWorkSummaryWorksheet, chartRowModel.TotalPoorBridgesDeckAreaSectionYearsRow, simulationYearsCount);

            // Poor Bridge DA By BPN Tab
            worksheet = excelPackage.Workbook.Worksheets.Add("Poor Bridge DA By BPN");
            _poorBridgeDeckAreaByBPN.Fill(worksheet, bridgeWorkSummaryWorksheet, chartRowModel.TotalPoorDeckAreaByBPNSectionYearsRow, simulationYearsCount);

            // Posted BPN count TAB
            worksheet = excelPackage.Workbook.Worksheets.Add("Posted BPN Count");
            _postedBPNCount.Fill(worksheet, bridgeWorkSummaryWorksheet, chartRowModel.TotalBridgePostedCountByBPNYearsRow, simulationYearsCount);


            // Create the "Graph Data" tab
            worksheet = excelPackage.Workbook.Worksheets.Add("Graph Data");
            int startColumn = 1;
            startColumn = _graphData.Fill(worksheet, bridgeWorkSummaryWorksheet, chartRowModel.NHSBridgeCountPercentSectionYearsRow, startColumn, simulationYearsCount);
            startColumn = _graphData.Fill(worksheet, bridgeWorkSummaryWorksheet, chartRowModel.NHSBridgeDeckAreaPercentSectionYearsRow, startColumn, simulationYearsCount);
            startColumn = _graphData.Fill(worksheet, bridgeWorkSummaryWorksheet, chartRowModel.NonNHSBridgeCountPercentSectionYearsRow, startColumn, simulationYearsCount);
            startColumn = _graphData.Fill(worksheet, bridgeWorkSummaryWorksheet, chartRowModel.NonNHSDeckAreaPercentSectionYearsRow, startColumn, simulationYearsCount);
            startColumn = _graphData.Fill(worksheet, bridgeWorkSummaryWorksheet, chartRowModel.TotalBridgeCountPercentYearsRow, startColumn, simulationYearsCount);
            _graphData.Fill(worksheet, bridgeWorkSummaryWorksheet, chartRowModel.TotalDeckAreaPercentYearsRow, startColumn, simulationYearsCount);


            // Create the tabs that are dependent on the "Graph Data" tab

            // NHS Condition Bridge Cnt tab
            worksheet = GraphDataDependentTabs["NHS Condition Bridge Cnt"];
            _nhsConditionChart.Fill(worksheet, bridgeWorkSummaryWorksheet, chartRowModel.NHSBridgeCountPercentSectionYearsRow, Properties.Resources.NHSConditionByBridgeCountLLCC, simulationYearsCount);

            // NHS Condition DA tab
            worksheet = GraphDataDependentTabs["NHS Condition DA"];
            _nhsConditionChart.Fill(worksheet, bridgeWorkSummaryWorksheet, chartRowModel.NHSBridgeDeckAreaPercentSectionYearsRow, Properties.Resources.NHSConditionByDeckAreaLLCC, simulationYearsCount);

            // Non-NHS Condition Bridge Count
            worksheet = GraphDataDependentTabs["Non-NHS Condition Bridge Cnt"];
            _nonNHSconditionBridgeCount.Fill(worksheet, bridgeWorkSummaryWorksheet, chartRowModel.NonNHSBridgeCountPercentSectionYearsRow, simulationYearsCount);

            // Non-NHS Condition DA
            worksheet = GraphDataDependentTabs["Non-NHS Condition DA"];
            _nonNHSConditionDeckArea.Fill(worksheet, bridgeWorkSummaryWorksheet, chartRowModel.NonNHSDeckAreaPercentSectionYearsRow, simulationYearsCount);

            // Condition Bridge Cnt tab
            worksheet = GraphDataDependentTabs["Combined Condition Bridge Cnt"];
            _conditionBridgeCount.Fill(worksheet, bridgeWorkSummaryWorksheet, chartRowModel.TotalBridgeCountPercentYearsRow, simulationYearsCount);

            // Condition DA tab
            worksheet = GraphDataDependentTabs["Combined Condition DA"];
            _conditionDeckArea.Fill(worksheet, bridgeWorkSummaryWorksheet, chartRowModel.TotalDeckAreaPercentYearsRow, simulationYearsCount);
        }
    }
}
