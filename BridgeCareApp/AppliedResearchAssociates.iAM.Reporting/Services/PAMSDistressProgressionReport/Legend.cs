using System;
using System.Collections.Generic;
using AppliedResearchAssociates.iAM.ExcelHelpers;
using AppliedResearchAssociates.iAM.Reporting.Models;
using OfficeOpenXml;

namespace AppliedResearchAssociates.iAM.Reporting.Services.PAMSDistressProgressionReport
{
    public class Legend
    {
        public static void Fill(ExcelWorksheet worksheet)
        {            
            var currentCell = FillHeaders(worksheet);
            FillData(worksheet, currentCell);
        }

        private static void FillData(ExcelWorksheet worksheet, CurrentCell currentCell)
        {
            var startColumn = 1;
            var initialLegendData = GetInitialLegendData();
            FillLegendDetails(worksheet, currentCell, initialLegendData);
            currentCell.Row++;
            var partOneLegendData = GetPartOneLegendData();
            FillLegendDetails(worksheet, currentCell, partOneLegendData);
            currentCell.Row++;

            worksheet.Cells[++currentCell.Row, startColumn].Value = "**BITUMINOUS CONDITIONS**";
            ExcelHelper.MergeCells(worksheet, currentCell.Row, startColumn, currentCell.Row, startColumn + 1, false);
            var bituminousLegendData = GetBituminousLegendData();
            FillLegendDetails(worksheet, currentCell, bituminousLegendData);
            currentCell.Row++;

            worksheet.Cells[++currentCell.Row, startColumn].Value = "**CONCRETE CONDITIONS**";
            ExcelHelper.MergeCells(worksheet, currentCell.Row, startColumn, currentCell.Row, startColumn + 1, false);
            var concreteLegendData = GetConcreteLegendData();
            FillLegendDetails(worksheet, currentCell, concreteLegendData);

            var startCell = worksheet.Cells.Start;
            worksheet.Cells[startCell.Row, startCell.Column, currentCell.Row, currentCell.Column].AutoFitColumns();
        }        

        private static void FillLegendDetails(ExcelWorksheet worksheet, CurrentCell currentCell, List<Tuple<string, string>> initialLegendData)
        {
            var row = currentCell.Row;            
            foreach(var tuple in initialLegendData)
            {
                var column = 1;
                worksheet.Cells[++row, column++].Value = tuple.Item1;
                worksheet.Cells[row, column].Value = tuple.Item2;
            }

            currentCell.Row = row;
        }

        private static CurrentCell FillHeaders(ExcelWorksheet worksheet)
        {
            int headerRow = 1;
            int startColumn = 1;
            var column = startColumn;
            worksheet.Cells[headerRow, column++].Value = "COLUMN NAME";
            worksheet.Cells[headerRow, column].Value = "DESCRIPTION";
            ExcelHelper.ApplyBorder(worksheet.Cells[headerRow, startColumn, headerRow, column]);
            ExcelHelper.ApplyStyle(worksheet.Cells[headerRow, startColumn, headerRow, column]);

            return new CurrentCell { Row = headerRow, Column = column };
        }

        private static List<Tuple<string, string>> GetConcreteLegendData() =>
            // list of columnName and Description pairs
            new() {
                new Tuple<string, string>("CJOINTCT", "CONCRETE JOINT COUNT"),
                new Tuple<string, string>("CNSLABCT", "CONCRETE SLAB COUNT"),
                new Tuple<string, string>("CFLTJNT2", "CONCRETE FAULTED JOINT COUNT - M"),
                new Tuple<string, string>("CFLTJNT3", "CONCRETE FAULTED JOINT COUNT - H"),
                new Tuple<string, string>("CBRKSLB1", "CONCRETE BROKEN SLAB COUNT - L"),
                new Tuple<string, string>("CBRKSLB2", "CONCRETE BROKEN SLAB COUNT - M"),
                new Tuple<string, string>("CBRKSLB3", "CONCRETE BROKEN SLAB COUNT - H"),
                new Tuple<string, string>("CTRNCRK1", "CONCRETE TRANS. CRACK COUNT - L"),
                new Tuple<string, string>("CTRNCRK2", "CONCRETE TRANS. CRACK COUNT - M"),
                new Tuple<string, string>("CTRNCRK3", "CONCRETE TRANS. CRACK COUNT - H"),
                new Tuple<string, string>("CTRNJNT1", "CONCRETE TRANS. JOINT SPALL COUNT - L"),
                new Tuple<string, string>("CTRNJNT2", "CONCRETE TRANS. JOINT SPALL COUNT - M"),
                new Tuple<string, string>("CTRNJNT3", "CONCRETE TRANS. JOINT SPALL COUNT - H"),
                new Tuple<string, string>("CLNGCRK1", "CONCRETE LONG. CRACK COUNT - L"),
                new Tuple<string, string>("CLNGCRK2", "CONCRETE LONG. CRACK COUNT - M"),
                new Tuple<string, string>("CLNGCRK3", "CONCRETE LONG. CRACK COUNT - H"),
                new Tuple<string, string>("CLNGJNT1", "CONCRETE LONG. JOINT SPALL LENGTH - L"),
                new Tuple<string, string>("CLNGJNT2", "CONCRETE LONG. JOINT SPALL LENGTH - M"),
                new Tuple<string, string>("CLNGJNT3", "CONCRETE LONG. JOINT SPALL LENGTH - H"),
                new Tuple<string, string>("CBPATCCT", "CONCRETE BITUMINIOUS PATCH COUNT"),
                new Tuple<string, string>("CBPATCSF", "CONCRETE BITUMINIOUS PATCH AREA (SF)"),
                new Tuple<string, string>("CPCCPACT", "CONCRETE PCC PATCH COUNT"),
                new Tuple<string, string>("CPCCPASF", "CONCRETE PCC PATCH AREA (SF)"),
                new Tuple<string, string>("CLJCPRU1", "CONCRETE LEFT JCP RUTTING - L"),
                new Tuple<string, string>("CLJCPRU2", "CONCRETE LEFT JCP RUTTING - M"),
                new Tuple<string, string>("CLJCPRU3", "CONCRETE LEFT JCP RUTTING - H"),
                new Tuple<string, string>("CRJCPRU1", "CONCRETE RIGHT JCP RUTTING - L"),
                new Tuple<string, string>("CRJCPRU2", "CONCRETE RIGHT JCP RUTTING - M"),
                new Tuple<string, string>("CRJCPRU3", "CONCRETE RIGHT JCP RUTTING - H"),
            };

        private static List<Tuple<string, string>> GetBituminousLegendData() =>
            // list of columnName and Description pairs
            new() {
                new Tuple<string, string>("BLRUTDP1", "BITUMINIOUS RUTTING LEFT - L"),
                new Tuple<string, string>("BLRUTDP2", "BITUMINIOUS RUTTING LEFT - M"),
                new Tuple<string, string>("BLRUTDP3", "BITUMINIOUS RUTTING LEFT - H"),
                new Tuple<string, string>("BRRUTDP1", "BITUMINIOUS RUTTING RIGHT - L"),
                new Tuple<string, string>("BRRUTDP2", "BITUMINIOUS RUTTING RIGHT - M"),
                new Tuple<string, string>("BRRUTDP3", "BITUMINIOUS RUTTING RIGHT - H"),
                new Tuple<string, string>("BFATICR1", "BITUMINIOUS FATIGUE CRACK - L"),
                new Tuple<string, string>("BFATICR2", "BITUMINIOUS FATIGUE CRACK - M"),
                new Tuple<string, string>("BFATICR3", "BITUMINIOUS FATIGUE CRACK - H"),
                new Tuple<string, string>("BTRNSCT1", "BITUMINIOUS TRANS. CRACK COUNT - L"),
                new Tuple<string, string>("BTRNSFT1", "BITUMINIOUS TRANS. CRACK LENGTH - L"),
                new Tuple<string, string>("BTRNSCT2", "BITUMINIOUS TRANS. CRACK COUNT - M"),
                new Tuple<string, string>("BTRNSFT2", "BITUMINIOUS TRANS. CRACK LENGTH - M"),
                new Tuple<string, string>("BTRNSCT3", "BITUMINIOUS TRANS. CRACK COUNT - H"),
                new Tuple<string, string>("BTRNSFT3", "BITUMINIOUS TRANS. CRACK LENGTH - H"),
                new Tuple<string, string>("BMISCCK1", "BITUMINIOUS MISCELLANEOUS CRACK - L"),
                new Tuple<string, string>("BMISCCK2", "BITUMINIOUS MISCELLANEOUS CRACK - M"),
                new Tuple<string, string>("BMISCCK3", "BITUMINIOUS MISCELLANEOUS CRACK - H"),
                new Tuple<string, string>("BEDGDTR1", "BITUMINIOUS EDGE DETER. LENGTH - L"),
                new Tuple<string, string>("BEDGDTR2", "BITUMINIOUS EDGE DETER. LENGTH - M"),
                new Tuple<string, string>("BEDGDTR3", "BITUMINIOUS EDGE DETER. LENGTH - H"),
                new Tuple<string, string>("BPATCHCT", "BITUMINIOUS PATCHING COUNT"),
                new Tuple<string, string>("BPATCHSF", "BITUMINIOUS PATCHING AREA (SF)"),
                new Tuple<string, string>("BRAVLWT2", "BITUMINIOUS RAVEL./WEATHER. AREA - M"),
                new Tuple<string, string>("BRAVLWT3", "BITUMINIOUS RAVEL./WEATHER. AREA - H"),
                new Tuple<string, string>("BRAVLWT2", "BITUMINIOUS RAVEL./WEATHER. FEET - M"),
                new Tuple<string, string>("BRAVLWT3", "BITUMINIOUS RAVEL./WEATHER. FEET - H"),
                new Tuple<string, string>("BLTEDGE1", "BITUMINIOUS LEFT EDGE JOINT - L"),
                new Tuple<string, string>("BLTEDGE2", "BITUMINIOUS LEFT EDGE JOINT - M"),
                new Tuple<string, string>("BLTEDGE3", "BITUMINIOUS LEFT EDGE JOINT - H"),
            };

        private static List<Tuple<string, string>> GetPartOneLegendData() =>
            // list of columnName and Description pairs
            new() {
                new Tuple<string, string>("AVERAGE OPI", "AVERAGE OPI OF ALL SEGEMENTS"),
                new Tuple<string, string>("CALCULATED OPI", "OPI CALCULATED ON PROJECTED INDIVIDUAL DISTRESSES"),
                new Tuple<string, string>("AVERAGE ROUGHNESS", "AVERAGE ROUGHNESS OF ALL SEGEMENTS"),
            };

        private static List<Tuple<string, string>> GetInitialLegendData() =>
            // list of columnName and Description pairs
            new() {
                new Tuple<string, string>("CONDITION YEAR", "CONDITION SURVEY YEAR"),
                new Tuple<string, string>("CRS", "COUNTY, ROUTE, SEGMENTS"),
                new Tuple<string, string>("DISTRICT", ""),
                new Tuple<string, string>("COUNTY", ""),
                new Tuple<string, string>("STATE ROUTE", ""),
                new Tuple<string, string>("SEGMENT START", "FIRST SEGMENT IN CRS SECTION"),
                new Tuple<string, string>("SEGMENT END", "LAST SEGMENT IN CRS SECTION"),
                new Tuple<string, string>("DIRECTION", ""),
                new Tuple<string, string>("LENGTH (FT)", "TOTAL LENGTH, IN FEET, OF ALL SEGEMENTS"),
                new Tuple<string, string>("WIDTH (FT)", "TOTAL WIDTH, IN FEET, OF ALL SEGEMENTS"),
                new Tuple<string, string>("BPN", "BUSINESS PLAN NETWORK"),
                new Tuple<string, string>("PAVEMENT SURFACE TYPE", ""),
            };
    }
}
