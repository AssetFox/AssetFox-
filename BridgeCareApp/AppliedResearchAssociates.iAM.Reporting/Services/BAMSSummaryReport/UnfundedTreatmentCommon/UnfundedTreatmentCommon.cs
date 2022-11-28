using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

using OfficeOpenXml;

using AppliedResearchAssociates.iAM.Analysis.Engine;
using AppliedResearchAssociates.iAM.Reporting.Interfaces.BAMSSummaryReport;
using AppliedResearchAssociates.iAM.Reporting.Models.BAMSSummaryReport;
using AppliedResearchAssociates.iAM.ExcelHelpers;

namespace AppliedResearchAssociates.iAM.Reporting.Services.BAMSSummaryReport.UnfundedTreatmentCommon
{
    public class UnfundedTreatmentCommon : IUnfundedTreatmentCommon
    {
        private ISummaryReportHelper _summaryReportHelper;

        public UnfundedTreatmentCommon()
        {
            _summaryReportHelper = new SummaryReportHelper();
        }

        public void FillDataInWorkSheet(ExcelWorksheet worksheet, CurrentCell currentCell, AssetDetail section, int Year, TreatmentOptionDetail treatment)
        {
            var row = currentCell.Row;
            var columnNo = currentCell.Column;

            worksheet.Cells[row, columnNo++].Value = _summaryReportHelper.checkAndGetValue<string>(section.ValuePerTextAttribute, "BMSID");

            var latitude = _summaryReportHelper.checkAndGetValue<double>(section.ValuePerNumericAttribute, "LAT");
            var longitude = _summaryReportHelper.checkAndGetValue<double>(section.ValuePerNumericAttribute, "LONG");

            // LAT and LONG appear to be in Degree/Minute/Second form, but concatenated into a single number without delimiters.

            var lat_degrees = Math.Floor(latitude / 10_000);
            var lat_minutes = Math.Floor((latitude - 10_000 * lat_degrees) / 100);
            var lat_seconds = latitude - 10_000 * lat_degrees - 100 * lat_minutes;

            var long_degrees = Math.Floor(longitude / 10_000);
            var long_minutes = Math.Floor((longitude - 10_000 * long_degrees) / 100);
            var long_seconds = longitude - 10_000 * long_degrees - 100 * long_minutes;

            var lat_string = $"{lat_degrees}°{lat_minutes}'{lat_seconds}\"N";
            var long_string = $"{long_degrees}°{long_minutes}'{long_seconds}\"W";

            worksheet.Cells[row, columnNo].Hyperlink = new Uri($"https://www.google.com/maps/place/{lat_string},{long_string}/data=!3m1!1e3", UriKind.Absolute);
            worksheet.Cells[row, columnNo].Style.Font.UnderLine = true;
            worksheet.Cells[row, columnNo].Style.Font.Color.SetColor(Color.Blue);
            ExcelHelper.HorizontalCenterAlign(worksheet.Cells[row, columnNo]);
            worksheet.Cells[row, columnNo++].Value = _summaryReportHelper.checkAndGetValue<double>(section.ValuePerNumericAttribute, "BRKEY_");

            ExcelHelper.HorizontalCenterAlign(worksheet.Cells[row, columnNo]);
            worksheet.Cells[row, columnNo++].Value = _summaryReportHelper.checkAndGetValue<string>(section.ValuePerTextAttribute, "DISTRICT");
            worksheet.Cells[row, columnNo++].Value = _summaryReportHelper.checkAndGetValue<string>(section.ValuePerTextAttribute, "COUNTY");
            worksheet.Cells[row, columnNo++].Value = _summaryReportHelper.checkAndGetValue<string>(section.ValuePerTextAttribute, "MPO_NAME");

            worksheet.Cells[row, columnNo].Style.Numberformat.Format = "###,###,###,###,##0";
            worksheet.Cells[row, columnNo++].Value = _summaryReportHelper.checkAndGetValue<double>(section.ValuePerNumericAttribute, "LENGTH");

            worksheet.Cells[row, columnNo].Style.Numberformat.Format = "###,###,###,###,##0";
            var deckArea = _summaryReportHelper.checkAndGetValue<double>(section.ValuePerNumericAttribute, "DECK_AREA");
            worksheet.Cells[row, columnNo++].Value = deckArea;

            ExcelHelper.HorizontalCenterAlign(worksheet.Cells[row, columnNo]);
            worksheet.Cells[row, columnNo++].Value = deckArea >= 28500 ? BAMSConstants.Yes : BAMSConstants.No; // Large Bridge

            worksheet.Cells[row, columnNo++].Value = _summaryReportHelper.checkAndGetValue<string>(section.ValuePerTextAttribute, "STRUCTURE_TYPE");

            var functionalClassAbbr = _summaryReportHelper.checkAndGetValue<string>(section.ValuePerTextAttribute, "FUNC_CLASS");
            var functionalClassDescription = _summaryReportHelper.FullFunctionalClassDescription(functionalClassAbbr);
            worksheet.Cells[row, columnNo++].Value = functionalClassDescription;

            ExcelHelper.HorizontalCenterAlign(worksheet.Cells[row, columnNo]);
            worksheet.Cells[row, columnNo++].Value = _summaryReportHelper.checkAndGetValue<string>(section.ValuePerTextAttribute, "BUS_PLAN_NETWORK");
            ExcelHelper.HorizontalCenterAlign(worksheet.Cells[row, columnNo]);
            worksheet.Cells[row, columnNo++].Value = _summaryReportHelper.checkAndGetValue<string>(section.ValuePerTextAttribute, "NHS_IND") == "0" ? "N" : "Y";
            ExcelHelper.HorizontalCenterAlign(worksheet.Cells[row, columnNo]);
            worksheet.Cells[row, columnNo++].Value = _summaryReportHelper.checkAndGetValue<string>(section.ValuePerTextAttribute, "INTERSTATE");

            worksheet.Cells[row, columnNo].Style.Numberformat.Format = "###,###,###,###,##0";
            worksheet.Cells[row, columnNo++].Value = _summaryReportHelper.checkAndGetValue<double>(section.ValuePerNumericAttribute, "RISK_SCORE");


            ExcelHelper.HorizontalCenterAlign(worksheet.Cells[row, columnNo]);
            worksheet.Cells[row, columnNo++].Value = _summaryReportHelper.BridgeFundingNHPP(section) ? BAMSConstants.Yes : BAMSConstants.No;
            ExcelHelper.HorizontalCenterAlign(worksheet.Cells[row, columnNo]);
            worksheet.Cells[row, columnNo++].Value = _summaryReportHelper.BridgeFundingSTP(section) ? BAMSConstants.Yes : BAMSConstants.No;
            ExcelHelper.HorizontalCenterAlign(worksheet.Cells[row, columnNo]);
            worksheet.Cells[row, columnNo++].Value = _summaryReportHelper.BridgeFundingBOF(section) ? BAMSConstants.Yes : BAMSConstants.No;
            ExcelHelper.HorizontalCenterAlign(worksheet.Cells[row, columnNo]);
            worksheet.Cells[row, columnNo++].Value = _summaryReportHelper.BridgeFundingBRIP(section) ? BAMSConstants.Yes : BAMSConstants.No;
            ExcelHelper.HorizontalCenterAlign(worksheet.Cells[row, columnNo]);
            worksheet.Cells[row, columnNo++].Value = _summaryReportHelper.BridgeFundingState(section) ? BAMSConstants.Yes : BAMSConstants.No;
            ExcelHelper.HorizontalCenterAlign(worksheet.Cells[row, columnNo]);
            worksheet.Cells[row, columnNo++].Value = _summaryReportHelper.BridgeFundingNotApplicable(section) ? BAMSConstants.Yes : BAMSConstants.No;

            ExcelHelper.HorizontalCenterAlign(worksheet.Cells[row, columnNo]);
            worksheet.Cells[row, columnNo++].Value = Year;

            var familyId = int.Parse(_summaryReportHelper.checkAndGetValue<string>(section.ValuePerTextAttribute, "FAMILY_ID"));
            if (familyId < 11)
            {
                ExcelHelper.HorizontalCenterAlign(worksheet.Cells[row, columnNo]);
                worksheet.Cells[row, columnNo].Style.Numberformat.Format = "0.000";
                worksheet.Cells[row, columnNo++].Value = _summaryReportHelper.checkAndGetValue<double>(section.ValuePerNumericAttribute, "DECK_SEEDED");
                ExcelHelper.HorizontalCenterAlign(worksheet.Cells[row, columnNo]);
                worksheet.Cells[row, columnNo].Style.Numberformat.Format = "0.000";
                worksheet.Cells[row, columnNo++].Value = _summaryReportHelper.checkAndGetValue<double>(section.ValuePerNumericAttribute, "SUB_SEEDED");
                ExcelHelper.HorizontalCenterAlign(worksheet.Cells[row, columnNo]);
                worksheet.Cells[row, columnNo].Style.Numberformat.Format = "0.000";
                worksheet.Cells[row, columnNo++].Value = _summaryReportHelper.checkAndGetValue<double>(section.ValuePerNumericAttribute, "SUB_SEEDED");

                ExcelHelper.HorizontalCenterAlign(worksheet.Cells[row, columnNo]);
                worksheet.Cells[row, columnNo].Style.Numberformat.Format = "0.000";
                worksheet.Cells[row, columnNo++].Value = "N"; // CULV_SEEDED

                ExcelHelper.HorizontalCenterAlign(worksheet.Cells[row, columnNo]);
                worksheet.Cells[row, columnNo].Style.Numberformat.Format = "0";
                worksheet.Cells[row, columnNo++].Value = _summaryReportHelper.checkAndGetValue<double>(section.ValuePerNumericAttribute, "DECK_DURATION_N");
                ExcelHelper.HorizontalCenterAlign(worksheet.Cells[row, columnNo]);
                worksheet.Cells[row, columnNo].Style.Numberformat.Format = "0";
                worksheet.Cells[row, columnNo++].Value = _summaryReportHelper.checkAndGetValue<double>(section.ValuePerNumericAttribute, "SUB_DURATION_N");
                ExcelHelper.HorizontalCenterAlign(worksheet.Cells[row, columnNo]);
                worksheet.Cells[row, columnNo].Style.Numberformat.Format = "0";
                worksheet.Cells[row, columnNo++].Value = _summaryReportHelper.checkAndGetValue<double>(section.ValuePerNumericAttribute, "SUB_DURATION_N");

                ExcelHelper.HorizontalCenterAlign(worksheet.Cells[row, columnNo]);
                worksheet.Cells[row, columnNo].Style.Numberformat.Format = "0";
                worksheet.Cells[row, columnNo++].Value = "N"; // CULV_DURATION_N
            }
            else
            {
                ExcelHelper.HorizontalCenterAlign(worksheet.Cells[row, columnNo]);
                worksheet.Cells[row, columnNo].Style.Numberformat.Format = "0.000";
                worksheet.Cells[row, columnNo++].Value = "N"; // DECK_SEEDED
                ExcelHelper.HorizontalCenterAlign(worksheet.Cells[row, columnNo]);
                worksheet.Cells[row, columnNo].Style.Numberformat.Format = "0.000";
                worksheet.Cells[row, columnNo++].Value = "N"; // SUP_SEEDED
                ExcelHelper.HorizontalCenterAlign(worksheet.Cells[row, columnNo]);
                worksheet.Cells[row, columnNo].Style.Numberformat.Format = "0.000";
                worksheet.Cells[row, columnNo++].Value = "N"; // SUB_SEEDED

                ExcelHelper.HorizontalCenterAlign(worksheet.Cells[row, columnNo]);
                worksheet.Cells[row, columnNo].Style.Numberformat.Format = "0.000";
                worksheet.Cells[row, columnNo++].Value = section.ValuePerNumericAttribute["CULV_SEEDED"];

                ExcelHelper.HorizontalCenterAlign(worksheet.Cells[row, columnNo]);
                worksheet.Cells[row, columnNo].Style.Numberformat.Format = "0";
                worksheet.Cells[row, columnNo++].Value = "N"; // DECK_DURATION_N
                ExcelHelper.HorizontalCenterAlign(worksheet.Cells[row, columnNo]);
                worksheet.Cells[row, columnNo].Style.Numberformat.Format = "0";
                worksheet.Cells[row, columnNo++].Value = "N"; // SUP_DURATION_N
                ExcelHelper.HorizontalCenterAlign(worksheet.Cells[row, columnNo]);
                worksheet.Cells[row, columnNo].Style.Numberformat.Format = "0";
                worksheet.Cells[row, columnNo++].Value = "N"; // SUB_DURATION_N

                ExcelHelper.HorizontalCenterAlign(worksheet.Cells[row, columnNo]);
                worksheet.Cells[row, columnNo].Style.Numberformat.Format = "0";
                worksheet.Cells[row, columnNo++].Value = section.ValuePerNumericAttribute["CULV_DURATION_N"];
            }

            worksheet.Cells[row, columnNo++].Value = treatment?.TreatmentName;
            worksheet.Cells[row, columnNo].Style.Numberformat.Format = @"_($* #,##0_);_($*  #,##0);_($* "" - ""??_);(@_)";
            worksheet.Cells[row, columnNo++].Value = treatment?.Cost;

            if (row % 2 == 0)
            {
                ExcelHelper.ApplyColor(worksheet.Cells[row, 1, row, columnNo - 1], Color.LightGray);
            }
            ExcelHelper.ApplyBorder(worksheet.Cells[row, 1, row, columnNo - 1]);

            currentCell.Column = columnNo;
        }

        public CurrentCell AddHeadersCells(ExcelWorksheet worksheet)
        {
            // Row 1
            int headerRow = 1;
            var headersRow1 = GetHeadersRow1();
            var headersRow2 = GetHeadersRow2();

            var bridgeFundingColumn = headersRow1.IndexOf(BRIDGE_FUNDING) + 1;
            var analysisColumn = bridgeFundingColumn + headersRow2.Count;

            worksheet.Cells.Style.WrapText = false;

            // Add all Row 1 headers
            for (int column = 0; column < headersRow1.Count; column++)
            {
                worksheet.Cells[headerRow, column + 1].Value = headersRow1[column];
            }

            // Add Bridge Funding cells for Row 2
            for (int column = 0; column < headersRow2.Count; column++)
            {
                worksheet.Cells[headerRow + 1, column + bridgeFundingColumn].Value = headersRow2[column];
            }

            var row = headerRow;

            worksheet.Row(row).Height = 15;
            worksheet.Row(row + 1).Height = 15;
            // Autofit before the merges
            worksheet.Cells.AutoFitColumns(0);

            // Merge Bridge Funding cells in Row 1
            ExcelHelper.MergeCells(worksheet, row, bridgeFundingColumn, row, analysisColumn - 1);

            // Merge 2 rows for headers EXCEPT Bridge Funding columns

            // Merge rows for Columns prior to Bridge Funding
            for (int cellColumn = 1; cellColumn < bridgeFundingColumn; cellColumn++)
            {
                ExcelHelper.MergeCells(worksheet, row, cellColumn, row + 1, cellColumn);
            }

            // Merge rows for Columns after Bridge Funding
            for (int cellColumn = analysisColumn; cellColumn <= worksheet.Dimension.Columns; cellColumn++)
            {
                ExcelHelper.MergeCells(worksheet, row, cellColumn, row + 1, cellColumn);

                // Color condition headers
                if (headersRow1[cellColumn - 1].Contains("DUR") || headersRow1[cellColumn - 1].Contains("GCR"))
                {
                    ExcelHelper.ApplyColor(worksheet.Cells[row, cellColumn], Color.FromArgb(255, 242, 204));
                }
            }

            ExcelHelper.ApplyBorder(worksheet.Cells[headerRow, 1, headerRow + 1, worksheet.Dimension.Columns]);
            ExcelHelper.ApplyStyle(worksheet.Cells[headerRow + 1, bridgeFundingColumn, headerRow + 1, analysisColumn - 1]);
            worksheet.Cells.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Bottom;

            var currentCell = new CurrentCell { Row = headerRow + 2, Column = worksheet.Dimension.Columns + 1 };
            return currentCell;
        }

        public List<AssetDetail> GetUntreatedSections(SimulationYearDetail simulationYearDetail)
        {
            var untreatedSections =
                    simulationYearDetail.Assets.Where(
                        section => section.TreatmentCause == TreatmentCause.NoSelection && section.TreatmentOptions.Count > 0
                        &&
                        ((!string.IsNullOrEmpty(_summaryReportHelper.checkAndGetValue<string>(section.ValuePerTextAttribute, "NHS_IND")) && int.Parse(_summaryReportHelper.checkAndGetValue<string>(section.ValuePerTextAttribute, "NHS_IND")) == 1)
                        ||
                        _summaryReportHelper.checkAndGetValue<double>(section.ValuePerNumericAttribute, "DECK_AREA") > 28500
                        )).ToList();
            return untreatedSections;
        }

        private const string BRIDGE_FUNDING = "Bridge Funding";

        private List<string> GetHeadersRow1()
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
                "Large\r\nBridge",
                "Structure\r\nType",
                "Functional\r\nClass",
                "BPN",
                "NHS",
                "Interstate",
                "Risk\r\nScore",

                BRIDGE_FUNDING, // row 1 header for six sub-sections
                "",
                "",
                "",
                "",
                "",

                "Analysis\r\nYear",
                "GCR\r\nDECK",
                "GCR\r\nSUP",
                "GCR\r\nSUB",
                "GCR\r\nCULV",
                "DECK\r\nDUR",
                "SUP\r\nDUR",
                "SUB\r\nDUR",
                "CULV\r\nDUR",
                "Unfunded Treatment",
                "Cost",
            };
        }

        private List<string> GetHeadersRow2()
        {
            return new List<string>
            {
                "NHPP", // Six sub-sections for "Bridge Funding"
                "STP",
                "BOF",
                "BRIP", 
                "STATE",
                "N/A",
            };
        }
    }
}
