using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using AppliedResearchAssociates.iAM.Analysis.Engine;
using AppliedResearchAssociates.iAM.ExcelHelpers;
using AppliedResearchAssociates.iAM.Reporting.Models;
using MathNet.Numerics.Financial;
using OfficeOpenXml;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using OfficeOpenXml.FormulaParsing.Excel.Functions.RefAndLookup;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Text;
using OfficeOpenXml.Style;
using Org.BouncyCastle.Utilities.Encoders;

namespace AppliedResearchAssociates.iAM.Reporting.Services.BAMSPBExportReport.Treatments
{
    public class TreatmentForPBExportReport
    {
        private ReportHelper _reportHelper;

        public TreatmentForPBExportReport()
        {
            _reportHelper = new ReportHelper();
        }

        public void Fill(ExcelWorksheet worksheet, SimulationOutput reportOutputData)
        {
            //set default width
            worksheet.DefaultColWidth = 13;

            // Add data to excel.
            var dataHeaders = GetStaticDataHeaders();

            //add header
            var currentCell = AddDataHeadersCells(worksheet, dataHeaders);

            //add data to cells
            FillTreatmentData(worksheet, reportOutputData, currentCell);
        }

        #region Private Methods

        private List<string> GetStaticDataHeaders()
        {
            return new List<string>
            {
                "AssetType",
                "MaintainableAssetid",
                "AssetName",
                "District",
                "Cnty",
                "Route",
                "OddityDirection",
                "BRKEY",
                "BRIDGE_ID",
                "FromSection",
                "ToSection",
                "Offset",
                "OWNER_CODE",
                "INTERSTATE",
                "PreferredYear",
                "Appliedtreatment",
                "TreatmentFundingIgnoresSpendingLimit",
                "TreatmentStatus",
                "TreatmentStatusTreatmentCause",
                "Cost",
                "Benefit",
                "PriorityLevel",
                "RISK_SCORE",
                "RemainingLife",
                "SimulationOutputId",
                "SimulationId",
                "COUNTY"
            };
        }

        private CurrentCell AddDataHeadersCells(ExcelWorksheet worksheet, List<string> dataHeaders)
        {
            //section header
            int dataHeaderRow = 1; 

            //data header
            for (int column = 0; column < dataHeaders.Count; column++)
            {
                var dataHeaderText = dataHeaders[column];
                worksheet.Cells[dataHeaderRow, column + 1].Value = dataHeaderText;
            }

            var currentCell = new CurrentCell { Row = dataHeaderRow, Column = dataHeaders.Count };
            ExcelHelper.ApplyStyle(worksheet.Cells[dataHeaderRow + 1, dataHeaders.Count, dataHeaderRow + 1, dataHeaders.Count]);
            ExcelHelper.ApplyBorder(worksheet.Cells[dataHeaderRow, 1, dataHeaderRow + 1, worksheet.Dimension.Columns]);

            return currentCell;
        }

        private void FillTreatmentData(ExcelWorksheet worksheet, SimulationOutput reportOutputData, CurrentCell currentCell)
        {
            var rowNo = currentCell.Row;
            var columnNo = currentCell.Column;
            foreach (var sectionSummary in reportOutputData.InitialAssetSummaries)
            {
                rowNo++; columnNo = 1;

                worksheet.Cells[rowNo, columnNo++].Value = _reportHelper.CheckAndGetValue<string>(sectionSummary.ValuePerTextAttribute, "AssetType"); //Asset Type 
                worksheet.Cells[rowNo, columnNo++].Value = _reportHelper.CheckAndGetValue<string>(sectionSummary.ValuePerTextAttribute, "MaintainableAssetid"); //Maintainable Asset Id
                worksheet.Cells[rowNo, columnNo++].Value = _reportHelper.CheckAndGetValue<string>(sectionSummary.ValuePerTextAttribute, "AssetName"); //AssetName

                worksheet.Cells[rowNo, columnNo++].Value = _reportHelper.CheckAndGetValue<string>(sectionSummary.ValuePerTextAttribute, "District"); //District
                ExcelHelper.HorizontalRightAlign(worksheet.Cells[rowNo, columnNo - 1]);

                worksheet.Cells[rowNo, columnNo++].Value = _reportHelper.CheckAndGetValue<string>(sectionSummary.ValuePerTextAttribute, "Cnty"); //Cnty
                ExcelHelper.HorizontalRightAlign(worksheet.Cells[rowNo, columnNo - 1]);

                worksheet.Cells[rowNo, columnNo++].Value = _reportHelper.CheckAndGetValue<string>(sectionSummary.ValuePerTextAttribute, "Route"); //Route
                ExcelHelper.HorizontalRightAlign(worksheet.Cells[rowNo, columnNo - 1]);

                worksheet.Cells[rowNo, columnNo++].Value = _reportHelper.CheckAndGetValue<string>(sectionSummary.ValuePerTextAttribute, "OddityDirection"); //Oddity Direction
                ExcelHelper.HorizontalRightAlign(worksheet.Cells[rowNo, columnNo - 1]);

                worksheet.Cells[rowNo, columnNo++].Value = _reportHelper.CheckAndGetValue<double>(sectionSummary.ValuePerNumericAttribute, "BRKEY_"); //BRKey
                ExcelHelper.HorizontalRightAlign(worksheet.Cells[rowNo, columnNo - 1]);

                worksheet.Cells[rowNo, columnNo++].Value = _reportHelper.CheckAndGetValue<string>(sectionSummary.ValuePerTextAttribute, "BRIDGE_ID"); //Bridge ID
                ExcelHelper.HorizontalRightAlign(worksheet.Cells[rowNo, columnNo - 1]);

                worksheet.Cells[rowNo, columnNo++].Value = _reportHelper.CheckAndGetValue<string>(sectionSummary.ValuePerTextAttribute, "FromSection"); //From Section
                ExcelHelper.HorizontalRightAlign(worksheet.Cells[rowNo, columnNo - 1]);

                worksheet.Cells[rowNo, columnNo++].Value = _reportHelper.CheckAndGetValue<string>(sectionSummary.ValuePerTextAttribute, "ToSection"); //To Section
                ExcelHelper.HorizontalRightAlign(worksheet.Cells[rowNo, columnNo - 1]);

                worksheet.Cells[rowNo, columnNo++].Value = _reportHelper.CheckAndGetValue<string>(sectionSummary.ValuePerTextAttribute, "Offset"); //Offset
                ExcelHelper.HorizontalRightAlign(worksheet.Cells[rowNo, columnNo - 1]);

                worksheet.Cells[rowNo, columnNo++].Value = _reportHelper.CheckAndGetValue<string>(sectionSummary.ValuePerTextAttribute, "OWNER_CODE"); //Owner Code
                ExcelHelper.HorizontalRightAlign(worksheet.Cells[rowNo, columnNo - 1]);

                worksheet.Cells[rowNo, columnNo++].Value = _reportHelper.CheckAndGetValue<string>(sectionSummary.ValuePerTextAttribute, "INTERSTATE"); //Interstate

                worksheet.Cells[rowNo, columnNo++].Value = _reportHelper.CheckAndGetValue<string>(sectionSummary.ValuePerTextAttribute, "PreferredYear"); //Preferred Year
                ExcelHelper.HorizontalRightAlign(worksheet.Cells[rowNo, columnNo - 1]);

                worksheet.Cells[rowNo, columnNo++].Value = _reportHelper.CheckAndGetValue<string>(sectionSummary.ValuePerTextAttribute, "Appliedtreatment"); //Applied treatment
                worksheet.Cells[rowNo, columnNo++].Value = _reportHelper.CheckAndGetValue<string>(sectionSummary.ValuePerTextAttribute, "TreatmentFundingIgnoresSpendingLimit"); //Treatment Funding Ignores Spending Limit
                ExcelHelper.HorizontalRightAlign(worksheet.Cells[rowNo, columnNo - 1]);

                worksheet.Cells[rowNo, columnNo++].Value = _reportHelper.CheckAndGetValue<string>(sectionSummary.ValuePerTextAttribute, "TreatmentStatus\r\n"); //Treatment Status
                ExcelHelper.HorizontalRightAlign(worksheet.Cells[rowNo, columnNo - 1]);

                worksheet.Cells[rowNo, columnNo++].Value = _reportHelper.CheckAndGetValue<string>(sectionSummary.ValuePerTextAttribute, "TreatmentStatusTreatmentCause"); //Treatment Status Treatment Cause
                ExcelHelper.HorizontalRightAlign(worksheet.Cells[rowNo, columnNo - 1]);

                worksheet.Cells[rowNo, columnNo++].Value = _reportHelper.CheckAndGetValue<string>(sectionSummary.ValuePerTextAttribute, "Cost"); //Cost
                ExcelHelper.SetCurrencyFormat(worksheet.Cells[rowNo, columnNo - 1], ExcelFormatStrings.CurrencyWithoutCents);

                worksheet.Cells[rowNo, columnNo++].Value = _reportHelper.CheckAndGetValue<string>(sectionSummary.ValuePerTextAttribute, "Benefit"); //Benefit
                ExcelHelper.HorizontalRightAlign(worksheet.Cells[rowNo, columnNo - 1]);

                worksheet.Cells[rowNo, columnNo++].Value = _reportHelper.CheckAndGetValue<string>(sectionSummary.ValuePerTextAttribute, "PriorityLevel"); //Priority Level
                ExcelHelper.HorizontalRightAlign(worksheet.Cells[rowNo, columnNo - 1]);

                worksheet.Cells[rowNo, columnNo++].Value = _reportHelper.CheckAndGetValue<string>(sectionSummary.ValuePerTextAttribute, "RISK_SCORE"); //Risk Score
                ExcelHelper.HorizontalRightAlign(worksheet.Cells[rowNo, columnNo - 1]);
                ExcelHelper.SetCustomFormat(worksheet.Cells[rowNo, columnNo - 1], ExcelHelperCellFormat.Number);

                worksheet.Cells[rowNo, columnNo++].Value = _reportHelper.CheckAndGetValue<string>(sectionSummary.ValuePerTextAttribute, "RemainingLife"); //Remaining Life
                worksheet.Cells[rowNo, columnNo++].Value = _reportHelper.CheckAndGetValue<string>(sectionSummary.ValuePerTextAttribute, "SimulationOutputId"); //Simulation Output Id
                worksheet.Cells[rowNo, columnNo++].Value = _reportHelper.CheckAndGetValue<string>(sectionSummary.ValuePerTextAttribute, "SimulationId"); //Simulation Id
                worksheet.Cells[rowNo, columnNo++].Value = _reportHelper.CheckAndGetValue<string>(sectionSummary.ValuePerTextAttribute, "COUNTY"); //County

                if (rowNo % 2 == 0)
                {
                    ExcelHelper.ApplyColor(worksheet.Cells[rowNo, 1, rowNo, columnNo], Color.LightGray);
                }
            }
            currentCell.Row = rowNo;
            currentCell.Column = columnNo;
        }

        #endregion Private Methods
    }
}
