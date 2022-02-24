using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BridgeCareCore.Interfaces.SummaryReport;
using BridgeCareCore.Models.SummaryReport;
using OfficeOpenXml;

namespace BridgeCareCore.Services.SummaryReport.GraphTabs.BPN
{
    public class AddBPNGraphTab : IAddBPNGraphTab
    {
        private readonly CombinedPostedAndClosed _combinedPostedAndClosed;
        private readonly CashNeededByBPN _cashNeededByBPN;

        private readonly BPNAreaChart _bpnAreaChart;
        private readonly BPNCountChart _bpnCountChart;

        public AddBPNGraphTab(CombinedPostedAndClosed combinedPostedAndClosed, CashNeededByBPN cashNeededByBPN,
                                BPNAreaChart bpnAreaChart, BPNCountChart bpnCountChart)
        {
            _combinedPostedAndClosed = combinedPostedAndClosed ?? throw new ArgumentNullException(nameof(combinedPostedAndClosed));
            _cashNeededByBPN = cashNeededByBPN ?? throw new ArgumentNullException(nameof(cashNeededByBPN));

            _bpnAreaChart = bpnAreaChart ?? throw new ArgumentNullException(nameof(bpnAreaChart));
            _bpnCountChart = bpnCountChart ?? throw new ArgumentNullException(nameof(bpnCountChart));
        }
        public void AddBPNTab(ExcelPackage excelPackage, ExcelWorksheet worksheet, ExcelWorksheet bridgeWorkSummaryWorksheet, ChartRowsModel chartRowModel, int simulationYearsCount)
        {
            var GraphDataDependentTabs = new Dictionary<string, GraphDataTab>();

            void AddGraphDataDependentTab(string tabTitle, string graphTitle) => GraphDataDependentTabs.Add(tabTitle, new GraphDataTab(excelPackage.Workbook.Worksheets.Add(tabTitle), graphTitle));

            AddGraphDataDependentTab(Properties.Resources.Graph_PoorDAByBPN_Tab, Properties.Resources.PoorDeckAreaByBPN);
            AddGraphDataDependentTab(Properties.Resources.Graph_PostedBPNCount_Tab, Properties.Resources.PostedBridgeCountByBPN);
            AddGraphDataDependentTab(Properties.Resources.Graph_PostedBPNDA_Tab, Properties.Resources.PostedBridgeDeckAreaByBPN);
            AddGraphDataDependentTab(Properties.Resources.Graph_ClosedBPNCount_Tab, Properties.Resources.ClosedBridgeCountByBPN);
            AddGraphDataDependentTab(Properties.Resources.Graph_ClosedBPNDA_Tab, Properties.Resources.ClosedBridgeDeckAreaByBPN);
            AddGraphDataDependentTab(Properties.Resources.Graph_CombinedPostedandClosed_Tab, Properties.Resources.CombinedPostedAndClosed);
            AddGraphDataDependentTab(Properties.Resources.Graph_DollarNeededDABPN_Tab, Properties.Resources.CashNeededByBPN);

            GraphDataDependentTabs[Properties.Resources.Graph_PoorDAByBPN_Tab].DataColumn = chartRowModel.TotalPoorDeckAreaByBPNSectionYearsRow;
            GraphDataDependentTabs[Properties.Resources.Graph_PoorDAByBPN_Tab].type = ChartType.AreaChart;
            GraphDataDependentTabs[Properties.Resources.Graph_PostedBPNCount_Tab].DataColumn = chartRowModel.TotalBridgePostedCountByBPNYearsRow;
            GraphDataDependentTabs[Properties.Resources.Graph_PostedBPNCount_Tab].type = ChartType.CountChart;
            GraphDataDependentTabs[Properties.Resources.Graph_PostedBPNDA_Tab].DataColumn = chartRowModel.TotalPostedBridgeDeckAreaByBPNYearsRow;
            GraphDataDependentTabs[Properties.Resources.Graph_PostedBPNDA_Tab].type = ChartType.AreaChart;
            GraphDataDependentTabs[Properties.Resources.Graph_ClosedBPNCount_Tab].DataColumn = chartRowModel.TotalClosedBridgeCountByBPNYearsRow;
            GraphDataDependentTabs[Properties.Resources.Graph_ClosedBPNCount_Tab].type = ChartType.CountChart;
            GraphDataDependentTabs[Properties.Resources.Graph_ClosedBPNDA_Tab].DataColumn = chartRowModel.TotalClosedBridgeDeckAreaByBPNYearsRow;
            GraphDataDependentTabs[Properties.Resources.Graph_ClosedBPNDA_Tab].type = ChartType.AreaChart;
            GraphDataDependentTabs[Properties.Resources.Graph_CombinedPostedandClosed_Tab].DataColumn = chartRowModel.TotalPostedAndClosedByBPNYearsRow;
            GraphDataDependentTabs[Properties.Resources.Graph_CombinedPostedandClosed_Tab].type = ChartType.CombinedChart;
            GraphDataDependentTabs[Properties.Resources.Graph_DollarNeededDABPN_Tab].DataColumn = chartRowModel.TotalCashNeededByBPNYearsRow;
            GraphDataDependentTabs[Properties.Resources.Graph_DollarNeededDABPN_Tab].type = ChartType.CashChart;

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
