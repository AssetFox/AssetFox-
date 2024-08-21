using System;
using System.Collections.Generic;
using System.Drawing;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.ExcelHelpers;
using AppliedResearchAssociates.iAM.Reporting.Models;
using AppliedResearchAssociates.iAM.Reporting.Models.PAMSAuditReport;
using OfficeOpenXml;

namespace AppliedResearchAssociates.iAM.Reporting.Services.PAMSAuditReport
{
    public class PavementTreatments
    {
        private ReportHelper _reportHelper;
        private readonly IUnitOfWork _unitOfWork;

        public PavementTreatments(IUnitOfWork unitOfWork)
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

        public void FillDataInWorksheet(ExcelWorksheet worksheet, CurrentCell currentCell, PavementDataModel pavementDataModel)
        {
            var row = currentCell.Row;
            var columnNo = currentCell.Column;
            var assetSummaryDetail = pavementDataModel.AssetSummaryDetail;

            ExcelHelper.HorizontalCenterAlign(worksheet.Cells[row, columnNo]);
            var crs = _reportHelper.CheckAndGetValue<string>(assetSummaryDetail.ValuePerTextAttribute, "CRS");
            worksheet.Cells[row, columnNo++].Value = crs;

            var lastUnderScoreIndex = crs.LastIndexOf('_');
            var hyphenIndex = crs.IndexOf('-');
            var startSeg = crs.Substring(lastUnderScoreIndex + 1, hyphenIndex - lastUnderScoreIndex - 1);
            var endSeg = crs.Substring(hyphenIndex + 1);

            worksheet.Cells[row, columnNo++].Value = startSeg;
            worksheet.Cells[row, columnNo++].Value = endSeg;
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
            worksheet.Cells[row, columnNo++].Value = _reportHelper.CheckAndGetValue<string>(assetSummaryDetail.ValuePerTextAttribute, "FAMILY");
            worksheet.Cells[row, columnNo++].Value = _reportHelper.CheckAndGetValue<double>(assetSummaryDetail.ValuePerNumericAttribute, "SEGMENT_LENGTH");
            worksheet.Cells[row, columnNo].Style.Numberformat.Format = "###,###,###,###,##0";
            worksheet.Cells[row, columnNo++].Value = _reportHelper.CheckAndGetValue<string>(assetSummaryDetail.ValuePerTextAttribute, "MPO_RPO");
            worksheet.Cells[row, columnNo++].Value = _reportHelper.CheckAndGetValue<string>(assetSummaryDetail.ValuePerTextAttribute, "POSTED");

            ExcelHelper.HorizontalCenterAlign(worksheet.Cells[row, columnNo]);
            var bpn_string = _reportHelper.CheckAndGetValue<string>(assetSummaryDetail.ValuePerTextAttribute, "BUSIPLAN");
            if (int.TryParse(bpn_string, out var bpn_int))
            {
                worksheet.Cells[row, columnNo++].Value = bpn_int;
            }
            else
            {
                worksheet.Cells[row, columnNo++].Value = bpn_string;
            }
            ExcelHelper.HorizontalCenterAlign(worksheet.Cells[row, columnNo]);
            worksheet.Cells[row, columnNo++].Value = _reportHelper.CheckAndGetValue<string>(assetSummaryDetail.ValuePerTextAttribute, "NHS_IND") == "0" ? PAMSAuditReportConstants.No : PAMSAuditReportConstants.Yes;
            ExcelHelper.HorizontalCenterAlign(worksheet.Cells[row, columnNo]);
            worksheet.Cells[row, columnNo++].Value = _reportHelper.CheckAndGetValue<string>(assetSummaryDetail.ValuePerTextAttribute, "INTERSTATE");
            worksheet.Cells[row, columnNo].Style.Numberformat.Format = "###,###,###,###,##0";
            worksheet.Cells[row, columnNo++].Value = _reportHelper.CheckAndGetValue<double>(assetSummaryDetail.ValuePerNumericAttribute, "RISKSCORE");

            if (row % 2 == 0)
            {
                ExcelHelper.ApplyColor(worksheet.Cells[row, 1, row, columnNo - 1], Color.LightGray);
            }
            ExcelHelper.ApplyBorder(worksheet.Cells[row, 1, row, columnNo - 1]);

            currentCell.Column = columnNo;
        }

        private static List<string> GetHeadersRow()
        {
            return new List<string>
            {
                "Section",
                "Start Segment",
                "End Segment",
                "District",
                "County",
                "Family ID",
                "Length",
                "MPO/RPO",
                "Posted Roads",
                "BPN",
                "NHS",
                "Interstate",
                "Risk\r\nScore"
            };
        }
        public void PerformPostAutofitAdjustments(ExcelWorksheet worksheet)
        {
            var columnNumber = GetHeadersRow().IndexOf("Interstate") + 1;
            worksheet.Column(columnNumber).SetTrueWidth(9);
        }
    }
}
