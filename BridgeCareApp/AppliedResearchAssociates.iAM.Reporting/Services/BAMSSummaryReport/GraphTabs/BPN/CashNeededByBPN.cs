using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using OfficeOpenXml;
using OfficeOpenXml.Drawing.Chart;

namespace AppliedResearchAssociates.iAM.Reporting.Services.BAMSSummaryReport.GraphTabs.BPN
{
    public class CashNeededByBPN
    {
        private StackedColumnChartCommon _stackedColumnChartCommon;

        public CashNeededByBPN()
        {
            _stackedColumnChartCommon = new StackedColumnChartCommon();
        }
        public void Fill(ExcelWorksheet worksheet, ExcelWorksheet bridgeWorkSummaryWorksheet, int totalCashNeededByBPNYearsRow, int simulationYearsCount, string title)
        {
            _stackedColumnChartCommon.SetWorksheetProperties(worksheet);
            var chart = worksheet.Drawings.AddChart(title, eChartType.ColumnStacked);
            _stackedColumnChartCommon.SetChartProperties(chart, title, 950, 700, 6, 7);

            SetChartAxes(chart);
            AddSeries(bridgeWorkSummaryWorksheet, totalCashNeededByBPNYearsRow, simulationYearsCount, chart);

            chart.AdjustPositionAndSize();
            chart.Locked = true;
        }
        private void AddSeries(ExcelWorksheet bridgeWorkSummaryWorkSheet, int totalCashNeededByBPNYearsRow, int count, ExcelChart chart)
        {
            CreateSeries(bridgeWorkSummaryWorkSheet, totalCashNeededByBPNYearsRow, count, chart, totalCashNeededByBPNYearsRow + 1, AuditReportConstants.BPN1, Color.FromArgb(68, 114, 196));
            CreateSeries(bridgeWorkSummaryWorkSheet, totalCashNeededByBPNYearsRow, count, chart, totalCashNeededByBPNYearsRow + 2, AuditReportConstants.BPN2, Color.FromArgb(237, 125, 49));
            CreateSeries(bridgeWorkSummaryWorkSheet, totalCashNeededByBPNYearsRow, count, chart, totalCashNeededByBPNYearsRow + 3, AuditReportConstants.BPN3, Color.FromArgb(165, 165, 165));
            CreateSeries(bridgeWorkSummaryWorkSheet, totalCashNeededByBPNYearsRow, count, chart, totalCashNeededByBPNYearsRow + 4, AuditReportConstants.BPN4, Color.FromArgb(255, 192, 0));
            CreateSeries(bridgeWorkSummaryWorkSheet, totalCashNeededByBPNYearsRow, count, chart, totalCashNeededByBPNYearsRow + 5, AuditReportConstants.BPND, Color.FromArgb(91, 155, 21));
            CreateSeries(bridgeWorkSummaryWorkSheet, totalCashNeededByBPNYearsRow, count, chart, totalCashNeededByBPNYearsRow + 6, AuditReportConstants.BPNH, Color.FromArgb(112, 173, 71));
            CreateSeries(bridgeWorkSummaryWorkSheet, totalCashNeededByBPNYearsRow, count, chart, totalCashNeededByBPNYearsRow + 7, AuditReportConstants.BPNL, Color.FromArgb(38, 68, 120));
            CreateSeries(bridgeWorkSummaryWorkSheet, totalCashNeededByBPNYearsRow, count, chart, totalCashNeededByBPNYearsRow + 8, AuditReportConstants.BPNN, Color.FromArgb(158, 72, 14));
            CreateSeries(bridgeWorkSummaryWorkSheet, totalCashNeededByBPNYearsRow, count, chart, totalCashNeededByBPNYearsRow + 9, AuditReportConstants.BPNT, Color.FromArgb(99, 99, 99));

            CreateLine(bridgeWorkSummaryWorkSheet, totalCashNeededByBPNYearsRow, count, chart, totalCashNeededByBPNYearsRow + 10, AuditReportConstants.AnnualizedAmount, Color.Blue);
        }

        private void CreateLine(ExcelWorksheet bridgeWorkSummaryWorkSheet, int totalCashNeededByBPNYearsRow, int count, ExcelChart chart, int fromRow, string header, Color color)
        {
            var serie = bridgeWorkSummaryWorkSheet.Cells[fromRow, 3, fromRow, count + 2];
            var xSerie = bridgeWorkSummaryWorkSheet.Cells[totalCashNeededByBPNYearsRow, 3, totalCashNeededByBPNYearsRow, count + 2];

            var lineChart = chart.PlotArea.ChartTypes.Add(eChartType.Line);
            var excelChartSerie = lineChart.Series.Add(serie, xSerie);
            excelChartSerie.Header = header;
            excelChartSerie.Border.Fill.Color = color;
        }

        private void CreateSeries(ExcelWorksheet bridgeWorkSummaryWorkSheet, int totalCashNeededByBPNYearsRow, int count, ExcelChart chart, int fromRow, string header, Color color)
        {
            var serie = bridgeWorkSummaryWorkSheet.Cells[fromRow, 3, fromRow, count + 2];
            var xSerie = bridgeWorkSummaryWorkSheet.Cells[totalCashNeededByBPNYearsRow, 3, totalCashNeededByBPNYearsRow, count + 2];
            var excelChartSerie = chart.Series.Add(serie, xSerie);
            excelChartSerie.Header = header;
            excelChartSerie.Fill.Color = color;
        }
        private void SetChartAxes(ExcelChart chart)
        {
            _stackedColumnChartCommon.SetChartAxes(chart);
            var yAxis = chart.YAxis;
            yAxis.DisplayUnit = 1000000;
            yAxis.Format = "_-$* #,##0_-;$* (#,##0)_-;_-$* \"-\"??_-;_-@_-";
            yAxis.Title.TextVertical = OfficeOpenXml.Drawing.eTextVerticalType.Vertical;
            yAxis.Title.Font.Size = 10;
            yAxis.Title.Text = "Millions ($)";
        }
    }
}
