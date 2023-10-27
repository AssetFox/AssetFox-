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
using AppliedResearchAssociates.iAM.Reporting.Models.PAMSSummaryReport;
using AppliedResearchAssociates.iAM.Reporting.Services.PAMSSummaryReport.PavementWorkSummary;
using AppliedResearchAssociates.iAM.Reporting.Services.PAMSSummaryReport.StaticContent;
using OfficeOpenXml;

namespace AppliedResearchAssociates.iAM.Reporting.Services.PAMSSummaryReport.PavementWorkSummaryByBudget
{
    public class PavementWorkSummaryByBudget
    {
        private PavementWorkSummaryComputationHelper _pavementWorkSummaryComputationHelper;

        public PavementWorkSummaryByBudget()
        {
            _pavementWorkSummaryComputationHelper = new PavementWorkSummaryComputationHelper();
        }

        public void Fill(
            ExcelWorksheet worksheet,
            SimulationOutput reportOutputData,
            List<int> simulationYears,
            Dictionary<string, Budget> yearlyBudgetAmount,
            Dictionary<int, Dictionary<string, (decimal treatmentCost, int pavementCount, string projectSource)>> yearlyCostCommittedProj,
            IReadOnlyCollection<SelectableTreatment> selectableTreatments,
            ICollection<CommittedProject> committedProjects)
        {
            var workSummaryByBudgetModels = CreateWorkSummaryByBudgetModels(reportOutputData);
            var committedTreatments = new HashSet<string>();

            SetupBudgetModelsAndCommittedTreatments(reportOutputData, selectableTreatments, workSummaryByBudgetModels, committedTreatments);

            var simulationTreatments = new List<(string Name, AssetCategories AssetType, TreatmentCategory Category)>();
            foreach (var item in selectableTreatments)
            {
                simulationTreatments.Add((item.Name, (AssetCategories)item.AssetCategory, item.Category));
            }
            simulationTreatments.Sort((a, b) => a.Name.CompareTo(b.Name));

            var currentCell = new CurrentCell { Row = 1, Column = 1 };

            foreach (var budgetSummaryModel in workSummaryByBudgetModels)
            {
                var noData = !budgetSummaryModel.YearlyData.Any(datum => datum.Amount != 0);
                if (noData)
                {
                    continue;
                }
                PopulateYearlyCostCommittedProj(reportOutputData, budgetSummaryModel, yearlyCostCommittedProj);

                // Inside iteration since each section has its own budget analysis section.
                var costBudgetsWorkSummary = new CostBudgetsWorkSummary();

                var costAndLengthPerTreatmentPerYear = new Dictionary<int, Dictionary<string, (decimal treatmentCost, decimal compositeTreatmentCost, int length)>>();
                var costAndLengthPerTreatmentGroupPerYear = new Dictionary<int, Dictionary<PavementTreatmentHelper.TreatmentGroup, (decimal treatmentCost, int length)>>();

                currentCell.Column = 1;
                worksheet.Cells[currentCell.Row, currentCell.Column].Value = budgetSummaryModel.BudgetName;
                worksheet.Cells[currentCell.Row, currentCell.Column].Style.Font.Name = "Calibri";
                worksheet.Cells[currentCell.Row, currentCell.Column].Style.Font.Size = 18;
                worksheet.Cells[currentCell.Row, currentCell.Column].Style.Font.Bold = true;
                ExcelHelper.HorizontalCenterAlign(worksheet.Cells[currentCell.Row, currentCell.Column]);
                ExcelHelper.MergeCells(worksheet, currentCell.Row, currentCell.Column, currentCell.Row, simulationYears.Count + 2);

                if (budgetSummaryModel.YearlyData.Count == 0)
                {
                    // Nothing to see here, but include the empty budgets anyway for now.
                    // Add all treatments here, set all values to 0
                    foreach (var year in simulationYears)
                    {
                        if (!costAndLengthPerTreatmentPerYear.ContainsKey(year))
                        {
                            costAndLengthPerTreatmentPerYear.Add(year, new Dictionary<string, (decimal treatmentCost, decimal compositeTreatmentCost, int length)>());
                        }
                        var treatmentData = costAndLengthPerTreatmentPerYear[year];

                        if (!costAndLengthPerTreatmentGroupPerYear.ContainsKey(year))
                        {
                            costAndLengthPerTreatmentGroupPerYear.Add(year, new Dictionary<PavementTreatmentHelper.TreatmentGroup, (decimal treatmentCost, int length)>());
                        }
                        var treatmentGroupData = costAndLengthPerTreatmentGroupPerYear[year];

                        foreach (var treatment in simulationTreatments)
                        {
                            if (treatment.Name != PAMSConstants.NoTreatmentForWorkSummary)
                            {
                                treatmentData.Add(treatment.Name, (0, 0, 0));

                                var treatmentGroup = PavementTreatmentHelper.GetTreatmentGroup(treatment.Name);
                                if (!treatmentGroupData.ContainsKey(treatmentGroup))
                                {
                                    treatmentGroupData.Add(treatmentGroup, (0, 0));
                                }
                            }
                        }
                    }
                }
                else
                {
                    _pavementWorkSummaryComputationHelper.FillDataToUseInExcel(budgetSummaryModel, costAndLengthPerTreatmentPerYear, costAndLengthPerTreatmentGroupPerYear);
                }

                var workTypeTotals = _pavementWorkSummaryComputationHelper.CalculateWorkTypeTotals(costAndLengthPerTreatmentPerYear, simulationTreatments);
                costBudgetsWorkSummary.FillCostBudgetWorkSummarySections(worksheet, currentCell, simulationYears,
                    yearlyBudgetAmount,
                    costAndLengthPerTreatmentPerYear,
                    yearlyCostCommittedProj,
                    costAndLengthPerTreatmentGroupPerYear, // We only care about cost here
                    simulationTreatments, // This should be filtered by budget/year; do we already have this by this point?
                    workTypeTotals,
                    committedProjects);

                // Finally, advance for next budget label
                currentCell.Row++;
            }
            worksheet.Cells.AutoFitColumns();
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
                                TreatmentName = section.AppliedTreatment,
                                Amount = budgetAmount,
                                isCommitted = true,
                                //costPerBPN = (_summaryReportHelper.checkAndGetValue<string>(section.ValuePerTextAttribute, "BUS_PLAN_NETWORK"), budgetAmount),
                                TreatmentCategory = category,
                                SurfaceId = (int)section.ValuePerNumericAttribute["SURFACEID"]
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
                                TreatmentName = section.AppliedTreatment,
                                Amount = budgetAmount,
                                //costPerBPN = (_summaryReportHelper.checkAndGetValue<string>(section.ValuePerTextAttribute, "BUS_PLAN_NETWORK"), budgetAmount),
                                TreatmentCategory = treatmentData.Category,
                                AssetType = (AssetCategories)treatmentData.AssetCategory,
                                SurfaceId = (int)section.ValuePerNumericAttribute["SURFACEID"]
                            });
                        }
                    }
                }
            }
        }

            private static void PopulateYearlyCostCommittedProj(
                                    SimulationOutput reportOutputData,
                                    WorkSummaryByBudgetModel summaryModel,
                                    Dictionary<int, Dictionary<string,
                                    (decimal treatmentCost, int pavementCount,
                                    string projectSource)>> yearlyCostCommittedProj)
        {
            yearlyCostCommittedProj.Clear();

            foreach (var yearData in reportOutputData.Years)
            {
                foreach (var section in yearData.Assets)
                {
                    if (section.TreatmentConsiderations.Any(tc => tc.BudgetUsages.Any(bu => bu.BudgetName == summaryModel.BudgetName)))
                    {
                        if (section.TreatmentCause == TreatmentCause.CommittedProject &&
                            section.AppliedTreatment.ToLower() != PAMSConstants.NoTreatment)
                        {
                            var committedCost = section.TreatmentConsiderations.Sum(_ =>
                                _.BudgetUsages.Where(b => b.BudgetName == summaryModel.BudgetName).Sum(bu => bu.CoveredCost));

                            // Populating yearlyCostCommittedProj dictionary
                            if (!yearlyCostCommittedProj.ContainsKey(yearData.Year))
                            {
                                yearlyCostCommittedProj[yearData.Year] = new Dictionary<string, (decimal, int, string)>();
                            }

                            if (!yearlyCostCommittedProj[yearData.Year].ContainsKey(section.AppliedTreatment))
                            {
                                yearlyCostCommittedProj[yearData.Year].Add(section.AppliedTreatment, (committedCost, 1, section.ProjectSource));
                            }
                            else
                            {
                                var currentRecord = yearlyCostCommittedProj[yearData.Year][section.AppliedTreatment];
                                var treatmentCost = currentRecord.treatmentCost + committedCost;
                                var pavementCount = currentRecord.pavementCount + 1;
                                var projectSource = currentRecord.projectSource;
                                yearlyCostCommittedProj[yearData.Year][section.AppliedTreatment] = (treatmentCost, pavementCount, projectSource);
                            }
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
