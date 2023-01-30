using System;
using System.Collections.Generic;
using OfficeOpenXml;

using AppliedResearchAssociates.iAM.Reporting.Interfaces.BAMSSummaryReport;
using AppliedResearchAssociates.iAM.Reporting.Models.BAMSSummaryReport;
using AppliedResearchAssociates.iAM.Reporting.Services.BAMSSummaryReport.GraphTabs.NHSConditionCharts;
using AppliedResearchAssociates.iAM.Reporting.Services.BAMSSummaryReport.GraphTabs.BPN;

namespace AppliedResearchAssociates.iAM.Reporting.Services.BAMSSummaryReport.GraphTabs
{
    public class AddGraphsInTabs : IAddGraphsInTabs
    {
        private ConditionPercentageChart _conditionPercentageChart;
        private GraphData _graphData;

        private IAddBPNGraphTab _addBPNGraphTab;
        private IAddPoorCountGraphTab _addPoorCountGraphTab;
        private IAddPoorDeckAreaGraphTab _addPoorDeckAreaGraphTab;

        public AddGraphsInTabs()
        {
            _graphData = new GraphData();
            _conditionPercentageChart = new ConditionPercentageChart();

            _addBPNGraphTab = new AddBPNGraphTab();
            _addPoorCountGraphTab = new AddPoorCountGraphTab();
            _addPoorDeckAreaGraphTab = new AddPoorDeckAreaGraphTab();
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
            AddGraphDataDependentTab(AuditReportConstants.Graph_NHSConditionByBridgeCount_Tab, AuditReportConstants.Graph_NHSConditionByBridgeCount_Title);
            AddGraphDataDependentTab(AuditReportConstants.Graph_NHSConditionByDeckArea_Tab, AuditReportConstants.Graph_NHSConditionByDeckArea_Title);
            AddGraphDataDependentTab(AuditReportConstants.Graph_NonNHSConditionByBridgeCount_Tab, AuditReportConstants.Graph_NonNHSConditionByBridgeCount_Title);
            AddGraphDataDependentTab(AuditReportConstants.Graph_NonNHSConditionByDeckArea_Tab, AuditReportConstants.Graph_NonNHSConditionByDeckArea_Title);
            AddGraphDataDependentTab(AuditReportConstants.Graph_CombineNHSNonNHSConditionByBridgeCount_Tab, AuditReportConstants.Graph_CombineNHSNonNHSConditionByBridgeCount_Title);
            AddGraphDataDependentTab(AuditReportConstants.Graph_CombineNHSNonNHSConditionByDeckArea_Tab, AuditReportConstants.Graph_CombineNHSNonNHSConditionByDeckArea_Title);

            // Create the "Graph Data" tab
            var graphDataWorksheet = excelPackage.Workbook.Worksheets.Add("Graph Data");
            int startColumn = 1;
            GraphDataDependentTabs[AuditReportConstants.Graph_NHSConditionByBridgeCount_Tab].DataColumn = startColumn;
            startColumn = _graphData.Fill(graphDataWorksheet, bridgeWorkSummaryWorksheet, chartRowModel.NHSBridgeCountPercentSectionYearsRow, startColumn, simulationYearsCount);
            GraphDataDependentTabs[AuditReportConstants.Graph_NHSConditionByDeckArea_Tab].DataColumn = startColumn;
            startColumn = _graphData.Fill(graphDataWorksheet, bridgeWorkSummaryWorksheet, chartRowModel.NHSBridgeDeckAreaPercentSectionYearsRow, startColumn, simulationYearsCount);
            GraphDataDependentTabs[AuditReportConstants.Graph_NonNHSConditionByBridgeCount_Tab].DataColumn = startColumn;
            startColumn = _graphData.Fill(graphDataWorksheet, bridgeWorkSummaryWorksheet, chartRowModel.NonNHSBridgeCountPercentSectionYearsRow, startColumn, simulationYearsCount);
            GraphDataDependentTabs[AuditReportConstants.Graph_NonNHSConditionByDeckArea_Tab].DataColumn = startColumn;
            startColumn = _graphData.Fill(graphDataWorksheet, bridgeWorkSummaryWorksheet, chartRowModel.NonNHSDeckAreaPercentSectionYearsRow, startColumn, simulationYearsCount);
            GraphDataDependentTabs[AuditReportConstants.Graph_CombineNHSNonNHSConditionByBridgeCount_Tab].DataColumn = startColumn;
            startColumn = _graphData.Fill(graphDataWorksheet, bridgeWorkSummaryWorksheet, chartRowModel.TotalBridgeCountPercentYearsRow, startColumn, simulationYearsCount);
            GraphDataDependentTabs[AuditReportConstants.Graph_CombineNHSNonNHSConditionByDeckArea_Tab].DataColumn = startColumn;
            _graphData.Fill(graphDataWorksheet, bridgeWorkSummaryWorksheet, chartRowModel.TotalDeckAreaPercentYearsRow, startColumn, simulationYearsCount);
            graphDataWorksheet.Hidden = eWorkSheetHidden.Hidden;

            // Fill the already created tabs that are dependent on the "Graph Data" tab; order of fill does not matter, worksheets were added in order
            foreach (var graphDataTab in GraphDataDependentTabs.Values)
            {
                _conditionPercentageChart.Fill(graphDataTab.Worksheet, graphDataWorksheet, graphDataTab.DataColumn, graphDataTab.Title, simulationYearsCount);
            }
                       
            var poorCountGraphTab = excelPackage.Workbook.Worksheets.Add(AuditReportConstants.Graph_PoorCountGraph_Tab);
            _addPoorCountGraphTab.AddPoorCountTab(poorCountGraphTab, bridgeWorkSummaryWorksheet, chartRowModel.TotalPoorBridgesCountSectionYearsRow, simulationYearsCount);

            var poorDeckAreaGraphTab = excelPackage.Workbook.Worksheets.Add(AuditReportConstants.Graph_PoorDeckAreaGraph_Tab);
            _addPoorDeckAreaGraphTab.AddPoorDeckAreaTab(poorDeckAreaGraphTab, bridgeWorkSummaryWorksheet, chartRowModel.TotalPoorBridgesDeckAreaSectionYearsRow, simulationYearsCount);

           _addBPNGraphTab.AddBPNTab(excelPackage, worksheet, bridgeWorkSummaryWorksheet, chartRowModel, simulationYearsCount);
        }
    }
}
