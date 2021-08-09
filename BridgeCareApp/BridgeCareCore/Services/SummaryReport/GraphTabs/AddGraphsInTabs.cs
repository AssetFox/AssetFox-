﻿using System;
using System.Collections.Generic;
using BridgeCareCore.Interfaces.SummaryReport;
using BridgeCareCore.Models.SummaryReport;
using BridgeCareCore.Services.SummaryReport.GraphTabs.NHSConditionCharts;
using OfficeOpenXml;

namespace BridgeCareCore.Services.SummaryReport.GraphTabs
{
    public class AddGraphsInTabs : IAddGraphsInTabs
    {
        private readonly ConditionPercentageChart _conditionPercentageChart;
        private readonly GraphData _graphData;

        private readonly IAddBPNGraphTab _addBPNGraphTab;
        private readonly IAddPoorCountGraphTab _addPoorCountGraphTab;
        private readonly IAddPoorDeckAreaGraphTab _addPoorDeckAreaGraphTab;

        public AddGraphsInTabs(GraphData graphData,
            ConditionPercentageChart conditionPercentageChart,
            IAddBPNGraphTab addBPNGraphTab, IAddPoorCountGraphTab addPoorCountGraphTab, IAddPoorDeckAreaGraphTab addPoorDeckAreaGraphTab)
        {
            _graphData = graphData ?? throw new ArgumentNullException(nameof(graphData));
            _conditionPercentageChart = conditionPercentageChart ?? throw new ArgumentNullException(nameof(conditionPercentageChart));

            _addBPNGraphTab = addBPNGraphTab ?? throw new ArgumentNullException(nameof(addBPNGraphTab));
            _addPoorCountGraphTab = addPoorCountGraphTab ?? throw new ArgumentNullException(nameof(addPoorCountGraphTab));
            _addPoorDeckAreaGraphTab = addPoorDeckAreaGraphTab ?? throw new ArgumentNullException(nameof(addPoorDeckAreaGraphTab));
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
            graphDataWorksheet.Hidden = eWorkSheetHidden.Hidden;

            // Fill the already created tabs that are dependent on the "Graph Data" tab; order of fill does not matter, worksheets were added in order
            foreach (var graphDataTab in GraphDataDependentTabs.Values)
            {
                _conditionPercentageChart.Fill(graphDataTab.Worksheet, graphDataWorksheet, graphDataTab.DataColumn, graphDataTab.Title, simulationYearsCount);
            }
                       
            var poorCountGraphTab = excelPackage.Workbook.Worksheets.Add(Properties.Resources.Graph_PoorCountGraph_Tab);
            _addPoorCountGraphTab.AddPoorCountTab(poorCountGraphTab, bridgeWorkSummaryWorksheet, chartRowModel.TotalPoorBridgesCountSectionYearsRow, simulationYearsCount);

            var poorDeckAreaGraphTab = excelPackage.Workbook.Worksheets.Add(Properties.Resources.Graph_PoorDeckAreaGraph_Tab);
            _addPoorDeckAreaGraphTab.AddPoorDeckAreaTab(poorDeckAreaGraphTab, bridgeWorkSummaryWorksheet, chartRowModel.TotalPoorBridgesDeckAreaSectionYearsRow, simulationYearsCount);

            _addBPNGraphTab.AddBPNTab(excelPackage, worksheet, bridgeWorkSummaryWorksheet, chartRowModel, simulationYearsCount);
        }
    }
}
