using System.Drawing;
using OfficeOpenXml;
using OfficeOpenXml.Drawing.Chart;

namespace AppliedResearchAssociates.iAM.Reporting.Services.BAMSSummaryReport.GraphTabs
{
    public class PoorBridgeCount
    {
        private StackedColumnChartCommon _stackedColumnChartCommon;

        public PoorBridgeCount()
        {
            _stackedColumnChartCommon = new StackedColumnChartCommon();
        }

        public void Fill(ExcelWorksheet worksheet, ExcelWorksheet bridgeWorkSummaryWorksheet, int totalPoorBridgesCountSectionYearsRow, int simulationYearsCount)
        {
            _stackedColumnChartCommon.SetWorksheetProperties(worksheet);
            var title = AuditReportConstants.PoorBridgeCompareBridgeCount;
            var chart = worksheet.Drawings.AddChart(title, eChartType.ColumnStacked);
            _stackedColumnChartCommon.SetChartProperties(chart, title, 1050, 800, 2, 5);

            _stackedColumnChartCommon.SetChartAxes(chart);
            AddSeries(bridgeWorkSummaryWorksheet, totalPoorBridgesCountSectionYearsRow, simulationYearsCount, chart);

            chart.Locked = true;
        }

        private void AddSeries(ExcelWorksheet bridgeWorkSummaryWorkSheet, int totalPoorBridgesCountSectionYearsRow, int count, ExcelChart chart)
        {
            CreateSeries(bridgeWorkSummaryWorkSheet, totalPoorBridgesCountSectionYearsRow, count, chart, totalPoorBridgesCountSectionYearsRow + 1, AuditReportConstants.Overall, Color.FromArgb(68, 114, 196));
        }

        private void CreateSeries(ExcelWorksheet bridgeWorkSummaryWorkSheet, int totalPoorBridgesCountSectionYearsRow, int count, ExcelChart chart, int fromRow, string header, Color color)
        {
            var serie = bridgeWorkSummaryWorkSheet.Cells[fromRow, 2, fromRow, count + 2];
            var xSerie = bridgeWorkSummaryWorkSheet.Cells[totalPoorBridgesCountSectionYearsRow, 2, totalPoorBridgesCountSectionYearsRow, count + 2];
            var excelChartSerie = chart.Series.Add(serie, xSerie);
            excelChartSerie.Header = header;
            excelChartSerie.Fill.Color = color;
        }
    }
}
