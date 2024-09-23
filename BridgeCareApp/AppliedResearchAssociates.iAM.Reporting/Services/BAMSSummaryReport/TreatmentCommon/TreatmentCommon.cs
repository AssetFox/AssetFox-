using System;
using System.Collections.Generic;
using System.Drawing;
using AppliedResearchAssociates.iAM.Analysis.Engine;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.ExcelHelpers;
using AppliedResearchAssociates.iAM.Reporting.Models;
using OfficeOpenXml;

namespace AppliedResearchAssociates.iAM.Reporting.Services.BAMSSummaryReport
{
    public class TreatmentCommon
    {
        private ReportHelper _reportHelper;
        private readonly IUnitOfWork _unitOfWork;

        public TreatmentCommon(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _reportHelper = new ReportHelper(_unitOfWork);
        }

        public CurrentCell AddHeadersCells(ExcelWorksheet worksheet)
        {
            // Row 1
            int headerRow = 1;
            var headersRow = GetHeadersRow();

            worksheet.Cells.Style.WrapText = false;

            // Add all Row 1 headers
            for (int column = 0; column < headersRow.Count; column++)
            {
                worksheet.Cells[headerRow, column + 1].Value = headersRow[column];
            }

            var row = headerRow;

            worksheet.Row(row).Height = 15;
            worksheet.Row(row + 1).Height = 15;
            // Autofit before the merges
            worksheet.Cells.AutoFitColumns(0);
            worksheet.Cells.AutoFitColumns(5);

            // Merge rows for columns
            for (int cellColumn = 1; cellColumn <= headersRow.Count; cellColumn++)
            {
                ExcelHelper.MergeCells(worksheet, row, cellColumn, row + 1, cellColumn);
            }

            ExcelHelper.ApplyBorder(worksheet.Cells[headerRow, 1, headerRow + 1, worksheet.Dimension.Columns]);
            //ExcelHelper.ApplyStyle(worksheet.Cells[headerRow + 1, bridgeFundingColumn, headerRow + 1, analysisColumn - 1]);
            worksheet.Cells.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Bottom;

            return new CurrentCell { Row = 3, Column = headersRow.Count + 1 };
        }

        public void FillDataInWorkSheet(ExcelWorksheet worksheet, CurrentCell currentCell, AssetDetail section, int Year)
        {
            var row = currentCell.Row;
            var columnNo = currentCell.Column;

            worksheet.Cells[row, columnNo++].Value = _reportHelper.CheckAndGetValue<string>(section.ValuePerTextAttribute, "BMSID");

            var latitude = _reportHelper.CheckAndGetValue<double>(section.ValuePerNumericAttribute, "LAT");
            var longitude = _reportHelper.CheckAndGetValue<double>(section.ValuePerNumericAttribute, "LONG");

            // LAT and LONG appear to be in Degree/Minute/Second form, but concatenated into a single number without delimiters.

            var lat_degrees = Math.Floor(latitude / 10_000);
            var lat_minutes = Math.Floor((latitude - 10_000 * lat_degrees) / 100);
            var lat_seconds = latitude - 10_000 * lat_degrees - 100 * lat_minutes;

            var long_degrees = Math.Floor(longitude / 10_000);
            var long_minutes = Math.Floor((longitude - 10_000 * long_degrees) / 100);
            var long_seconds = longitude - 10_000 * long_degrees - 100 * long_minutes;

            // The "s are doubled up here so that they will be properly escaped in the excel formula.
            var lat_string = $"{lat_degrees}°{lat_minutes}'{lat_seconds}\"\"N";
            var long_string = $"{long_degrees}°{long_minutes}'{long_seconds}\"\"W";

            worksheet.Cells[row, columnNo].Style.Font.UnderLine = true;
            worksheet.Cells[row, columnNo].Style.Font.Color.SetColor(Color.Blue);
            ExcelHelper.HorizontalCenterAlign(worksheet.Cells[row, columnNo]);
            var key = _reportHelper.CheckAndGetValue<double>(section.ValuePerNumericAttribute, "BRKEY_");
            worksheet.Cells[row, columnNo++].Formula = $"HYPERLINK(\"https://www.google.com/maps/place/{lat_string},{long_string}/data=!3m1!1e3\", \"{key}\")";

            ExcelHelper.HorizontalCenterAlign(worksheet.Cells[row, columnNo]);
            var district_string = _reportHelper.CheckAndGetValue<string>(section.ValuePerTextAttribute, "DISTRICT");
            if (int.TryParse(district_string, out var district_int))
            {
                worksheet.Cells[row, columnNo++].Value = district_int;
            }
            else
            {
                worksheet.Cells[row, columnNo++].Value = district_string;
            }

            worksheet.Cells[row, columnNo++].Value = _reportHelper.CheckAndGetValue<string>(section.ValuePerTextAttribute, "COUNTY");
            worksheet.Cells[row, columnNo++].Value = _reportHelper.CheckAndGetValue<string>(section.ValuePerTextAttribute, "MPO_NAME");

            worksheet.Cells[row, columnNo++].Value = "";
            worksheet.Cells[row, columnNo++].Value = _reportHelper.CheckAndGetValue<string>(section.ValuePerTextAttribute, "FEATURE_INTERSECTED");
            worksheet.Cells[row, columnNo++].Value = _reportHelper.CheckAndGetValue<string>(section.ValuePerTextAttribute, "FEATURE_CARRIED");
            worksheet.Cells[row, columnNo++].Value = _reportHelper.CheckAndGetValue<string>(section.ValuePerTextAttribute, "LOCATION");

            worksheet.Cells[row, columnNo].Style.Numberformat.Format = "###,###,###,###,##0";
            worksheet.Cells[row, columnNo++].Value = _reportHelper.CheckAndGetValue<double>(section.ValuePerNumericAttribute, "LENGTH");

            worksheet.Cells[row, columnNo].Style.Numberformat.Format = "###,###,###,###,##0";
            var deckArea = _reportHelper.CheckAndGetValue<double>(section.ValuePerNumericAttribute, "DECK_AREA");
            worksheet.Cells[row, columnNo++].Value = deckArea;

            ExcelHelper.HorizontalCenterAlign(worksheet.Cells[row, columnNo]);
            worksheet.Cells[row, columnNo++].Value = deckArea >= 28500 ? BAMSConstants.Yes : BAMSConstants.No; // Large Bridge

            worksheet.Cells[row, columnNo++].Value = _reportHelper.CheckAndGetValue<string>(section.ValuePerTextAttribute, "STRUCTURE_TYPE");

            var functionalClassAbbr = _reportHelper.CheckAndGetValue<string>(section.ValuePerTextAttribute, "FUNC_CLASS");
            var functionalClassDescription = _reportHelper.FullFunctionalClassDescription(functionalClassAbbr);
            worksheet.Cells[row, columnNo++].Value = functionalClassDescription;

            ExcelHelper.HorizontalCenterAlign(worksheet.Cells[row, columnNo]);
            var bpn_string = _reportHelper.CheckAndGetValue<string>(section.ValuePerTextAttribute, "BUS_PLAN_NETWORK");
            if (int.TryParse(bpn_string, out var bpn_int))
            {
                worksheet.Cells[row, columnNo++].Value = bpn_int;
            }
            else
            {
                worksheet.Cells[row, columnNo++].Value = bpn_string;
            }
            ExcelHelper.HorizontalCenterAlign(worksheet.Cells[row, columnNo]);
            worksheet.Cells[row, columnNo++].Value = _reportHelper.CheckAndGetValue<string>(section.ValuePerTextAttribute, "NHS_IND") == "0" ? "N" : "Y";
            ExcelHelper.HorizontalCenterAlign(worksheet.Cells[row, columnNo]);
            worksheet.Cells[row, columnNo++].Value = _reportHelper.CheckAndGetValue<string>(section.ValuePerTextAttribute, "INTERSTATE");

            worksheet.Cells[row, columnNo].Style.Numberformat.Format = "###,###,###,###,##0";
            worksheet.Cells[row, columnNo++].Value = _reportHelper.CheckAndGetValue<double>(section.ValuePerNumericAttribute, "RISK_SCORE");

            if (row % 2 == 0)
            {
                ExcelHelper.ApplyColor(worksheet.Cells[row, 1, row, columnNo - 1], Color.LightGray);
            }
            ExcelHelper.ApplyBorder(worksheet.Cells[row, 1, row, columnNo - 1]);

            currentCell.Column = columnNo;
        }

        private const string INTERSTATE = "Interstate";
        private const string BRKEY = "BRKey\r\n(Hyperlink)";

        private static List<string> GetHeadersRow()
        {
            return new List<string>
            {
                "BridgeID",
                BRKEY,
                "District",
                "County",
                "MPO/RPO",
                "City/Town/Place",
                "Feature\r\nintersected",
                "Facility\r\nCarried",
                "Location/StructureName",
                "Bridge\r\nLength",
                "Deck\r\nArea",
                "Large\r\nBridge",
                "Structure\r\nType",
                "Functional\r\nClass",
                "BPN",
                "NHS",
                INTERSTATE,
                "Risk\r\nScore"
            };
        }

        public void PerformPostAutofitAdjustments(ExcelWorksheet worksheet)
        {
            var columnNumber = GetHeadersRow().IndexOf(INTERSTATE) + 1;
            worksheet.Column(columnNumber).SetTrueWidth(9);

            columnNumber = GetHeadersRow().IndexOf(BRKEY) + 1;
            worksheet.Column(columnNumber).SetTrueWidth(10);
        }
    }
}
