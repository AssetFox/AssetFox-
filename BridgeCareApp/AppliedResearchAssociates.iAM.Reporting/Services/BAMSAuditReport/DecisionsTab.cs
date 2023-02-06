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

        public DecisionsTab()
        {     
            _reportHelper = new ReportHelper();
        }

        // TODO Budgets data to be tested later when engine side updates ready
        public void Fill(ExcelWorksheet decisionsWorksheet, SimulationOutput simulationOutput, Simulation simulation)
        {
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

            // Add excel headers to excel.
            var currentCell = AddHeadersCells(decisionsWorksheet, currentAttributes, budgets, treatments);
            decisionsWorksheet.Cells.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Bottom;

            // Create report model
            var decisionsDataModels = CreateDecisionsDataModels(simulationOutput, currentAttributes, budgets, treatments);

            // TODO Add data to worksheet
            FillDynamicDataInWorkSheet(decisionsWorksheet, decisionsDataModels, currentCell);                          

            decisionsWorksheet.Cells.AutoFitColumns();

            // PerformPostAutofitAdjustments any here?? 
        }

        private void FillDynamicDataInWorkSheet(ExcelWorksheet decisionsWorksheet, List<DecisionsDataModel> decisionsDataModels, CurrentCell currentCell)
        {


        }

        private List<DecisionsDataModel> CreateDecisionsDataModels(SimulationOutput simulationOutput, HashSet<string> currentAttributes, HashSet<string> budgets, List<string> treatments)
        {
            List<DecisionsDataModel> decisionsDataModels = new List<DecisionsDataModel>();
            
            // TODO - define obj that holds data to be displayed, use similar to make sorted dict like bridgetab with some changes, no dict just list of DecisionsDataModels   
            // Use treatments, Pcurves, AnalysisMethod
            foreach (var initialAssetSummary in simulationOutput.InitialAssetSummaries)
            {                
                var brKey = _reportHelper.CheckAndGetValue<double>(initialAssetSummary.ValuePerNumericAttribute, "BRKEY_");
                foreach (var year in simulationOutput.Years.OrderBy(yr => yr.Year))
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
                        decisionsTreatment.BudgetsUsed = treatmentConsideration != null ? string.Join(",", treatmentConsideration.BudgetUsages.Select(_ => _.BudgetName)) : string.Empty;
                        decisionsTreatment.RejectionReason = GetRejectionReason(treatmentConsideration);

                        decisionsTreatments.Add(decisionsTreatment);
                    }
                    decisionsDataModel.decisionsTreatments = decisionsTreatments;

                    decisionsDataModels.Add(decisionsDataModel);
                }
            }

            return decisionsDataModels;
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
            // Row 1            
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
            // check if any other style needed or to be removed by new method in helper
            ExcelHelper.ApplyStyle(worksheet.Cells[headerRow2, 1, headerRow2, column - 1]);
            worksheet.Cells.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Bottom;

            return new CurrentCell { Row = headerRow1 + 2, Column = worksheet.Dimension.Columns + 1 };
        }

        private static int AddCurrentTreatmentsHeaders(ExcelWorksheet worksheet, List<string> treatments, int column)
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
    }
}
