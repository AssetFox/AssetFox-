using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using AppliedResearchAssociates.iAM.Analysis.Engine;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.ExcelHelpers;
using AppliedResearchAssociates.iAM.Reporting.Models;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace AppliedResearchAssociates.iAM.Reporting.Services.PAMSDistressProgressionReport
{
    public class OPICalculations
    {
        private readonly IUnitOfWork _unitOfWork;
        private ReportHelper _reportHelper;

        public OPICalculations(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _reportHelper = new ReportHelper(_unitOfWork);
        }

        public void Fill(ExcelWorksheet worksheet, SimulationOutput reportOutputData, List<int> simulationYears)
        {
            var currentCell = FillHeaders(worksheet, simulationYears);

            FillDynamicData(worksheet, simulationYears, reportOutputData, currentCell);
        }

        private void FillDynamicData(ExcelWorksheet worksheet, List<int> simulationYears, SimulationOutput reportOutputData, CurrentCell currentCell)
        {
            var row = currentCell.Row;
            var column = currentCell.Column;

            foreach (var initialAssetSummary in reportOutputData.InitialAssetSummaries)
            {
                row++; column = 1;
                var valuePerNumericAttribute = initialAssetSummary.ValuePerNumericAttribute;
                var valuePerTextAttribute = initialAssetSummary.ValuePerTextAttribute;
                var crs = CheckGetTextValue(valuePerTextAttribute, "CRS");
                worksheet.Cells[row, column++].Value = crs;
                worksheet.Cells[row, column].Value = CheckGetValue(valuePerNumericAttribute, "SURFACEID");
                // right border line
                ExcelHelper.ApplyRightBorder(worksheet.Cells[row, column++]);
                foreach (var yearData in reportOutputData.Years)
                {
                    var section = yearData.Assets.FirstOrDefault(_ => CheckGetTextValue(_.ValuePerTextAttribute, "CRS") == crs);
                    var sectionValuePerNumericAttribute = section.ValuePerNumericAttribute;
                    if (yearData.Year == simulationYears[0])
                    {
                        worksheet.Cells[row, column].Value = CheckGetValue(sectionValuePerNumericAttribute, "OPI");
                        // double left border line
                        ExcelHelper.ApplyLeftBorder(worksheet.Cells[row, column]);
                        ExcelHelper.SetCustomFormat(worksheet.Cells[row, column++], ExcelHelperCellFormat.DecimalPrecision2);
                    }

                    // part 1 data
                    worksheet.Cells[row, column].Value = CheckGetValue(sectionValuePerNumericAttribute, "OPI_CALCULATED");
                    // double left border line if not first yr
                    if(yearData.Year != simulationYears[0])
                    {
                        ExcelHelper.ApplyLeftBorder(worksheet.Cells[row, column]);
                    }
                    ExcelHelper.SetCustomFormat(worksheet.Cells[row, column++], ExcelHelperCellFormat.DecimalPrecision2);
                    worksheet.Cells[row, column++].Value = CheckGetValue(sectionValuePerNumericAttribute, "SEGMENT_LENGTH");
                    worksheet.Cells[row, column].Value = CheckGetValue(sectionValuePerNumericAttribute, "WIDTH");
                    ExcelHelper.SetCustomFormat(worksheet.Cells[row, column++], ExcelHelperCellFormat.DecimalPrecision2);
                    worksheet.Cells[row, column].Value = CheckGetValue(sectionValuePerNumericAttribute, "ROUGHNESS");
                    // right border line
                    ExcelHelper.ApplyRightBorder(worksheet.Cells[row, column]);
                    ExcelHelper.SetCustomFormat(worksheet.Cells[row, column++], ExcelHelperCellFormat.DecimalPrecision2);

                    // Bituminous data
                    worksheet.Cells[row, column].Value = CheckGetValue(sectionValuePerNumericAttribute, "BLRUTDP1");
                    ExcelHelper.SetCustomFormat(worksheet.Cells[row, column++], ExcelHelperCellFormat.DecimalPrecision2);
                    worksheet.Cells[row, column].Value = CheckGetValue(sectionValuePerNumericAttribute, "BLRUTDP2");
                    ExcelHelper.SetCustomFormat(worksheet.Cells[row, column++], ExcelHelperCellFormat.DecimalPrecision2);
                    worksheet.Cells[row, column].Value = CheckGetValue(sectionValuePerNumericAttribute, "BLRUTDP3");
                    ExcelHelper.SetCustomFormat(worksheet.Cells[row, column++], ExcelHelperCellFormat.DecimalPrecision2);
                    worksheet.Cells[row, column].Value = CheckGetValue(sectionValuePerNumericAttribute, "BLRUTDP_Total");
                    // right border line
                    ExcelHelper.ApplyRightBorder(worksheet.Cells[row, column]);
                    ExcelHelper.SetCustomFormat(worksheet.Cells[row, column++], ExcelHelperCellFormat.DecimalPrecision2);
                    worksheet.Cells[row, column].Value = CheckGetValue(sectionValuePerNumericAttribute, "BRRUTDP1");
                    ExcelHelper.SetCustomFormat(worksheet.Cells[row, column++], ExcelHelperCellFormat.DecimalPrecision2);
                    worksheet.Cells[row, column].Value = CheckGetValue(sectionValuePerNumericAttribute, "BRRUTDP2");
                    ExcelHelper.SetCustomFormat(worksheet.Cells[row, column++], ExcelHelperCellFormat.DecimalPrecision2);
                    worksheet.Cells[row, column].Value = CheckGetValue(sectionValuePerNumericAttribute, "BRRUTDP3");
                    ExcelHelper.SetCustomFormat(worksheet.Cells[row, column++], ExcelHelperCellFormat.DecimalPrecision2);
                    worksheet.Cells[row, column].Value = CheckGetValue(sectionValuePerNumericAttribute, "BRRUTDP_Total");
                    // right border line
                    ExcelHelper.ApplyRightBorder(worksheet.Cells[row, column]);
                    ExcelHelper.SetCustomFormat(worksheet.Cells[row, column++], ExcelHelperCellFormat.DecimalPrecision2);
                    worksheet.Cells[row, column].Value = CheckGetValue(sectionValuePerNumericAttribute, "BFATICR1");
                    ExcelHelper.SetCustomFormat(worksheet.Cells[row, column++], ExcelHelperCellFormat.DecimalPrecision2);
                    worksheet.Cells[row, column].Value = CheckGetValue(sectionValuePerNumericAttribute, "BFATICR2");
                    ExcelHelper.SetCustomFormat(worksheet.Cells[row, column++], ExcelHelperCellFormat.DecimalPrecision2);
                    worksheet.Cells[row, column].Value = CheckGetValue(sectionValuePerNumericAttribute, "BFATICR3");
                    ExcelHelper.SetCustomFormat(worksheet.Cells[row, column++], ExcelHelperCellFormat.DecimalPrecision2);
                    worksheet.Cells[row, column].Value = CheckGetValue(sectionValuePerNumericAttribute, "BFATICR_Total");
                    // right border line
                    ExcelHelper.ApplyRightBorder(worksheet.Cells[row, column]);
                    ExcelHelper.SetCustomFormat(worksheet.Cells[row, column++], ExcelHelperCellFormat.DecimalPrecision2);
                    worksheet.Cells[row, column].Value = CheckGetValue(sectionValuePerNumericAttribute, "BTRNSCT1");
                    ExcelHelper.SetCustomFormat(worksheet.Cells[row, column++], ExcelHelperCellFormat.DecimalPrecision2);
                    worksheet.Cells[row, column].Value = CheckGetValue(sectionValuePerNumericAttribute, "BTRNSFT1");
                    ExcelHelper.SetCustomFormat(worksheet.Cells[row, column++], ExcelHelperCellFormat.DecimalPrecision2);
                    worksheet.Cells[row, column].Value = CheckGetValue(sectionValuePerNumericAttribute, "BTRNSCT2");
                    ExcelHelper.SetCustomFormat(worksheet.Cells[row, column++], ExcelHelperCellFormat.DecimalPrecision2);
                    worksheet.Cells[row, column].Value = CheckGetValue(sectionValuePerNumericAttribute, "BTRNSFT2");
                    ExcelHelper.SetCustomFormat(worksheet.Cells[row, column++], ExcelHelperCellFormat.DecimalPrecision2);
                    worksheet.Cells[row, column].Value = CheckGetValue(sectionValuePerNumericAttribute, "BTRNSCT3");
                    ExcelHelper.SetCustomFormat(worksheet.Cells[row, column++], ExcelHelperCellFormat.DecimalPrecision2);
                    worksheet.Cells[row, column].Value = CheckGetValue(sectionValuePerNumericAttribute, "BTRNSFT3");
                    ExcelHelper.SetCustomFormat(worksheet.Cells[row, column++], ExcelHelperCellFormat.DecimalPrecision2);
                    worksheet.Cells[row, column].Value = CheckGetValue(sectionValuePerNumericAttribute, "BTRNSCT_Total");
                    ExcelHelper.SetCustomFormat(worksheet.Cells[row, column++], ExcelHelperCellFormat.DecimalPrecision2);
                    worksheet.Cells[row, column].Value = CheckGetValue(sectionValuePerNumericAttribute, "BTRNSFT_Total");
                    // right border line
                    ExcelHelper.ApplyRightBorder(worksheet.Cells[row, column]);
                    ExcelHelper.SetCustomFormat(worksheet.Cells[row, column++], ExcelHelperCellFormat.DecimalPrecision2);
                    worksheet.Cells[row, column].Value = CheckGetValue(sectionValuePerNumericAttribute, "BMISCCK1");
                    ExcelHelper.SetCustomFormat(worksheet.Cells[row, column++], ExcelHelperCellFormat.DecimalPrecision2);
                    worksheet.Cells[row, column].Value = CheckGetValue(sectionValuePerNumericAttribute, "BMISCCK2");
                    ExcelHelper.SetCustomFormat(worksheet.Cells[row, column++], ExcelHelperCellFormat.DecimalPrecision2);
                    worksheet.Cells[row, column].Value = CheckGetValue(sectionValuePerNumericAttribute, "BMISCCK3");
                    ExcelHelper.SetCustomFormat(worksheet.Cells[row, column++], ExcelHelperCellFormat.DecimalPrecision2);
                    worksheet.Cells[row, column].Value = CheckGetValue(sectionValuePerNumericAttribute, "BMISCCK_Total");
                    // right border line
                    ExcelHelper.ApplyRightBorder(worksheet.Cells[row, column]);
                    ExcelHelper.SetCustomFormat(worksheet.Cells[row, column++], ExcelHelperCellFormat.DecimalPrecision2);
                    worksheet.Cells[row, column].Value = CheckGetValue(sectionValuePerNumericAttribute, "BMISCCK1");
                    ExcelHelper.SetCustomFormat(worksheet.Cells[row, column++], ExcelHelperCellFormat.DecimalPrecision2);
                    worksheet.Cells[row, column].Value = CheckGetValue(sectionValuePerNumericAttribute, "BMISCCK2");
                    ExcelHelper.SetCustomFormat(worksheet.Cells[row, column++], ExcelHelperCellFormat.DecimalPrecision2);
                    worksheet.Cells[row, column].Value = CheckGetValue(sectionValuePerNumericAttribute, "BMISCCK3");
                    ExcelHelper.SetCustomFormat(worksheet.Cells[row, column++], ExcelHelperCellFormat.DecimalPrecision2);
                    worksheet.Cells[row, column].Value = CheckGetValue(sectionValuePerNumericAttribute, "BMISCCK_Total");
                    // right border line
                    ExcelHelper.ApplyRightBorder(worksheet.Cells[row, column]);
                    ExcelHelper.SetCustomFormat(worksheet.Cells[row, column++], ExcelHelperCellFormat.DecimalPrecision2);
                    worksheet.Cells[row, column].Value = CheckGetValue(sectionValuePerNumericAttribute, "BPATCHCT");
                    ExcelHelper.SetCustomFormat(worksheet.Cells[row, column++], ExcelHelperCellFormat.DecimalPrecision2);
                    worksheet.Cells[row, column].Value = CheckGetValue(sectionValuePerNumericAttribute, "BPATCHSF");
                    ExcelHelper.SetCustomFormat(worksheet.Cells[row, column++], ExcelHelperCellFormat.DecimalPrecision2);
                    worksheet.Cells[row, column].Value = CheckGetValue(sectionValuePerNumericAttribute, "BRAVLWT2");
                    ExcelHelper.SetCustomFormat(worksheet.Cells[row, column++], ExcelHelperCellFormat.DecimalPrecision2);
                    worksheet.Cells[row, column].Value = CheckGetValue(sectionValuePerNumericAttribute, "BRAVLWT3");
                    ExcelHelper.SetCustomFormat(worksheet.Cells[row, column++], ExcelHelperCellFormat.DecimalPrecision2);
                    worksheet.Cells[row, column].Value = CheckGetValue(sectionValuePerNumericAttribute, "BRAVLWT_Total");
                    // right border line
                    ExcelHelper.ApplyRightBorder(worksheet.Cells[row, column]);
                    ExcelHelper.SetCustomFormat(worksheet.Cells[row, column++], ExcelHelperCellFormat.DecimalPrecision2);
                    worksheet.Cells[row, column].Value = CheckGetValue(sectionValuePerNumericAttribute, "BLTEDGE1");
                    ExcelHelper.SetCustomFormat(worksheet.Cells[row, column++], ExcelHelperCellFormat.DecimalPrecision2);
                    worksheet.Cells[row, column].Value = CheckGetValue(sectionValuePerNumericAttribute, "BLTEDGE2");
                    ExcelHelper.SetCustomFormat(worksheet.Cells[row, column++], ExcelHelperCellFormat.DecimalPrecision2);
                    worksheet.Cells[row, column].Value = CheckGetValue(sectionValuePerNumericAttribute, "BLTEDGE3");
                    ExcelHelper.SetCustomFormat(worksheet.Cells[row, column++], ExcelHelperCellFormat.DecimalPrecision2);
                    worksheet.Cells[row, column].Value = CheckGetValue(sectionValuePerNumericAttribute, "BLTEDGE_Total");
                    // right border line
                    ExcelHelper.ApplyRightBorder(worksheet.Cells[row, column]);
                    ExcelHelper.SetCustomFormat(worksheet.Cells[row, column++], ExcelHelperCellFormat.DecimalPrecision2);

                    // Concrete data
                    worksheet.Cells[row, column].Value = CheckGetValue(sectionValuePerNumericAttribute, "CNSLABCT");
                    ExcelHelper.SetCustomFormat(worksheet.Cells[row, column++], ExcelHelperCellFormat.DecimalPrecision2);
                    worksheet.Cells[row, column].Value = CheckGetValue(sectionValuePerNumericAttribute, "CJOINTCT");
                    ExcelHelper.SetCustomFormat(worksheet.Cells[row, column++], ExcelHelperCellFormat.DecimalPrecision2);
                    worksheet.Cells[row, column].Value = CheckGetValue(sectionValuePerNumericAttribute, "CFLTJNT2");
                    ExcelHelper.SetCustomFormat(worksheet.Cells[row, column++], ExcelHelperCellFormat.DecimalPrecision2);
                    worksheet.Cells[row, column].Value = CheckGetValue(sectionValuePerNumericAttribute, "CFLTJNT3");
                    ExcelHelper.SetCustomFormat(worksheet.Cells[row, column++], ExcelHelperCellFormat.DecimalPrecision2);
                    worksheet.Cells[row, column].Value = CheckGetValue(sectionValuePerNumericAttribute, "CFLTJNT_Total");
                    // right border line
                    ExcelHelper.ApplyRightBorder(worksheet.Cells[row, column]);
                    ExcelHelper.SetCustomFormat(worksheet.Cells[row, column++], ExcelHelperCellFormat.DecimalPrecision2);
                    worksheet.Cells[row, column].Value = CheckGetValue(sectionValuePerNumericAttribute, "CBRKSLB1");
                    ExcelHelper.SetCustomFormat(worksheet.Cells[row, column++], ExcelHelperCellFormat.DecimalPrecision2);
                    worksheet.Cells[row, column].Value = CheckGetValue(sectionValuePerNumericAttribute, "CBRKSLB2");
                    ExcelHelper.SetCustomFormat(worksheet.Cells[row, column++], ExcelHelperCellFormat.DecimalPrecision2);
                    worksheet.Cells[row, column].Value = CheckGetValue(sectionValuePerNumericAttribute, "CBRKSLB3");
                    ExcelHelper.SetCustomFormat(worksheet.Cells[row, column++], ExcelHelperCellFormat.DecimalPrecision2);
                    worksheet.Cells[row, column].Value = CheckGetValue(sectionValuePerNumericAttribute, "CBRKSLB_Total");
                    // right border line
                    ExcelHelper.ApplyRightBorder(worksheet.Cells[row, column]);
                    ExcelHelper.SetCustomFormat(worksheet.Cells[row, column++], ExcelHelperCellFormat.DecimalPrecision2);
                    worksheet.Cells[row, column].Value = CheckGetValue(sectionValuePerNumericAttribute, "CTRNCRK1");
                    ExcelHelper.SetCustomFormat(worksheet.Cells[row, column++], ExcelHelperCellFormat.DecimalPrecision2);
                    worksheet.Cells[row, column].Value = CheckGetValue(sectionValuePerNumericAttribute, "CTRNCRK2");
                    ExcelHelper.SetCustomFormat(worksheet.Cells[row, column++], ExcelHelperCellFormat.DecimalPrecision2);
                    worksheet.Cells[row, column].Value = CheckGetValue(sectionValuePerNumericAttribute, "CTRNCRK3");
                    ExcelHelper.SetCustomFormat(worksheet.Cells[row, column++], ExcelHelperCellFormat.DecimalPrecision2);
                    worksheet.Cells[row, column].Value = CheckGetValue(sectionValuePerNumericAttribute, "CTRNCRK_Total");
                    // right border line
                    ExcelHelper.ApplyRightBorder(worksheet.Cells[row, column]);
                    ExcelHelper.SetCustomFormat(worksheet.Cells[row, column++], ExcelHelperCellFormat.DecimalPrecision2);
                    worksheet.Cells[row, column].Value = CheckGetValue(sectionValuePerNumericAttribute, "CTRNJNT1");
                    ExcelHelper.SetCustomFormat(worksheet.Cells[row, column++], ExcelHelperCellFormat.DecimalPrecision2);
                    worksheet.Cells[row, column].Value = CheckGetValue(sectionValuePerNumericAttribute, "CTRNJNT2");
                    ExcelHelper.SetCustomFormat(worksheet.Cells[row, column++], ExcelHelperCellFormat.DecimalPrecision2);
                    worksheet.Cells[row, column].Value = CheckGetValue(sectionValuePerNumericAttribute, "CTRNJNT3");
                    ExcelHelper.SetCustomFormat(worksheet.Cells[row, column++], ExcelHelperCellFormat.DecimalPrecision2);
                    worksheet.Cells[row, column].Value = CheckGetValue(sectionValuePerNumericAttribute, "CTRNJNT_Total");
                    // right border line
                    ExcelHelper.ApplyRightBorder(worksheet.Cells[row, column]);
                    ExcelHelper.SetCustomFormat(worksheet.Cells[row, column++], ExcelHelperCellFormat.DecimalPrecision2);
                    worksheet.Cells[row, column].Value = CheckGetValue(sectionValuePerNumericAttribute, "CLNGCRK1");
                    ExcelHelper.SetCustomFormat(worksheet.Cells[row, column++], ExcelHelperCellFormat.DecimalPrecision2);
                    worksheet.Cells[row, column].Value = CheckGetValue(sectionValuePerNumericAttribute, "CLNGCRK2");
                    ExcelHelper.SetCustomFormat(worksheet.Cells[row, column++], ExcelHelperCellFormat.DecimalPrecision2);
                    worksheet.Cells[row, column].Value = CheckGetValue(sectionValuePerNumericAttribute, "CLNGCRK3");
                    ExcelHelper.SetCustomFormat(worksheet.Cells[row, column++], ExcelHelperCellFormat.DecimalPrecision2);
                    worksheet.Cells[row, column].Value = CheckGetValue(sectionValuePerNumericAttribute, "CLNGCRK_Total");
                    // right border line
                    ExcelHelper.ApplyRightBorder(worksheet.Cells[row, column]);
                    ExcelHelper.SetCustomFormat(worksheet.Cells[row, column++], ExcelHelperCellFormat.DecimalPrecision2);
                    worksheet.Cells[row, column].Value = CheckGetValue(sectionValuePerNumericAttribute, "CLNGJNT1");
                    ExcelHelper.SetCustomFormat(worksheet.Cells[row, column++], ExcelHelperCellFormat.DecimalPrecision2);
                    worksheet.Cells[row, column].Value = CheckGetValue(sectionValuePerNumericAttribute, "CLNGJNT2");
                    ExcelHelper.SetCustomFormat(worksheet.Cells[row, column++], ExcelHelperCellFormat.DecimalPrecision2);
                    worksheet.Cells[row, column].Value = CheckGetValue(sectionValuePerNumericAttribute, "CLNGJNT3");
                    ExcelHelper.SetCustomFormat(worksheet.Cells[row, column++], ExcelHelperCellFormat.DecimalPrecision2);
                    worksheet.Cells[row, column].Value = CheckGetValue(sectionValuePerNumericAttribute, "CLNGJNT_Total");
                    // right border line
                    ExcelHelper.ApplyRightBorder(worksheet.Cells[row, column]);
                    ExcelHelper.SetCustomFormat(worksheet.Cells[row, column++], ExcelHelperCellFormat.DecimalPrecision2);
                    worksheet.Cells[row, column].Value = CheckGetValue(sectionValuePerNumericAttribute, "CBPATCCT");
                    ExcelHelper.SetCustomFormat(worksheet.Cells[row, column++], ExcelHelperCellFormat.DecimalPrecision2);
                    worksheet.Cells[row, column].Value = CheckGetValue(sectionValuePerNumericAttribute, "CBPATCSF");
                    // right border line
                    ExcelHelper.ApplyRightBorder(worksheet.Cells[row, column]);
                    ExcelHelper.SetCustomFormat(worksheet.Cells[row, column++], ExcelHelperCellFormat.DecimalPrecision2);
                    worksheet.Cells[row, column].Value = CheckGetValue(sectionValuePerNumericAttribute, "CPCCPACT");
                    ExcelHelper.SetCustomFormat(worksheet.Cells[row, column++], ExcelHelperCellFormat.DecimalPrecision2);
                    worksheet.Cells[row, column].Value = CheckGetValue(sectionValuePerNumericAttribute, "CPCCPASF");
                    // right border line
                    ExcelHelper.ApplyRightBorder(worksheet.Cells[row, column]);
                    ExcelHelper.SetCustomFormat(worksheet.Cells[row, column++], ExcelHelperCellFormat.DecimalPrecision2);
                    worksheet.Cells[row, column].Value = CheckGetValue(sectionValuePerNumericAttribute, "CLJCPRU1");
                    ExcelHelper.SetCustomFormat(worksheet.Cells[row, column++], ExcelHelperCellFormat.DecimalPrecision2);
                    worksheet.Cells[row, column].Value = CheckGetValue(sectionValuePerNumericAttribute, "CLJCPRU2");
                    ExcelHelper.SetCustomFormat(worksheet.Cells[row, column++], ExcelHelperCellFormat.DecimalPrecision2);
                    worksheet.Cells[row, column].Value = CheckGetValue(sectionValuePerNumericAttribute, "CLJCPRU3");
                    ExcelHelper.SetCustomFormat(worksheet.Cells[row, column++], ExcelHelperCellFormat.DecimalPrecision2);
                    worksheet.Cells[row, column].Value = CheckGetValue(sectionValuePerNumericAttribute, "CLJCPRU_Total");
                    // right border line
                    ExcelHelper.ApplyRightBorder(worksheet.Cells[row, column]);
                    ExcelHelper.SetCustomFormat(worksheet.Cells[row, column++], ExcelHelperCellFormat.DecimalPrecision2);
                    worksheet.Cells[row, column].Value = CheckGetValue(sectionValuePerNumericAttribute, "CRJCPRU1");
                    ExcelHelper.SetCustomFormat(worksheet.Cells[row, column++], ExcelHelperCellFormat.DecimalPrecision2);
                    worksheet.Cells[row, column].Value = CheckGetValue(sectionValuePerNumericAttribute, "CRJCPRU2");
                    ExcelHelper.SetCustomFormat(worksheet.Cells[row, column++], ExcelHelperCellFormat.DecimalPrecision2);
                    worksheet.Cells[row, column].Value = CheckGetValue(sectionValuePerNumericAttribute, "CRJCPRU3");
                    ExcelHelper.SetCustomFormat(worksheet.Cells[row, column++], ExcelHelperCellFormat.DecimalPrecision2);
                    worksheet.Cells[row, column].Value = CheckGetValue(sectionValuePerNumericAttribute, "CRJCPRU_Total");
                    // double right border line
                    ExcelHelper.ApplyRightBorder(worksheet.Cells[row, column]);
                    ExcelHelper.SetCustomFormat(worksheet.Cells[row, column++], ExcelHelperCellFormat.DecimalPrecision2);                    
                    worksheet.Column(column).Width = 3;
                    column++;
                }
            }
        }

        private static CurrentCell FillHeaders(ExcelWorksheet worksheet, List<int> simulationYears)
        {
            int headerRow = 1;
            const double minimumColumnWidth = 18;
            var headers = GetInitialHeaders();
            var currentCell = new CurrentCell { Row = headerRow, Column = headers.Count };
            for (int column = 0; column < headers.Count; column++)
            {                      
                if (column != headers.Count - 1)
                {
                    worksheet.Column(column + 1).Width = minimumColumnWidth;
                    worksheet.Cells[headerRow, column + 1].Value = headers[column];
                    ExcelHelper.MergeCells(worksheet, headerRow, column + 1, headerRow + 1, column + 1);                    
                }
                else
                {
                    worksheet.Column(column + 1).Width = minimumColumnWidth - 3;
                    worksheet.Cells[headerRow + 1, column + 1].Value = headers[column];
                }
                ExcelHelper.ApplyBorder(worksheet.Cells[headerRow, column + 1, headerRow + 1, column + 1]);                
            }

            currentCell = FillDynamicHeaders(worksheet, currentCell, simulationYears);

            return currentCell;
        }

        private static CurrentCell FillDynamicHeaders(ExcelWorksheet worksheet, CurrentCell currentCell, List<int> simulationYears)
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
                if (simulationYear == simulationYears[0])
                {
                    // For OPI
                    yearStartColumn--;
                    worksheet.Cells[row, yearStartColumn].Value = simulationYear;
                    worksheet.Cells[row + 1, yearStartColumn].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet.Cells[row + 1, yearStartColumn].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#70AD47"));
                }

                ExcelHelper.MergeCells(worksheet, row, yearStartColumn, row, column - 1);
                ExcelHelper.ApplyBorder(worksheet.Cells[row, yearStartColumn, row, column - 1]);
                worksheet.Cells[row, yearStartColumn, row, column - 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                currentCell.Column = column;
            }

            const double minimumColumnWidth = 15;
            for (int col = 1; col <= worksheet.Dimension.End.Column; col++)
            {
                if (worksheet.Column(col).Width < minimumColumnWidth)
                {
                    worksheet.Column(col).Width = minimumColumnWidth;
                }
            }
            currentCell.Row = row + 1;
            return currentCell;
        }

        private static int BuildDataSubHeaders(ExcelWorksheet worksheet, int row, int column, List<string> subHeaders, Color color)
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
            ExcelHelper.ApplyBorder(worksheet.Cells[row, startColumn, row, column]);
            if (color == Color.Black)
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
            "OPI",
        };

        private double CheckGetValue(Dictionary<string, double> valuePerNumericAttribute, string attribute) => _reportHelper.CheckAndGetValue<double>(valuePerNumericAttribute, attribute);

        private string CheckGetTextValue(Dictionary<string, string> valuePerTextAttribute, string attribute) => _reportHelper.CheckAndGetValue<string>(valuePerTextAttribute, attribute);
    }
}
