﻿using System;
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

            AddGraphDataDependentTab(BAMSConstants.Graph_PoorDAByBPN_Tab, BAMSConstants.PoorDeckAreaByBPN);
            AddGraphDataDependentTab(BAMSConstants.Graph_PostedBPNCount_Tab, BAMSConstants.PostedBridgeCountByBPN);
            AddGraphDataDependentTab(BAMSConstants.Graph_PostedBPNDA_Tab, BAMSConstants.PostedBridgeDeckAreaByBPN);
            AddGraphDataDependentTab(BAMSConstants.Graph_ClosedBPNCount_Tab, BAMSConstants.ClosedBridgeCountByBPN);
            AddGraphDataDependentTab(BAMSConstants.Graph_ClosedBPNDA_Tab, BAMSConstants.ClosedBridgeDeckAreaByBPN);
            AddGraphDataDependentTab(BAMSConstants.Graph_CombinedPostedandClosed_Tab, BAMSConstants.CombinedPostedAndClosed);
            AddGraphDataDependentTab(BAMSConstants.Graph_DollarNeededDABPN_Tab, BAMSConstants.CashNeededByBPN);

            GraphDataDependentTabs[BAMSConstants.Graph_PoorDAByBPN_Tab].DataColumn = chartRowModel.TotalPoorDeckAreaByBPNSectionYearsRow;
            GraphDataDependentTabs[BAMSConstants.Graph_PoorDAByBPN_Tab].type = ChartType.AreaChart;
            GraphDataDependentTabs[BAMSConstants.Graph_PostedBPNCount_Tab].DataColumn = chartRowModel.TotalBridgePostedCountByBPNYearsRow;
            GraphDataDependentTabs[BAMSConstants.Graph_PostedBPNCount_Tab].type = ChartType.CountChart;
            GraphDataDependentTabs[BAMSConstants.Graph_PostedBPNDA_Tab].DataColumn = chartRowModel.TotalPostedBridgeDeckAreaByBPNYearsRow;
            GraphDataDependentTabs[BAMSConstants.Graph_PostedBPNDA_Tab].type = ChartType.AreaChart;
            GraphDataDependentTabs[BAMSConstants.Graph_ClosedBPNCount_Tab].DataColumn = chartRowModel.TotalClosedBridgeCountByBPNYearsRow;
            GraphDataDependentTabs[BAMSConstants.Graph_ClosedBPNCount_Tab].type = ChartType.CountChart;
            GraphDataDependentTabs[BAMSConstants.Graph_ClosedBPNDA_Tab].DataColumn = chartRowModel.TotalClosedBridgeDeckAreaByBPNYearsRow;
            GraphDataDependentTabs[BAMSConstants.Graph_ClosedBPNDA_Tab].type = ChartType.AreaChart;
            GraphDataDependentTabs[BAMSConstants.Graph_CombinedPostedandClosed_Tab].DataColumn = chartRowModel.TotalPostedAndClosedByBPNYearsRow;
            GraphDataDependentTabs[BAMSConstants.Graph_CombinedPostedandClosed_Tab].type = ChartType.CombinedChart;
            GraphDataDependentTabs[BAMSConstants.Graph_DollarNeededDABPN_Tab].DataColumn = chartRowModel.TotalCashNeededByBPNYearsRow;
            GraphDataDependentTabs[BAMSConstants.Graph_DollarNeededDABPN_Tab].type = ChartType.CashChart;

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
