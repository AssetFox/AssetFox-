using System;
using System.Drawing;
using OfficeOpenXml;
using OfficeOpenXml.Drawing.Chart;

namespace AppliedResearchAssociates.iAM.Reporting.Services.BAMSSummaryReport.GraphTabs.NHSConditionCharts
{
    public class ConditionPercentageChart
    {
        private StackedColumnChartCommon _stackedColumnChartCommon;

        public ConditionPercentageChart()
        {
            _stackedColumnChartCommon = new StackedColumnChartCommon();
        }


        public void Fill(ExcelWorksheet worksheet, ExcelWorksheet bridgeWorkSummaryWorkSheet, int dataStartColumn, string title, int simulationYearsCount)
        {
            _stackedColumnChartCommon.SetWorksheetProperties(worksheet);
            var chart = worksheet.Drawings.AddChart(title, eChartType.ColumnStacked);
            _stackedColumnChartCommon.SetChartProperties(chart, title, 1050, 700, 6, 6);

            SetChartAxes(chart);
            AddSeries(bridgeWorkSummaryWorkSheet, dataStartColumn, simulationYearsCount, chart);

            ((ExcelBarChart)chart).GapWidth = 75;
            chart.AdjustPositionAndSize();
            chart.Locked = true;
        }

        private void AddSeries(ExcelWorksheet bridgeWorkSummaryWorkSheet, int dataStartColumn, int count, ExcelChart chart)
        {
            CreateSeries(bridgeWorkSummaryWorkSheet, dataStartColumn, count, chart, dataStartColumn + 4, BAMSConstants.Closed, Color.Black);

            CreateSeries(bridgeWorkSummaryWorkSheet, dataStartColumn, count, chart, dataStartColumn + 3, BAMSConstants.Poor, Color.Red);

            CreateSeries(bridgeWorkSummaryWorkSheet, dataStartColumn, count, chart, dataStartColumn + 2, BAMSConstants.Fair, Color.Yellow);

            CreateSeries(bridgeWorkSummaryWorkSheet, dataStartColumn, count, chart, dataStartColumn + 1, BAMSConstants.Good, Color.FromArgb(0, 176, 80));
        }

        private void CreateSeries(ExcelWorksheet bridgeWorkSummaryWorkSheet, int dataColumn, int count, ExcelChart chart, int fromColumn, string header, Color color)
        {
            var serie = bridgeWorkSummaryWorkSheet.Cells[3, fromColumn, count * 3 + 4, fromColumn];
            var xSerie = bridgeWorkSummaryWorkSheet.Cells[3, dataColumn, count * 3 + 4, dataColumn];
            var excelChartSerie = chart.Series.Add(serie, xSerie);
            excelChartSerie.Header = header;
            excelChartSerie.Fill.Color = color;
        }

        private void SetChartAxes(ExcelChart chart)
        {
            _stackedColumnChartCommon.SetChartAxes(chart);
            chart.YAxis.Format = "#0%";
            chart.YAxis.MaxValue = 1;
        }
    }
}
