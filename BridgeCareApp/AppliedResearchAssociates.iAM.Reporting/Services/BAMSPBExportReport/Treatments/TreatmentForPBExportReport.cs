using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using AppliedResearchAssociates.iAM.Analysis;
using AppliedResearchAssociates.iAM.Analysis.Engine;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.ExcelHelpers;
using AppliedResearchAssociates.iAM.Reporting.Common;
using AppliedResearchAssociates.iAM.Reporting.Models;
using AppliedResearchAssociates.iAM.Reporting.Services.BAMSSummaryReport;
using OfficeOpenXml;
using static System.Collections.Specialized.BitVector32;
using static AppliedResearchAssociates.iAM.Analysis.Engine.FundingCalculationOutput;

namespace AppliedResearchAssociates.iAM.Reporting.Services.BAMSPBExportReport.Treatments
{
    public class TreatmentForPBExportReport
    {
        private ReportHelper _reportHelper;
        private readonly IUnitOfWork _unitOfWork;

        public TreatmentForPBExportReport(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _reportHelper = new ReportHelper(_unitOfWork);
        }

        public void Fill(ExcelWorksheet worksheet, Simulation simulationObject, SimulationOutput reportOutputData, bool shouldBundleFeasibleTreatments)
        {
            //set default width
            worksheet.DefaultColWidth = 13;

            // Add data to excel.
            var dataHeaders = GetStaticDataHeaders();

            //add header
            var headerRow = 1; var headerBGColor = ColorTranslator.FromHtml("#FFD966");
            var currentCell = AddDataHeadersCells(worksheet, dataHeaders);
            ExcelHelper.ApplyColor(worksheet.Cells[headerRow, 1, headerRow, worksheet.Dimension.Columns], headerBGColor);

            //add data to cells
            FillDynamicDataForHeaders(worksheet, simulationObject, reportOutputData, currentCell, shouldBundleFeasibleTreatments);

            //autofit columns
            worksheet.Cells.AutoFitColumns();

            //Apply border
            ExcelHelper.ApplyBorder(worksheet.Cells[headerRow, 1, currentCell.Row, worksheet.Dimension.Columns]);
        }

        #region Private Methods

        private List<string> GetStaticDataHeaders()
        {
            return new List<string>
            {
                "SimulationId",
                "NetworkId",
                "AssetId",                
                "District",
                "Cnty",
                "Route",
                "Asset",
                "Direction",
                "Segment",
                "Year",
                "MinYear",
                "MaxYear",
                "Treatment",
                "Cost",
                "Benefit",
                "Risk",
                "RemainingLife",
                "PriorityOrder",
                "TreatmentFundingIgnoresSpendingLimit",
                "TreatmentStatus",
                "TreatmentCause",
                "Budget",
                "Category",

                "Offset",
                "Interstate", 
                                
                "BRKEY",                
                "OWNER_CODE",                
                "YEARANY",
                "YEARSAME",                
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
                "SUB_SEEDED"  
            };
        }

        private CurrentCell AddDataHeadersCells(ExcelWorksheet worksheet, List<string> dataHeaders)
        {
            //assetDetailObject header
            int dataHeaderRow = 1; 

            //data header
            for (int column = 0; column < dataHeaders.Count; column++)
            {
                var dataHeaderText = dataHeaders[column];
                worksheet.Cells[dataHeaderRow, column + 1].Value = dataHeaderText;
            }

            var currentCell = new CurrentCell { Row = dataHeaderRow, Column = dataHeaders.Count };
            ExcelHelper.ApplyBorder(worksheet.Cells[dataHeaderRow, 1, dataHeaderRow + 1, worksheet.Dimension.Columns]);

            return currentCell;
        }

        private void FillDynamicDataForHeaders(ExcelWorksheet worksheet, Simulation simulationObject, SimulationOutput reportOutputData, CurrentCell currentCell, bool shouldBundleFeasibleTreatments)
        {
            var rowNo = currentCell.Row;
            var columnNo = currentCell.Column;

            if (reportOutputData?.Years?.Any() == true)
            {
                Dictionary<double, List<TreatmentConsiderationDetail>> keyCashFlowFundingDetails = new();
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
                            var brKey = _reportHelper.CheckAndGetValue<double>(assetDetailObject.ValuePerNumericAttribute, "BRKEY_");
                            var bmsID = _reportHelper.CheckAndGetValue<string>(assetDetailObject.ValuePerTextAttribute, "BMSID"); //BMSID
                            if (!string.IsNullOrEmpty(bmsID) && !string.IsNullOrWhiteSpace(bmsID)) { bmsID = bmsID.PadLeft(14, '0'); } // chaeck and add padding to BMSID

                            // Build keyCashFlowFundingDetails                    
                            _reportHelper.BuildKeyCashFlowFundingDetails(yearObject, assetDetailObject, brKey, keyCashFlowFundingDetails);

                            //get budget usages
                            // If CF then use obj from keyCashFlowFundingDetails otherwise from assetDetailObject
                            var treatmentConsiderations = ((assetDetailObject.TreatmentCause == TreatmentCause.SelectedTreatment &&
                                                          assetDetailObject.TreatmentStatus == TreatmentStatus.Progressed) ||
                                                          (assetDetailObject.TreatmentCause == TreatmentCause.CashFlowProject &&
                                                          assetDetailObject.TreatmentStatus == TreatmentStatus.Progressed) ||
                                                          (assetDetailObject.TreatmentCause == TreatmentCause.CashFlowProject &&
                                                          assetDetailObject.TreatmentStatus == TreatmentStatus.Applied)) ?
                                                          keyCashFlowFundingDetails[brKey] :
                                                          assetDetailObject.TreatmentConsiderations ?? new();

                            var treatmentConsideration = shouldBundleFeasibleTreatments ?
                                                         treatmentConsiderations.FirstOrDefault(_ => _.FundingCalculationOutput != null &&
                                                            _.FundingCalculationOutput.AllocationMatrix.Any(_ => _.Year == yearObject.Year) &&
                                                            assetDetailObject.AppliedTreatment.Contains(_.TreatmentName)) :
                                                         treatmentConsiderations.FirstOrDefault(_ => _.FundingCalculationOutput != null &&
                                                            _.FundingCalculationOutput.AllocationMatrix.Any(_ => _.Year == yearObject.Year) &&
                                                            _.TreatmentName == assetDetailObject.AppliedTreatment);

                            var cost = treatmentConsideration == null ? 0 : Math.Round(treatmentConsideration.FundingCalculationOutput?.AllocationMatrix.Where(_ => _.Year == yearObject.Year)?.Sum(b => b.AllocatedAmount) ?? 0, 0); // Rounded cost to whole number based on comments from Jeff Davis
                            var appliedTreatment = assetDetailObject.AppliedTreatment ?? "";

                            SelectableTreatment treatment = null;
                            var treatments = simulationObject.Treatments?.Where(_ => _.Name == appliedTreatment).ToList();
                            if (treatments?.Count == 1) { treatment = treatments.First(); }

                            TreatmentOptionDetail treatmentOptionDetail = null;
                            var treatmentOptions = assetDetailObject.TreatmentOptions?.FindAll(_ => _.TreatmentName == appliedTreatment);
                            if(treatmentOptions?.Count == 1) { treatmentOptionDetail = treatmentOptions.First(); }

                            TreatmentConsiderationDetail treatmentConsiderationDetail = null;
                            var treatmentConsiderations_ = treatmentConsiderations?.FindAll(_ => _.TreatmentName == appliedTreatment);
                            if (treatmentConsiderations?.Count == 1) { treatmentConsiderationDetail = treatmentConsiderations_.First(); }

                            //get budget usages
                            var allocations = new List<Allocation>();
                            var allocationMatrices = treatmentConsiderations.Select(_ => _.FundingCalculationOutput?.AllocationMatrix.Where(_ => _.Year == yearObject.Year)).ToList() ?? new();
                            foreach (var allocationMatrix in allocationMatrices)
                            {
                                if (allocationMatrix != null && allocationMatrix.Any())
                                {
                                    allocations.AddRange(allocationMatrix);
                                }
                            }

                            //check budget usages
                            var budgetName = "";
                            if (allocationMatrices?.Any() == true && cost > 0)
                            {
                                var budgetNames = allocations.Select(_ => _.BudgetName).Distinct().ToList();
                                if (allocationMatrices.Count == 1 && budgetNames.Count() == 1) //single budget
                                {
                                    budgetName = budgetNames.First() ?? ""; // Budget
                                    if (string.IsNullOrEmpty(budgetName) || string.IsNullOrWhiteSpace(budgetName))
                                    {
                                        budgetName = BAMSConstants.Unspecified_Budget;
                                    }
                                }
                                else //multiple budgets
                                {
                                    //check for multi year budget
                                    var allowFundingFromMultipleBudgets = simulationObject?.AnalysisMethod?.AllowFundingFromMultipleBudgets ?? false;
                                    if (allowFundingFromMultipleBudgets == true || budgetNames.Count > 1)
                                    {
                                        foreach (var allocationBudgetName in budgetNames)
                                        {
                                            var multiYearBudgetCost = allocations.Where(_ => _.BudgetName == allocationBudgetName).Sum(_ => _.AllocatedAmount);
                                            var multiYearBudgetName = allocationBudgetName ?? ""; // Budget;
                                            if (string.IsNullOrEmpty(multiYearBudgetName) || string.IsNullOrWhiteSpace(multiYearBudgetName))
                                            {
                                                multiYearBudgetName = BAMSConstants.Unspecified_Budget;
                                            }

                                            var budgetAmountAbbrName = "";
                                            if (multiYearBudgetCost > 0)
                                            {
                                                //check budget and add abbreviation
                                                budgetAmountAbbrName = "$" + ReportCommon.FormatNumber(multiYearBudgetCost, 1);

                                                //set budget header name
                                                if (!string.IsNullOrEmpty(budgetAmountAbbrName) && !string.IsNullOrWhiteSpace(budgetAmountAbbrName))
                                                {
                                                    multiYearBudgetName += " (" + budgetAmountAbbrName + ")";
                                                }

                                                if (string.IsNullOrEmpty(allocationBudgetName) || string.IsNullOrWhiteSpace(allocationBudgetName))
                                                {
                                                    budgetName = multiYearBudgetName;
                                                }
                                                else
                                                {
                                                    budgetName += ", " + multiYearBudgetName;
                                                }
                                            }
                                        }
                                    }
                                }
                            }

                            worksheet.Cells[rowNo, columnNo++].Value = simulationObject.Id.ToString(); //SimulationId
                            worksheet.Cells[rowNo, columnNo++].Value = simulationObject?.Network?.Id.ToString(); //NetworkId
                            worksheet.Cells[rowNo, columnNo++].Value = assetDetailObject.AssetId.ToString(); //Asset Id
                                                       
                            worksheet.Cells[rowNo, columnNo++].Value = _reportHelper.CheckAndGetValue<string>(assetDetailObject.ValuePerTextAttribute, "DISTRICT"); //District
                            ExcelHelper.HorizontalRightAlign(worksheet.Cells[rowNo, columnNo - 1]);

                            var cnty = "";
                            if (!string.IsNullOrEmpty(bmsID) && !string.IsNullOrWhiteSpace(bmsID)) {
                                if (bmsID.Length > 2) { cnty = bmsID.Substring(0, 2);  }
                            }                                                        
                            worksheet.Cells[rowNo, columnNo++].Value = cnty; //Cnty
                            ExcelHelper.HorizontalRightAlign(worksheet.Cells[rowNo, columnNo - 1]);

                            worksheet.Cells[rowNo, columnNo++].Value = _reportHelper.CheckAndGetValue<double>(assetDetailObject.ValuePerNumericAttribute, "ROUTENUM"); //Route
                            ExcelHelper.HorizontalRightAlign(worksheet.Cells[rowNo, columnNo - 1]);

                            worksheet.Cells[rowNo, columnNo++].Value = _reportHelper.CheckAndGetValue<double>(assetDetailObject.ValuePerNumericAttribute, "BRKEY_"); //AssetName

                            //TODO: Get value for direction column
                            worksheet.Cells[rowNo, columnNo++].Value = "0"; //Direction
                            ExcelHelper.HorizontalRightAlign(worksheet.Cells[rowNo, columnNo - 1]);

                            worksheet.Cells[rowNo, columnNo++].Value = _reportHelper.CheckAndGetValue<double>(assetDetailObject.ValuePerNumericAttribute, "SEGMENT"); //Segment
                            ExcelHelper.HorizontalRightAlign(worksheet.Cells[rowNo, columnNo - 1]);

                            worksheet.Cells[rowNo, columnNo++].Value = yearObject.Year.ToString(); //Year
                            ExcelHelper.HorizontalRightAlign(worksheet.Cells[rowNo, columnNo - 1]);

                            //TODO: Get value for the column
                            worksheet.Cells[rowNo, columnNo++].Value = yearObject.Year.ToString(); //MinYear
                            ExcelHelper.HorizontalRightAlign(worksheet.Cells[rowNo, columnNo - 1]);

                            //TODO: Get value for the column
                            worksheet.Cells[rowNo, columnNo++].Value = yearObject.Year.ToString(); //MaxYear
                            ExcelHelper.HorizontalRightAlign(worksheet.Cells[rowNo, columnNo - 1]);

                            worksheet.Cells[rowNo, columnNo++].Value = appliedTreatment; //Treatment

                            worksheet.Cells[rowNo, columnNo++].Value = cost; //Cost
                            ExcelHelper.HorizontalRightAlign(worksheet.Cells[rowNo, columnNo - 1]);
                            ExcelHelper.SetCurrencyFormat(worksheet.Cells[rowNo, columnNo - 1], ExcelFormatStrings.CurrencyWithoutCents);

                            worksheet.Cells[rowNo, columnNo++].Value = treatmentOptionDetail?.Benefit ?? 0; //Benefit
                            ExcelHelper.HorizontalRightAlign(worksheet.Cells[rowNo, columnNo - 1]);

                            worksheet.Cells[rowNo, columnNo++].Value = _reportHelper.CheckAndGetValue<double>(assetDetailObject.ValuePerNumericAttribute, "RISK_SCORE"); //Risk
                            ExcelHelper.HorizontalRightAlign(worksheet.Cells[rowNo, columnNo - 1]);
                            ExcelHelper.SetCustomFormat(worksheet.Cells[rowNo, columnNo - 1], ExcelHelperCellFormat.Number);

                            worksheet.Cells[rowNo, columnNo++].Value = treatmentOptionDetail?.RemainingLife ?? 0; //Remaining Life
                            ExcelHelper.HorizontalRightAlign(worksheet.Cells[rowNo, columnNo - 1]);

                            worksheet.Cells[rowNo, columnNo++].Value = treatmentConsiderationDetail?.BudgetPriorityLevel ?? 0; //PriorityLevel
                            ExcelHelper.HorizontalRightAlign(worksheet.Cells[rowNo, columnNo - 1]);

                            worksheet.Cells[rowNo, columnNo++].Value = assetDetailObject.TreatmentFundingIgnoresSpendingLimit == true ? 1 : 0; //TreatmentFundingIgnoresSpendingLimit
                            ExcelHelper.HorizontalRightAlign(worksheet.Cells[rowNo, columnNo - 1]);

                            worksheet.Cells[rowNo, columnNo++].Value = assetDetailObject.TreatmentStatus.ToString(); //TreatmentStatus
                            ExcelHelper.HorizontalRightAlign(worksheet.Cells[rowNo, columnNo - 1]);

                            worksheet.Cells[rowNo, columnNo++].Value = assetDetailObject.TreatmentCause.ToString(); //TreatmentCause
                            ExcelHelper.HorizontalRightAlign(worksheet.Cells[rowNo, columnNo - 1]);

                            worksheet.Cells[rowNo, columnNo++].Value = budgetName; //BUDGET

                            worksheet.Cells[rowNo, columnNo++].Value = treatment?.Category.ToString() ?? ""; //CATEGORY


                            var offset = "";
                            if (!string.IsNullOrEmpty(bmsID) && !string.IsNullOrWhiteSpace(bmsID)) {
                                if(bmsID.Length > 4) { offset = bmsID.Substring(bmsID.Length - 4); }
                            }
                            worksheet.Cells[rowNo, columnNo++].Value = offset; //Offset
                            ExcelHelper.HorizontalRightAlign(worksheet.Cells[rowNo, columnNo - 1]);

                            worksheet.Cells[rowNo, columnNo++].Value = _reportHelper.CheckAndGetValue<string>(assetDetailObject.ValuePerTextAttribute, "INTERSTATE"); //Interstate
                            ExcelHelper.HorizontalRightAlign(worksheet.Cells[rowNo, columnNo - 1]);                            
                                                        

                            worksheet.Cells[rowNo, columnNo++].Value = _reportHelper.CheckAndGetValue<double>(assetDetailObject.ValuePerNumericAttribute, "BRKEY_"); //BRKey
                            ExcelHelper.HorizontalRightAlign(worksheet.Cells[rowNo, columnNo - 1]);                            

                            worksheet.Cells[rowNo, columnNo++].Value = _reportHelper.CheckAndGetValue<string>(assetDetailObject.ValuePerTextAttribute, "OWNER_CODE"); //Owner Code
                            ExcelHelper.HorizontalRightAlign(worksheet.Cells[rowNo, columnNo - 1]);                                                       

                            worksheet.Cells[rowNo, columnNo++].Value = treatment?.ShadowForAnyTreatment ?? 0; //YEARANY
                            ExcelHelper.HorizontalRightAlign(worksheet.Cells[rowNo, columnNo - 1]);

                            worksheet.Cells[rowNo, columnNo++].Value = treatment?.ShadowForSameTreatment ?? 0; //YEARSAME
                            ExcelHelper.HorizontalRightAlign(worksheet.Cells[rowNo, columnNo - 1]);
                            

                            worksheet.Cells[rowNo, columnNo++].Value = _reportHelper.CheckAndGetValue<double>(assetDetailObject.ValuePerNumericAttribute, "AGE"); //AGE
                            ExcelHelper.HorizontalRightAlign(worksheet.Cells[rowNo, columnNo - 1]);

                            worksheet.Cells[rowNo, columnNo++].Value = _reportHelper.CheckAndGetValue<double>(assetDetailObject.ValuePerNumericAttribute, "LATX_CNT"); //LATX_CNT
                            ExcelHelper.HorizontalRightAlign(worksheet.Cells[rowNo, columnNo - 1]);

                            worksheet.Cells[rowNo, columnNo++].Value = _reportHelper.CheckAndGetValue<double>(assetDetailObject.ValuePerNumericAttribute, "WS_SEEDED"); //WS_SEEDED
                            ExcelHelper.HorizontalRightAlign(worksheet.Cells[rowNo, columnNo - 1]);

                            worksheet.Cells[rowNo, columnNo++].Value = _reportHelper.CheckAndGetValue<double>(assetDetailObject.ValuePerNumericAttribute, "CULV_DURATION_N"); //CULV_DURATION_N
                            ExcelHelper.HorizontalRightAlign(worksheet.Cells[rowNo, columnNo - 1]);

                            worksheet.Cells[rowNo, columnNo++].Value = _reportHelper.CheckAndGetValue<double>(assetDetailObject.ValuePerNumericAttribute, "DECK_DURATION_N"); //DECK_DURATION_N
                            ExcelHelper.HorizontalRightAlign(worksheet.Cells[rowNo, columnNo - 1]);

                            worksheet.Cells[rowNo, columnNo++].Value = _reportHelper.CheckAndGetValue<double>(assetDetailObject.ValuePerNumericAttribute, "SUP_DURATION_N"); //SUP_DURATION_N
                            ExcelHelper.HorizontalRightAlign(worksheet.Cells[rowNo, columnNo - 1]);

                            worksheet.Cells[rowNo, columnNo++].Value = _reportHelper.CheckAndGetValue<double>(assetDetailObject.ValuePerNumericAttribute, "SUB_DURATION_N"); //SUB_DURATION_N
                            ExcelHelper.HorizontalRightAlign(worksheet.Cells[rowNo, columnNo - 1]);

                            worksheet.Cells[rowNo, columnNo++].Value = _reportHelper.CheckAndGetValue<double>(assetDetailObject.ValuePerNumericAttribute, "CULV_SEEDED"); //CULV_SEEDED
                            ExcelHelper.HorizontalRightAlign(worksheet.Cells[rowNo, columnNo - 1]);

                            worksheet.Cells[rowNo, columnNo++].Value = _reportHelper.CheckAndGetValue<double>(assetDetailObject.ValuePerNumericAttribute, "DECK_SEEDED"); //DECK_SEEDED
                            ExcelHelper.HorizontalRightAlign(worksheet.Cells[rowNo, columnNo - 1]);

                            worksheet.Cells[rowNo, columnNo++].Value = _reportHelper.CheckAndGetValue<double>(assetDetailObject.ValuePerNumericAttribute, "SUP_SEEDED"); //SUP_SEEDED
                            ExcelHelper.HorizontalRightAlign(worksheet.Cells[rowNo, columnNo - 1]);

                            worksheet.Cells[rowNo, columnNo++].Value = _reportHelper.CheckAndGetValue<double>(assetDetailObject.ValuePerNumericAttribute, "SUB_SEEDED"); //SUB_SEEDED
                            ExcelHelper.HorizontalRightAlign(worksheet.Cells[rowNo, columnNo - 1]);

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
