using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;
using AppliedResearchAssociates.iAM.Analysis;
using AppliedResearchAssociates.iAM.Analysis.Engine;
using AppliedResearchAssociates.iAM.ExcelHelpers;
using AppliedResearchAssociates.iAM.Reporting.Models;
using AppliedResearchAssociates.iAM.Reporting.Services.BAMSSummaryReport;
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

        public void Fill(ExcelWorksheet worksheet, Simulation simulationObject, SimulationOutput reportOutputData)
        {
            //set default width
            worksheet.DefaultColWidth = 13;

            // Add data to excel.
            var dataHeaders = GetStaticDataHeaders();

            //add header
            var currentCell = AddDataHeadersCells(worksheet, dataHeaders);

            //add data to cells
            FillDynamicDataForHeaders(worksheet, simulationObject, reportOutputData, currentCell);
        }

        #region Private Methods

        private List<string> GetStaticDataHeaders()
        {
            return new List<string>
            {
                "AssetId",
                "Asset",
                "District",
                "Cnty",
                "Route",
                "Direction",
                "FromSection",
                "ToSection",
                "Offset",
                "Interstate",
                "Treatment",
                "Benefit",
                "Cost",
                "Risk",
                "IsCommitted",
                "PriorityOrder",
                "PreferredYear",
                "MinYear",
                "MaxYear",
                "BRKEY",
                "SimulationId",
                "SimulationOutputId",
                "NetworkId",
                "ToDelete",
                "OWNER_CODE",
                "CATEGORY",
                "YEARANY",
                "YEARSAME",
                "BUDGET",
                "AGE",
                "LATX_CNT",
                "WS_SEEDED",
                "CULV_DURATION_N",
                "DECK_DURATION_N",
                "SUP_DURATION_N",
                "SUB_DURATION_N",
                "CULV_SEEDED",
                "DECK_SEEDED",
                "SUP_SEEDED",
                "SUB_SEEDED",
                "TreatmentFundingIgnoresSpendingLimit",
                "TreatmentStatus",
                "TreatmentCause",
                "TreatmentConsiderationDetailId",
                "TreatmentOptionDetailId",
                "RemainingLife",
                "AssetDetailId",
                "SimulationYearDetailId"
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
            ExcelHelper.ApplyBorder(worksheet.Cells[dataHeaderRow, 1, dataHeaderRow, worksheet.Dimension.Columns]);

            return currentCell;
        }

        private void FillDynamicDataForHeaders(ExcelWorksheet worksheet, Simulation simulationObject, SimulationOutput reportOutputData, CurrentCell currentCell)
        {
            var rowNo = currentCell.Row;
            var columnNo = currentCell.Column;

            if (reportOutputData?.Years?.Any() == true)
            {
                foreach (var yearObject in reportOutputData.Years)
                {
                    //filter assets for no treatment records
                    
                    var filteredAssetDetails = yearObject.Assets
                                                        .Where(w => w.AppliedTreatment != BAMSConstants.NoTreatmentForWorkSummary
                                                            && !string.IsNullOrEmpty(w.AppliedTreatment)
                                                            && !string.IsNullOrWhiteSpace(w.AppliedTreatment))
                                                        .ToList();

                    if (filteredAssetDetails?.Any() == true)
                    {
                        foreach (var assetDetailObject in filteredAssetDetails)
                        {
                            rowNo++; columnNo = 1;

                            var bmsID = _reportHelper.CheckAndGetValue<string>(assetDetailObject.ValuePerTextAttribute, "BMSID"); //BMSID

                            worksheet.Cells[rowNo, columnNo++].Value = assetDetailObject.AssetId.ToString(); //Asset Id
                            worksheet.Cells[rowNo, columnNo++].Value = assetDetailObject.AssetName.ToString(); //Asset

                            worksheet.Cells[rowNo, columnNo++].Value = _reportHelper.CheckAndGetValue<string>(assetDetailObject.ValuePerTextAttribute, "DISTRICT"); //District
                            ExcelHelper.HorizontalRightAlign(worksheet.Cells[rowNo, columnNo - 1]);

                            var cnty = bmsID.PadLeft(14, '0').Substring(0, 2);
                            worksheet.Cells[rowNo, columnNo++].Value = cnty; //Cnty
                            ExcelHelper.HorizontalRightAlign(worksheet.Cells[rowNo, columnNo - 1]);

                            worksheet.Cells[rowNo, columnNo++].Value = _reportHelper.CheckAndGetValue<string>(assetDetailObject.ValuePerTextAttribute, "ROUTENUM"); //Route
                            ExcelHelper.HorizontalRightAlign(worksheet.Cells[rowNo, columnNo - 1]);

                            worksheet.Cells[rowNo, columnNo++].Value = _reportHelper.CheckAndGetValue<string>(assetDetailObject.ValuePerTextAttribute, "Direction"); //Direction
                            ExcelHelper.HorizontalRightAlign(worksheet.Cells[rowNo, columnNo - 1]);

                            worksheet.Cells[rowNo, columnNo++].Value = _reportHelper.CheckAndGetValue<string>(assetDetailObject.ValuePerTextAttribute, "FromSection"); //From Section
                            ExcelHelper.HorizontalRightAlign(worksheet.Cells[rowNo, columnNo - 1]);

                            worksheet.Cells[rowNo, columnNo++].Value = _reportHelper.CheckAndGetValue<string>(assetDetailObject.ValuePerTextAttribute, "ToSection"); //To Section
                            ExcelHelper.HorizontalRightAlign(worksheet.Cells[rowNo, columnNo - 1]);

                            worksheet.Cells[rowNo, columnNo++].Value = _reportHelper.CheckAndGetValue<string>(assetDetailObject.ValuePerTextAttribute, "Offset"); //Offset
                            ExcelHelper.HorizontalRightAlign(worksheet.Cells[rowNo, columnNo - 1]);

                            worksheet.Cells[rowNo, columnNo++].Value = _reportHelper.CheckAndGetValue<string>(assetDetailObject.ValuePerTextAttribute, "INTERSTATE"); //Interstate

                            worksheet.Cells[rowNo, columnNo++].Value = assetDetailObject.AppliedTreatment; //Treatment

                            worksheet.Cells[rowNo, columnNo++].Value = _reportHelper.CheckAndGetValue<string>(assetDetailObject.ValuePerTextAttribute, "Benefit"); //Benefit
                            ExcelHelper.HorizontalRightAlign(worksheet.Cells[rowNo, columnNo - 1]);

                            worksheet.Cells[rowNo, columnNo++].Value = _reportHelper.CheckAndGetValue<string>(assetDetailObject.ValuePerTextAttribute, "Cost"); //Cost
                            ExcelHelper.SetCurrencyFormat(worksheet.Cells[rowNo, columnNo - 1], ExcelFormatStrings.CurrencyWithoutCents);

                            worksheet.Cells[rowNo, columnNo++].Value = _reportHelper.CheckAndGetValue<double>(assetDetailObject.ValuePerNumericAttribute, "RISK_SCORE"); //Risk
                            ExcelHelper.HorizontalRightAlign(worksheet.Cells[rowNo, columnNo - 1]);
                            ExcelHelper.SetCustomFormat(worksheet.Cells[rowNo, columnNo - 1], ExcelHelperCellFormat.Number);

                            worksheet.Cells[rowNo, columnNo++].Value = _reportHelper.CheckAndGetValue<string>(assetDetailObject.ValuePerTextAttribute, "IsCommitted"); //IsCommitted
                            ExcelHelper.HorizontalRightAlign(worksheet.Cells[rowNo, columnNo - 1]);

                            worksheet.Cells[rowNo, columnNo++].Value = _reportHelper.CheckAndGetValue<string>(assetDetailObject.ValuePerTextAttribute, "PriorityOrder"); //PriorityOrder
                            ExcelHelper.HorizontalRightAlign(worksheet.Cells[rowNo, columnNo - 1]);

                            worksheet.Cells[rowNo, columnNo++].Value = yearObject.Year.ToString(); //Preferred Year
                            ExcelHelper.HorizontalRightAlign(worksheet.Cells[rowNo, columnNo - 1]);

                            worksheet.Cells[rowNo, columnNo++].Value = _reportHelper.CheckAndGetValue<string>(assetDetailObject.ValuePerTextAttribute, "MinYear"); //MinYear

                            worksheet.Cells[rowNo, columnNo++].Value = _reportHelper.CheckAndGetValue<string>(assetDetailObject.ValuePerTextAttribute, "MaxYear"); //MaxYear

                            worksheet.Cells[rowNo, columnNo++].Value = _reportHelper.CheckAndGetValue<double>(assetDetailObject.ValuePerNumericAttribute, "BRKEY_"); //BRKey
                            ExcelHelper.HorizontalRightAlign(worksheet.Cells[rowNo, columnNo - 1]);

                            worksheet.Cells[rowNo, columnNo++].Value = simulationObject.Id.ToString(); //SimulationId

                            worksheet.Cells[rowNo, columnNo++].Value = _reportHelper.CheckAndGetValue<string>(assetDetailObject.ValuePerTextAttribute, "SimulationOutputId"); //SimulationOutputId

                            worksheet.Cells[rowNo, columnNo++].Value = simulationObject?.Network?.Id.ToString(); //NetworkId

                            worksheet.Cells[rowNo, columnNo++].Value = _reportHelper.CheckAndGetValue<string>(assetDetailObject.ValuePerTextAttribute, "ToDelete"); //ToDelete

                            worksheet.Cells[rowNo, columnNo++].Value = _reportHelper.CheckAndGetValue<string>(assetDetailObject.ValuePerTextAttribute, "OWNER_CODE"); //Owner Code
                            ExcelHelper.HorizontalRightAlign(worksheet.Cells[rowNo, columnNo - 1]);

                            worksheet.Cells[rowNo, columnNo++].Value = _reportHelper.CheckAndGetValue<string>(assetDetailObject.ValuePerTextAttribute, "CATEGORY"); //CATEGORY

                            worksheet.Cells[rowNo, columnNo++].Value = _reportHelper.CheckAndGetValue<string>(assetDetailObject.ValuePerTextAttribute, "YEARANY"); //YEARANY

                            worksheet.Cells[rowNo, columnNo++].Value = _reportHelper.CheckAndGetValue<string>(assetDetailObject.ValuePerTextAttribute, "YEARSAME"); //YEARSAME

                            worksheet.Cells[rowNo, columnNo++].Value = _reportHelper.CheckAndGetValue<string>(assetDetailObject.ValuePerTextAttribute, "BUDGET"); //BUDGET


                            worksheet.Cells[rowNo, columnNo++].Value = _reportHelper.CheckAndGetValue<double>(assetDetailObject.ValuePerNumericAttribute, "AGE"); //AGE

                            worksheet.Cells[rowNo, columnNo++].Value = _reportHelper.CheckAndGetValue<double>(assetDetailObject.ValuePerNumericAttribute, "LATX_CNT"); //LATX_CNT



                            worksheet.Cells[rowNo, columnNo++].Value = _reportHelper.CheckAndGetValue<double>(assetDetailObject.ValuePerNumericAttribute, "WS_SEEDED"); //WS_SEEDED

                            worksheet.Cells[rowNo, columnNo++].Value = _reportHelper.CheckAndGetValue<double>(assetDetailObject.ValuePerNumericAttribute, "CULV_DURATION_N"); //CULV_DURATION_N

                            worksheet.Cells[rowNo, columnNo++].Value = _reportHelper.CheckAndGetValue<double>(assetDetailObject.ValuePerNumericAttribute, "DECK_DURATION_N"); //DECK_DURATION_N

                            worksheet.Cells[rowNo, columnNo++].Value = _reportHelper.CheckAndGetValue<double>(assetDetailObject.ValuePerNumericAttribute, "SUP_DURATION_N"); //SUP_DURATION_N

                            worksheet.Cells[rowNo, columnNo++].Value = _reportHelper.CheckAndGetValue<double>(assetDetailObject.ValuePerNumericAttribute, "SUB_DURATION_N"); //SUB_DURATION_N

                            worksheet.Cells[rowNo, columnNo++].Value = _reportHelper.CheckAndGetValue<double>(assetDetailObject.ValuePerNumericAttribute, "CULV_SEEDED"); //CULV_SEEDED

                            worksheet.Cells[rowNo, columnNo++].Value = _reportHelper.CheckAndGetValue<double>(assetDetailObject.ValuePerNumericAttribute, "DECK_SEEDED"); //DECK_SEEDED

                            worksheet.Cells[rowNo, columnNo++].Value = _reportHelper.CheckAndGetValue<double>(assetDetailObject.ValuePerNumericAttribute, "SUP_SEEDED"); //SUP_SEEDED

                            worksheet.Cells[rowNo, columnNo++].Value = _reportHelper.CheckAndGetValue<double>(assetDetailObject.ValuePerNumericAttribute, "SUB_SEEDED"); //SUB_SEEDED


                            worksheet.Cells[rowNo, columnNo++].Value = assetDetailObject.TreatmentFundingIgnoresSpendingLimit == true ? 1 : 0; //TreatmentFundingIgnoresSpendingLimit

                            worksheet.Cells[rowNo, columnNo++].Value = assetDetailObject.TreatmentStatus.ToString(); //TreatmentStatus

                            worksheet.Cells[rowNo, columnNo++].Value = assetDetailObject.TreatmentCause.ToString(); //TreatmentCause

                            worksheet.Cells[rowNo, columnNo++].Value = _reportHelper.CheckAndGetValue<string>(assetDetailObject.ValuePerTextAttribute, "TreatmentConsiderationDetailId"); //TreatmentConsiderationDetailId

                            worksheet.Cells[rowNo, columnNo++].Value = _reportHelper.CheckAndGetValue<string>(assetDetailObject.ValuePerTextAttribute, "TreatmentOptionDetailId"); //TreatmentOptionDetailId

                            worksheet.Cells[rowNo, columnNo++].Value = _reportHelper.CheckAndGetValue<string>(assetDetailObject.ValuePerTextAttribute, "RemainingLife"); //Remaining Life

                            worksheet.Cells[rowNo, columnNo++].Value = _reportHelper.CheckAndGetValue<string>(assetDetailObject.ValuePerTextAttribute, "AssetDetailId"); //AssetDetailId

                            worksheet.Cells[rowNo, columnNo++].Value = _reportHelper.CheckAndGetValue<string>(assetDetailObject.ValuePerTextAttribute, "SimulationYearDetailId"); //SimulationYearDetailId

                            if (rowNo % 2 == 0) { ExcelHelper.ApplyColor(worksheet.Cells[rowNo, 1, rowNo, columnNo], Color.LightGray); }
                        }
                    }
                }
            }            

            currentCell.Row = rowNo;
            currentCell.Column = columnNo;
        }

        #endregion Private Methods
    }
}
