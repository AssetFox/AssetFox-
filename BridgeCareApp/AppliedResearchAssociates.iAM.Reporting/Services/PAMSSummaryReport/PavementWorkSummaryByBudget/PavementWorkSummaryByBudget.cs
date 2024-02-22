using System;
using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.iAM.Analysis;
using AppliedResearchAssociates.iAM.Analysis.Engine;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.DTOs.Enums;
using AppliedResearchAssociates.iAM.ExcelHelpers;
using AppliedResearchAssociates.iAM.Reporting.Models.PAMSSummaryReport;
using AppliedResearchAssociates.iAM.Reporting.Services.BAMSSummaryReport;
using AppliedResearchAssociates.iAM.Reporting.Services.PAMSSummaryReport.PavementWorkSummary;
using AppliedResearchAssociates.iAM.Reporting.Services.PAMSSummaryReport.StaticContent;
using OfficeOpenXml;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;

namespace AppliedResearchAssociates.iAM.Reporting.Services.PAMSSummaryReport.PavementWorkSummaryByBudget
{
    public class PavementWorkSummaryByBudget
    {
        private PavementWorkSummaryComputationHelper _pavementWorkSummaryComputationHelper;
        private SummaryReportHelper _summaryReportHelper;
        private Dictionary<string, List<TreatmentConsiderationDetail>> keyCashFlowFundingDetails = new();

        public PavementWorkSummaryByBudget()
        {
            _pavementWorkSummaryComputationHelper = new PavementWorkSummaryComputationHelper();
            _summaryReportHelper = new SummaryReportHelper();
            if (_summaryReportHelper == null) { throw new ArgumentNullException(nameof(_summaryReportHelper)); }
        }

        public void Fill(
            ExcelWorksheet worksheet,
            SimulationOutput reportOutputData,
            List<int> simulationYears,
            Dictionary<string, Budget> yearlyBudgetAmount,
            Dictionary<int, Dictionary<string, (decimal treatmentCost, int pavementCount, string projectSource, string treatmentCategory)>> yearlyCostCommittedProj,
            IReadOnlyCollection<SelectableTreatment> selectableTreatments,
            ICollection<CommittedProject> committedProjects,
            Dictionary<string, string> treatmentCategoryLookup,
            List<DTOs.Abstract.BaseCommittedProjectDTO> committedProjectsForWorkOutsideScope,
            bool shouldBundleFeasibleTreatments)
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
                
                PopulateYearlyCostCommittedProj(reportOutputData, budgetSummaryModel, yearlyCostCommittedProj, treatmentCategoryLookup);

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
                costBudgetsWorkSummary.FillCostBudgetWorkSummarySectionsbyBudget(worksheet, currentCell, simulationYears,
                    yearlyBudgetAmount,
                    costAndLengthPerTreatmentPerYear,
                    yearlyCostCommittedProj,
                    costAndLengthPerTreatmentGroupPerYear, // We only care about cost here
                    simulationTreatments, // This should be filtered by budget/year; do we already have this by this point?
                    workTypeTotals,                    
                    budgetSummaryModel,
                    reportOutputData,
                    committedProjectsForWorkOutsideScope,
                    shouldBundleFeasibleTreatments);

                // Finally, advance for next budget label
                currentCell.Row++;
            }
            worksheet.Cells.AutoFitColumns();
        }

        private void SetupBudgetModelsAndCommittedTreatments(SimulationOutput reportOutputData, IReadOnlyCollection<SelectableTreatment> selectableTreatments, List<WorkSummaryByBudgetModel> workSummaryByBudgetModels, HashSet<string> committedTreatments)
        {
            foreach (var summaryModel in workSummaryByBudgetModels)
            {                
                foreach (var yearData in reportOutputData.Years)
                {
                    foreach (var section in yearData.Assets)
                    {
                        // Build keyCashFlowFundingDetails
                        var crs = _summaryReportHelper.checkAndGetValue<string>(section.ValuePerTextAttribute, "CRS");
                        if (section.TreatmentStatus != TreatmentStatus.Applied)
                        {
                            var fundingSection = yearData.Assets.FirstOrDefault(_ => _summaryReportHelper.checkAndGetValue<string>(section.ValuePerTextAttribute, "CRS") == crs && _.TreatmentCause == TreatmentCause.SelectedTreatment && _.AppliedTreatment.ToLower() != BAMSConstants.NoTreatment && _.AppliedTreatment == section.AppliedTreatment);
                            if (fundingSection != null && !keyCashFlowFundingDetails.ContainsKey(crs))
                            {
                                keyCashFlowFundingDetails.Add(crs, fundingSection?.TreatmentConsiderations ?? new());
                            }
                        }

                        if (section.TreatmentCause == TreatmentCause.CommittedProject &&
                            section.AppliedTreatment.ToLower() != PAMSConstants.NoTreatment)
                        {
                            // If TreatmentStatus Applied and TreatmentCause is not CashFlowProject it means no CF then consider section obj and if Progressed that means it is CF then use obj from dict
                            var treatmentConsiderations = section.TreatmentStatus == TreatmentStatus.Applied && section.TreatmentCause != TreatmentCause.CashFlowProject ? section.TreatmentConsiderations : keyCashFlowFundingDetails[crs];
                            var budgetAmount = (double)treatmentConsiderations.Sum(_ =>
                                _.FundingCalculationOutput?.AllocationMatrix
                                    .Where(b => b.BudgetName == summaryModel.BudgetName)
                                    .Sum(bu => bu.AllocatedAmount));
                            var category = TreatmentCategory.Other;
                            if (WorkTypeMap.Map.ContainsKey(section.AppliedTreatment))
                            {
                                category = WorkTypeMap.Map[section.AppliedTreatment];
                            }
                            category = section.AppliedTreatment.Contains("Bundle") ? TreatmentCategory.Bundled : category;

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
                        else
                        {
                            // If TreatmentStatus Applied and TreatmentCause is not CashFlowProject it means no CF then consider section obj and if Progressed that means it is CF then use obj from dict
                            var treatmentConsiderations = section.TreatmentStatus == TreatmentStatus.Applied && section.TreatmentCause != TreatmentCause.CashFlowProject ?
                                                          section.TreatmentConsiderations : keyCashFlowFundingDetails[crs];
                            var budgetAmount = (double)treatmentConsiderations.Sum(_ => _.FundingCalculationOutput?.AllocationMatrix
                                .Where(b => b.BudgetName == summaryModel.BudgetName)
                                .Sum(bu => bu.AllocatedAmount));
                            var treatmentData = selectableTreatments.FirstOrDefault(_ => _.Name == section.AppliedTreatment);
                            var category = section.AppliedTreatment.Contains("Bundle") ? TreatmentCategory.Bundled : treatmentData.Category;

                            if (section.AppliedTreatment.Contains("Bundle")) { } // TODO remove later

                            var assetCategory = treatmentData != null ? (AssetCategories)treatmentData.AssetCategory : AssetCategories.Bridge; // TODO for Bundled
                            summaryModel.YearlyData.Add(new YearsData
                            {
                                Year = yearData.Year,
                                TreatmentName = section.AppliedTreatment,
                                Amount = budgetAmount,
                                //costPerBPN = (_summaryReportHelper.checkAndGetValue<string>(section.ValuePerTextAttribute, "BUS_PLAN_NETWORK"), budgetAmount),
                                TreatmentCategory = category,
                                AssetType = assetCategory,
                                SurfaceId = (int)section.ValuePerNumericAttribute["SURFACEID"]
                            });
                        }
                    }
                }
            }
        }

        private void PopulateYearlyCostCommittedProj(
                                SimulationOutput reportOutputData,
                                WorkSummaryByBudgetModel summaryModel,
                                Dictionary<int, Dictionary<string,
                                (decimal treatmentCost, int pavementCount,
                                string projectSource, string treatmentCategory)>> yearlyCostCommittedProj,
                                Dictionary<string, string> treatmentCategoryLookup)
        {
            yearlyCostCommittedProj.Clear();

            foreach (var yearData in reportOutputData.Years)
            {
                foreach (var section in yearData.Assets)
                {
                    var crs = _summaryReportHelper.checkAndGetValue<string>(section.ValuePerTextAttribute, "CRS");
                    // If TreatmentStatus Applied and TreatmentCause is not CashFlowProject it means no CF then consider section obj and if Progressed that means it is CF then use obj from dict
                    var treatmentConsiderations = section.TreatmentStatus == TreatmentStatus.Applied && section.TreatmentCause != TreatmentCause.CashFlowProject ?
                                                    section.TreatmentConsiderations : keyCashFlowFundingDetails[crs];
                    if (treatmentConsiderations.Any(tc => tc.FundingCalculationOutput != null && tc.FundingCalculationOutput.AllocationMatrix.Any(bu => bu.BudgetName == summaryModel.BudgetName)))
                    {
                        if (section.TreatmentCause == TreatmentCause.CommittedProject &&
                            section.AppliedTreatment.ToLower() != PAMSConstants.NoTreatment)
                        {
                            var committedCost = treatmentConsiderations.Sum(_ =>
                                _.FundingCalculationOutput?.AllocationMatrix.Where(b => b.BudgetName == summaryModel.BudgetName).Sum(bu => bu.AllocatedAmount)) ?? 0;
                            var appliedTreatment = section.AppliedTreatment;
                            var treatmentCategory = section.AppliedTreatment.Contains("Bundle") ? PAMSConstants.Bundled : treatmentCategoryLookup[appliedTreatment];

                            if (section.AppliedTreatment.Contains("Bundle")) { } // TODO remove later

                            // Populating yearlyCostCommittedProj dictionary
                            if (!yearlyCostCommittedProj.ContainsKey(yearData.Year))
                            {
                                yearlyCostCommittedProj[yearData.Year] = new Dictionary<string, (decimal, int, string, string)>();
                            }

                            if (!yearlyCostCommittedProj[yearData.Year].ContainsKey(appliedTreatment))
                            {
                                yearlyCostCommittedProj[yearData.Year].Add(appliedTreatment, (committedCost, 1, section.ProjectSource, treatmentCategory));
                            }
                            else
                            {
                                var currentRecord = yearlyCostCommittedProj[yearData.Year][appliedTreatment];
                                var treatmentCost = currentRecord.treatmentCost + committedCost;
                                var pavementCount = currentRecord.pavementCount + 1;
                                var projectSource = currentRecord.projectSource;
                                yearlyCostCommittedProj[yearData.Year][appliedTreatment] = (treatmentCost, pavementCount, projectSource, treatmentCategory);
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
