using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using AppliedResearchAssociates.iAM.Analysis.Engine;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.ExcelHelpers;
using AppliedResearchAssociates.iAM.Reporting.Models;
using AppliedResearchAssociates.iAM.Reporting.Models.PAMSAuditReport;
using AppliedResearchAssociates.iAM.Reporting.Services.FlexibileAuditReport;
using AppliedResearchAssociates.iAM.Reporting.Services.PAMSAuditReport;
using OfficeOpenXml;

namespace AppliedResearchAssociates.iAM.Reporting.Services.FlexibleAuditReport
{
    public class FlexibleAssetsTab
    {
        private FlexibleTreatments _flexibleTreatments;
        private ReportHelper _reportHelper;
        private readonly IUnitOfWork _unitOfWork;

        public FlexibleAssetsTab(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _reportHelper = new ReportHelper(_unitOfWork);
            _flexibleTreatments = new FlexibleTreatments(_unitOfWork);
        }

        public void Fill(ExcelWorksheet pavementWorksheet, SimulationOutput simulationOutput)
        {
            var fillHeaders = _flexibleTreatments.GetHeadersRow(simulationOutput.InitialAssetSummaries[0].ValuePerNumericAttribute, simulationOutput.InitialAssetSummaries[0].ValuePerTextAttribute);
            // Add excel headers to excel with the headers row data
            var currentCell = AddHeadersCells(pavementWorksheet);

            // Add row next to headers for filters and year numbers for dynamic data. Cover from
            // top, left to right, and bottom set of data.
            using (var autoFilterCells = pavementWorksheet.Cells[3, 1, currentCell.Row, currentCell.Column - 1])
            {
                autoFilterCells.AutoFilter = true;
            }

            pavementWorksheet.Cells.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Bottom;

            AddDynamicDataCells(pavementWorksheet, simulationOutput, currentCell);

            pavementWorksheet.Cells.AutoFitColumns();
            //_flexibleUnfundedTreatments.PerformPostAutofitAdjustments(pavementWorksheet);
        }

        public CurrentCell AddHeadersCells(ExcelWorksheet worksheet)
        {
            var currentCell = _flexibleTreatments.AddHeadersCells(worksheet);
            var columnNo = currentCell.Column;

            // Row 1
            int headerRow = 1;
            var headersRow1 = GetHeadersRow1();
            //var headersRow2 = GetHeadersRow2();

            var StressesColumn = headersRow1.IndexOf("OPI") + columnNo;

            StressHeaders(worksheet, StressesColumn, headerRow, headersRow1);

            // Add all Row 1 headers
            for (int column = 0; column < headersRow1.Count; column++)
            {
                worksheet.Cells[headerRow, column + columnNo].Style.WrapText = false;
                worksheet.Cells[headerRow, column + columnNo].Value = headersRow1[column];
            }

            var row = headerRow;
            worksheet.Row(row).Height = 15;
            worksheet.Row(row + 1).Height = 15;
            // Autofit before the merges
            worksheet.Cells.AutoFitColumns(0);
            ExcelHelper.ApplyBorder(worksheet.Cells[headerRow, 1, headerRow + 1, worksheet.Dimension.Columns]);
            //ExcelHelper.ApplyStyle(worksheet.Cells[headerRow + 1, bridgeFundingColumn, headerRow + 1, analysisColumn - 1]);
            worksheet.Cells.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Bottom;

            currentCell = new CurrentCell { Row = headerRow + 2, Column = worksheet.Dimension.Columns + 1 };
            return currentCell;
        }

        private List<string> GetHeadersRow1()
        {
            return new List<string>
            {
            };
        }
        private void StressHeaders(ExcelWorksheet worksheet, int column, int row, List<string> stressHeaders)
        {
            for (int cell = 0; cell < stressHeaders.Count; cell++)
            {
                ExcelHelper.MergeCells(worksheet, row, column + cell, row + 1, column + cell);//stress header cells
            }

        }



        private void AddDynamicDataCells(ExcelWorksheet worksheet, SimulationOutput simulationOutput, CurrentCell currentCell)
        {
            var primaryKeyField = _unitOfWork.AdminSettingsRepo.GetKeyFields();

            // TODO bridges in data tab need to match with bridges in Decision tab           
            foreach (var initialAssetSummary in simulationOutput.InitialAssetSummaries)
            {
                var primaryKey = CheckGetValue(initialAssetSummary.ValuePerNumericAttribute, primaryKeyField[0].ToString());

                // Generate data model
                var DataModel = GeneratePavementDataModel(primaryKey, initialAssetSummary);

                // Fill in excel
                FillDataInWorksheet(worksheet, currentCell, DataModel);
            }
        }

        public void FillDataInWorksheet(ExcelWorksheet worksheet, CurrentCell currentCell, PavementDataModel DataModel)
        {
            currentCell.Row++;
            currentCell.Column = 1;

            _flexibleTreatments.FillDataInWorksheet(worksheet, currentCell, DataModel);

            var row = currentCell.Row;
            var columnNo = currentCell.Column;
            var assetSummaryDetail = DataModel.AssetSummaryDetail;

            ExcelHelper.HorizontalCenterAlign(worksheet.Cells[row, columnNo]);


            if (row % 2 == 0)
            {
                ExcelHelper.ApplyColor(worksheet.Cells[row, 1, row, columnNo - 1], Color.LightGray);
            }
            ExcelHelper.ApplyBorder(worksheet.Cells[row, 1, row, columnNo - 1]);

            currentCell.Column = columnNo;
        }

        private static PavementDataModel GeneratePavementDataModel(double primaryKey, AssetSummaryDetail initialAssetSummary) => new()
        {
            CRS = primaryKey,
            AssetSummaryDetail = initialAssetSummary
        };

        private double CheckGetValue(Dictionary<string, double> valuePerNumericAttribute, string attribute) => _reportHelper.CheckAndGetValue<double>(valuePerNumericAttribute, attribute);
    }
}
