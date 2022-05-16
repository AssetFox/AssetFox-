using System.Drawing;
using OfficeOpenXml;
using OfficeOpenXml.Drawing.Chart;

namespace AppliedResearchAssociates.iAM.Reporting.Services.BAMSSummaryReport.GraphTabs
{
    public class PoorBridgeDeckArea
    {
        private StackedColumnChartCommon _stackedColumnChartCommon;

        public PoorBridgeDeckArea()
        {
            _stackedColumnChartCommon = new StackedColumnChartCommon();
        }

        public void Fill(ExcelWorksheet worksheet, ExcelWorksheet bridgeWorkSummaryWorksheet, int totalPoorBridgesDeckAreaSectionYearsRow, int simulationYearsCount)
        {
            _stackedColumnChartCommon.SetWorksheetProperties(worksheet);
            var title = BAMSConstants.PoorBridgeCompareDeckArea;
            var chart = worksheet.Drawings.AddChart(title, eChartType.ColumnStacked);
            _stackedColumnChartCommon.SetChartProperties(chart, title, 1050, 800, 2, 5);

            SetChartAxes(chart);
            AddSeries(bridgeWorkSummaryWorksheet, totalPoorBridgesDeckAreaSectionYearsRow, simulationYearsCount, chart);

            chart.Locked = true;
        }

        private void AddSeries(ExcelWorksheet bridgeWorkSummaryWorkSheet, int totalPoorBridgesDeckAreaSectionYearsRow, int count, ExcelChart chart)
        {
            CreateSeries(bridgeWorkSummaryWorkSheet, totalPoorBridgesDeckAreaSectionYearsRow, count, chart, totalPoorBridgesDeckAreaSectionYearsRow + 1, BAMSConstants.Overall, Color.FromArgb(68, 114, 196));
        }

        private void CreateSeries(ExcelWorksheet bridgeWorkSummaryWorkSheet, int totalPoorBridgesDeckAreaSectionYearsRow, int count, ExcelChart chart, int fromRow, string header, Color color)
        {
            var serie = bridgeWorkSummaryWorkSheet.Cells[fromRow, 2, fromRow, count + 2];
            var xSerie = bridgeWorkSummaryWorkSheet.Cells[totalPoorBridgesDeckAreaSectionYearsRow, 2, totalPoorBridgesDeckAreaSectionYearsRow, count + 2];
            var excelChartSerie = chart.Series.Add(serie, xSerie);
            excelChartSerie.Header = header;
            excelChartSerie.Fill.Color = color;
        }

        private void SetChartAxes(ExcelChart chart)
        {
            _stackedColumnChartCommon.SetChartAxes(chart);
            var yAxis = chart.YAxis;
            yAxis.Format = "_(* #,##0.00_);_(* (#,##0.00);_(* -??_);_(@_)";
        }
    }
}
