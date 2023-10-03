﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using AppliedResearchAssociates.iAM.Analysis;
using AppliedResearchAssociates.iAM.Analysis.Engine;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.ExcelHelpers;
using AppliedResearchAssociates.iAM.Reporting.Models;
using AppliedResearchAssociates.iAM.Reporting.Models.PAMSAuditReport;
using OfficeOpenXml;

namespace AppliedResearchAssociates.iAM.Reporting.Services.PAMSAuditReport
{
    public class PAMSDecisionTab
    {
        private ReportHelper _reportHelper;
        private const int headerRow1 = 1;
        private const int headerRow2 = 2;
        private List<int> columnNumbersBudgetsUsed;
        private readonly IUnitOfWork _unitOfWork;

        public PAMSDecisionTab(IUnitOfWork unitOfWork)
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
                var CRS = _reportHelper.CheckAndGetValue<string>(initialAssetSummary.ValuePerTextAttribute, "CRS");
                //var familyId = int.Parse(_reportHelper.CheckAndGetValue<string>(initialAssetSummary.ValuePerTextAttribute, "FAMILY_ID"));
                var years = simulationOutput.Years.OrderBy(yr => yr.Year);

                // Year 0
                var PAMSdecisionDataModel = GetInitialDecisionDataModel(currentAttributes, CRS, years.FirstOrDefault().Year - 1, initialAssetSummary);
                FillInitialDataInWorksheet(decisionsWorksheet, PAMSdecisionDataModel, currentAttributes, currentCell.Row, 1);

                var yearZeroRow = currentCell.Row++;
                foreach (var year in years)
                {
                    var section = year.Assets.FirstOrDefault(_ => _reportHelper.CheckAndGetValue<string>(initialAssetSummary.ValuePerTextAttribute, "CRS") == CRS);
                    if (section.TreatmentCause == TreatmentCause.CommittedProject)
                    {
                        continue;
                    }

                    // Generate data model                    
                    var decisionsDataModel = GenerateDecisionDataModel(currentAttributes, budgets, treatments, CRS, year, section);

                    // Fill in excel
                    currentCell = FillDataInWorksheet(decisionsWorksheet, decisionsDataModel, budgets.Count, currentAttributes, currentCell);
                }
                ExcelHelper.ApplyBorder(decisionsWorksheet.Cells[yearZeroRow, 1, yearZeroRow, currentCell.Column]);
            }
        }

        private PAMSDecisionDataModel GenerateDecisionDataModel(HashSet<string> currentAttributes, HashSet<string> budgets, List<string> treatments, string brKey, SimulationYearDetail year, AssetDetail section)
        {
            var decisionDataModel = GetInitialDecisionDataModel(currentAttributes, brKey, year.Year, section);

            // Budget levels
            var budgetsAtDecisionTime = section.TreatmentConsiderations.FirstOrDefault(_ => _.TreatmentName == section.AppliedTreatment)?.BudgetsAtDecisionTime ?? (section.TreatmentConsiderations.FirstOrDefault()?.BudgetsAtDecisionTime ?? new List<BudgetDetail>());
            var budgetLevels = new List<decimal>();
            if (budgetsAtDecisionTime.Count > 0)
            {
                foreach (var budget in budgets)
                {
                    var budgetAtDecisionTime = budgetsAtDecisionTime.FirstOrDefault(_ => _.BudgetName == budget);
                    if (budgetAtDecisionTime != null)
                    {
                        budgetLevels.Add(budgetAtDecisionTime.AvailableFunding);
                    }
                }
            }
            decisionDataModel.BudgetLevels = budgetLevels;

            // Treatments
            var isCashFlowProject = section.TreatmentCause == TreatmentCause.CashFlowProject;
            var decisionsTreatments = new List<PAMSDecisionTreatment>();
            foreach (var treatment in treatments)
            {
                var decisionsTreatment = new PAMSDecisionTreatment();
                var treatmentRejection = section.TreatmentRejections.FirstOrDefault(_ => _.TreatmentName == treatment);
                decisionsTreatment.Feasiable = isCashFlowProject ? "-" : (treatmentRejection == null ? PAMSAuditReportConstants.Yes : PAMSAuditReportConstants.No);
                var currentCIImprovement = Convert.ToDouble(decisionDataModel.CurrentAttributesValues.Last());
                var treatmentOption = section.TreatmentOptions.FirstOrDefault(_ => _.TreatmentName == treatment);
                decisionsTreatment.CIImprovement = treatmentOption?.ConditionChange;
                decisionsTreatment.Cost = treatmentOption != null ? treatmentOption.Cost : 0;
                decisionsTreatment.BCRatio = treatmentOption != null ? treatmentOption.Benefit / treatmentOption.Cost : 0;
                decisionsTreatment.Selected = isCashFlowProject ? PAMSAuditReportConstants.CashFlow : (section.AppliedTreatment == treatment ? PAMSAuditReportConstants.Yes : PAMSAuditReportConstants.No);
                var treatmentConsideration = section.TreatmentConsiderations.FirstOrDefault(_ => _.TreatmentName == treatment);
                decisionsTreatment.AmountSpent = treatmentConsideration != null ? treatmentConsideration.BudgetUsages.Sum(_ => _.CoveredCost) : 0;
                var budgetsUsed = treatmentConsideration?.BudgetUsages.Where(_ => _.CoveredCost > 0);
                var budgetsUsedValue = budgetsUsed != null && budgetsUsed.Any() ? string.Join(", ", budgetsUsed.Select(_ => _.BudgetName)) : string.Empty; // currently this will be single value
                decisionsTreatment.BudgetsUsed = budgetsUsedValue;
                decisionsTreatment.RejectionReason = treatmentConsideration == null ? string.Empty : (budgetsUsed != null && budgetsUsed.Any() ? string.Join(", ", budgetsUsed.Select(_ => _.BudgetName + ": " + _.Status)) : string.Join(", ", treatmentConsideration.BudgetUsages.Where(_ => _.Status != BudgetUsageStatus.ConditionNotMet).Select(_ => _.BudgetName + ": " + _.Status)));

                decisionsTreatments.Add(decisionsTreatment);
            }
            decisionDataModel.DecisionsTreatments = decisionsTreatments;
            return decisionDataModel;
        }

        private PAMSDecisionDataModel GetInitialDecisionDataModel(HashSet<string> currentAttributes, string CRS, int year, AssetSummaryDetail section)
        {
            var decisionDataModel = new PAMSDecisionDataModel
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

        private CurrentCell FillDataInWorksheet(ExcelWorksheet decisionsWorksheet, PAMSDecisionDataModel decisionsDataModel, int budgetsCount, HashSet<string> currentAttributes, CurrentCell currentCell)
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
                decisionsWorksheet.Cells[row, column++].Value = decisionsTreatment.Feasiable;
                SetDecimalFormat(decisionsWorksheet.Cells[row, column]);
                decisionsWorksheet.Cells[row, column++].Value = decisionsTreatment.CIImprovement;
                SetAccountingFormat(decisionsWorksheet.Cells[row, column]);
                decisionsWorksheet.Cells[row, column++].Value = decisionsTreatment.Cost;
                SetDecimalFormat(decisionsWorksheet.Cells[row, column]);
                decisionsWorksheet.Cells[row, column++].Value = decisionsTreatment.BCRatio;
                ExcelHelper.HorizontalCenterAlign(decisionsWorksheet.Cells[row, column]);
                decisionsWorksheet.Cells[row, column++].Value = decisionsTreatment.Selected;
                SetAccountingFormat(decisionsWorksheet.Cells[row, column]);
                decisionsWorksheet.Cells[row, column++].Value = decisionsTreatment.AmountSpent;
                decisionsWorksheet.Cells[row, column++].Value = decisionsTreatment.BudgetsUsed;
                decisionsWorksheet.Cells[row, column - 1].Style.WrapText = true;
                decisionsWorksheet.Cells[row, column++].Value = decisionsTreatment.RejectionReason;
            }
            ExcelHelper.ApplyBorder(decisionsWorksheet.Cells[row, 1, row, column - 1]);

            return new CurrentCell { Row = row + 1, Column = column - 1 };
        }

        private int FillInitialDataInWorksheet(ExcelWorksheet decisionsWorksheet, PAMSDecisionDataModel decisionsDataModel, HashSet<string> currentAttributes, int row, int column)
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

        private string ApplyApplicableFiller( string attribute, double attributeValue)
        {
            return "";
        }

        private static void SetDecimalFormat(ExcelRange cell) => ExcelHelper.SetCustomFormat(cell, ExcelHelperCellFormat.DecimalPrecision3);

        private static void SetAccountingFormat(ExcelRange cell) => ExcelHelper.SetCustomFormat(cell, ExcelHelperCellFormat.Accounting);

        private CurrentCell AddHeadersCells(ExcelWorksheet worksheet, HashSet<string> currentAttributes, HashSet<string> budgets, List<string> treatments)
        {
            int column = 1;

            worksheet.Cells.Style.WrapText = false;
            worksheet.Cells[headerRow2, column++].Value = "CRS";
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
                "Feasiable?",
                "CI\r\nImprovement",
                "Cost",
                "B/C\r\nRatio",
                "Selected?",
                "Amount\r\nSpent",
                "Budget(s)\r\nUsed",
                "Rejection Reason"
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