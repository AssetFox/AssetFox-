using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.Reporting.Services.BAMSSummaryReport;
using OfficeOpenXml;
using OfficeOpenXml.Drawing.Chart;

namespace AppliedResearchAssociates.iAM.Reporting.Services.PAMSSummaryReport.GraphTabs.Condition
{
    public class ConditionChart
    {
        private StackedColumnChartCommon _stackedColumnChartCommon;

        public ConditionChart()
        {
            _stackedColumnChartCommon = new StackedColumnChartCommon();
        }

        public void Fill(ExcelWorksheet worksheet, ExcelWorksheet graphWorksheet, int dataStartColumn, int simulationYearsCount, string title, string yAxisTitle, string xAxisTitle)
        {
            _stackedColumnChartCommon.SetWorksheetProperties(worksheet);
            var chart = worksheet.Drawings.AddChart(title, eChartType.ColumnStacked);
            _stackedColumnChartCommon.SetChartProperties(chart, title, 1050, 700, 6, 6);

            _stackedColumnChartCommon.SetChartAxes(chart, yAxisTitle, xAxisTitle);
            AddSeries(graphWorksheet, dataStartColumn, simulationYearsCount, chart);

            ((ExcelBarChart)chart).GapWidth = 75;
            chart.AdjustPositionAndSize();
            chart.Locked = true;
        }

        private void AddSeries(ExcelWorksheet graphWorksheet, int dataStartColumn, int count, ExcelChart chart)
        {
            CreateSeries(graphWorksheet, dataStartColumn, count, chart, dataStartColumn + 4, PAMSConstants.Poor, Color.Red);
            CreateSeries(graphWorksheet, dataStartColumn, count, chart, dataStartColumn + 3, PAMSConstants.Fair, Color.Yellow);
            CreateSeries(graphWorksheet, dataStartColumn, count, chart, dataStartColumn + 2, PAMSConstants.Good, Color.Green);
            CreateSeries(graphWorksheet, dataStartColumn, count, chart, dataStartColumn + 1, PAMSConstants.Excellent, Color.RoyalBlue);
        }

        private void CreateSeries(ExcelWorksheet graphWorksheet, int dataStartRow, int count, ExcelChart chart, int fromCol, string header, Color color)
        {
            var serie = graphWorksheet.Cells[3, fromCol, count + 2, fromCol];
            var xSerie = graphWorksheet.Cells[3, dataStartRow, count + 2, dataStartRow];
            
            var excelChartSerie = chart.Series.Add(serie, xSerie);
            excelChartSerie.Header = header;
            excelChartSerie.Fill.Color = color;
        }
    }
}
