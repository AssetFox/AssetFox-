using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using OfficeOpenXml;
using OfficeOpenXml.Drawing.Chart;

namespace BridgeCareCore.Services.SummaryReport.GraphTabs.BPN
{
    public class CombinedPostedAndClosed
    {
        private readonly StackedColumnChartCommon _stackedColumnChartCommon;
        public CombinedPostedAndClosed(StackedColumnChartCommon stackedColumnChartCommon)
        {
            _stackedColumnChartCommon = stackedColumnChartCommon;
        }

        public void Fill(ExcelWorksheet worksheet, ExcelWorksheet bridgeWorkSummaryWorksheet, int totalPostedAndClosedByBPNYearsRow, int simulationYearsCount)
        {
            _stackedColumnChartCommon.SetWorksheetProperties(worksheet);
            var title = Properties.Resources.CombinedPostedAndClosed;
            var chart = worksheet.Drawings.AddChart(title, eChartType.ColumnStacked);
            _stackedColumnChartCommon.SetChartProperties(chart, title, 950, 700, 6, 7);

            SetChartAxes(chart);
            AddSeries(bridgeWorkSummaryWorksheet, totalPostedAndClosedByBPNYearsRow, simulationYearsCount, chart);

            chart.AdjustPositionAndSize();
            chart.Locked = true;
        }
        private void AddSeries(ExcelWorksheet bridgeWorkSummaryWorkSheet, int totalPostedAndClosedByBPNYearsRow, int count, ExcelChart chart)
        {
            CreateSeries(bridgeWorkSummaryWorkSheet, totalPostedAndClosedByBPNYearsRow, count, chart, totalPostedAndClosedByBPNYearsRow + 1, Properties.Resources.BPN1, Color.FromArgb(68, 114, 196));
            CreateSeries(bridgeWorkSummaryWorkSheet, totalPostedAndClosedByBPNYearsRow, count, chart, totalPostedAndClosedByBPNYearsRow + 2, Properties.Resources.BPN2, Color.FromArgb(237, 125, 49));
            CreateSeries(bridgeWorkSummaryWorkSheet, totalPostedAndClosedByBPNYearsRow, count, chart, totalPostedAndClosedByBPNYearsRow + 3, Properties.Resources.BPN3, Color.FromArgb(165, 165, 165));
            CreateSeries(bridgeWorkSummaryWorkSheet, totalPostedAndClosedByBPNYearsRow, count, chart, totalPostedAndClosedByBPNYearsRow + 4, Properties.Resources.BPN4, Color.FromArgb(255, 192, 0));
            CreateSeries(bridgeWorkSummaryWorkSheet, totalPostedAndClosedByBPNYearsRow, count, chart, totalPostedAndClosedByBPNYearsRow + 5, Properties.Resources.BPND, Color.FromArgb(91, 155, 21));
            CreateSeries(bridgeWorkSummaryWorkSheet, totalPostedAndClosedByBPNYearsRow, count, chart, totalPostedAndClosedByBPNYearsRow + 6, Properties.Resources.BPNH, Color.FromArgb(112, 173, 71));
            CreateSeries(bridgeWorkSummaryWorkSheet, totalPostedAndClosedByBPNYearsRow, count, chart, totalPostedAndClosedByBPNYearsRow + 7, Properties.Resources.BPNL, Color.FromArgb(38, 68, 120));
            CreateSeries(bridgeWorkSummaryWorkSheet, totalPostedAndClosedByBPNYearsRow, count, chart, totalPostedAndClosedByBPNYearsRow + 8, Properties.Resources.BPNN, Color.FromArgb(158, 72, 14));
            CreateSeries(bridgeWorkSummaryWorkSheet, totalPostedAndClosedByBPNYearsRow, count, chart, totalPostedAndClosedByBPNYearsRow + 9, Properties.Resources.BPNT, Color.FromArgb(99, 99, 99));
        }

        private void CreateSeries(ExcelWorksheet bridgeWorkSummaryWorkSheet, int totalPostedAndClosedByBPNYearsRow, int count, ExcelChart chart, int fromRow, string header, Color color)
        {
            var serie = bridgeWorkSummaryWorkSheet.Cells[fromRow, 2, fromRow, count + 2];
            var xSerie = bridgeWorkSummaryWorkSheet.Cells[totalPostedAndClosedByBPNYearsRow, 2, totalPostedAndClosedByBPNYearsRow, count + 2];
            var excelChartSerie = chart.Series.Add(serie, xSerie);
            excelChartSerie.Header = header;
            excelChartSerie.Fill.Color = color;
        }
        private void SetChartAxes(ExcelChart chart)
        {
            _stackedColumnChartCommon.SetChartAxes(chart);
            var yAxis = chart.YAxis;
            yAxis.DisplayUnit = 100000;
            yAxis.Format = "_($* #,##0_);_($* (#,##0);_($* -??_);_(@_)";
            yAxis.Title.TextVertical = OfficeOpenXml.Drawing.eTextVerticalType.Vertical;
            yAxis.Title.Font.Size = 10;
            yAxis.Title.Text = "Millions ($)";
        }
    }
}
