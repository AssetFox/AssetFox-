﻿using System.Collections.Generic;
using System.Drawing;
using OfficeOpenXml;
using AppliedResearchAssociates.iAM.ExcelHelpers;
using AppliedResearchAssociates.iAM.Reporting.Models;
using AppliedResearchAssociates.iAM.Reporting.Models.PAMSAuditReport;
using System;
using OfficeOpenXml.FormulaParsing.Excel.Functions.RefAndLookup;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.Reporting.Services.BAMSAuditReport;
using AppliedResearchAssociates.iAM.Analysis.Engine;

namespace AppliedResearchAssociates.iAM.Reporting.Services.PAMSAuditReport
{
    public class PavementUnfundedTreatments
    {
        private ReportHelper _reportHelper;
        private PavementTreatments _pavementTreatments;
        private const string BRIDGE_FUNDING = "Pavement Funding";
        private readonly IUnitOfWork _unitOfWork;

        public PavementUnfundedTreatments(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _reportHelper = new ReportHelper(_unitOfWork);
            _pavementTreatments = new PavementTreatments(_unitOfWork);
        }

        public void FillDataInWorksheet(ExcelWorksheet worksheet, CurrentCell currentCell, PavementDataModel bridgeDataModel)
        {
            currentCell.Row++;
            currentCell.Column = 1;

            _pavementTreatments.FillDataInWorksheet(worksheet, currentCell, bridgeDataModel);

            var row = currentCell.Row;
            var columnNo = currentCell.Column;
            var assetSummaryDetail = bridgeDataModel.AssetSummaryDetail;
            var ruttingAverageTotal = CalculateRuttingBasedOnSurfaceType(assetSummaryDetail);

            ExcelHelper.HorizontalCenterAlign(worksheet.Cells[row, columnNo]);
            worksheet.Cells[row, columnNo++].Value = Math.Round(Convert.ToDecimal(_reportHelper.CheckAndGetValue<double>(assetSummaryDetail.ValuePerNumericAttribute, PAMSAuditReportConstants.OPI)));
            worksheet.Cells[row, columnNo++].Value = Math.Round(Convert.ToDecimal(_reportHelper.CheckAndGetValue<double>(assetSummaryDetail.ValuePerNumericAttribute, PAMSAuditReportConstants.IRI)));
            worksheet.Cells[row, columnNo++].Value = Math.Round(Convert.ToDecimal(ruttingAverageTotal), 3);
            worksheet.Cells[row, columnNo++].Value = Math.Round(Convert.ToDecimal(_reportHelper.CheckAndGetValue<double>(assetSummaryDetail.ValuePerNumericAttribute, PAMSAuditReportConstants.FAULT)), 3);

            //var familyId = int.Parse(_reportHelper.CheckAndGetValue<string>(assetSummaryDetail.ValuePerTextAttribute, "FAMILY_ID"));
            //if (familyId < 11)
            //{
            //    ExcelHelper.HorizontalCenterAlign(worksheet.Cells[row, columnNo]);
            //    worksheet.Cells[row, columnNo].Style.Numberformat.Format = "0.000";
            //    worksheet.Cells[row, columnNo++].Value = _reportHelper.CheckAndGetValue<double>(assetSummaryDetail.ValuePerNumericAttribute, "DECK_SEEDED");
            //    ExcelHelper.HorizontalCenterAlign(worksheet.Cells[row, columnNo]);
            //    worksheet.Cells[row, columnNo].Style.Numberformat.Format = "0.000";
            //    worksheet.Cells[row, columnNo++].Value = _reportHelper.CheckAndGetValue<double>(assetSummaryDetail.ValuePerNumericAttribute, "SUP_SEEDED");

            //    ExcelHelper.HorizontalCenterAlign(worksheet.Cells[row, columnNo]);
            //    worksheet.Cells[row, columnNo].Style.Numberformat.Format = "0.000";
            //    worksheet.Cells[row, columnNo++].Value = _reportHelper.CheckAndGetValue<double>(assetSummaryDetail.ValuePerNumericAttribute, "SUB_SEEDED");

            //    ExcelHelper.HorizontalCenterAlign(worksheet.Cells[row, columnNo]);
            //    worksheet.Cells[row, columnNo].Style.Numberformat.Format = "0.000";
            //    worksheet.Cells[row, columnNo++].Value = "N"; // CULV_SEEDED

            //    ExcelHelper.HorizontalCenterAlign(worksheet.Cells[row, columnNo]);
            //    worksheet.Cells[row, columnNo].Style.Numberformat.Format = "0";
            //    worksheet.Cells[row, columnNo++].Value = _reportHelper.CheckAndGetValue<double>(assetSummaryDetail.ValuePerNumericAttribute, "DECK_DURATION_N");

            //    ExcelHelper.HorizontalCenterAlign(worksheet.Cells[row, columnNo]);
            //    worksheet.Cells[row, columnNo].Style.Numberformat.Format = "0";
            //    worksheet.Cells[row, columnNo++].Value = _reportHelper.CheckAndGetValue<double>(assetSummaryDetail.ValuePerNumericAttribute, "SUP_DURATION_N");

            //    ExcelHelper.HorizontalCenterAlign(worksheet.Cells[row, columnNo]);
            //    worksheet.Cells[row, columnNo].Style.Numberformat.Format = "0";
            //    worksheet.Cells[row, columnNo++].Value = _reportHelper.CheckAndGetValue<double>(assetSummaryDetail.ValuePerNumericAttribute, "SUB_DURATION_N");

            //    ExcelHelper.HorizontalCenterAlign(worksheet.Cells[row, columnNo]);
            //    worksheet.Cells[row, columnNo].Style.Numberformat.Format = "0";
            //    worksheet.Cells[row, columnNo++].Value = "N"; // CULV_DURATION_N
            //}
            //else
            //{
            //    ExcelHelper.HorizontalCenterAlign(worksheet.Cells[row, columnNo]);
            //    worksheet.Cells[row, columnNo].Style.Numberformat.Format = "0.000";
            //    worksheet.Cells[row, columnNo++].Value = "N"; // DECK_SEEDED

            //    ExcelHelper.HorizontalCenterAlign(worksheet.Cells[row, columnNo]);
            //    worksheet.Cells[row, columnNo].Style.Numberformat.Format = "0.000";
            //    worksheet.Cells[row, columnNo++].Value = "N"; // SUP_SEEDED

            //    ExcelHelper.HorizontalCenterAlign(worksheet.Cells[row, columnNo]);
            //    worksheet.Cells[row, columnNo].Style.Numberformat.Format = "0.000";
            //    worksheet.Cells[row, columnNo++].Value = "N"; // SUB_SEEDED

            //    ExcelHelper.HorizontalCenterAlign(worksheet.Cells[row, columnNo]);
            //    worksheet.Cells[row, columnNo].Style.Numberformat.Format = "0.000";
            //    worksheet.Cells[row, columnNo++].Value = assetSummaryDetail.ValuePerNumericAttribute["CULV_SEEDED"];

            //    ExcelHelper.HorizontalCenterAlign(worksheet.Cells[row, columnNo]);
            //    worksheet.Cells[row, columnNo].Style.Numberformat.Format = "0";
            //    worksheet.Cells[row, columnNo++].Value = "N"; // DECK_DURATION_N

            //    ExcelHelper.HorizontalCenterAlign(worksheet.Cells[row, columnNo]);
            //    worksheet.Cells[row, columnNo].Style.Numberformat.Format = "0";
            //    worksheet.Cells[row, columnNo++].Value = "N"; // SUP_DURATION_N

            //    ExcelHelper.HorizontalCenterAlign(worksheet.Cells[row, columnNo]);
            //    worksheet.Cells[row, columnNo].Style.Numberformat.Format = "0";
            //    worksheet.Cells[row, columnNo++].Value = "N"; // SUB_DURATION_N

            //    ExcelHelper.HorizontalCenterAlign(worksheet.Cells[row, columnNo]);
            //    worksheet.Cells[row, columnNo].Style.Numberformat.Format = "0";
            //    worksheet.Cells[row, columnNo++].Value = assetSummaryDetail.ValuePerNumericAttribute["CULV_DURATION_N"];
            //}

            if (row % 2 == 0)
            {
                ExcelHelper.ApplyColor(worksheet.Cells[row, 1, row, columnNo - 1], Color.LightGray);
            }
            ExcelHelper.ApplyBorder(worksheet.Cells[row, 1, row, columnNo - 1]);

            currentCell.Column = columnNo;
        }

        private double CalculateRuttingBasedOnSurfaceType(AssetSummaryDetail selectedSection)
        {
            var surfaceType = GetSurfaceType(selectedSection);

            if (surfaceType == SurfaceType.Asphalt)
            {
                return CalculateAverageRutting(PAMSAuditReportConstants.RUT_LEFT, PAMSAuditReportConstants.RUT_RIGHT, selectedSection);
            }
            else if (surfaceType == SurfaceType.Concrete)
            {
                return CalculateAverageRutting("CRJCPRU_Total", "CLJCPRU_Total", selectedSection);
            }
            return 0;
        }

        private double CalculateAverageRutting(string firstAttributeName, string secondAttributeName, AssetSummaryDetail assetSummaryDetail)
        {
            double firstValue = _reportHelper.CheckAndGetValue<double>(assetSummaryDetail.ValuePerNumericAttribute, firstAttributeName);
            double secondValue = _reportHelper.CheckAndGetValue<double>(assetSummaryDetail.ValuePerNumericAttribute, secondAttributeName);
            return (firstValue + secondValue) / 2;
        }

        private SurfaceType GetSurfaceType(AssetSummaryDetail assetSummaryDetail)
        {
            int surfaceId = Convert.ToInt32(_reportHelper.CheckAndGetValue<double>(assetSummaryDetail.ValuePerNumericAttribute, "SURFACEID"));
            return surfaceId > 62 ? SurfaceType.Concrete : SurfaceType.Asphalt;
        }

        enum SurfaceType
        {
            Asphalt,
            Concrete
        }

        public CurrentCell AddHeadersCells(ExcelWorksheet worksheet)
        {
            var currentCell = _pavementTreatments.AddHeadersCells(worksheet);
            var columnNo = currentCell.Column;

            // Row 1
            int headerRow = 1;
            var headersRow1 = GetHeadersRow1();
            //var headersRow2 = GetHeadersRow2();

            var StressesColumn = headersRow1.IndexOf("OPI") + columnNo;

            StressHeaders(worksheet,StressesColumn,headerRow,headersRow1);

            // Add all Row 1 headers
            for (int column = 0; column < headersRow1.Count; column++)
            {
                worksheet.Cells[headerRow, column + columnNo].Style.WrapText = false;
                worksheet.Cells[headerRow, column + columnNo].Value = headersRow1[column];
            }

            // Add Bridge Funding cells for Row 2
            //for (int column = 0; column < headersRow2.Count; column++)
            //{
            //    worksheet.Cells[headerRow + 1, column + bridgeFundingColumn].Value = headersRow2[column];
            //}

            var row = headerRow;
            worksheet.Row(row).Height = 15;
            worksheet.Row(row + 1).Height = 15;
            // Autofit before the merges
            worksheet.Cells.AutoFitColumns(0);

            // Merge Bridge Funding cells in Row 1
            

            //// Merge rows for Columns after Bridge Funding
            //for (int cellColumn = analysisColumn; cellColumn <= worksheet.Dimension.Columns; cellColumn++)
            //{
            //    ExcelHelper.MergeCells(worksheet, row, cellColumn, row + 1, cellColumn);

            //    // Color condition headers
            //    if (headersRow1[cellColumn - columnNo].Contains("OPI") || headersRow1[cellColumn - columnNo].Contains("Fault"))
            //    {
            //        ExcelHelper.ApplyColor(worksheet.Cells[row, cellColumn], Color.FromArgb(255, 242, 204));
            //    }
            //}

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
                "OPI",
                "IRI",
                "Rut",
                "Fault"
            };
        }
        private void StressHeaders(ExcelWorksheet worksheet,int column,int row,List<string> stressHeaders)
        {
            for (int cell = 0; cell < stressHeaders.Count; cell++)
            {
                ExcelHelper.MergeCells(worksheet, row, column +cell , row + 1, column+cell);//stress header cells
            }

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
            _pavementTreatments.PerformPostAutofitAdjustments(worksheet);
        }
    }
}
