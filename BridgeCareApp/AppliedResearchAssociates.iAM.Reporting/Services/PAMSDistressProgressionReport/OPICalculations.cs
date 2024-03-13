using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using AppliedResearchAssociates.iAM.Analysis.Engine;
using AppliedResearchAssociates.iAM.ExcelHelpers;
using AppliedResearchAssociates.iAM.Reporting.Models;
using AppliedResearchAssociates.iAM.Reporting.Services.PAMSSummaryReport;
using OfficeOpenXml;
using OfficeOpenXml.FormulaParsing.Excel.Functions.DateTime;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using OfficeOpenXml.Style;

namespace AppliedResearchAssociates.iAM.Reporting.Services.PAMSDistressProgressionReport
{
    public class OPICalculations
    {
        private SummaryReportHelper _summaryReportHelper;

        public OPICalculations()
        {
            _summaryReportHelper = new SummaryReportHelper();
        }

        public void Fill(ExcelWorksheet worksheet, SimulationOutput reportOutputData, List<int> simulationYears)
        {
            var currentCell = FillHeaders(worksheet, simulationYears);

            FillDynamicData(worksheet, simulationYears, currentCell); // TODO
        }

        private void FillDynamicData(ExcelWorksheet worksheet, List<int> simulationYears, CurrentCell currentCell)
        {

        }

        private CurrentCell FillHeaders(ExcelWorksheet worksheet, List<int> simulationYears)
        {
            int headerRow = 1;
            var headers = GetInitialHeaders();
            var currentCell = new CurrentCell { Row = headerRow, Column = headers.Count };
            for (int column = 0; column < headers.Count; column++)
            {
                worksheet.Cells[headerRow, column + 1].Value = headers[column];
                ExcelHelper.MergeCells(worksheet, headerRow, column + 1, headerRow + 1, column + 1);
            }
            worksheet.Cells[headerRow, 1, headerRow + 1, headers.Count].AutoFitColumns();

            currentCell = FillDynamicHeaders(worksheet, currentCell, simulationYears);

            return currentCell;
        }

        private CurrentCell FillDynamicHeaders(ExcelWorksheet worksheet, CurrentCell currentCell, List<int> simulationYears)
        {            
            var row = currentCell.Row;
            var yearPartOneHeaders = GetPartOneHeaders();
            var yearBituminousHeaders = GetBituminousHeaders();
            var yearConcreteHeaders = GetConcreteHeaders();

            foreach (var simulationYear in simulationYears)
            {
                var column = currentCell.Column + 1;
                worksheet.Cells[row, column].Value = simulationYear;
                var yearStartColumn = column;
                column = BuildDataSubHeaders(worksheet, row + 1, column, yearPartOneHeaders, ColorTranslator.FromHtml("#70AD47"));
                column = BuildDataSubHeaders(worksheet, row + 1, column, yearBituminousHeaders, Color.Black);
                column = BuildDataSubHeaders(worksheet, row + 1, column, yearConcreteHeaders, ColorTranslator.FromHtml("#FFF2CC"));
                ExcelHelper.MergeCells(worksheet, row, yearStartColumn, row, column - 1);
                worksheet.Cells[row, yearStartColumn, row, column - 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                currentCell.Column = column - 1;
            }

            const double minimumColumnWidth = 15;
            for (int col = 1; col <= worksheet.Dimension.End.Column; col++)
            {
                if (worksheet.Column(col).Width < minimumColumnWidth)
                {
                    worksheet.Column(col).Width = minimumColumnWidth;
                }
            }

            return currentCell;
        }

        private int BuildDataSubHeaders(ExcelWorksheet worksheet, int row, int column, List<string> subHeaders, Color color)
        {
            var startColumn = column;
            for (var index = 0; index < subHeaders.Count; index++)
            {
                worksheet.Cells[row, column].Value = subHeaders[index];
                ExcelHelper.ApplyStyle(worksheet.Cells[row, column]);
                column++;
            }
            column--;
            worksheet.Cells[row, startColumn, row, column].Style.Fill.PatternType = ExcelFillStyle.Solid;
            worksheet.Cells[row, startColumn, row, column].Style.Fill.BackgroundColor.SetColor(color);            
            if(color == Color.Black)
            {
                worksheet.Cells[row, startColumn, row, column].Style.Font.Color.SetColor(Color.White);
            }
            worksheet.Cells[row, startColumn, row, column].AutoFitColumns();

            return ++column;
        }

        // yellow shade       
        private static List<string> GetConcreteHeaders() => new()
        {            
            "SLAB COUNT",
            "JOINT COUNT",
            "CFLTJNT2",
            "CFLTJNT3",
            "Total CFLTJNT",
            "CBRKSLB1",
            "CBRKSLB2",
            "CBRKSLB3",
            "Total CBRKSLB",
            "CTRNCRK1",
            "CTRNCRK2",
            "CTRNCRK3",
            "Total CTRNCRK",
            "CTRNJNT1",
            "CTRNJNT2",
            "CTRNJNT3",
            "Total CTRNJNT",
            "CLNGCRK1",
            "CLNGCRK2",
            "CLNGCRK3",
            "Total CLNGCRK",
            "CLNGJNT1",
            "CLNGJNT2",
            "CLNGJNT3",
            "Total CLNGJNT",
            "CBPATCCT",
            "CBPATCSF",
            "CPCCPACT",
            "CPCCPASF",
            "CLJCPRU1",
            "CLJCPRU2",
            "CLJCPRU3",
            "Total CLJCPRU",
            "CRJCPRU1",
            "CRJCPRU2",
            "CRJCPRU3",
            "Total CRJCPRU",
        };

        // black background with text white
        private static List<string> GetBituminousHeaders() => new()
        {
            "BLRUTDP1",
            "BLRUTDP2",
            "BLRUTDP3",
            "Total BLRUTDP",
            "BRRUTDP1",
            "BRRUTDP2",
            "BRRUTDP3",
            "Total BRRUTDP",
            "BFATICR1",
            "BFATICR2",
            "BFATICR3",
            "Total BFATICR",
            "BTRNSCT1",
            "BTRNSFT1",
            "BTRNSCT2",
            "BTRNSFT2",
            "BTRNSCT3",
            "BTRNSFT3",
            "Total BTRNSCT",
            "Total BTRNSFT",
            "BMISCCK1",
            "BMISCCK2",
            "BMISCCK3",
            "Total BMISCCK", 
            "BEDGDTR1",
            "BEDGDTR2",
            "BEDGDTR3",
            "Total BEDGDTR",
            "BPATCHCT",
            "BPATCHSF",
            "BRAVLWT2",
            "BRAVLWT3",
            "Total BRAVLWT",
            "BLTEDGE1",
            "BLTEDGE2",
            "BLTEDGE3",
            "Total BLTEDGE",
        };

        // green shade
        private static List<string> GetPartOneHeaders() => new()
        {
            "CALCULATED OPI",
            "SEGMENT LENGTH",
            "WIDTH",
            "ROUGHNESS",
        };

        private static List<string> GetInitialHeaders() => new()
        {
            "CRS",
            "Pavement Surface Type",
            "OPI", // TODO green
        };
    }
}
