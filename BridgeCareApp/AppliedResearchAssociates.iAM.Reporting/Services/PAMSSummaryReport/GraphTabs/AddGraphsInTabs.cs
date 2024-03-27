using System;
using System.Collections.Generic;
using OfficeOpenXml;
using AppliedResearchAssociates.iAM.Reporting.Models.PAMSSummaryReport;
using AppliedResearchAssociates.iAM.Reporting.Services.PAMSSummaryReport.GraphTabs.Condition;

namespace AppliedResearchAssociates.iAM.Reporting.Services.PAMSSummaryReport.GraphTabs
{
    public class AddGraphsInTabs
    {
        private GraphData _graphData;
        private ConditionChart _conditionChart;

        public AddGraphsInTabs()
        {
            _graphData = new GraphData();
            _conditionChart = new ConditionChart();
        }


        public enum ChartType
        {
            CountChart,
            AreaChart,
            CashChart,
            CombinedChart
        }

        private class GraphDataTab
        {
            public GraphDataTab(ExcelWorksheet workSheet, string title)
            {
                Worksheet = workSheet;
                Title = title;
                YAxisTitle = PAMSConstants.Graph_Tabs_YAxisTitle;
                XAxisTitle = PAMSConstants.Graph_Tabs_XAxisTitle;
            }
            public string Title { get; }
            public ExcelWorksheet Worksheet { get; }
            public int DataColumn { get; set; } = 0;
            public ChartType type { get; set; }
            public string YAxisTitle { get; set; }
            public string XAxisTitle { get; set; }
        }


        public void Add(ExcelPackage excelPackage, ExcelWorksheet pamsWorkSummaryWorksheet, ChartRowsModel chartRowModel, int simulationYearsCount)
        {
            var graphDataDependentTabs = new Dictionary<string, GraphDataTab>();
            void AddGraphDataDependentTab(string tabTitle, string graphTitle) => graphDataDependentTabs.Add(tabTitle, new GraphDataTab(excelPackage.Workbook.Worksheets.Add(tabTitle), graphTitle));

            // Create the "Graph Data" tab
            int sourceStartRow = 0; int startColumn = 1; int columnsPerSet = 6;
            var graphDataWorksheet = excelPackage.Workbook.Worksheets.Add("Graph Data");

            //IRI Charts
            AddGraphDataDependentTab(PAMSConstants.IRI_BPN1_Tab, PAMSConstants.IRI_BPN1_Tab_Title);
            AddGraphDataDependentTab(PAMSConstants.IRI_BPN2_Tab, PAMSConstants.IRI_BPN2_Tab_Title);
            AddGraphDataDependentTab(PAMSConstants.IRI_BPN3_Tab, PAMSConstants.IRI_BPN3_Tab_Title);
            AddGraphDataDependentTab(PAMSConstants.IRI_BPN4_Tab, PAMSConstants.IRI_BPN4_Tab_Title);
            AddGraphDataDependentTab(PAMSConstants.IRI_Statewide_Tab, PAMSConstants.IRI_Statewide_Tab_Title);

            //OPI Charts
            AddGraphDataDependentTab(PAMSConstants.OPI_BPN1_Tab, PAMSConstants.OPI_BPN1_Tab_Title);
            AddGraphDataDependentTab(PAMSConstants.OPI_BPN2_Tab, PAMSConstants.OPI_BPN2_Tab_Title);
            AddGraphDataDependentTab(PAMSConstants.OPI_BPN3_Tab, PAMSConstants.OPI_BPN3_Tab_Title);
            AddGraphDataDependentTab(PAMSConstants.OPI_BPN4_Tab, PAMSConstants.OPI_BPN4_Tab_Title);
            AddGraphDataDependentTab(PAMSConstants.OPI_Statewide_Tab, PAMSConstants.OPI_Statewide_Tab_Title);

            //fill IRI chart data
            sourceStartRow = chartRowModel.IRI_BPN_1_ChartModel.sourceStartRow;
            startColumn = _graphData.Fill(graphDataWorksheet, pamsWorkSummaryWorksheet, sourceStartRow, startColumn, simulationYearsCount);
            graphDataDependentTabs[PAMSConstants.IRI_BPN1_Tab].DataColumn = startColumn - columnsPerSet;
            graphDataDependentTabs[PAMSConstants.IRI_BPN1_Tab].type = ChartType.CountChart;
            
            sourceStartRow = chartRowModel.IRI_BPN_2_ChartModel.sourceStartRow;
            startColumn = _graphData.Fill(graphDataWorksheet, pamsWorkSummaryWorksheet, sourceStartRow, startColumn, simulationYearsCount);
            graphDataDependentTabs[PAMSConstants.IRI_BPN2_Tab].DataColumn = startColumn - columnsPerSet;
            graphDataDependentTabs[PAMSConstants.IRI_BPN2_Tab].type = ChartType.CountChart;

            sourceStartRow = chartRowModel.IRI_BPN_3_ChartModel.sourceStartRow;
            startColumn = _graphData.Fill(graphDataWorksheet, pamsWorkSummaryWorksheet, sourceStartRow, startColumn, simulationYearsCount);
            graphDataDependentTabs[PAMSConstants.IRI_BPN3_Tab].DataColumn = startColumn - columnsPerSet;
            graphDataDependentTabs[PAMSConstants.IRI_BPN3_Tab].type = ChartType.CountChart;

            sourceStartRow = chartRowModel.IRI_BPN_4_ChartModel.sourceStartRow;
            startColumn = _graphData.Fill(graphDataWorksheet, pamsWorkSummaryWorksheet, sourceStartRow, startColumn, simulationYearsCount);
            graphDataDependentTabs[PAMSConstants.IRI_BPN4_Tab].DataColumn = startColumn - columnsPerSet;
            graphDataDependentTabs[PAMSConstants.IRI_BPN4_Tab].type = ChartType.CountChart;

            sourceStartRow = chartRowModel.IRI_StateWide_ChartModel.sourceStartRow;
            startColumn = _graphData.Fill(graphDataWorksheet, pamsWorkSummaryWorksheet, sourceStartRow, startColumn, simulationYearsCount);
            graphDataDependentTabs[PAMSConstants.IRI_Statewide_Tab].DataColumn = startColumn - columnsPerSet;
            graphDataDependentTabs[PAMSConstants.IRI_Statewide_Tab].type = ChartType.CountChart;

            //fill OPI chart data
            sourceStartRow = chartRowModel.OPI_BPN_1_ChartModel.sourceStartRow;
            startColumn = _graphData.Fill(graphDataWorksheet, pamsWorkSummaryWorksheet, sourceStartRow, startColumn, simulationYearsCount);
            graphDataDependentTabs[PAMSConstants.OPI_BPN1_Tab].DataColumn = startColumn - columnsPerSet;
            graphDataDependentTabs[PAMSConstants.OPI_BPN1_Tab].type = ChartType.CountChart;

            sourceStartRow = chartRowModel.OPI_BPN_2_ChartModel.sourceStartRow;
            startColumn = _graphData.Fill(graphDataWorksheet, pamsWorkSummaryWorksheet, sourceStartRow, startColumn, simulationYearsCount);
            graphDataDependentTabs[PAMSConstants.OPI_BPN2_Tab].DataColumn = startColumn - columnsPerSet;
            graphDataDependentTabs[PAMSConstants.OPI_BPN2_Tab].type = ChartType.CountChart;

            sourceStartRow = chartRowModel.OPI_BPN_3_ChartModel.sourceStartRow;
            startColumn = _graphData.Fill(graphDataWorksheet, pamsWorkSummaryWorksheet, sourceStartRow, startColumn, simulationYearsCount);
            graphDataDependentTabs[PAMSConstants.OPI_BPN3_Tab].DataColumn = startColumn - columnsPerSet;
            graphDataDependentTabs[PAMSConstants.OPI_BPN3_Tab].type = ChartType.CountChart;

            sourceStartRow = chartRowModel.OPI_BPN_4_ChartModel.sourceStartRow;
            startColumn = _graphData.Fill(graphDataWorksheet, pamsWorkSummaryWorksheet, sourceStartRow, startColumn, simulationYearsCount);
            graphDataDependentTabs[PAMSConstants.OPI_BPN4_Tab].DataColumn = startColumn - columnsPerSet;
            graphDataDependentTabs[PAMSConstants.OPI_BPN4_Tab].type = ChartType.CountChart;

            sourceStartRow = chartRowModel.OPI_StateWide_ChartModel.sourceStartRow;
            startColumn = _graphData.Fill(graphDataWorksheet, pamsWorkSummaryWorksheet, sourceStartRow, startColumn, simulationYearsCount);
            graphDataDependentTabs[PAMSConstants.OPI_Statewide_Tab].DataColumn = startColumn - columnsPerSet;
            graphDataDependentTabs[PAMSConstants.OPI_Statewide_Tab].type = ChartType.CountChart;

            //hide the graph data worksheet;
            graphDataWorksheet.Hidden = eWorkSheetHidden.Hidden;

            //Add graphs
            foreach (var graphDataTab in graphDataDependentTabs.Values)
            {
                switch (graphDataTab.type)
                {
                case ChartType.CountChart:
                    _conditionChart.Fill(graphDataTab.Worksheet, graphDataWorksheet, graphDataTab.DataColumn, simulationYearsCount, graphDataTab.Title, graphDataTab.YAxisTitle, graphDataTab.XAxisTitle);
                    break;

                default:
                    break;
                }
            }
        }
    }
}
