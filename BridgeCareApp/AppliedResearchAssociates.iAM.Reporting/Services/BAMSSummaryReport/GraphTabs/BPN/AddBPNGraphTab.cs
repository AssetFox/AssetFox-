using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OfficeOpenXml;

using AppliedResearchAssociates.iAM.Reporting.Interfaces.BAMSSummaryReport;
using AppliedResearchAssociates.iAM.Reporting.Models.BAMSSummaryReport;

namespace AppliedResearchAssociates.iAM.Reporting.Services.BAMSSummaryReport.GraphTabs.BPN
{
    public class AddBPNGraphTab : IAddBPNGraphTab
    {
        private CombinedPostedAndClosed _combinedPostedAndClosed;
        private CashNeededByBPN _cashNeededByBPN;

        private BPNAreaChart _bpnAreaChart;
        private BPNCountChart _bpnCountChart;

        public AddBPNGraphTab()
        {
            _combinedPostedAndClosed = new CombinedPostedAndClosed();
            _cashNeededByBPN = new CashNeededByBPN();

            _bpnAreaChart = new BPNAreaChart();
            _bpnCountChart = new BPNCountChart();
        }
        public void AddBPNTab(ExcelPackage excelPackage, ExcelWorksheet worksheet, ExcelWorksheet bridgeWorkSummaryWorksheet, ChartRowsModel chartRowModel, int simulationYearsCount)
        {
            var GraphDataDependentTabs = new Dictionary<string, GraphDataTab>();

            void AddGraphDataDependentTab(string tabTitle, string graphTitle) => GraphDataDependentTabs.Add(tabTitle, new GraphDataTab(excelPackage.Workbook.Worksheets.Add(tabTitle), graphTitle));

            AddGraphDataDependentTab(AuditReportConstants.Graph_PoorDAByBPN_Tab, AuditReportConstants.PoorDeckAreaByBPN);
            AddGraphDataDependentTab(AuditReportConstants.Graph_PostedBPNCount_Tab, AuditReportConstants.PostedBridgeCountByBPN);
            AddGraphDataDependentTab(AuditReportConstants.Graph_PostedBPNDA_Tab, AuditReportConstants.PostedBridgeDeckAreaByBPN);
            AddGraphDataDependentTab(AuditReportConstants.Graph_ClosedBPNCount_Tab, AuditReportConstants.ClosedBridgeCountByBPN);
            AddGraphDataDependentTab(AuditReportConstants.Graph_ClosedBPNDA_Tab, AuditReportConstants.ClosedBridgeDeckAreaByBPN);
            AddGraphDataDependentTab(AuditReportConstants.Graph_CombinedPostedandClosed_Tab, AuditReportConstants.CombinedPostedAndClosed);
            AddGraphDataDependentTab(AuditReportConstants.Graph_DollarNeededDABPN_Tab, AuditReportConstants.CashNeededByBPN);

            GraphDataDependentTabs[AuditReportConstants.Graph_PoorDAByBPN_Tab].DataColumn = chartRowModel.TotalPoorDeckAreaByBPNSectionYearsRow;
            GraphDataDependentTabs[AuditReportConstants.Graph_PoorDAByBPN_Tab].type = ChartType.AreaChart;
            GraphDataDependentTabs[AuditReportConstants.Graph_PostedBPNCount_Tab].DataColumn = chartRowModel.TotalBridgePostedCountByBPNYearsRow;
            GraphDataDependentTabs[AuditReportConstants.Graph_PostedBPNCount_Tab].type = ChartType.CountChart;
            GraphDataDependentTabs[AuditReportConstants.Graph_PostedBPNDA_Tab].DataColumn = chartRowModel.TotalPostedBridgeDeckAreaByBPNYearsRow;
            GraphDataDependentTabs[AuditReportConstants.Graph_PostedBPNDA_Tab].type = ChartType.AreaChart;
            GraphDataDependentTabs[AuditReportConstants.Graph_ClosedBPNCount_Tab].DataColumn = chartRowModel.TotalClosedBridgeCountByBPNYearsRow;
            GraphDataDependentTabs[AuditReportConstants.Graph_ClosedBPNCount_Tab].type = ChartType.CountChart;
            GraphDataDependentTabs[AuditReportConstants.Graph_ClosedBPNDA_Tab].DataColumn = chartRowModel.TotalClosedBridgeDeckAreaByBPNYearsRow;
            GraphDataDependentTabs[AuditReportConstants.Graph_ClosedBPNDA_Tab].type = ChartType.AreaChart;
            GraphDataDependentTabs[AuditReportConstants.Graph_CombinedPostedandClosed_Tab].DataColumn = chartRowModel.TotalPostedAndClosedByBPNYearsRow;
            GraphDataDependentTabs[AuditReportConstants.Graph_CombinedPostedandClosed_Tab].type = ChartType.CombinedChart;
            GraphDataDependentTabs[AuditReportConstants.Graph_DollarNeededDABPN_Tab].DataColumn = chartRowModel.TotalCashNeededByBPNYearsRow;
            GraphDataDependentTabs[AuditReportConstants.Graph_DollarNeededDABPN_Tab].type = ChartType.CashChart;

            foreach (var graphDataTab in GraphDataDependentTabs.Values)
            {

                switch (graphDataTab.type)
                {
                case ChartType.AreaChart:
                    _bpnAreaChart.Fill(graphDataTab.Worksheet, bridgeWorkSummaryWorksheet, graphDataTab.DataColumn, simulationYearsCount, graphDataTab.Title);
                    break;
                    case ChartType.CountChart:
                    _bpnCountChart.Fill(graphDataTab.Worksheet, bridgeWorkSummaryWorksheet, graphDataTab.DataColumn, simulationYearsCount, graphDataTab.Title);
                    break;
                case ChartType.CombinedChart:
                    _combinedPostedAndClosed.Fill(graphDataTab.Worksheet, bridgeWorkSummaryWorksheet, graphDataTab.DataColumn, simulationYearsCount, graphDataTab.Title);
                    break;
                case ChartType.CashChart:
                    _cashNeededByBPN.Fill(graphDataTab.Worksheet, bridgeWorkSummaryWorksheet, graphDataTab.DataColumn, simulationYearsCount, graphDataTab.Title);
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
