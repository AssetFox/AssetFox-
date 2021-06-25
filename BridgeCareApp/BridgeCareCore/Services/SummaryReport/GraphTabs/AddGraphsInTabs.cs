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
        private readonly ConditionPercentageChart _conditionPercentageChart;
        private readonly PoorBridgeCount _poorBridgeCount;
        private readonly PoorBridgeDeckArea _poorBridgeDeckArea;
        private readonly PoorBridgeDeckAreaByBPN _poorBridgeDeckAreaByBPN;
        private readonly GraphData _graphData;

        private readonly PostedBPNCount _postedBPNCount;

        public AddGraphsInTabs(GraphData graphData,
            ConditionPercentageChart conditionPercentageChart,
            PoorBridgeCount poorBridgeCount,
            PoorBridgeDeckArea poorBridgeDeckArea,
            PoorBridgeDeckAreaByBPN poorBridgeDeckAreaByBPN,

            PostedBPNCount postedBPNCount)
        {
            _graphData = graphData ?? throw new ArgumentNullException(nameof(graphData));
            _conditionPercentageChart = conditionPercentageChart ?? throw new ArgumentNullException(nameof(conditionPercentageChart));
            _poorBridgeCount = poorBridgeCount ?? throw new ArgumentNullException(nameof(poorBridgeCount));
            _poorBridgeDeckArea = poorBridgeDeckArea ?? throw new ArgumentNullException(nameof(poorBridgeDeckArea));
            _poorBridgeDeckAreaByBPN = poorBridgeDeckAreaByBPN ?? throw new ArgumentNullException(nameof(poorBridgeDeckAreaByBPN));

            _postedBPNCount = postedBPNCount ?? throw new ArgumentException(nameof(postedBPNCount));
        }


        private class GraphDataTab
        {
            public GraphDataTab(ExcelWorksheet workSheet, string title)
            {
                Worksheet = workSheet;
                Title = title;
            }
            public string Title { get; }
            public ExcelWorksheet Worksheet { get; }
            public int DataColumn { get; set; } = 0;
        }


        public void Add(ExcelPackage excelPackage, ExcelWorksheet worksheet, ExcelWorksheet bridgeWorkSummaryWorksheet,
            ChartRowsModel chartRowModel, int simulationYearsCount)
        {
            var GraphDataDependentTabs = new Dictionary<string, GraphDataTab>();
            void AddGraphDataDependentTab(string tabTitle, string graphTitle) => GraphDataDependentTabs.Add(tabTitle, new GraphDataTab(excelPackage.Workbook.Worksheets.Add(tabTitle), graphTitle));

            // "Graph Data" tab is required for these, but we don't want "Graph Data" showing up until the end since it isn't really a report.
            // Create the tabs and cache until "Graph Data" tab is available.
            AddGraphDataDependentTab(Properties.Resources.Graph_NHSConditionByBridgeCount_Tab, Properties.Resources.Graph_NHSConditionByBridgeCount_Title);
            AddGraphDataDependentTab(Properties.Resources.Graph_NHSConditionByDeckArea_Tab, Properties.Resources.Graph_NHSConditionByDeckArea_Title);
            AddGraphDataDependentTab(Properties.Resources.Graph_NonNHSConditionByBridgeCount_Tab, Properties.Resources.Graph_NonNHSConditionByBridgeCount_Title);
            AddGraphDataDependentTab(Properties.Resources.Graph_NonNHSConditionByDeckArea_Tab, Properties.Resources.Graph_NonNHSConditionByDeckArea_Title);
            AddGraphDataDependentTab(Properties.Resources.Graph_CombineNHSNonNHSConditionByBridgeCount_Tab, Properties.Resources.Graph_CombineNHSNonNHSConditionByBridgeCount_Title);
            AddGraphDataDependentTab(Properties.Resources.Graph_CombineNHSNonNHSConditionByDeckArea_Tab, Properties.Resources.Graph_CombineNHSNonNHSConditionByDeckArea_Title);


            // Create the tabs that are NOT dependent on "Graph Data" tab (these will precede the "Graph Data" tab)

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
            var graphDataWorksheet = excelPackage.Workbook.Worksheets.Add("Graph Data");
            int startColumn = 1;
            GraphDataDependentTabs[Properties.Resources.Graph_NHSConditionByBridgeCount_Tab].DataColumn = startColumn;
            startColumn = _graphData.Fill(graphDataWorksheet, bridgeWorkSummaryWorksheet, chartRowModel.NHSBridgeCountPercentSectionYearsRow, startColumn, simulationYearsCount);
            GraphDataDependentTabs[Properties.Resources.Graph_NHSConditionByDeckArea_Tab].DataColumn = startColumn;
            startColumn = _graphData.Fill(graphDataWorksheet, bridgeWorkSummaryWorksheet, chartRowModel.NHSBridgeDeckAreaPercentSectionYearsRow, startColumn, simulationYearsCount);
            GraphDataDependentTabs[Properties.Resources.Graph_NonNHSConditionByBridgeCount_Tab].DataColumn = startColumn;
            startColumn = _graphData.Fill(graphDataWorksheet, bridgeWorkSummaryWorksheet, chartRowModel.NonNHSBridgeCountPercentSectionYearsRow, startColumn, simulationYearsCount);
            GraphDataDependentTabs[Properties.Resources.Graph_NonNHSConditionByDeckArea_Tab].DataColumn = startColumn;
            startColumn = _graphData.Fill(graphDataWorksheet, bridgeWorkSummaryWorksheet, chartRowModel.NonNHSDeckAreaPercentSectionYearsRow, startColumn, simulationYearsCount);
            GraphDataDependentTabs[Properties.Resources.Graph_CombineNHSNonNHSConditionByBridgeCount_Tab].DataColumn = startColumn;
            startColumn = _graphData.Fill(graphDataWorksheet, bridgeWorkSummaryWorksheet, chartRowModel.TotalBridgeCountPercentYearsRow, startColumn, simulationYearsCount);
            GraphDataDependentTabs[Properties.Resources.Graph_CombineNHSNonNHSConditionByDeckArea_Tab].DataColumn = startColumn;
            _graphData.Fill(graphDataWorksheet, bridgeWorkSummaryWorksheet, chartRowModel.TotalDeckAreaPercentYearsRow, startColumn, simulationYearsCount);


            // Fill the already created tabs that are dependent on the "Graph Data" tab; order of fill does not matter, worksheets were added in order
            foreach (var graphDataTab in GraphDataDependentTabs.Values)
            {
                _conditionPercentageChart.Fill(graphDataTab.Worksheet, graphDataWorksheet, graphDataTab.DataColumn, graphDataTab.Title, simulationYearsCount);
            }
        }
    }
}
