using System;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection.PortableExecutable;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.ExcelHelpers;
using AppliedResearchAssociates.iAM.Reporting.Models;
using AppliedResearchAssociates.iAM.Reporting.Models.PAMSAuditReport;
using AppliedResearchAssociates.iAM.Reporting.Services.BAMSAuditReport;
using AppliedResearchAssociates.iAM.Reporting.Services.FlexibileAuditReport;
using OfficeOpenXml;

namespace AppliedResearchAssociates.iAM.Reporting.Services.FlexibleAuditReport
{
    public class FlexibleTreatments
    {
        private ReportHelper _reportHelper;
        private readonly IUnitOfWork _unitOfWork;
        private List<string> _headers;

        public FlexibleTreatments(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _reportHelper = new ReportHelper(_unitOfWork);
            _headers = new List<string>();
        }

        public CurrentCell AddHeadersCells(ExcelWorksheet worksheet)
        {
            // Row 1
            int headerRow = 1;
            var headersRow = _headers;

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

            // Populate numeric attributes
            foreach (var numericAttribute in assetSummaryDetail.ValuePerNumericAttribute)
            {
                worksheet.Cells[row, columnNo++].Value = numericAttribute.Value;
            }

            // Populate text attributes
            foreach (var textAttribute in assetSummaryDetail.ValuePerTextAttribute)
            {
                worksheet.Cells[row, columnNo++].Value = textAttribute.Value;
            }

            // Update the column number for the next row
            currentCell.Column = columnNo;
        }

        public List<string> GetHeadersRow(Dictionary<string, double> valuePerNumericAttribute, Dictionary<string, string> valuePerTextAttribute)
        {
            if (valuePerNumericAttribute == null)
            {
                throw new ArgumentNullException(nameof(valuePerNumericAttribute), "Value per numeric attribute cannot be null.");
            }

            if (valuePerTextAttribute == null)
            {
                throw new ArgumentNullException(nameof(valuePerTextAttribute), "Value per text attribute cannot be null.");
            }

            // Extract attribute keys from valuePerNumericAttribute
            foreach (var attribute in valuePerNumericAttribute)
            {
                _headers.Add(attribute.Key);
            }

            foreach (var attribute in valuePerTextAttribute)
            {
                _headers.Add(attribute.Key);
            }

            return _headers;
        }

        public void PerformPostAutofitAdjustments(ExcelWorksheet worksheet)
        {
            var columnNumber = _headers.IndexOf("Interstate") + 1;
            worksheet.Column(columnNumber).SetTrueWidth(9);
        }
    }
}
