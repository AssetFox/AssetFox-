using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using AppliedResearchAssociates.iAM.Analysis;
using AppliedResearchAssociates.iAM.Analysis.Engine;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.ExcelHelpers;
using AppliedResearchAssociates.iAM.Reporting.Models;
using AppliedResearchAssociates.iAM.Reporting.Models.BAMSAuditReport;
using OfficeOpenXml;
using OfficeOpenXml.FormulaParsing.Excel.Functions.DateTime;

namespace AppliedResearchAssociates.iAM.Reporting.Services.BAMSAuditReport
{
    public class DecisionTab
    {
        private ReportHelper _reportHelper;
        private const int headerRow1 = 1;
        private const int headerRow2 = 2;
        private List<int> columnNumbersBudgetsUsed;
        private readonly IUnitOfWork _unitOfWork;

        public DecisionTab(IUnitOfWork unitOfWork)
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
            foreach (var initialAssetSummary in simulationOutput.InitialAssetSummaries)
            {
                var brKey = CheckGetValue(initialAssetSummary.ValuePerNumericAttribute, "BRKEY_");
                var familyId = int.Parse(_reportHelper.CheckAndGetValue<string>(initialAssetSummary.ValuePerTextAttribute, "FAMILY_ID"));
                var years = simulationOutput.Years.OrderBy(yr => yr.Year);

                // Year 0
                var decisionDataModel = GetInitialDecisionDataModel(currentAttributes, brKey, years.FirstOrDefault().Year - 1, initialAssetSummary);
                FillInitialDataInWorksheet(decisionsWorksheet, decisionDataModel, currentAttributes, familyId, currentCell.Row, 1);

                var yearZeroRow = currentCell.Row++;
                var firstYearSection = years.First().Assets.FirstOrDefault(_ => CheckGetValue(_.ValuePerNumericAttribute, "BRKEY_") == brKey);
                foreach (var year in years)
                {
                    var section = year.Assets.FirstOrDefault(_ => CheckGetValue(_.ValuePerNumericAttribute, "BRKEY_") == brKey);
                    if (section.TreatmentCause == TreatmentCause.CommittedProject)
                    {
                        continue;
                    }

                    // Generate data model                    
                    var decisionsDataModel = GenerateDecisionDataModel(currentAttributes, budgets, treatments, brKey, year, section, firstYearSection);

                    // Fill in excel
                    currentCell = FillDataInWorksheet(decisionsWorksheet, decisionsDataModel, budgets.Count, currentAttributes, familyId, currentCell);
                }
                ExcelHelper.ApplyBorder(decisionsWorksheet.Cells[yearZeroRow, 1, yearZeroRow, currentCell.Column]);
            }
        }

        private DecisionDataModel GenerateDecisionDataModel(HashSet<string> currentAttributes, HashSet<string> budgets, List<string> treatments, double brKey, SimulationYearDetail year, AssetDetail section, AssetDetail firstYearSection)
        {
            var decisionDataModel = GetInitialDecisionDataModel(currentAttributes, brKey, year.Year, section);

            // Budget levels
            var budgetsAtDecisionTime = section.TreatmentConsiderations.FirstOrDefault(_ => _.TreatmentName == section.AppliedTreatment)?.FundingCalculationInput?.CurrentBudgetsToSpend.Where(_ => _.Year == year.Year).ToList() ??
              section.TreatmentConsiderations.FirstOrDefault()?.FundingCalculationInput?.CurrentBudgetsToSpend.Where(_ => _.Year == year.Year).ToList() ??
                new();

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
            var isCashFlowProject = section.TreatmentCause == TreatmentCause.CashFlowProject;
            var decisionsTreatments = new List<DecisionTreatment>();
            foreach (var treatment in treatments)
            {
                var decisionsTreatment = new DecisionTreatment();
                var treatmentRejection = section.TreatmentRejections.FirstOrDefault(_ => _.TreatmentName == treatment);
                decisionsTreatment.Feasible = isCashFlowProject ? "-" : (treatmentRejection == null ? BAMSAuditReportConstants.Yes : BAMSAuditReportConstants.No);
                var currentCIImprovement = Convert.ToDouble(decisionDataModel.CurrentAttributesValues.Last());                
                var treatmentOption = section.TreatmentOptions.FirstOrDefault(_ => _.TreatmentName == treatment);
                decisionsTreatment.CIImprovement = treatmentOption?.ConditionChange;
                decisionsTreatment.Cost = treatmentOption != null ? treatmentOption.Cost : 0;
                decisionsTreatment.BCRatio = treatmentOption != null ? treatmentOption.Benefit / treatmentOption.Cost : 0;
                decisionsTreatment.Benefit = treatmentOption != null ? decisionsTreatment.BCRatio * decisionsTreatment.Cost : 0;
                decisionsTreatment.Selected = isCashFlowProject ? BAMSAuditReportConstants.CashFlow : (section.AppliedTreatment == treatment ? BAMSAuditReportConstants.Yes : BAMSAuditReportConstants.No);

                var treatmentConsideration = firstYearSection.TreatmentConsiderations.FirstOrDefault(_ => _.TreatmentName == treatment);               
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

                var budgetPriorityLevel = treatmentConsideration?.BudgetPriorityLevel != null ? treatmentConsideration.BudgetPriorityLevel.Value.ToString() : string.Empty;
                decisionsTreatment.BudgetPriorityLevel = budgetPriorityLevel;

                decisionsTreatments.Add(decisionsTreatment);
            }
            decisionDataModel.DecisionsTreatments = decisionsTreatments;
            return decisionDataModel;
        }

        private DecisionDataModel GetInitialDecisionDataModel(HashSet<string> currentAttributes, double brKey, int year, AssetSummaryDetail section)
        {
            var decisionDataModel = new DecisionDataModel
            {
                BRKey = brKey,
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

        private CurrentCell FillDataInWorksheet(ExcelWorksheet decisionsWorksheet, DecisionDataModel decisionsDataModel, int budgetsCount, HashSet<string> currentAttributes, int familyId, CurrentCell currentCell)
        {
            var row = currentCell.Row;
            int column = 1;

            column = FillInitialDataInWorksheet(decisionsWorksheet, decisionsDataModel, currentAttributes, familyId, row, column);

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
                decisionsWorksheet.Cells[row, column++].Value = decisionsTreatment.Feasible;
                SetDecimalFormat(decisionsWorksheet.Cells[row, column]);
                decisionsWorksheet.Cells[row, column++].Value = decisionsTreatment.CIImprovement;
                ExcelHelper.HorizontalCenterAlign(decisionsWorksheet.Cells[row, column]);
                decisionsWorksheet.Cells[row, column++].Value = decisionsTreatment.BudgetPriorityLevel;
                SetAccountingFormat(decisionsWorksheet.Cells[row, column]);
                decisionsWorksheet.Cells[row, column++].Value = decisionsTreatment.Cost;
                SetDecimalFormat(decisionsWorksheet.Cells[row, column]);
                decisionsWorksheet.Cells[row, column++].Value = decisionsTreatment.BCRatio;
                SetDecimalFormat(decisionsWorksheet.Cells[row, column]);
                decisionsWorksheet.Cells[row, column++].Value = decisionsTreatment.Benefit;               
                ExcelHelper.HorizontalCenterAlign(decisionsWorksheet.Cells[row, column]);
                decisionsWorksheet.Cells[row, column++].Value = decisionsTreatment.Selected;
                SetAccountingFormat(decisionsWorksheet.Cells[row, column]);
                decisionsWorksheet.Cells[row, column++].Value = decisionsTreatment.AmountSpent;
                decisionsWorksheet.Cells[row, column++].Value = decisionsTreatment.BudgetsUsed;
                decisionsWorksheet.Cells[row, column - 1].Style.WrapText = true;
                decisionsWorksheet.Cells[row, column++].Value = decisionsTreatment.BudgetUsageStatuses;
            }
            ExcelHelper.ApplyBorder(decisionsWorksheet.Cells[row, 1, row, column - 1]);

            return new CurrentCell { Row = row + 1, Column = column - 1 };
        }

        private int FillInitialDataInWorksheet(ExcelWorksheet decisionsWorksheet, DecisionDataModel decisionsDataModel, HashSet<string> currentAttributes, int familyId, int row, int column)
        {
            ExcelHelper.HorizontalCenterAlign(decisionsWorksheet.Cells[row, column]);
            decisionsWorksheet.Cells[row, column++].Value = decisionsDataModel.BRKey;
            ExcelHelper.HorizontalCenterAlign(decisionsWorksheet.Cells[row, column]);
            decisionsWorksheet.Cells[row, column++].Value = decisionsDataModel.AnalysisYear;

            for (int index = 0; index < decisionsDataModel.CurrentAttributesValues.Count; index++)
            {
                SetDecimalFormat(decisionsWorksheet.Cells[row, column]);
                var attribute = currentAttributes.ElementAt(index);
                var currentAttributesValue = decisionsDataModel.CurrentAttributesValues[index];
                var fillerValue = ApplyApplicableFiller(familyId, attribute, currentAttributesValue);
                decisionsWorksheet.Cells[row, column++].Value = fillerValue != null ? fillerValue : currentAttributesValue;
            }

            return column;
        }

        private string ApplyApplicableFiller(int familyId, string attribute, double attributeValue)
        {
            return familyId < 11
                ? attribute switch
                {
                    "CULV_SEEDED" or "CULV_DURATION_N" => BAMSAuditReportConstants.No,
                    _ => null,
                }
                : attribute switch
                {
                    "DECK_SEEDED" or "SUP_SEEDED" or "SUB_SEEDED" or "DECK_DURATION_N" or "SUP_DURATION_N" or "SUB_DURATION_N" => BAMSAuditReportConstants.No,
                    _ => null,
                };
        }

        private static void SetDecimalFormat(ExcelRange cell) => ExcelHelper.SetCustomFormat(cell, ExcelHelperCellFormat.DecimalPrecision3);

        private static void SetAccountingFormat(ExcelRange cell) => ExcelHelper.SetCustomFormat(cell, ExcelHelperCellFormat.Accounting);        

        private CurrentCell AddHeadersCells(ExcelWorksheet worksheet, HashSet<string> currentAttributes, HashSet<string> budgets, List<string> treatments)
        {            
            int column = 1;

            worksheet.Cells.Style.WrapText = false;
            worksheet.Cells[headerRow2, column++].Value = "BRKey";
            worksheet.Cells[headerRow2, column++].Value = "Analysis\r\nYear";

            // Current
            column = AddCurrentAttributesHeaders(worksheet, currentAttributes, column);

            // Budget levels
            column = AddBudgetLevelsHeaders(worksheet, budgets, column);

            // Treatments
            column = AddCurrentTreatmentsHeaders(worksheet, treatments, column);

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

        private static int AddBudgetLevelsHeaders(ExcelWorksheet worksheet, HashSet<string> budgets, int column)
        {
            var budgetLevelsColumn = column;
            worksheet.Cells[headerRow1, budgetLevelsColumn].Value = "Budget Levels";
            // Dynamic hearders in row 2
            foreach (var budget in budgets)
            {
                worksheet.Cells[headerRow2, column++].Value = budget;
            }
            // Merge cells for "Budget levels"
            ExcelHelper.MergeCells(worksheet, headerRow1, budgetLevelsColumn, headerRow1, column - 1);
            return column;
        }

        private static int AddCurrentAttributesHeaders(ExcelWorksheet worksheet, HashSet<string> currentAttributes, int column)
        {
            var currentAttributesColumn = column;
            worksheet.Cells[headerRow1, currentAttributesColumn].Value = "Current";
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
                "Feasible?",
                "CI\r\nImprovement",
                "Priority Level",
                "Cost",
                "B/C\r\nRatio",
                "Benefit",
                "Selected?",
                "Amount\r\nSpent",
                "Budget(s)\r\nUsed",
                "Budget Usage Status(es)"
            };
        }

        public static void PerformPostAutofitAdjustments(ExcelWorksheet worksheet,List<int> columnNumbersBudgetsUsed)
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
