using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using OfficeOpenXml;
using OfficeOpenXml.Drawing.Chart;

namespace AppliedResearchAssociates.iAM.Reporting.Services.BAMSSummaryReport.GraphTabs.BPN
{
    public class CombinedPostedAndClosed
    {
        private StackedColumnChartCommon _stackedColumnChartCommon;

        public CombinedPostedAndClosed()
        {
            _stackedColumnChartCommon = new StackedColumnChartCommon();
        }

        public void Fill(ExcelWorksheet worksheet, ExcelWorksheet bridgeWorkSummaryWorksheet, int totalPostedAndClosedByBPNYearsRow, int simulationYearsCount, string title)
        {
            _stackedColumnChartCommon.SetWorksheetProperties(worksheet);            
            var chart = worksheet.Drawings.AddChart(title, eChartType.ColumnStacked);
            _stackedColumnChartCommon.SetChartProperties(chart, title, 950, 700, 6, 7);

            _stackedColumnChartCommon.SetChartAxes(chart);
            AddSeries(bridgeWorkSummaryWorksheet, totalPostedAndClosedByBPNYearsRow, simulationYearsCount, chart);

            chart.AdjustPositionAndSize();
            chart.Locked = true;
        }
        private void AddSeries(ExcelWorksheet bridgeWorkSummaryWorkSheet, int totalPostedAndClosedByBPNYearsRow, int count, ExcelChart chart)
        {
            CreateSeries(bridgeWorkSummaryWorkSheet, totalPostedAndClosedByBPNYearsRow, count, chart, totalPostedAndClosedByBPNYearsRow + 1, BAMSConstants.Closed, Color.FromArgb(255, 0, 0));
            CreateSeries(bridgeWorkSummaryWorkSheet, totalPostedAndClosedByBPNYearsRow, count, chart, totalPostedAndClosedByBPNYearsRow + 2, BAMSConstants.Posted, Color.FromArgb(255, 255, 0));            
        }

        private void CreateSeries(ExcelWorksheet bridgeWorkSummaryWorkSheet, int totalPostedAndClosedByBPNYearsRow, int count, ExcelChart chart, int fromRow, string header, Color color)
        {
            var serie = bridgeWorkSummaryWorkSheet.Cells[fromRow, 2, fromRow, count + 2];
            var xSerie = bridgeWorkSummaryWorkSheet.Cells[totalPostedAndClosedByBPNYearsRow, 2, totalPostedAndClosedByBPNYearsRow, count + 2];
            var excelChartSerie = chart.Series.Add(serie, xSerie);
            excelChartSerie.Header = header;
            excelChartSerie.Fill.Color = color;
        }
    }
}
