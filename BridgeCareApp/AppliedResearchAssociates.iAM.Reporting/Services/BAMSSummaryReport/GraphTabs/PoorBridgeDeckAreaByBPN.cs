using System.Drawing;
using OfficeOpenXml;
using OfficeOpenXml.Drawing.Chart;

namespace AppliedResearchAssociates.iAM.Reporting.Services.BAMSSummaryReport.GraphTabs
{
    public class PoorBridgeDeckAreaByBPN
    {
        private readonly StackedColumnChartCommon _stackedColumnChartCommon;

        public PoorBridgeDeckAreaByBPN(StackedColumnChartCommon stackedColumnChartCommon)
        {
            _stackedColumnChartCommon = stackedColumnChartCommon;
        }

        public void Fill(ExcelWorksheet worksheet, ExcelWorksheet bridgeWorkSummaryWorksheet, int totalPoorDeckAreaByBPNSectionYearsRow, int simulationYearsCount)
        {
            _stackedColumnChartCommon.SetWorksheetProperties(worksheet);
            var title = BAMSConstants.PoorDeckAreaByBPN;
            var chart = worksheet.Drawings.AddChart(title, eChartType.ColumnStacked);
            _stackedColumnChartCommon.SetChartProperties(chart, title, 950, 700, 6, 7);

            SetChartAxes(chart);
            AddSeries(bridgeWorkSummaryWorksheet, totalPoorDeckAreaByBPNSectionYearsRow, simulationYearsCount, chart);

            chart.AdjustPositionAndSize();
            chart.Locked = true;
        }

        private void AddSeries(ExcelWorksheet bridgeWorkSummaryWorkSheet, int totalPoorDeckAreaByBPNYearsRow, int count, ExcelChart chart)
        {
            CreateSeries(bridgeWorkSummaryWorkSheet, totalPoorDeckAreaByBPNYearsRow, count, chart, totalPoorDeckAreaByBPNYearsRow + 1, BAMSConstants.BPN1, Color.FromArgb(68,114,196));
            CreateSeries(bridgeWorkSummaryWorkSheet, totalPoorDeckAreaByBPNYearsRow, count, chart, totalPoorDeckAreaByBPNYearsRow + 2, BAMSConstants.BPN2, Color.FromArgb(237, 125, 49));
            CreateSeries(bridgeWorkSummaryWorkSheet, totalPoorDeckAreaByBPNYearsRow, count, chart, totalPoorDeckAreaByBPNYearsRow + 3, BAMSConstants.BPN3, Color.FromArgb(165, 165, 165));
            CreateSeries(bridgeWorkSummaryWorkSheet, totalPoorDeckAreaByBPNYearsRow, count, chart, totalPoorDeckAreaByBPNYearsRow + 4, BAMSConstants.BPN4, Color.FromArgb(255, 192, 0));
            CreateSeries(bridgeWorkSummaryWorkSheet, totalPoorDeckAreaByBPNYearsRow, count, chart, totalPoorDeckAreaByBPNYearsRow + 5, BAMSConstants.BPND, Color.FromArgb(91, 155, 21));
            CreateSeries(bridgeWorkSummaryWorkSheet, totalPoorDeckAreaByBPNYearsRow, count, chart, totalPoorDeckAreaByBPNYearsRow + 6, BAMSConstants.BPNH, Color.FromArgb(112, 173, 71));
            CreateSeries(bridgeWorkSummaryWorkSheet, totalPoorDeckAreaByBPNYearsRow, count, chart, totalPoorDeckAreaByBPNYearsRow + 7, BAMSConstants.BPNL, Color.FromArgb(38, 68, 120));
            CreateSeries(bridgeWorkSummaryWorkSheet, totalPoorDeckAreaByBPNYearsRow, count, chart, totalPoorDeckAreaByBPNYearsRow + 8, BAMSConstants.BPNN, Color.FromArgb(158, 72, 14));
            CreateSeries(bridgeWorkSummaryWorkSheet, totalPoorDeckAreaByBPNYearsRow, count, chart, totalPoorDeckAreaByBPNYearsRow + 9, BAMSConstants.BPNT, Color.FromArgb(99, 99, 99));
        }

        private void CreateSeries(ExcelWorksheet bridgeWorkSummaryWorkSheet, int totalPoorDeckAreaByBPNYearsRow, int count, ExcelChart chart, int fromRow, string header, Color color)
        {
            var serie = bridgeWorkSummaryWorkSheet.Cells[fromRow, 2, fromRow, count + 2];
            var xSerie = bridgeWorkSummaryWorkSheet.Cells[totalPoorDeckAreaByBPNYearsRow, 2, totalPoorDeckAreaByBPNYearsRow, count + 2];
            var excelChartSerie = chart.Series.Add(serie, xSerie);
            excelChartSerie.Header = header;
            excelChartSerie.Fill.Color = color;
        }

        private void SetChartAxes(ExcelChart chart)
        {
            _stackedColumnChartCommon.SetChartAxes(chart);
            var yAxis = chart.YAxis;
            yAxis.DisplayUnit = 1000000;
            yAxis.Format = "_(* #,##0.00_);_(* (#,##0.00);_(* -??_);_(@_)";
            yAxis.Title.TextVertical = OfficeOpenXml.Drawing.eTextVerticalType.Vertical;
            yAxis.Title.Font.Size = 10;
            yAxis.Title.Text = "Millions sqft";
        }
    }
}
