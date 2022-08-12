using System;
using System.Collections.Generic;
using OfficeOpenXml;

using AppliedResearchAssociates.iAM.Reporting.Interfaces.PAMSSummaryReport;
using AppliedResearchAssociates.iAM.Reporting.Models.PAMSSummaryReport;

using AppliedResearchAssociates.iAM.Reporting.Services.PAMSSummaryReport.GraphTabs.BPN;

namespace AppliedResearchAssociates.iAM.Reporting.Services.PAMSSummaryReport.GraphTabs
{
    public class AddGraphsInTabs : IAddGraphsInTabs
    {
        private GraphData _graphData;

        private IAddBPNGraphTab _addBPNGraphTab;
        private IAddPoorCountGraphTab _addPoorCountGraphTab;

        public AddGraphsInTabs()
        {
            _graphData = new GraphData();
            _addBPNGraphTab = new AddBPNGraphTab();
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


        public void Add(ExcelPackage excelPackage, ExcelWorksheet worksheet, ExcelWorksheet pamsWorkSummaryWorksheet, ChartRowsModel chartRowModel, int simulationYearsCount)
        {
            var graphDataDependentTabs = new Dictionary<string, GraphDataTab>();
            void AddGraphDataDependentTab(string tabTitle, string graphTitle) => graphDataDependentTabs.Add(tabTitle, new GraphDataTab(excelPackage.Workbook.Worksheets.Add(tabTitle), graphTitle));
                        
            // Create the tabs and cache until "Graph Data" tab is available.
            AddGraphDataDependentTab(PAMSConstants.IRI_BPN1_Tab, PAMSConstants.IRI_BPN1_Tab_Title);
            AddGraphDataDependentTab(PAMSConstants.IRI_BPN2_Tab, PAMSConstants.IRI_BPN2_Tab_Title);
            AddGraphDataDependentTab(PAMSConstants.IRI_BPN3_Tab, PAMSConstants.IRI_BPN3_Tab_Title);
            AddGraphDataDependentTab(PAMSConstants.IRI_BPN4_Tab, PAMSConstants.IRI_BPN4_Tab_Title);
            AddGraphDataDependentTab(PAMSConstants.IRI_Statewide_Tab, PAMSConstants.IRI_Statewide_Tab_Title);

            AddGraphDataDependentTab(PAMSConstants.OPI_BPN1_Tab, PAMSConstants.OPI_BPN1_Tab_Title);
            AddGraphDataDependentTab(PAMSConstants.OPI_BPN2_Tab, PAMSConstants.OPI_BPN2_Tab_Title);
            AddGraphDataDependentTab(PAMSConstants.OPI_BPN3_Tab, PAMSConstants.OPI_BPN3_Tab_Title);
            AddGraphDataDependentTab(PAMSConstants.OPI_BPN4_Tab, PAMSConstants.OPI_BPN4_Tab_Title);
            AddGraphDataDependentTab(PAMSConstants.OPI_Statewide_Tab, PAMSConstants.OPI_Statewide_Tab_Title);

            // Create the "Graph Data" tab
            var graphDataWorksheet = excelPackage.Workbook.Worksheets.Add("Graph Data");

            //IRI Charts
            int startColumn = 1; startColumn = _graphData.Fill(graphDataWorksheet, pamsWorkSummaryWorksheet, chartRowModel.IRI_BPN_1_ChartModel.sourceStartRow, startColumn, simulationYearsCount);
            graphDataDependentTabs[PAMSConstants.IRI_BPN1_Tab].DataColumn = startColumn;

            startColumn = _graphData.Fill(graphDataWorksheet, pamsWorkSummaryWorksheet, chartRowModel.IRI_BPN_2_ChartModel.sourceStartRow, startColumn, simulationYearsCount);
            graphDataDependentTabs[PAMSConstants.IRI_BPN2_Tab].DataColumn = startColumn;

            startColumn = _graphData.Fill(graphDataWorksheet, pamsWorkSummaryWorksheet, chartRowModel.IRI_BPN_3_ChartModel.sourceStartRow, startColumn, simulationYearsCount);
            graphDataDependentTabs[PAMSConstants.IRI_BPN3_Tab].DataColumn = startColumn;

            startColumn = _graphData.Fill(graphDataWorksheet, pamsWorkSummaryWorksheet, chartRowModel.IRI_BPN_4_ChartModel.sourceStartRow, startColumn, simulationYearsCount);
            graphDataDependentTabs[PAMSConstants.IRI_BPN4_Tab].DataColumn = startColumn;

            startColumn = _graphData.Fill(graphDataWorksheet, pamsWorkSummaryWorksheet, chartRowModel.IRI_StateWide_ChartModel.sourceStartRow, startColumn, simulationYearsCount);
            graphDataDependentTabs[PAMSConstants.IRI_Statewide_Tab].DataColumn = startColumn;

            //OPI Charts
            startColumn = _graphData.Fill(graphDataWorksheet, pamsWorkSummaryWorksheet, chartRowModel.OPI_BPN_1_ChartModel.sourceStartRow, startColumn, simulationYearsCount);
            graphDataDependentTabs[PAMSConstants.OPI_BPN1_Tab].DataColumn = startColumn;

            startColumn = _graphData.Fill(graphDataWorksheet, pamsWorkSummaryWorksheet, chartRowModel.OPI_BPN_2_ChartModel.sourceStartRow, startColumn, simulationYearsCount);
            graphDataDependentTabs[PAMSConstants.OPI_BPN2_Tab].DataColumn = startColumn;

            startColumn = _graphData.Fill(graphDataWorksheet, pamsWorkSummaryWorksheet, chartRowModel.OPI_BPN_3_ChartModel.sourceStartRow, startColumn, simulationYearsCount);
            graphDataDependentTabs[PAMSConstants.OPI_BPN3_Tab].DataColumn = startColumn;

            startColumn = _graphData.Fill(graphDataWorksheet, pamsWorkSummaryWorksheet, chartRowModel.OPI_BPN_4_ChartModel.sourceStartRow, startColumn, simulationYearsCount);
            graphDataDependentTabs[PAMSConstants.OPI_BPN4_Tab].DataColumn = startColumn;

            startColumn = _graphData.Fill(graphDataWorksheet, pamsWorkSummaryWorksheet, chartRowModel.OPI_StateWide_ChartModel.sourceStartRow, startColumn, simulationYearsCount);
            graphDataDependentTabs[PAMSConstants.OPI_Statewide_Tab].DataColumn = startColumn;

            //hide the graph data worksheet;
            graphDataWorksheet.Hidden = eWorkSheetHidden.Hidden;
        }
    }
}
