using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OfficeOpenXml;

using AppliedResearchAssociates.iAM.Reporting.Interfaces.PAMSSummaryReport;
using AppliedResearchAssociates.iAM.Reporting.Models.PAMSSummaryReport;

namespace AppliedResearchAssociates.iAM.Reporting.Services.PAMSSummaryReport.GraphTabs.BPN
{
    public class AddBPNGraphTab : IAddBPNGraphTab
    {
        public AddBPNGraphTab()
        {
            
        }

        public void AddBPNTab(ExcelPackage excelPackage, ExcelWorksheet worksheet, ExcelWorksheet pamsWorkSummaryWorksheet, ChartRowsModel chartRowModel, int simulationYearsCount)
        {
            var graphDataDependentTabs = new Dictionary<string, GraphDataTab>();
            void AddGraphDataDependentTab(string tabTitle, string graphTitle) => graphDataDependentTabs.Add(tabTitle, new GraphDataTab(excelPackage.Workbook.Worksheets.Add(tabTitle), graphTitle));

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


            graphDataDependentTabs[PAMSConstants.IRI_BPN1_Tab].DataColumn = chartRowModel.TotalPamsPostedCountByBPNYearsRow;
            graphDataDependentTabs[PAMSConstants.IRI_BPN1_Tab].type = ChartType.CountChart;

            graphDataDependentTabs[PAMSConstants.IRI_BPN2_Tab].DataColumn = chartRowModel.TotalPamsPostedCountByBPNYearsRow;
            graphDataDependentTabs[PAMSConstants.IRI_BPN2_Tab].type = ChartType.CountChart;

            graphDataDependentTabs[PAMSConstants.IRI_BPN3_Tab].DataColumn = chartRowModel.TotalPamsPostedCountByBPNYearsRow;
            graphDataDependentTabs[PAMSConstants.IRI_BPN3_Tab].type = ChartType.CountChart;

            graphDataDependentTabs[PAMSConstants.IRI_BPN4_Tab].DataColumn = chartRowModel.TotalPamsPostedCountByBPNYearsRow;
            graphDataDependentTabs[PAMSConstants.IRI_BPN4_Tab].type = ChartType.CountChart;

            graphDataDependentTabs[PAMSConstants.IRI_Statewide_Tab].DataColumn = chartRowModel.TotalPamsPostedCountByBPNYearsRow;
            graphDataDependentTabs[PAMSConstants.IRI_Statewide_Tab].type = ChartType.CountChart;



            graphDataDependentTabs[PAMSConstants.OPI_BPN1_Tab].DataColumn = chartRowModel.TotalPamsPostedCountByBPNYearsRow;
            graphDataDependentTabs[PAMSConstants.OPI_BPN1_Tab].type = ChartType.CountChart;

            graphDataDependentTabs[PAMSConstants.OPI_BPN2_Tab].DataColumn = chartRowModel.TotalPamsPostedCountByBPNYearsRow;
            graphDataDependentTabs[PAMSConstants.OPI_BPN2_Tab].type = ChartType.CountChart;

            graphDataDependentTabs[PAMSConstants.OPI_BPN3_Tab].DataColumn = chartRowModel.TotalPamsPostedCountByBPNYearsRow;
            graphDataDependentTabs[PAMSConstants.OPI_BPN3_Tab].type = ChartType.CountChart;

            graphDataDependentTabs[PAMSConstants.OPI_BPN4_Tab].DataColumn = chartRowModel.TotalPamsPostedCountByBPNYearsRow;
            graphDataDependentTabs[PAMSConstants.OPI_BPN4_Tab].type = ChartType.CountChart;

            graphDataDependentTabs[PAMSConstants.OPI_Statewide_Tab].DataColumn = chartRowModel.TotalPamsPostedCountByBPNYearsRow;
            graphDataDependentTabs[PAMSConstants.OPI_Statewide_Tab].type = ChartType.CountChart;


            foreach (var graphDataTab in graphDataDependentTabs.Values)
            {
                switch (graphDataTab.type)
                {
                    case ChartType.AreaChart:
                        //_bpnAreaChart.Fill(graphDataTab.Worksheet, pamsWorkSummaryWorksheet, graphDataTab.DataColumn, simulationYearsCount, graphDataTab.Title);
                        break;

                    case ChartType.CountChart:
                        //_bpnCountChart.Fill(graphDataTab.Worksheet, pamsWorkSummaryWorksheet, graphDataTab.DataColumn, simulationYearsCount, graphDataTab.Title);
                        break;

                    case ChartType.CombinedChart:
                        //_combinedPostedAndClosed.Fill(graphDataTab.Worksheet, pamsWorkSummaryWorksheet, graphDataTab.DataColumn, simulationYearsCount, graphDataTab.Title);
                        break;

                    case ChartType.CashChart:
                        //_cashNeededByBPN.Fill(graphDataTab.Worksheet, pamsWorkSummaryWorksheet, graphDataTab.DataColumn, simulationYearsCount, graphDataTab.Title);
                        break;
                }
            }
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
            public ChartType type { get; set; }
        }
    }
    public enum ChartType
    {
        CountChart,
        AreaChart,
        CashChart,
        CombinedChart
    }
}
