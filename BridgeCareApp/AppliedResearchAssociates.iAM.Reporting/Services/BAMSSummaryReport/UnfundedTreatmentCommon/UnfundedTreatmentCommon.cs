using System.Collections.Generic;
using System.Drawing;
using OfficeOpenXml;
using AppliedResearchAssociates.iAM.Analysis.Engine;
using AppliedResearchAssociates.iAM.ExcelHelpers;
using AppliedResearchAssociates.iAM.Reporting.Models;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using System;

namespace AppliedResearchAssociates.iAM.Reporting.Services.BAMSSummaryReport
{
    public class UnfundedTreatmentCommon
    {
        private SummaryReportHelper _summaryReportHelper;
        private TreatmentCommon _treatmentCommon;
        private ReportHelper _reportHelper;
        private readonly IUnitOfWork _unitOfWork;

        public UnfundedTreatmentCommon(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _summaryReportHelper = new SummaryReportHelper();
            _treatmentCommon = new TreatmentCommon(_unitOfWork);
            _reportHelper = new ReportHelper(_unitOfWork);
        }

        public void FillDataInWorkSheet(ExcelWorksheet worksheet, CurrentCell currentCell, AssetDetail section, int Year, TreatmentOptionDetail treatment, Color color)
        {
            _treatmentCommon.FillDataInWorkSheet(worksheet, currentCell, section, Year);

            var row = currentCell.Row;
            var columnNo = currentCell.Column;

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

            var familyId = int.Parse(_reportHelper.CheckAndGetValue<string>(section.ValuePerTextAttribute, "FAMILY_ID"));
            if (familyId < 11)
            {
                ExcelHelper.HorizontalCenterAlign(worksheet.Cells[row, columnNo]);
                worksheet.Cells[row, columnNo].Style.Numberformat.Format = "0.000";
                worksheet.Cells[row, columnNo++].Value = _reportHelper.CheckAndGetValue<double>(section.ValuePerNumericAttribute, "DECK_SEEDED");
                ExcelHelper.HorizontalCenterAlign(worksheet.Cells[row, columnNo]);
                worksheet.Cells[row, columnNo].Style.Numberformat.Format = "0.000";
                worksheet.Cells[row, columnNo++].Value = _reportHelper.CheckAndGetValue<double>(section.ValuePerNumericAttribute, "SUP_SEEDED");
                ExcelHelper.HorizontalCenterAlign(worksheet.Cells[row, columnNo]);
                worksheet.Cells[row, columnNo].Style.Numberformat.Format = "0.000";
                worksheet.Cells[row, columnNo++].Value = _reportHelper.CheckAndGetValue<double>(section.ValuePerNumericAttribute, "SUB_SEEDED");

                ExcelHelper.HorizontalCenterAlign(worksheet.Cells[row, columnNo]);
                worksheet.Cells[row, columnNo].Style.Numberformat.Format = "0.000";
                worksheet.Cells[row, columnNo++].Value = "N"; // CULV_SEEDED

                ExcelHelper.HorizontalCenterAlign(worksheet.Cells[row, columnNo]);
                worksheet.Cells[row, columnNo].Style.Numberformat.Format = "0";
                worksheet.Cells[row, columnNo++].Value = _reportHelper.CheckAndGetValue<double>(section.ValuePerNumericAttribute, "DECK_DURATION_N");
                ExcelHelper.HorizontalCenterAlign(worksheet.Cells[row, columnNo]);
                worksheet.Cells[row, columnNo].Style.Numberformat.Format = "0";
                worksheet.Cells[row, columnNo++].Value = _reportHelper.CheckAndGetValue<double>(section.ValuePerNumericAttribute, "SUP_DURATION_N");
                ExcelHelper.HorizontalCenterAlign(worksheet.Cells[row, columnNo]);
                worksheet.Cells[row, columnNo].Style.Numberformat.Format = "0";
                worksheet.Cells[row, columnNo++].Value = _reportHelper.CheckAndGetValue<double>(section.ValuePerNumericAttribute, "SUB_DURATION_N");

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

            ExcelHelper.ApplyColor(worksheet.Cells[row, 1, row, columnNo - 1], color);            
            ExcelHelper.ApplyBorder(worksheet.Cells[row, 1, row, columnNo - 1]);

            currentCell.Column = columnNo;
        }

        public CurrentCell AddHeadersCells(ExcelWorksheet worksheet)
        {
            var currentCell = _treatmentCommon.AddHeadersCells(worksheet);

            var columnNo = currentCell.Column;

            // Row 1
            int headerRow = 1;
            var headersRow1 = GetHeadersRow1();
            var headersRow2 = GetHeadersRow2();

            var bridgeFundingColumn = headersRow1.IndexOf(BRIDGE_FUNDING) + columnNo;
            var analysisColumn = bridgeFundingColumn + headersRow2.Count;

            // Add all Row 1 headers
            for (int column = 0; column < headersRow1.Count; column++)
            {
                worksheet.Cells[headerRow, column + columnNo].Style.WrapText = false;
                worksheet.Cells[headerRow, column + columnNo].Value = headersRow1[column];
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

            // Merge rows for Columns after Bridge Funding
            for (int cellColumn = analysisColumn; cellColumn <= worksheet.Dimension.Columns; cellColumn++)
            {
                ExcelHelper.MergeCells(worksheet, row, cellColumn, row + 1, cellColumn);

                // Color condition headers
                if (headersRow1[cellColumn - columnNo].Contains("DUR") || headersRow1[cellColumn - columnNo].Contains("GCR"))
                {
                    ExcelHelper.ApplyColor(worksheet.Cells[row, cellColumn], Color.FromArgb(255, 242, 204));
                }
            }

            ExcelHelper.ApplyBorder(worksheet.Cells[headerRow, 1, headerRow + 1, worksheet.Dimension.Columns]);
            ExcelHelper.ApplyStyle(worksheet.Cells[headerRow + 1, bridgeFundingColumn, headerRow + 1, analysisColumn - 1]);
            worksheet.Cells.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Bottom;

            currentCell = new CurrentCell { Row = headerRow + 2, Column = worksheet.Dimension.Columns + 1 };
            return currentCell;
        }        

        private const string BRIDGE_FUNDING = "Bridge Funding";
        private const string INTERSTATE = "Interstate";

        private List<string> GetHeadersRow1()
        {
            return new List<string>
            {
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

        public void PerformPostAutofitAdjustments(ExcelWorksheet worksheet)
        {
            _treatmentCommon.PerformPostAutofitAdjustments(worksheet);
        }
    }
}
