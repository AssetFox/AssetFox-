using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

using OfficeOpenXml;

using AppliedResearchAssociates.iAM.Analysis.Engine;
using AppliedResearchAssociates.iAM.Reporting.Interfaces.BAMSSummaryReport;
using AppliedResearchAssociates.iAM.Reporting.Models.BAMSSummaryReport;

namespace AppliedResearchAssociates.iAM.Reporting.Services.BAMSSummaryReport.UnfundedTreatmentCommon
{
    public class UnfundedTreatmentCommon : IUnfundedTreatmentCommon
    {
        private readonly ISummaryReportHelper _summaryReportHelper;

        public UnfundedTreatmentCommon(ISummaryReportHelper summaryReportHelper)
        {
            _summaryReportHelper = summaryReportHelper;
        }

        public void FillDataInWorkSheet(ExcelWorksheet worksheet, CurrentCell currentCell, SectionDetail section, int Year, TreatmentOptionDetail treatment)
        {
            var row = currentCell.Row;
            var columnNo = currentCell.Column;

            worksheet.Cells[row, columnNo++].Value = section.ValuePerTextAttribute["DISTRICT"];
            worksheet.Cells[row, columnNo++].Value = section.ValuePerTextAttribute["COUNTY"];
            worksheet.Cells[row, columnNo++].Value = section.FacilityName.Split('-')[0];

            worksheet.Cells[row, columnNo].Style.Numberformat.Format = "0";
            var deckArea = section.ValuePerNumericAttribute["DECK_AREA"];
            worksheet.Cells[row, columnNo++].Value = deckArea;

            worksheet.Cells[row, columnNo++].Value = section.ValuePerNumericAttribute["LENGTH"];
            worksheet.Cells[row, columnNo++].Value = section.ValuePerTextAttribute["BUS_PLAN_NETWORK"];
            worksheet.Cells[row, columnNo++].Value = section.ValuePerTextAttribute["MPO_NAME"];

            var functionalClassAbbr = section.ValuePerTextAttribute["FUNC_CLASS"];
            var functionalClassDescription = _summaryReportHelper.FullFunctionalClassDescription(functionalClassAbbr);
            worksheet.Cells[row, columnNo++].Value = functionalClassDescription;

            worksheet.Cells[row, columnNo++].Value = section.ValuePerTextAttribute["NHS_IND"] == "0" ? "N" : "Y";
            worksheet.Cells[row, columnNo++].Value = section.ValuePerTextAttribute["INTERSTATE"];

            worksheet.Cells[row, columnNo].Style.Numberformat.Format = "0.00";
            worksheet.Cells[row, columnNo++].Value = section.ValuePerNumericAttribute["RISK_SCORE"];

            worksheet.Cells[row, columnNo++].Value = deckArea >= 28500 ? "Y" : "N";

            worksheet.Cells[row, columnNo++].Value = _summaryReportHelper.BridgeFunding185(section) ? "Y" : "N";
            worksheet.Cells[row, columnNo++].Value = _summaryReportHelper.BridgeFunding581(section) ? "Y" : "N";
            worksheet.Cells[row, columnNo++].Value = _summaryReportHelper.BridgeFundingSTP(section) ? "Y" : "N";
            worksheet.Cells[row, columnNo++].Value = _summaryReportHelper.BridgeFundingNHPP(section) ? "Y" : "N";
            worksheet.Cells[row, columnNo++].Value = _summaryReportHelper.BridgeFundingBOF(section) ? "Y" : "N";
            worksheet.Cells[row, columnNo++].Value = _summaryReportHelper.BridgeFunding183(section) ? "Y" : "N";

            worksheet.Cells[row, columnNo++].Value = Year;

            var familyId = int.Parse(section.ValuePerTextAttribute["FAMILY_ID"]);
            if (familyId < 11)
            {
                worksheet.Cells[row, columnNo].Style.Numberformat.Format = "0.000";
                worksheet.Cells[row, columnNo++].Value = section.ValuePerNumericAttribute["DECK_SEEDED"];
                worksheet.Cells[row, columnNo].Style.Numberformat.Format = "0.000";
                worksheet.Cells[row, columnNo++].Value = section.ValuePerNumericAttribute["SUP_SEEDED"];
                worksheet.Cells[row, columnNo].Style.Numberformat.Format = "0.000";
                worksheet.Cells[row, columnNo++].Value = section.ValuePerNumericAttribute["SUB_SEEDED"];

                worksheet.Cells[row, columnNo].Style.Numberformat.Format = "0.000";
                worksheet.Cells[row, columnNo++].Value = "N"; // CULV_SEEDED

                worksheet.Cells[row, columnNo].Style.Numberformat.Format = "0";
                worksheet.Cells[row, columnNo++].Value = section.ValuePerNumericAttribute["DECK_DURATION_N"];
                worksheet.Cells[row, columnNo].Style.Numberformat.Format = "0";
                worksheet.Cells[row, columnNo++].Value = section.ValuePerNumericAttribute["SUP_DURATION_N"];
                worksheet.Cells[row, columnNo].Style.Numberformat.Format = "0";
                worksheet.Cells[row, columnNo++].Value = section.ValuePerNumericAttribute["SUB_DURATION_N"];

                worksheet.Cells[row, columnNo].Style.Numberformat.Format = "0";
                worksheet.Cells[row, columnNo++].Value = "N"; // CULV_DURATION_N
            }
            else
            {
                worksheet.Cells[row, columnNo].Style.Numberformat.Format = "0.000";
                worksheet.Cells[row, columnNo++].Value = "N"; // DECK_SEEDED
                worksheet.Cells[row, columnNo].Style.Numberformat.Format = "0.000";
                worksheet.Cells[row, columnNo++].Value = "N"; // SUP_SEEDED
                worksheet.Cells[row, columnNo].Style.Numberformat.Format = "0.000";
                worksheet.Cells[row, columnNo++].Value = "N"; // SUB_SEEDED

                worksheet.Cells[row, columnNo].Style.Numberformat.Format = "0.000";
                worksheet.Cells[row, columnNo++].Value = section.ValuePerNumericAttribute["CULV_SEEDED"];

                worksheet.Cells[row, columnNo].Style.Numberformat.Format = "0";
                worksheet.Cells[row, columnNo++].Value = "N"; // DECK_DURATION_N
                worksheet.Cells[row, columnNo].Style.Numberformat.Format = "0";
                worksheet.Cells[row, columnNo++].Value = "N"; // SUP_DURATION_N
                worksheet.Cells[row, columnNo].Style.Numberformat.Format = "0";
                worksheet.Cells[row, columnNo++].Value = "N"; // SUB_DURATION_N

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
            }

            ExcelHelper.ApplyBorder(worksheet.Cells[headerRow, 1, headerRow + 1, worksheet.Dimension.Columns]);
            ExcelHelper.ApplyStyle(worksheet.Cells[headerRow + 1, bridgeFundingColumn, headerRow + 1, analysisColumn - 1]);
            worksheet.Cells.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Bottom;

            var currentCell = new CurrentCell { Row = headerRow + 2, Column = worksheet.Dimension.Columns + 1 };
            return currentCell;
        }

        public List<SectionDetail> GetUntreatedSections(SimulationYearDetail simulationYearDetail)
        {
            var untreatedSections =
                    simulationYearDetail.Sections.Where(
                        sect => sect.TreatmentCause == TreatmentCause.NoSelection &&
                        (int.Parse(sect.ValuePerTextAttribute["NHS_IND"]) == 1 ||
                        sect.ValuePerNumericAttribute["DECK_AREA"] > 28500) &&
                        sect.TreatmentOptions.Count > 0
                        ).ToList();
            return untreatedSections;
        }

        private const string BRIDGE_FUNDING = "Bridge Funding";

        private List<string> GetHeadersRow1()
        {
            return new List<string>
            {
                "District",
                "County",
                "BRKey",
                "Deck\r\nArea",
                "Bridge\r\nLength",
                "BPN",
                "MPO/RPO",
                "Functional\r\nClass",
                "NHS",
                "Interstate",
                "Risk\r\nScore",
                "Large\r\nBridge",

                BRIDGE_FUNDING, // row 1 header for six sub-sections
                "",
                "",
                "",
                "",
                "",

                "Analysis\r\nYear",
                "GCR DECK",
                "GCR SUP",
                "GCR SUB",
                "GCR CULV",
                "DECK DUR",
                "SUP DUR",
                "SUB DUR",
                "CULV DUR",
                "Unfunded Treatment",
                "Cost",
            };
        }

        private List<string> GetHeadersRow2()
        {
            return new List<string>
            {
                "185", // Six sub-sections for "Bridge Funding"
                "581",
                "STP",
                "NHPP",
                "BOF",
                "183",
            };
        }
    }
}
