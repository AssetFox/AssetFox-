﻿using System;
using System.Collections.Generic;
using System.Drawing;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.ExcelHelpers;
using AppliedResearchAssociates.iAM.Reporting.Models;
using AppliedResearchAssociates.iAM.Reporting.Models.BAMSAuditReport;
using OfficeOpenXml;

namespace AppliedResearchAssociates.iAM.Reporting.Services.BAMSAuditReport
{
    public class BridgesTreatments
    {
        private ReportHelper _reportHelper;
        private readonly IUnitOfWork _unitOfWork;

        public BridgesTreatments(IUnitOfWork unitOfWork)
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

            // Merge rows for columns
            for (int cellColumn = 1; cellColumn <= headersRow.Count; cellColumn++)
            {
                ExcelHelper.MergeCells(worksheet, row, cellColumn, row + 1, cellColumn);
            }

            ExcelHelper.ApplyBorder(worksheet.Cells[headerRow, 1, headerRow + 1, worksheet.Dimension.Columns]);
            worksheet.Cells.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Bottom;

            return new CurrentCell { Row = 3, Column = headersRow.Count + 1 };
        }

        public void FillDataInWorksheet(ExcelWorksheet worksheet, CurrentCell currentCell, BridgeDataModel bridgeDataModel)
        {
            var row = currentCell.Row;
            var columnNo = currentCell.Column;
            var assetSummaryDetail = bridgeDataModel.AssetSummaryDetail;

            worksheet.Cells[row, columnNo++].Value = _reportHelper.CheckAndGetValue<string>(assetSummaryDetail.ValuePerTextAttribute, "BMSID");

            var latitude = _reportHelper.CheckAndGetValue<double>(assetSummaryDetail.ValuePerNumericAttribute, "LAT");
            var longitude = _reportHelper.CheckAndGetValue<double>(assetSummaryDetail.ValuePerNumericAttribute, "LONG");

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
            worksheet.Cells[row, columnNo++].Formula = $"HYPERLINK(\"https://www.google.com/maps/place/{lat_string},{long_string}/data=!3m1!1e3\", \"{bridgeDataModel.BRKey}\")";

            ExcelHelper.HorizontalCenterAlign(worksheet.Cells[row, columnNo]);
            var district_string = _reportHelper.CheckAndGetValue<string>(assetSummaryDetail.ValuePerTextAttribute, "DISTRICT");
            if (int.TryParse(district_string, out var district_int))
            {
                worksheet.Cells[row, columnNo++].Value = district_int;
            }
            else
            {
                worksheet.Cells[row, columnNo++].Value = district_string;
            }

            worksheet.Cells[row, columnNo++].Value = _reportHelper.CheckAndGetValue<string>(assetSummaryDetail.ValuePerTextAttribute, "COUNTY");
            worksheet.Cells[row, columnNo++].Value = _reportHelper.CheckAndGetValue<string>(assetSummaryDetail.ValuePerTextAttribute, "MPO_NAME");

            worksheet.Cells[row, columnNo].Style.Numberformat.Format = "###,###,###,###,##0";
            worksheet.Cells[row, columnNo++].Value = _reportHelper.CheckAndGetValue<double>(assetSummaryDetail.ValuePerNumericAttribute, "LENGTH");

            worksheet.Cells[row, columnNo].Style.Numberformat.Format = "###,###,###,###,##0";
            var deckArea = _reportHelper.CheckAndGetValue<double>(assetSummaryDetail.ValuePerNumericAttribute, "DECK_AREA");
            worksheet.Cells[row, columnNo++].Value = deckArea;

            ExcelHelper.HorizontalCenterAlign(worksheet.Cells[row, columnNo]);
            var family = _reportHelper.CheckAndGetValue<string>(assetSummaryDetail.ValuePerTextAttribute, "FAMILY_ID");
            worksheet.Cells[row, columnNo++].Value = int.TryParse(family, out var familyNumber) ? familyNumber : family;

            worksheet.Cells[row, columnNo++].Value = _reportHelper.CheckAndGetValue<string>(assetSummaryDetail.ValuePerTextAttribute, "STRUCTURE_TYPE");

            var functionalClassAbbr = _reportHelper.CheckAndGetValue<string>(assetSummaryDetail.ValuePerTextAttribute, "FUNC_CLASS");
            var functionalClassDescription = _reportHelper.FullFunctionalClassDescription(functionalClassAbbr);
            worksheet.Cells[row, columnNo++].Value = functionalClassDescription;

            ExcelHelper.HorizontalCenterAlign(worksheet.Cells[row, columnNo]);
            var bpn_string = _reportHelper.CheckAndGetValue<string>(assetSummaryDetail.ValuePerTextAttribute, "BUS_PLAN_NETWORK");
            if (int.TryParse(bpn_string, out var bpn_int))
            {
                worksheet.Cells[row, columnNo++].Value = bpn_int;
            }
            else
            {
                worksheet.Cells[row, columnNo++].Value = bpn_string;
            }
            ExcelHelper.HorizontalCenterAlign(worksheet.Cells[row, columnNo]);
            worksheet.Cells[row, columnNo++].Value = _reportHelper.CheckAndGetValue<string>(assetSummaryDetail.ValuePerTextAttribute, "NHS_IND") == "0" ? BAMSAuditReportConstants.No : BAMSAuditReportConstants.Yes;
            ExcelHelper.HorizontalCenterAlign(worksheet.Cells[row, columnNo]);
            worksheet.Cells[row, columnNo++].Value = _reportHelper.CheckAndGetValue<string>(assetSummaryDetail.ValuePerTextAttribute, "INTERSTATE");
            worksheet.Cells[row, columnNo].Style.Numberformat.Format = "###,###,###,###,##0";
            worksheet.Cells[row, columnNo++].Value = _reportHelper.CheckAndGetValue<double>(assetSummaryDetail.ValuePerNumericAttribute, "RISK_SCORE");

            if (row % 2 == 0)
            {
                ExcelHelper.ApplyColor(worksheet.Cells[row, 1, row, columnNo - 1], Color.LightGray);
            }
            ExcelHelper.ApplyBorder(worksheet.Cells[row, 1, row, columnNo - 1]);

            currentCell.Column = columnNo;
        }

        private const string INTERSTATE = "Interstate";
        private static List<string> GetHeadersRow()
        {
            return new List<string>
            {
                "BridgeID",
                "BRKey",
                "District",
                "County",
                "MPO/RPO",
                "Bridge\r\nLength",
                "Deck\r\nArea",
                "Family",
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
        }
    }
}
