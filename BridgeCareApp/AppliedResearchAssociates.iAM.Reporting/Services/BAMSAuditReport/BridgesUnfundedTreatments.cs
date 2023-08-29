using System.Collections.Generic;
using System.Drawing;
using OfficeOpenXml;
using AppliedResearchAssociates.iAM.ExcelHelpers;
using AppliedResearchAssociates.iAM.Reporting.Models;
using AppliedResearchAssociates.iAM.Reporting.Models.BAMSAuditReport;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using System;

namespace AppliedResearchAssociates.iAM.Reporting.Services.BAMSAuditReport
{
    public class BridgesUnfundedTreatments
    {
        private ReportHelper _reportHelper;
        private BridgesTreatments _bridgesTreatments;
        private const string BRIDGE_FUNDING = "Bridge Funding";
        private readonly IUnitOfWork _unitOfWork;

        public BridgesUnfundedTreatments(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _reportHelper = new ReportHelper(_unitOfWork);
            _bridgesTreatments = new BridgesTreatments(_unitOfWork);
        }

        public void FillDataInWorksheet(ExcelWorksheet worksheet, CurrentCell currentCell, BridgeDataModel bridgeDataModel)
        {
            currentCell.Row ++;
            currentCell.Column = 1;

            _bridgesTreatments.FillDataInWorksheet(worksheet, currentCell, bridgeDataModel);

            var row = currentCell.Row;
            var columnNo = currentCell.Column;
            var assetSummaryDetail = bridgeDataModel.AssetSummaryDetail;

            ExcelHelper.HorizontalCenterAlign(worksheet.Cells[row, columnNo]);
            worksheet.Cells[row, columnNo++].Value = _reportHelper.BridgeFundingNHPP(assetSummaryDetail) ? BAMSAuditReportConstants.Yes : BAMSAuditReportConstants.No;
            ExcelHelper.HorizontalCenterAlign(worksheet.Cells[row, columnNo]);
            worksheet.Cells[row, columnNo++].Value = _reportHelper.BridgeFundingSTP(assetSummaryDetail) ? BAMSAuditReportConstants.Yes : BAMSAuditReportConstants.No;
            ExcelHelper.HorizontalCenterAlign(worksheet.Cells[row, columnNo]);
            worksheet.Cells[row, columnNo++].Value = _reportHelper.BridgeFundingBOF(assetSummaryDetail) ? BAMSAuditReportConstants.Yes : BAMSAuditReportConstants.No;
            ExcelHelper.HorizontalCenterAlign(worksheet.Cells[row, columnNo]);
            worksheet.Cells[row, columnNo++].Value = _reportHelper.BridgeFundingBRIP(assetSummaryDetail) ? BAMSAuditReportConstants.Yes : BAMSAuditReportConstants.No;
            ExcelHelper.HorizontalCenterAlign(worksheet.Cells[row, columnNo]);
            worksheet.Cells[row, columnNo++].Value = _reportHelper.BridgeFundingState(assetSummaryDetail) ? BAMSAuditReportConstants.Yes : BAMSAuditReportConstants.No;
            ExcelHelper.HorizontalCenterAlign(worksheet.Cells[row, columnNo]);
            worksheet.Cells[row, columnNo++].Value = _reportHelper.BridgeFundingNotApplicable(assetSummaryDetail) ? BAMSAuditReportConstants.Yes : BAMSAuditReportConstants.No;            

            var familyId = int.Parse(_reportHelper.CheckAndGetValue<string>(assetSummaryDetail.ValuePerTextAttribute, "FAMILY_ID"));
            if (familyId < 11)
            {
                ExcelHelper.HorizontalCenterAlign(worksheet.Cells[row, columnNo]);
                worksheet.Cells[row, columnNo].Style.Numberformat.Format = "0.000";
                worksheet.Cells[row, columnNo++].Value = _reportHelper.CheckAndGetValue<double>(assetSummaryDetail.ValuePerNumericAttribute, "DECK_SEEDED");

                ExcelHelper.HorizontalCenterAlign(worksheet.Cells[row, columnNo]);
                worksheet.Cells[row, columnNo].Style.Numberformat.Format = "0.000";
                worksheet.Cells[row, columnNo++].Value = _reportHelper.CheckAndGetValue<double>(assetSummaryDetail.ValuePerNumericAttribute, "SUP_SEEDED");

                ExcelHelper.HorizontalCenterAlign(worksheet.Cells[row, columnNo]);
                worksheet.Cells[row, columnNo].Style.Numberformat.Format = "0.000";
                worksheet.Cells[row, columnNo++].Value = _reportHelper.CheckAndGetValue<double>(assetSummaryDetail.ValuePerNumericAttribute, "SUB_SEEDED");

                ExcelHelper.HorizontalCenterAlign(worksheet.Cells[row, columnNo]);
                worksheet.Cells[row, columnNo].Style.Numberformat.Format = "0.000";
                worksheet.Cells[row, columnNo++].Value = "N"; // CULV_SEEDED

                ExcelHelper.HorizontalCenterAlign(worksheet.Cells[row, columnNo]);
                worksheet.Cells[row, columnNo].Style.Numberformat.Format = "0";
                worksheet.Cells[row, columnNo++].Value = _reportHelper.CheckAndGetValue<double>(assetSummaryDetail.ValuePerNumericAttribute, "DECK_DURATION_N");

                ExcelHelper.HorizontalCenterAlign(worksheet.Cells[row, columnNo]);
                worksheet.Cells[row, columnNo].Style.Numberformat.Format = "0";
                worksheet.Cells[row, columnNo++].Value = _reportHelper.CheckAndGetValue<double>(assetSummaryDetail.ValuePerNumericAttribute, "SUP_DURATION_N");

                ExcelHelper.HorizontalCenterAlign(worksheet.Cells[row, columnNo]);
                worksheet.Cells[row, columnNo].Style.Numberformat.Format = "0";
                worksheet.Cells[row, columnNo++].Value = _reportHelper.CheckAndGetValue<double>(assetSummaryDetail.ValuePerNumericAttribute, "SUB_DURATION_N");

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
                worksheet.Cells[row, columnNo++].Value = assetSummaryDetail.ValuePerNumericAttribute["CULV_SEEDED"];

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
                worksheet.Cells[row, columnNo++].Value = assetSummaryDetail.ValuePerNumericAttribute["CULV_DURATION_N"];
            }            

            if (row % 2 == 0)
            {
                ExcelHelper.ApplyColor(worksheet.Cells[row, 1, row, columnNo - 1], Color.LightGray);
            }
            ExcelHelper.ApplyBorder(worksheet.Cells[row, 1, row, columnNo - 1]);

            currentCell.Column = columnNo;
        }

        public CurrentCell AddHeadersCells(ExcelWorksheet worksheet)
        {
            var currentCell = _bridgesTreatments.AddHeadersCells(worksheet);
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

        private List<string> GetHeadersRow1()
        {
            return new List<string>
            {
                BRIDGE_FUNDING, // row 1 header for six sub-assetSummaryDetails
                "",
                "",
                "",
                "",
                "",                
                "GCR\r\nDECK",
                "GCR\r\nSUP",
                "GCR\r\nSUB",
                "GCR\r\nCULV",
                "DECK\r\nDUR",
                "SUP\r\nDUR",
                "SUB\r\nDUR",
                "CULV\r\nDUR",
            };
        }

        private List<string> GetHeadersRow2()
        {
            // Six sub-assetSummaryDetails for "Bridge Funding"
            return new List<string>
            {
                "NHPP",
                "STP",
                "BOF",
                "BRIP", 
                "STATE",
                "N/A",
            };
        }

        public void PerformPostAutofitAdjustments(ExcelWorksheet worksheet)
        {
            _bridgesTreatments.PerformPostAutofitAdjustments(worksheet);
        }
    }
}
