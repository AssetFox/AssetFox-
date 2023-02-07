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
            treatments = simulation.Treatments?.OrderBy(_ => _.Name).Select(_ => _.Name).ToList();

            // Add headers to excel
            var currentCell = AddHeadersCells(decisionsWorksheet, currentAttributes, budgets, treatments);

            // Fill data in excel
            FillDynamicDataInWorkSheet(simulationOutput, currentAttributes, budgets, treatments, decisionsWorksheet, currentCell);
                                                 
            decisionsWorksheet.Cells.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Bottom;
            decisionsWorksheet.Cells.AutoFitColumns();
            PerformPostAutofitAdjustments(decisionsWorksheet, columnNumbersBudgetsUsed, currentAttributes.Count);
        }        
        
        private void FillDynamicDataInWorkSheet(SimulationOutput simulationOutput, HashSet<string> currentAttributes, HashSet<string> budgets, List<string> treatments, ExcelWorksheet decisionsWorksheet, CurrentCell currentCell)
        {                        
            foreach (var initialAssetSummary in simulationOutput.InitialAssetSummaries)
            {                
                var brKey = _reportHelper.CheckAndGetValue<double>(initialAssetSummary.ValuePerNumericAttribute, "BRKEY_");
                foreach (var year in simulationOutput.Years.OrderBy(yr => yr.Year))
                {
                    // Generate data model
                    var decisionsDataModel = GenerateDecisionsDataModel(currentAttributes, budgets, treatments, brKey, year);

                    // Fill in excel
                    currentCell = FillDataInWorkSheet(decisionsWorksheet, decisionsDataModel, budgets.Count, currentCell);
                }
            }
        }

        private DecisionsDataModel GenerateDecisionsDataModel(HashSet<string> currentAttributes, HashSet<string> budgets, List<string> treatments, double brKey, SimulationYearDetail year)
        {
            var decisionsDataModel = new DecisionsDataModel();
            decisionsDataModel.BRKey = brKey;
            decisionsDataModel.AnalysisYear = year.Year;

            var section = year.Assets.FirstOrDefault(_ => _reportHelper.CheckAndGetValue<double>(_.ValuePerNumericAttribute, "BRKEY_") == brKey);
            // Current
            var currentAttributesValues = new List<double>();
            for (int index = 0; index < currentAttributes.Count - 1; index++)
            {
                var value = _reportHelper.CheckAndGetValue<double>(section.ValuePerNumericAttribute, currentAttributes.ElementAt(index));
                currentAttributesValues.Add(value);
            }
            // analysis benefit attribute
            currentAttributesValues.Add(_reportHelper.CheckAndGetValue<double>(section.ValuePerNumericAttribute, currentAttributes.Last()));
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
                        // TODO raise error?
                    }
                    else
                    {
                        budgetLevels.Add(budgetAtDecisionTime.AvailableFunding);
                    }
                }
            }
            decisionsDataModel.BudgetLevels = budgetLevels;

            // Treatments
            var decisionsTreatments = new List<DecisionTreatment>();
            foreach (var treatment in treatments)
            {
                var decisionsTreatment = new DecisionTreatment();
                var treatmentRejection = section.TreatmentRejections.FirstOrDefault(_ => _.TreatmentName == treatment);
                decisionsTreatment.Feasiable = treatmentRejection != null && treatmentRejection.TreatmentRejectionReason == TreatmentRejectionReason.NotFeasible ? false : true;
                decisionsTreatment.CIImprovement = 10 - Convert.ToDouble(decisionsDataModel.CurrentAttributesValues.Last());
                var treatmentOption = section.TreatmentOptions.FirstOrDefault(_ => _.TreatmentName == treatment);
                decisionsTreatment.Cost = treatmentOption != null ? treatmentOption.Cost : 0;
                decisionsTreatment.BCRatio = treatmentOption != null ? treatmentOption.Benefit / treatmentOption.Cost : 0;
                decisionsTreatment.Selected = section.AppliedTreatment == treatment;
                var treatmentConsideration = section.TreatmentConsiderations.FirstOrDefault(_ => _.TreatmentName == treatment);
                decisionsTreatment.AmountSpent = treatmentConsideration != null ? treatmentConsideration.BudgetUsages.Sum(_ => _.CoveredCost) : 0;
                decisionsTreatment.BudgetsUsed = treatmentConsideration != null ? string.Join(", ", treatmentConsideration.BudgetUsages.Select(_ => _.BudgetName)) : string.Empty;
                decisionsTreatment.RejectionReason = GetRejectionReason(treatmentConsideration);

                decisionsTreatments.Add(decisionsTreatment);
            }
            decisionsDataModel.DecisionsTreatments = decisionsTreatments;
            return decisionsDataModel;
        }

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
                ExcelHelper.SetCustomFormat(decisionsWorksheet.Cells[row, column], ExcelHelperCellFormat.DecimalPrecision3);
                decisionsWorksheet.Cells[row, column++].Value = currentAttributesValue;
            }

            foreach (var budgetLevel in decisionsDataModel.BudgetLevels)
            {
                ExcelHelper.SetCustomFormat(decisionsWorksheet.Cells[row, column], ExcelHelperCellFormat.DecimalPrecision3);
                decisionsWorksheet.Cells[row, column++].Value = budgetLevel;
            }
            // TODO rmove this once decisionsDataModel.BudgetLevels starts getting data
            if (decisionsDataModel.BudgetLevels.Count == 0)
            {
                column += budgetsCount;
            }

            foreach (var decisionsTreatment in decisionsDataModel.DecisionsTreatments)
            {
                ExcelHelper.HorizontalCenterAlign(decisionsWorksheet.Cells[row, column]);
                decisionsWorksheet.Cells[row, column++].Value = decisionsTreatment.Feasiable ? AuditReportConstants.Yes : AuditReportConstants.No;
                ExcelHelper.SetCustomFormat(decisionsWorksheet.Cells[row, column], ExcelHelperCellFormat.DecimalPrecision3);
                decisionsWorksheet.Cells[row, column++].Value = decisionsTreatment.CIImprovement;
                ExcelHelper.SetCustomFormat(decisionsWorksheet.Cells[row, column], ExcelHelperCellFormat.Accounting);
                decisionsWorksheet.Cells[row, column++].Value = decisionsTreatment.Cost;
                ExcelHelper.SetCustomFormat(decisionsWorksheet.Cells[row, column], ExcelHelperCellFormat.DecimalPrecision3);
                decisionsWorksheet.Cells[row, column++].Value = decisionsTreatment.BCRatio;
                ExcelHelper.HorizontalCenterAlign(decisionsWorksheet.Cells[row, column]);
                decisionsWorksheet.Cells[row, column++].Value = decisionsTreatment.Selected ? AuditReportConstants.Yes : AuditReportConstants.No;
                ExcelHelper.SetCustomFormat(decisionsWorksheet.Cells[row, column], ExcelHelperCellFormat.Accounting);
                decisionsWorksheet.Cells[row, column++].Value = decisionsTreatment.AmountSpent;
                decisionsWorksheet.Cells[row, column++].Value = decisionsTreatment.BudgetsUsed;
                decisionsWorksheet.Cells[row, column - 1].Style.WrapText = true;
                decisionsWorksheet.Cells[row, column++].Value = decisionsTreatment.RejectionReason;
            }
            ExcelHelper.ApplyBorder(decisionsWorksheet.Cells[row, 1, row, column - 1]);
            return new CurrentCell { Row = row + 1, Column = column - 1 };
        }

        private string GetRejectionReason(TreatmentConsiderationDetail treatmentConsideration)
        {
            if (treatmentConsideration == null) return string.Empty;
            var rejectionReason = treatmentConsideration.BudgetUsages.All(_ => _.Status == BudgetUsageStatus.CostNotCovered) ? "No available funding" : string.Empty;

            // TODO ask for more cases to be checked here
            return rejectionReason; 
        }

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
                        
            ExcelHelper.ApplyColor(worksheet.Cells[headerRow2, 3, headerRow2, column - 1], Color.FromArgb(255, 242, 204));
            ExcelHelper.ApplyBorder(worksheet.Cells[headerRow1, 1, headerRow2, worksheet.Dimension.Columns]);            
            ExcelHelper.ApplyStyle(worksheet.Cells[headerRow2, 1, headerRow2, column - 1]);                                  

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

        public void PerformPostAutofitAdjustments(ExcelWorksheet worksheet,List<int> columnNumbersBudgetsUsed, int currentAttributesCount)
        {
            foreach (var columnNumber in columnNumbersBudgetsUsed)
            {
                worksheet.Column(columnNumber).SetTrueWidth(30);
            }

            //var toCol = currentAttributesCount + 3;
            //for(int col = 3; col < currentAttributesCount; col++)
            //{
            //    worksheet.Column(col).SetTrueWidth(12);
            //}
        }
    }
}
