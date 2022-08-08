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
            var GraphDataDependentTabs = new Dictionary<string, GraphDataTab>();
            void AddGraphDataDependentTab(string tabTitle, string graphTitle) => GraphDataDependentTabs.Add(tabTitle, new GraphDataTab(excelPackage.Workbook.Worksheets.Add(tabTitle), graphTitle));

            // "Graph Data" tab is required for these, but we don't want "Graph Data" showing up until the end since it isn't really a report.
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

            int startColumn = 1; startColumn = _graphData.Fill(graphDataWorksheet, pamsWorkSummaryWorksheet, chartRowModel.OPI_BPN_1_ChartModel., startColumn, simulationYearsCount);
            GraphDataDependentTabs[PAMSConstants.IRI_BPN1_Tab].DataColumn = startColumn;

            //startColumn = _graphData.Fill(graphDataWorksheet, pamsWorkSummaryWorksheet, chartRowModel.NHSPamsCountPercentSectionYearsRow, startColumn, simulationYearsCount);
            //GraphDataDependentTabs[PAMSConstants.IRI_BPN2_Tab].DataColumn = startColumn;

            //startColumn = _graphData.Fill(graphDataWorksheet, pamsWorkSummaryWorksheet, chartRowModel.NHSPamsCountPercentSectionYearsRow, startColumn, simulationYearsCount);
            //GraphDataDependentTabs[PAMSConstants.IRI_BPN3_Tab].DataColumn = startColumn;

            //startColumn = _graphData.Fill(graphDataWorksheet, pamsWorkSummaryWorksheet, chartRowModel.NHSPamsCountPercentSectionYearsRow, startColumn, simulationYearsCount);
            //GraphDataDependentTabs[PAMSConstants.IRI_BPN4_Tab].DataColumn = startColumn;

            //startColumn = _graphData.Fill(graphDataWorksheet, pamsWorkSummaryWorksheet, chartRowModel.NHSPamsCountPercentSectionYearsRow, startColumn, simulationYearsCount);
            //GraphDataDependentTabs[PAMSConstants.IRI_Statewide_Tab].DataColumn = startColumn;


            //startColumn = _graphData.Fill(graphDataWorksheet, pamsWorkSummaryWorksheet, chartRowModel.TotalPamsCountPercentYearsRow, startColumn, simulationYearsCount);
            //GraphDataDependentTabs[PAMSConstants.OPI_BPN1_Tab].DataColumn = startColumn;

            //startColumn = _graphData.Fill(graphDataWorksheet, pamsWorkSummaryWorksheet, chartRowModel.TotalPamsCountPercentYearsRow, startColumn, simulationYearsCount);
            //GraphDataDependentTabs[PAMSConstants.OPI_BPN2_Tab].DataColumn = startColumn;

            //startColumn = _graphData.Fill(graphDataWorksheet, pamsWorkSummaryWorksheet, chartRowModel.TotalPamsCountPercentYearsRow, startColumn, simulationYearsCount);
            //GraphDataDependentTabs[PAMSConstants.OPI_BPN3_Tab].DataColumn = startColumn;

            //startColumn = _graphData.Fill(graphDataWorksheet, pamsWorkSummaryWorksheet, chartRowModel.TotalPamsCountPercentYearsRow, startColumn, simulationYearsCount);
            //GraphDataDependentTabs[PAMSConstants.OPI_BPN4_Tab].DataColumn = startColumn;

            //startColumn = _graphData.Fill(graphDataWorksheet, pamsWorkSummaryWorksheet, chartRowModel.TotalPamsCountPercentYearsRow, startColumn, simulationYearsCount);
            //GraphDataDependentTabs[PAMSConstants.OPI_Statewide_Tab].DataColumn = startColumn;


            //_graphData.Fill(graphDataWorksheet, pamsWorkSummaryWorksheet, chartRowModel.NHSPamsCountPercentSectionYearsRow, startColumn, simulationYearsCount);
            //graphDataWorksheet.Hidden = eWorkSheetHidden.Hidden;

            //// Fill the already created tabs that are dependent on the "Graph Data" tab; order of fill does not matter, worksheets were added in order
            //foreach (var graphDataTab in GraphDataDependentTabs.Values)
            //{
            //    _conditionPercentageChart.Fill(graphDataTab.Worksheet, graphDataWorksheet, graphDataTab.DataColumn, graphDataTab.Title, simulationYearsCount);
            //}

            //_addBPNGraphTab.AddBPNTab(excelPackage, worksheet, pamsWorkSummaryWorksheet, chartRowModel, simulationYearsCount);
        }
    }
}
