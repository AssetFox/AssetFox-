using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.Analysis;
using AppliedResearchAssociates.iAM.Analysis.Engine;
using AppliedResearchAssociates.iAM.DTOs.Enums;
using AppliedResearchAssociates.iAM.ExcelHelpers;
using AppliedResearchAssociates.iAM.Reporting.Interfaces.PAMSSummaryReport;
using AppliedResearchAssociates.iAM.Reporting.Models.PAMSSummaryReport;
using AppliedResearchAssociates.iAM.Reporting.Services.PAMSSummaryReport.StaticContent;
using OfficeOpenXml;

namespace AppliedResearchAssociates.iAM.Reporting.Services.PAMSSummaryReport.PavementWorkSummaryByBudget
{
    public class PavementWorkSummaryByBudget : IPavementWorkSummaryByBudget
    {
        public void Fill(
            ExcelWorksheet worksheet,
            SimulationOutput reportOutputData,
            List<int> simulationYears,
            Dictionary<string, Budget> yearlyBudgetAmount,
            IReadOnlyCollection<SelectableTreatment> selectableTreatments)
        {
            var workSummaryByBudgetModels = CreateWorkSummaryByBudgetModels(reportOutputData);
            var committedTreatments = new HashSet<string>();

            SetupBudgetModelsAndCommittedTreatments(reportOutputData, selectableTreatments, workSummaryByBudgetModels, committedTreatments);

            var currentCell = new CurrentCell { Row = 1, Column = 1 };

            foreach (var budgetSummaryModel in workSummaryByBudgetModels)
            {
                //Filter treatment costs for the given budget
                var treatmentsDataForYear = budgetSummaryModel.YearlyData
                                                    .FindAll(_ => _.isCommitted && _.Treatment.ToLower() != PAMSConstants.NoTreatment);

                var totalBudgetPerYear = CalculateTotalBudgetPerYear(simulationYears, treatmentsDataForYear);

                //var startYear = simulationYears[0];
                //var numberOfYears = simulationYears.Count;

                currentCell.Column = 1;
                worksheet.Cells[currentCell.Row, currentCell.Column].Value = budgetSummaryModel.BudgetName;
                worksheet.Cells[currentCell.Row, currentCell.Column].Style.Font.Name = "Calibri";
                worksheet.Cells[currentCell.Row, currentCell.Column].Style.Font.Size = 18;
                worksheet.Cells[currentCell.Row, currentCell.Column].Style.Font.Bold = true;
                ExcelHelper.HorizontalCenterAlign(worksheet.Cells[currentCell.Row, currentCell.Column]);
                ExcelHelper.MergeCells(worksheet, currentCell.Row, currentCell.Column, currentCell.Row, simulationYears.Count);

                currentCell.Column = 1;

                // Add all subsections here
                currentCell.Row += 1;
                worksheet.Cells[currentCell.Row, currentCell.Column].Value = "PAMS Full Depth Asphalt Work";
                worksheet.Cells[currentCell.Row, currentCell.Column].Style.Font.Bold = true;

                currentCell.Row += 1;
                worksheet.Cells[currentCell.Row, currentCell.Column].Value = "PAMS Composite Work";
                worksheet.Cells[currentCell.Row, currentCell.Column].Style.Font.Bold = true;

                currentCell.Row += 1;
                worksheet.Cells[currentCell.Row, currentCell.Column].Value = "PAMS Concrete Work";
                worksheet.Cells[currentCell.Row, currentCell.Column].Style.Font.Bold = true;

                currentCell.Row += 1;
                worksheet.Cells[currentCell.Row, currentCell.Column].Value = "PAMS Treatment Groups Totals";
                worksheet.Cells[currentCell.Row, currentCell.Column].Style.Font.Bold = true;

                currentCell.Row += 1;
                worksheet.Cells[currentCell.Row, currentCell.Column].Value = "PAMS Work Type Totals";
                worksheet.Cells[currentCell.Row, currentCell.Column].Style.Font.Bold = true;

                currentCell.Row += 1;
                worksheet.Cells[currentCell.Row, currentCell.Column].Value = "PAMS Budget Total";
                worksheet.Cells[currentCell.Row, currentCell.Column].Style.Font.Bold = true;

                // Finally, advance for next budget label
                currentCell.Row += 2;
            }
        }

        private Dictionary<int, double> CalculateTotalBudgetPerYear(List<int> simulationYears, List<YearsData> costForCommittedBudgets)
        {
            // Fill up the total costs
            var totalBudgetPerYear = new Dictionary<int, double>();
            //var totalSpent = new List<(int year, double amount)>();
            foreach (var year in simulationYears)
            {
                var yearlyBudget = costForCommittedBudgets.FindAll(_ => _.Year == year);
                var committedAmountSum = yearlyBudget.Sum(s => s.Amount);
                totalBudgetPerYear.Add(year, committedAmountSum);
                //totalSpent.Add((year, committedAmountSum));
            }
            return totalBudgetPerYear;
        }

        private static void SetupBudgetModelsAndCommittedTreatments(SimulationOutput reportOutputData, IReadOnlyCollection<SelectableTreatment> selectableTreatments, List<WorkSummaryByBudgetModel> workSummaryByBudgetModels, HashSet<string> committedTreatments)
        {
            foreach (var summaryModel in workSummaryByBudgetModels)
            {
                foreach (var yearData in reportOutputData.Years)
                {
                    foreach (var section in yearData.Assets)
                    {
                        if (section.TreatmentCause == TreatmentCause.CommittedProject &&
                            section.AppliedTreatment.ToLower() != PAMSConstants.NoTreatment)
                        {
                            var committedTreatment = section.TreatmentConsiderations;
                            var budgetAmount = (double)committedTreatment.Sum(_ =>
                                _.BudgetUsages
                                    .Where(b => b.BudgetName == summaryModel.BudgetName)
                                    .Sum(bu => bu.CoveredCost));
                            var category = TreatmentCategory.Other;
                            if (WorkTypeMap.Map.ContainsKey(section.AppliedTreatment))
                            {
                                category = WorkTypeMap.Map[section.AppliedTreatment];
                            }
                            summaryModel.YearlyData.Add(new YearsData
                            {
                                Year = yearData.Year,
                                Treatment = section.AppliedTreatment,
                                Amount = budgetAmount,
                                isCommitted = true,
                                //costPerBPN = (_summaryReportHelper.checkAndGetValue<string>(section.ValuePerTextAttribute, "BUS_PLAN_NETWORK"), budgetAmount),
                                TreatmentCategory = category
                            });
                            committedTreatments.Add(section.AppliedTreatment);
                        }
                        else if (section.TreatmentCause != TreatmentCause.NoSelection)
                        {
                            var treatmentConsideration = section.TreatmentConsiderations;
                            var budgetAmount = (double)treatmentConsideration.Sum(_ => _.BudgetUsages
                                .Where(b => b.BudgetName == summaryModel.BudgetName)
                                .Sum(bu => bu.CoveredCost));
                            var treatmentData = selectableTreatments.FirstOrDefault(_ => _.Name == section.AppliedTreatment);
                            summaryModel.YearlyData.Add(new YearsData
                            {
                                Year = yearData.Year,
                                Treatment = section.AppliedTreatment,
                                Amount = budgetAmount,
                                //costPerBPN = (_summaryReportHelper.checkAndGetValue<string>(section.ValuePerTextAttribute, "BUS_PLAN_NETWORK"), budgetAmount),
                                TreatmentCategory = treatmentData.Category,
                                AssetType = treatmentData.AssetCategory
                            });
                        }
                    }
                }
            }
        }

        private static List<WorkSummaryByBudgetModel> CreateWorkSummaryByBudgetModels(SimulationOutput reportOutputData)
        {
            var budgetNames = new HashSet<string>();
            foreach (var yearData in reportOutputData.Years)
            {
                foreach (var budgetDetail in yearData.Budgets)
                {
                    budgetNames.Add(budgetDetail.BudgetName);
                }
            }

            var workSummaryByBudgetData = budgetNames.OrderBy(_ => _).Select(
                budgetTitle => new WorkSummaryByBudgetModel
                {
                    BudgetName = budgetTitle,
                    YearlyData = new List<YearsData>()
                }).ToList();
            return workSummaryByBudgetData;
        }
    }
}
