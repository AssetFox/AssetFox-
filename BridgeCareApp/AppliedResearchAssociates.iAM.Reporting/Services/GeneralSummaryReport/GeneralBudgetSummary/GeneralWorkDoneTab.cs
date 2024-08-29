using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using AppliedResearchAssociates.iAM.Analysis;
using AppliedResearchAssociates.iAM.Analysis.Engine;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.ExcelHelpers;
using AppliedResearchAssociates.iAM.Reporting.Models;
using AppliedResearchAssociates.iAM.Reporting.Models.FlexibleAuditReport;
using AppliedResearchAssociates.iAM.Reporting.Services.FlexibileAuditReport;
using OfficeOpenXml;

namespace AppliedResearchAssociates.iAM.Reporting.Services.GeneralSummaryReport.GeneralBudgetSummary
{
    public class GeneralWorkDoneTab
    {
        private ReportHelper _reportHelper;
        private const int headerRow1 = 1;
        private const int headerRow2 = 2;
        private List<int> columnNumbersBudgetsUsed;
        private bool ShouldBundleFeasibleTreatments;
        private readonly IUnitOfWork _unitOfWork;

        public GeneralWorkDoneTab(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _reportHelper = new ReportHelper(_unitOfWork);
        }

        public void Fill(ExcelWorksheet decisionsWorksheet, SimulationOutput simulationOutput, Simulation simulation, HashSet<string> performanceCurvesAttributes)
        {
            columnNumbersBudgetsUsed = new List<int>();
            // Distinct performance curves' attributes
            var currentAttributes = performanceCurvesAttributes;

            // Benefit attribute
            currentAttributes.Add(_reportHelper.GetBenefitAttribute(simulation));

            // Distinct budgets            
            var budgets = _reportHelper.GetBudgets(simulationOutput.Years);

            var treatments = new List<string>();
            treatments = simulation.Treatments.Where(_ => _.Name != "No Treatment")?.OrderBy(_ => _.Name).Select(_ => _.Name).ToList();

            ShouldBundleFeasibleTreatments = simulation.ShouldBundleFeasibleTreatments;


            // Add headers to excel
            var currentCell = AddHeadersCells(decisionsWorksheet, currentAttributes, budgets, treatments);

            // Fill data in excel
            FillDynamicDataInWorkSheet(simulationOutput, currentAttributes, budgets, treatments, decisionsWorksheet, currentCell);

            decisionsWorksheet.Cells.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Bottom;
            decisionsWorksheet.Cells.AutoFitColumns();
            PerformPostAutofitAdjustments(decisionsWorksheet, columnNumbersBudgetsUsed);
        }

        private void FillDynamicDataInWorkSheet(SimulationOutput simulationOutput, HashSet<string> currentAttributes, HashSet<string> budgets, List<string> treatments, ExcelWorksheet decisionsWorksheet, CurrentCell currentCell)
        {
            Dictionary<string, List<TreatmentConsiderationDetail>> keyCashFlowFundingDetails = new();
            var primaryKey = _unitOfWork.AdminSettingsRepo.GetKeyFields();
            var firstPrimaryKey = primaryKey[0];
            bool isNumberic = false;

            foreach (var initialAssetSummary in simulationOutput.InitialAssetSummaries)
            {
                var primaryKeyField = _reportHelper.CheckAndGetValue<string>(initialAssetSummary.ValuePerTextAttribute, primaryKey[0].ToString());
                if (string.IsNullOrEmpty(primaryKeyField) && initialAssetSummary.ValuePerNumericAttribute != null)
                {
                    isNumberic = true;
                    primaryKeyField = initialAssetSummary.ValuePerNumericAttribute[primaryKey[0].ToString()].ToString();
                }
                var years = simulationOutput.Years.OrderBy(yr => yr.Year);

                // Year 0
                var decisionDataModel = GetInitialDecisionDataModel(currentAttributes, primaryKeyField, years.FirstOrDefault().Year - 1, initialAssetSummary);
                FillInitialDataInWorksheet(decisionsWorksheet, decisionDataModel, currentAttributes, currentCell.Row, 1);

                var yearZeroRow = currentCell.Row++;
                foreach (var year in years)
                {
                    var section = isNumberic ?
                                  year.Assets.FirstOrDefault(_ => _reportHelper.CheckAndGetValue<double>(_.ValuePerNumericAttribute, primaryKey[0].ToString()) == Convert.ToDouble(primaryKeyField)) :
                                  year.Assets.FirstOrDefault(_ => _reportHelper.CheckAndGetValue<string>(_.ValuePerTextAttribute, primaryKey[0].ToString()) == primaryKeyField);
                    if (section.TreatmentCause == TreatmentCause.CommittedProject)
                    {
                        continue;
                    }

                    // Build keyCashFlowFundingDetails
                    var cashFlowPrimaryKey = CheckGetTextValue(section.ValuePerTextAttribute, primaryKey[0].ToString());
                    if (section.TreatmentStatus != TreatmentStatus.Applied)
                    {
                        var fundingSection = year.Assets.
                                              FirstOrDefault(_ => CheckGetTextValue(_.ValuePerTextAttribute, primaryKey[0].ToString()) == cashFlowPrimaryKey &&
                                                            _.TreatmentCause == TreatmentCause.SelectedTreatment &&
                                                            _.AppliedTreatment.ToLower() != FlexibleAuditReportConstants.NoTreatment &&
                                                            _.AppliedTreatment == section.AppliedTreatment);
                        if (fundingSection != null)
                        {
                            if (!keyCashFlowFundingDetails.ContainsKey(cashFlowPrimaryKey))
                            {
                                keyCashFlowFundingDetails.Add(cashFlowPrimaryKey, fundingSection.TreatmentConsiderations ?? new());
                            }
                            else
                            {
                                keyCashFlowFundingDetails[cashFlowPrimaryKey].AddRange(fundingSection.TreatmentConsiderations);
                            }
                        }
                    }

                    // Generate data model                    
                    var decisionsDataModel = GenerateDecisionDataModel(currentAttributes, budgets, treatments, primaryKeyField, year, section, keyCashFlowFundingDetails);

                    // Fill in excel
                    currentCell = FillDataInWorksheet(decisionsWorksheet, decisionsDataModel, budgets.Count, currentAttributes, currentCell);
                }
                ExcelHelper.ApplyBorder(decisionsWorksheet.Cells[yearZeroRow, 1, yearZeroRow, currentCell.Column]);
            }
        }

        private FlexibleDecisionDataModel GenerateDecisionDataModel(HashSet<string> currentAttributes, HashSet<string> budgets, List<string> treatments, string primaryKeyField, SimulationYearDetail year, AssetDetail section, Dictionary<string, List<TreatmentConsiderationDetail>> keyCashFlowFundingDetails)
        {
            var decisionDataModel = GetInitialDecisionDataModel(currentAttributes, primaryKeyField, year.Year, section);

            // Budget levels
            var budgetsAtDecisionTime = section.TreatmentConsiderations.FirstOrDefault(_ => _.TreatmentName == section.AppliedTreatment)?.FundingCalculationInput?.CurrentBudgetsToSpend.Where(_ => _.Year == year.Year).ToList() ??
              section.TreatmentConsiderations.FirstOrDefault()?.FundingCalculationInput?.CurrentBudgetsToSpend.Where(_ => _.Year == year.Year).ToList() ?? new();
            var budgetLevels = new List<decimal>();
            if (budgetsAtDecisionTime.Count > 0)
            {
                foreach (var budget in budgets)
                {
                    var budgetAtDecisionTime = budgetsAtDecisionTime.FirstOrDefault(_ => _.Name == budget);
                    if (budgetAtDecisionTime != null)
                    {
                        budgetLevels.Add(budgetAtDecisionTime.Amount);
                    }
                }
            }
            decisionDataModel.BudgetLevels = budgetLevels;

            // Treatments
            var decisionsTreatments = new List<FlexibleDecisionTreatment>();
            var feasibleAndSelectedTreatments = new List<FlexibleDecisionTreatment>();
            var isCashFlowProject = section.TreatmentCause == TreatmentCause.CashFlowProject;
            foreach (var treatment in treatments)
            {
                var decisionsTreatment = new FlexibleDecisionTreatment();
                var treatmentRejection = section.TreatmentRejections.FirstOrDefault(_ => _.TreatmentName == treatment);
                decisionsTreatment.Feasible = isCashFlowProject ? "-" : (treatmentRejection == null ? FlexibleAuditReportConstants.Yes : FlexibleAuditReportConstants.No);
                var currentCIImprovement = Convert.ToDouble(decisionDataModel.CurrentAttributesValues.Last());
                var treatmentOption = section.TreatmentOptions.FirstOrDefault(_ => _.TreatmentName == treatment);
                decisionsTreatment.CIImprovement = treatmentOption?.ConditionChange;
                decisionsTreatment.Cost = treatmentOption != null ? treatmentOption.Cost : 0;
                decisionsTreatment.BCRatio = treatmentOption != null ? treatmentOption.Benefit / treatmentOption.Cost : 0;
                decisionsTreatment.Selected = isCashFlowProject ? FlexibleAuditReportConstants.CashFlow : (section.AppliedTreatment == treatment ? FlexibleAuditReportConstants.Yes : FlexibleAuditReportConstants.No);

                // If CF then use obj from keyCashFlowFundingDetails otherwise from section
                var treatmentConsiderations = ((section.TreatmentCause == TreatmentCause.SelectedTreatment &&
                                              section.TreatmentStatus == TreatmentStatus.Progressed) ||
                                              (section.TreatmentCause == TreatmentCause.CashFlowProject &&
                                              section.TreatmentStatus == TreatmentStatus.Progressed) ||
                                              (section.TreatmentCause == TreatmentCause.CashFlowProject &&
                                              section.TreatmentStatus == TreatmentStatus.Applied)) ?
                                              keyCashFlowFundingDetails[primaryKeyField] :
                                              section.TreatmentConsiderations ?? new();
                var treatmentConsideration = treatmentConsiderations.FirstOrDefault(_ => _.FundingCalculationOutput != null &&
                                             _.FundingCalculationOutput.AllocationMatrix.Any(_ => _.Year == year.Year));
                // AllocationMatrix includes cash flow funding of future years.
                var allocationMatrix = treatmentConsideration?.FundingCalculationOutput?.AllocationMatrix ?? new();
                var amountSpent = treatmentConsideration?.FundingCalculationOutput?.AllocationMatrix.
                                                Where(_ => _.Year == year.Year).Sum(_ => _.AllocatedAmount)
                                                ?? 0;
                decisionsTreatment.AmountSpent = amountSpent;

                var budgetsUsed = allocationMatrix.Where(_ => _.AllocatedAmount > 0 && _.Year == year.Year)
                                .Select(_ => _.BudgetName).Distinct().ToList()
                                ?? new();
                decisionsTreatment.BudgetsUsed = string.Join(", ", budgetsUsed);

                var budgetStatuses = allocationMatrix.Where(_ => _.AllocatedAmount > 0 && _.Year == year.Year)
                                    .Select(_ => treatmentConsideration.GetBudgetUsageStatus(_.Year, _.BudgetName, _.TreatmentName).ToString()).Distinct().ToList()
                                    ?? new();
                decisionsTreatment.BudgetUsageStatuses = string.Join(", ", budgetStatuses);

                decisionsTreatments.Add(decisionsTreatment);
                feasibleAndSelectedTreatments = decisionsTreatments
                    .Where(treatment => treatment.Feasible == "Y" && treatment.Selected == "Y")
                    .ToList();
            }
            decisionDataModel.DecisionsTreatments = feasibleAndSelectedTreatments;

            if (ShouldBundleFeasibleTreatments == true)
            {
                // Aggregated
                var decisionsAggregated = new List<FlexibleDecisionAggregated>();

                // If CF then use obj from keyCashFlowFundingDetails otherwise from section
                var aggregatedTreatmentConsiderations = ((section.TreatmentCause == TreatmentCause.SelectedTreatment &&
                                              section.TreatmentStatus == TreatmentStatus.Progressed) ||
                                              (section.TreatmentCause == TreatmentCause.CashFlowProject &&
                                              section.TreatmentStatus == TreatmentStatus.Progressed) ||
                                              (section.TreatmentCause == TreatmentCause.CashFlowProject &&
                                              section.TreatmentStatus == TreatmentStatus.Applied)) ?
                                              keyCashFlowFundingDetails[primaryKeyField] :
                                              section.TreatmentConsiderations ?? new();
                var aggregatedTreatmentConsideration = aggregatedTreatmentConsiderations.FirstOrDefault(_ => _.FundingCalculationOutput != null &&
                                             _.FundingCalculationOutput.AllocationMatrix.Any(_ => _.Year == year.Year));
                var includedBundles = aggregatedTreatmentConsiderations.FirstOrDefault()?.TreatmentName;
                var aggregatedTreatmentString = aggregatedTreatmentConsiderations.ToString();
                
                // AllocationMatrix includes cash flow funding of future years.
                var aggregatedAllocationMatrix = aggregatedTreatmentConsideration?.FundingCalculationOutput?.AllocationMatrix ?? new();

                var decisionsAggregate = new FlexibleDecisionAggregated();
                var aggregatedTreatmentRejection = section.TreatmentRejections;
                if (!string.IsNullOrEmpty(includedBundles))
                {
                    decisionsAggregate.Feasible = FlexibleAuditReportConstants.Yes;
                    decisionsAggregate.Selected = FlexibleAuditReportConstants.Yes;
                }
                else
                {
                    decisionsAggregate.Feasible = FlexibleAuditReportConstants.No;
                    decisionsAggregate.Selected = FlexibleAuditReportConstants.No;
                }
                var currentAggregatedCIImprovement = Convert.ToDouble(decisionDataModel.CurrentAttributesValues.Last());
                string aggregatedTreatmentName = null;
                foreach (var option in section.TreatmentOptions)
                {
                    string optionAsString = option.TreatmentName;
                    if (optionAsString.Contains("Bundle"))
                    {
                        aggregatedTreatmentName = optionAsString;
                    }
                }
                var aggregatedTreatmentOption = section.TreatmentOptions.FirstOrDefault(_ => _.TreatmentName == aggregatedTreatmentName);
                decisionsAggregate.IncludedBundles = includedBundles;
                decisionsAggregate.CIImprovement = aggregatedTreatmentOption?.ConditionChange;
                decisionsAggregate.Cost = aggregatedTreatmentOption != null ? aggregatedTreatmentOption.Cost : 0;
                decisionsAggregate.BCRatio = aggregatedTreatmentOption != null ? aggregatedTreatmentOption.Benefit / aggregatedTreatmentOption.Cost : 0;

                var aggregatedAmountSpent = aggregatedTreatmentConsideration?.FundingCalculationOutput?.AllocationMatrix.
                                    Where(_ => _.Year == year.Year).Sum(_ => _.AllocatedAmount)
                                    ?? 0;
                decisionsAggregate.AmountSpent = aggregatedAmountSpent;

                var aggregatedBudgetsUsed = aggregatedAllocationMatrix.Where(_ => _.AllocatedAmount > 0 && _.Year == year.Year)
                                .Select(_ => _.BudgetName).Distinct().ToList()
                                ?? new();
                decisionsAggregate.BudgetsUsed = string.Join(", ", aggregatedBudgetsUsed);

                var aggregatedBudgetStatuses = aggregatedAllocationMatrix.Where(_ => _.AllocatedAmount > 0 && _.Year == year.Year)
                                    .Select(_ => aggregatedTreatmentConsideration.GetBudgetUsageStatus(_.Year, _.BudgetName, _.TreatmentName).ToString()).Distinct().ToList()
                                    ?? new();
                decisionsAggregate.BudgetUsageStatuses = string.Join(", ", aggregatedBudgetStatuses);


                decisionsAggregated.Add(decisionsAggregate);
                decisionDataModel.DecisionsAggregated = decisionsAggregated;
            }
            return decisionDataModel;
        }

        private FlexibleDecisionDataModel GetInitialDecisionDataModel(HashSet<string> currentAttributes, string CRS, int year, AssetSummaryDetail section)
        {
            var decisionDataModel = new FlexibleDecisionDataModel
            {
                CRS = CRS,
                AnalysisYear = year,
            };

            // Current
            var currentAttributesValues = new List<double>();
            for (int index = 0; index < currentAttributes.Count - 1; index++)
            {
                var attributeValue = CheckGetValue(section.ValuePerNumericAttribute, currentAttributes.ElementAt(index));
                currentAttributesValues.Add(attributeValue);
            }
            // analysis benefit attribute
            currentAttributesValues.Add(CheckGetValue(section.ValuePerNumericAttribute, currentAttributes.Last()));
            decisionDataModel.CurrentAttributesValues = currentAttributesValues;

            return decisionDataModel;
        }

        private double CheckGetValue(Dictionary<string, double> valuePerNumericAttribute, string attribute) => _reportHelper.CheckAndGetValue<double>(valuePerNumericAttribute, attribute);
        private string CheckGetTextValue(Dictionary<string, string> valuePerTextAttribute, string attribute) => _reportHelper.CheckAndGetValue<string>(valuePerTextAttribute, attribute);

        private CurrentCell FillDataInWorksheet(ExcelWorksheet decisionsWorksheet, FlexibleDecisionDataModel decisionsDataModel, int budgetsCount, HashSet<string> currentAttributes, CurrentCell currentCell)
        {
            var row = currentCell.Row;
            int column = 1;

            column = FillInitialDataInWorksheet(decisionsWorksheet, decisionsDataModel, currentAttributes, row, column);

            var budgetsLevels = decisionsDataModel.BudgetLevels;
            foreach (var budgetLevel in budgetsLevels)
            {
                SetAccountingFormat(decisionsWorksheet.Cells[row, column]);
                decisionsWorksheet.Cells[row, column++].Value = budgetLevel;
            }
            if (budgetsLevels.Count == 0)
            {
                // Adjust column to correct position for next data
                column += budgetsCount;
            }

            foreach (var decisionsTreatment in decisionsDataModel.DecisionsTreatments)
            {
                ExcelHelper.HorizontalCenterAlign(decisionsWorksheet.Cells[row, column]);
                decisionsWorksheet.Cells[row, column++].Value = decisionsTreatment.Cost;
                SetDecimalFormat(decisionsWorksheet.Cells[row, column]);
            }

            if (ShouldBundleFeasibleTreatments == true)
            {
                foreach (var decisionAggregate in decisionsDataModel.DecisionsAggregated)
                {
                    ExcelHelper.HorizontalCenterAlign(decisionsWorksheet.Cells[row, column]);
                    decisionsWorksheet.Cells[row, column++].Value = decisionAggregate.Cost;
                    SetDecimalFormat(decisionsWorksheet.Cells[row, column]);
                }
            }

            ExcelHelper.ApplyBorder(decisionsWorksheet.Cells[row, 1, row, column - 1]);

            return new CurrentCell { Row = row + 1, Column = column - 1 };
        }

        private int FillInitialDataInWorksheet(ExcelWorksheet decisionsWorksheet, FlexibleDecisionDataModel decisionsDataModel, HashSet<string> currentAttributes, int row, int column)
        {
            ExcelHelper.HorizontalCenterAlign(decisionsWorksheet.Cells[row, column]);
            decisionsWorksheet.Cells[row, column++].Value = decisionsDataModel.CRS;
            ExcelHelper.HorizontalCenterAlign(decisionsWorksheet.Cells[row, column]);
            decisionsWorksheet.Cells[row, column++].Value = decisionsDataModel.AnalysisYear;

            for (int index = 0; index < decisionsDataModel.CurrentAttributesValues.Count; index++)
            {
                SetDecimalFormat(decisionsWorksheet.Cells[row, column]);
                var attribute = currentAttributes.ElementAt(index);
                var currentAttributesValue = decisionsDataModel.CurrentAttributesValues[index];
                var fillerValue = ApplyApplicableFiller(attribute, currentAttributesValue);
                decisionsWorksheet.Cells[row, column++].Value = currentAttributesValue;
            }

            return column;
        }

        private string ApplyApplicableFiller(string attribute, double attributeValue)
        {
            return "";
        }

        private static void SetDecimalFormat(ExcelRange cell) => ExcelHelper.SetCustomFormat(cell, ExcelHelperCellFormat.DecimalPrecision3);

        private static void SetAccountingFormat(ExcelRange cell) => ExcelHelper.SetCustomFormat(cell, ExcelHelperCellFormat.Accounting);

        private CurrentCell AddHeadersCells(ExcelWorksheet worksheet, HashSet<string> currentAttributes, HashSet<string> budgets, List<string> treatments)
        {
            int column = 1;

            var primaryKeyField = _unitOfWork.AdminSettingsRepo.GetKeyFields();

            worksheet.Cells.Style.WrapText = false;
            worksheet.Cells[headerRow2, column++].Value = primaryKeyField[0].ToString();
            worksheet.Cells[headerRow2, column++].Value = "Analysis\r\nYear";

            // Current
            column = AddCurrentAttributesHeaders(worksheet, currentAttributes, column);

            // Budget levels
            //column = AddBudgetLevelsHeaders(worksheet, budgets, column);

            // Treatments
            column = AddCurrentTreatmentsHeaders(worksheet, treatments, column);

            if (ShouldBundleFeasibleTreatments == true)
            {
                // Aggregated
                column = AddAggregatedHeaders(worksheet, treatments, column);
            }

            var currentAttributesCount = currentAttributes.Count;
            ExcelHelper.ApplyColor(worksheet.Cells[headerRow2, 3, headerRow2, column - 1], Color.FromArgb(255, 242, 204));
            ExcelHelper.ApplyBorder(worksheet.Cells[headerRow1, 1, headerRow2, worksheet.Dimension.Columns]);
            ExcelHelper.ApplyStyleNoWrap(worksheet.Cells[headerRow2, 3, headerRow2, 3 + currentAttributesCount - 1]);
            ExcelHelper.ApplyStyle(worksheet.Cells[headerRow2, 1, headerRow2, 2]);
            ExcelHelper.ApplyStyle(worksheet.Cells[headerRow2, 2 + currentAttributesCount + 1, headerRow2, column - 1]);

            return new CurrentCell { Row = headerRow1 + 2, Column = worksheet.Dimension.Columns + 1 };
        }

        private int AddCurrentTreatmentsHeaders(ExcelWorksheet worksheet, List<string> treatments, int column)
        {
            var treatmentsColumn = column;
            // Fixed treatment headers for row 2 per treatment
            var treatmentHeaders = GetTreatmentsHeaders();
            bool fillColor = false;
            // Dynamic headers for row 1 based on simulation treatments
            foreach (var treatment in treatments)
            {
                fillColor = !fillColor;
                worksheet.Cells[headerRow1, treatmentsColumn].Value = treatment;
                // Repeat treatmentHeaders for row 2 per treatment
                foreach (var treatmentHeader in treatmentHeaders)
                {
                    if (treatmentHeader.Equals("Budget(s)\r\nUsed"))
                    {
                        columnNumbersBudgetsUsed.Add(column);
                    }
                    worksheet.Cells[headerRow2, column++].Value = treatmentHeader;
                }

                if (fillColor)
                {
                    ExcelHelper.ApplyColor(worksheet.Cells[headerRow1, treatmentsColumn], Color.LightGray);
                }
                // Merge cells for each treatment
                ExcelHelper.MergeCells(worksheet, headerRow1, treatmentsColumn, headerRow1, column - 1);
                treatmentsColumn += treatmentHeaders.Count;
            }

            return column;
        }

        //private static int AddBudgetLevelsHeaders(ExcelWorksheet worksheet, HashSet<string> budgets, int column)
        //{
        //    var budgetLevelsColumn = column;
        //    worksheet.Cells[headerRow1, budgetLevelsColumn].Value = "Budget Levels";
        //    // Dynamic hearders in row 2
        //    foreach (var budget in budgets)
        //    {
        //        worksheet.Cells[headerRow2, column++].Value = budget;
        //    }
        //    // Merge cells for "Budget levels"
        //    ExcelHelper.MergeCells(worksheet, headerRow1, budgetLevelsColumn, headerRow1, column - 1);
        //    return column;
        //}

        private int AddAggregatedHeaders(ExcelWorksheet worksheet, List<string> treatments, int column)
        {
            var aggregatedString = new List<string> { "Aggregated" };
            var aggregatedColumn = column;
            worksheet.Cells[headerRow1, aggregatedColumn].Value = "Aggregated";
            // Fixed treatment headers for row 2 per treatment
            var treatmentHeaders = GetAggregatedTreatmentsHeaders();
            bool fillColor = false;
            // Dynamic headers for row 1 based on simulation treatments
            foreach (var aggregated in aggregatedString)
            {
                fillColor = !fillColor;
                // Repeat treatmentHeaders for row 2 per treatment
                foreach (var treatmentHeader in treatmentHeaders)
                {
                    if (treatmentHeader.Equals("Budget(s)\r\nUsed"))
                    {
                        columnNumbersBudgetsUsed.Add(column);
                    }
                    worksheet.Cells[headerRow2, column++].Value = treatmentHeader;
                }

                if (fillColor)
                {
                    ExcelHelper.ApplyColor(worksheet.Cells[headerRow1, aggregatedColumn], Color.LightGray);
                }
                // Merge cells for each treatment
                ExcelHelper.MergeCells(worksheet, headerRow1, aggregatedColumn, headerRow1, column - 1);
                aggregatedColumn += treatmentHeaders.Count;
            }

            return column;
        }

        private static int AddCurrentAttributesHeaders(ExcelWorksheet worksheet, HashSet<string> currentAttributes, int column)
        {
            var currentAttributesColumn = column;
            worksheet.Cells[headerRow1, currentAttributesColumn].Value = "Attributes";
            // Dynamic headers in row 2
            foreach (var currentAttribute in currentAttributes)
            {
                worksheet.Cells[headerRow2, column++].Value = currentAttribute;
            }
            // Merge cells for "Current"
            ExcelHelper.MergeCells(worksheet, headerRow1, currentAttributesColumn, headerRow1, column - 1);

            return column;
        }

        private static List<string> GetTreatmentsHeaders()
        {
            return new List<string>
            {
                "Work Done",
                "Cost",
            };
        }

        private static List<string> GetAggregatedTreatmentsHeaders()
        {
            return new List<string>
            {
                "Work Done",
                "Cost",
            };
        }


        public static void PerformPostAutofitAdjustments(ExcelWorksheet worksheet, List<int> columnNumbersBudgetsUsed)
        {
            foreach (var columnNumber in columnNumbersBudgetsUsed)
            {
                worksheet.Column(columnNumber).SetTrueWidth(20);
                // Rejection reason
                worksheet.Column(columnNumber + 1).SetTrueWidth(35);
                worksheet.Column(columnNumber + 1).Style.WrapText = true;
            }
        }
    }

}
