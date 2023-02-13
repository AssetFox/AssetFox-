using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using AppliedResearchAssociates.iAM.Analysis;
using AppliedResearchAssociates.iAM.Analysis.Engine;
using AppliedResearchAssociates.iAM.ExcelHelpers;
using AppliedResearchAssociates.iAM.Reporting.Interfaces;
using AppliedResearchAssociates.iAM.Reporting.Interfaces.BAMSAuditReport;
using AppliedResearchAssociates.iAM.Reporting.Models;
using AppliedResearchAssociates.iAM.Reporting.Models.BAMSAuditReport;
using OfficeOpenXml;

namespace AppliedResearchAssociates.iAM.Reporting.Services.BAMSAuditReport
{
    public class DecisionsTab : IDecisionsTab
    {
        private IReportHelper _reportHelper;
        private const int headerRow1 = 1;
        private const int headerRow2 = 2;
        private List<int> columnNumbersBudgetsUsed;

        public DecisionsTab()
        {     
            _reportHelper = new ReportHelper();
        }

        // TODO Budgets data to be tested later when engine side updates ready
        public void Fill(ExcelWorksheet decisionsWorksheet, SimulationOutput simulationOutput, Simulation simulation)
        {
            columnNumbersBudgetsUsed = new List<int>();
            var currentAttributes = new HashSet<string>();
            // Distinct performance curve attributes
            foreach(var performanceCurve in simulation.PerformanceCurves)
            {
                currentAttributes.Add(performanceCurve.Attribute.Name);
            }
            currentAttributes.Add(simulation.AnalysisMethod.Benefit.Attribute.Name);

            // Distinct budgets            
            var budgets = new HashSet<string>();            
            foreach (var item in simulationOutput.Years.FirstOrDefault()?.Budgets)
            {
                budgets.Add(item.BudgetName);
            }

            var treatments = new List<string>();
            treatments = simulation.Treatments.Where(_=>_.Name != "No Treatment")?.OrderBy(_ => _.Name).Select(_ => _.Name).ToList();

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
                // For initial compute
                var prevYearCIImprovement = CheckGetValue(initialAssetSummary.ValuePerNumericAttribute, currentAttributes.Last());
                foreach (var year in simulationOutput.Years.OrderBy(yr => yr.Year))
                {
                    var section = year.Assets.FirstOrDefault(_ => CheckGetValue(_.ValuePerNumericAttribute, "BRKEY_") == brKey);                    
                    if (section.TreatmentCause == TreatmentCause.CommittedProject)
                    {
                        continue;
                    }

                    // Generate data model
                    var decisionsDataModel = GenerateDecisionsDataModel(currentAttributes, budgets, treatments, brKey, year, section, prevYearCIImprovement);
                    prevYearCIImprovement = decisionsDataModel.CurrentAttributesValues.Last();

                    // Fill in excel
                    currentCell = FillDataInWorkSheet(decisionsWorksheet, decisionsDataModel, budgets.Count, currentCell);                    
                }
            }
        }

        private DecisionsDataModel GenerateDecisionsDataModel(HashSet<string> currentAttributes, HashSet<string> budgets, List<string> treatments, double brKey, SimulationYearDetail year, AssetDetail section, double prevYearCIImprovement)
        {
            var decisionsDataModel = new DecisionsDataModel
            {
                BRKey = brKey,
                AnalysisYear = year.Year,                
            };
            
            var isCashFlowProject = section.TreatmentCause == TreatmentCause.CashFlowProject;
            // Current
            var currentAttributesValues = new List<double>();
            for (int index = 0; index < currentAttributes.Count - 1; index++)
            {
                var value = CheckGetValue(section.ValuePerNumericAttribute, currentAttributes.ElementAt(index));
                currentAttributesValues.Add(value);
            }
            // analysis benefit attribute
            currentAttributesValues.Add(CheckGetValue(section.ValuePerNumericAttribute, currentAttributes.Last()));
            decisionsDataModel.CurrentAttributesValues = currentAttributesValues;

            // Budget levels
            var budgetsAtDecisionTime = section.BudgetsAtDecisionTime ?? new List<BudgetDetail>();
            var budgetLevels = new List<decimal>();
            if (budgetsAtDecisionTime.Count > 0)
            {
                foreach (var budget in budgets)
                {
                    var budgetAtDecisionTime = budgetsAtDecisionTime.FirstOrDefault(_ => _.BudgetName == budget);
                    if (budgetAtDecisionTime == null)
                    {
                        // TODO any action here?
                    }
                    else
                    {
                        budgetLevels.Add(budgetAtDecisionTime.AvailableFunding);
                    }
                }
            }
            decisionsDataModel.BudgetLevels = budgetLevels;

            if (brKey == 153 && year.Year == 2022)//== 21539)// 13648) //
            {

            }

            // Treatments
            var decisionsTreatments = new List<DecisionTreatment>();
            foreach (var treatment in treatments)
            {                
                var decisionsTreatment = new DecisionTreatment();                
                var treatmentRejection = section.TreatmentRejections.FirstOrDefault(_ => _.TreatmentName == treatment);
                decisionsTreatment.Feasiable = isCashFlowProject ? "-" : (treatmentRejection == null ? AuditReportConstants.Yes : AuditReportConstants.No);
                var currentCIImprovement = Convert.ToDouble(decisionsDataModel.CurrentAttributesValues.Last());
                decisionsTreatment.CIImprovement = Math.Abs(prevYearCIImprovement - currentCIImprovement);
                var treatmentOption = section.TreatmentOptions.FirstOrDefault(_ => _.TreatmentName == treatment);
                decisionsTreatment.Cost = treatmentOption != null ? treatmentOption.Cost : 0;
                decisionsTreatment.BCRatio = treatmentOption != null ? treatmentOption.Benefit / treatmentOption.Cost : 0;
                decisionsTreatment.Selected = isCashFlowProject ? AuditReportConstants.CashFlow : (section.AppliedTreatment == treatment ? AuditReportConstants.Yes : AuditReportConstants.No);
                var treatmentConsideration = section.TreatmentConsiderations.FirstOrDefault(_ => _.TreatmentName == treatment);
                decisionsTreatment.AmountSpent = treatmentConsideration != null ? treatmentConsideration.BudgetUsages.Sum(_ => _.CoveredCost) : 0;
                var budgetsUsed = treatmentConsideration?.BudgetUsages.Where(_ => _.CoveredCost > 0);
                var budgetsUsedValue = budgetsUsed != null && budgetsUsed.Count() != 0 ? string.Join(", ", budgetsUsed.Select(_ => _.BudgetName)) : string.Empty; // currently this will be single value
                decisionsTreatment.BudgetsUsed = budgetsUsedValue;
                decisionsTreatment.RejectionReason = treatmentConsideration == null ? string.Empty : (budgetsUsed != null && budgetsUsed.Count() != 0 ? string.Join(", ", budgetsUsed.Select(_ => _.BudgetName + ": " + _.Status)) : string.Join(", ", treatmentConsideration.BudgetUsages.Select(_ => _.BudgetName + ": " + _.Status)));

                decisionsTreatments.Add(decisionsTreatment);
            }
            decisionsDataModel.DecisionsTreatments = decisionsTreatments;
            return decisionsDataModel;
        }

        private double CheckGetValue(Dictionary<string, double> valuePerNumericAttribute, string attribute) => _reportHelper.CheckAndGetValue<double>(valuePerNumericAttribute, attribute);

        private CurrentCell FillDataInWorkSheet(ExcelWorksheet decisionsWorksheet, DecisionsDataModel decisionsDataModel, int budgetsCount, CurrentCell currentCell)
        {
            var row = currentCell.Row;
            int column = 1;

            ExcelHelper.HorizontalCenterAlign(decisionsWorksheet.Cells[row, column]);
            decisionsWorksheet.Cells[row, column++].Value = decisionsDataModel.BRKey;
            ExcelHelper.HorizontalCenterAlign(decisionsWorksheet.Cells[row, column]);
            decisionsWorksheet.Cells[row, column++].Value = decisionsDataModel.AnalysisYear;

            foreach (var currentAttributesValue in decisionsDataModel.CurrentAttributesValues)
            {
                SetDecimalFormat(decisionsWorksheet.Cells[row, column]);
                decisionsWorksheet.Cells[row, column++].Value = currentAttributesValue;
            }

            foreach (var budgetLevel in decisionsDataModel.BudgetLevels)
            {
                SetDecimalFormat(decisionsWorksheet.Cells[row, column]);
                decisionsWorksheet.Cells[row, column++].Value = budgetLevel;
            }
            // TODO remove this once decisionsDataModel.BudgetLevels starts getting data
            if (decisionsDataModel.BudgetLevels.Count == 0)
            {
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
            // Dynamic headers for row 1 based on simulation treatments
            foreach (var treatment in treatments)
            {
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

        public static void PerformPostAutofitAdjustments(ExcelWorksheet worksheet,List<int> columnNumbersBudgetsUsed)
        {
            foreach (var columnNumber in columnNumbersBudgetsUsed)
            {
                worksheet.Column(columnNumber).SetTrueWidth(25);
                // Rejection reason
                worksheet.Column(columnNumber + 1).SetTrueWidth(50);
                worksheet.Column(columnNumber + 1).Style.WrapText = true;
            }
        }
    }
}
