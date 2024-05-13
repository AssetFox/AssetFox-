using System;
using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.iAM.Analysis;
using AppliedResearchAssociates.iAM.Analysis.Engine;
using AppliedResearchAssociates.iAM.DTOs.Abstract;
using AppliedResearchAssociates.iAM.DTOs.Enums;
using AppliedResearchAssociates.iAM.ExcelHelpers;
using AppliedResearchAssociates.iAM.Reporting.Models.PAMSSummaryReport;
using AppliedResearchAssociates.iAM.Reporting.Services.BAMSSummaryReport;
using AppliedResearchAssociates.iAM.Reporting.Services.PAMSSummaryReport.PavementWorkSummary;
using AppliedResearchAssociates.iAM.Reporting.Services.PAMSSummaryReport.StaticContent;
using OfficeOpenXml;

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
            List<DTOs.Abstract.BaseCommittedProjectDTO> committedProjectList,
            List<DTOs.Abstract.BaseCommittedProjectDTO> committedProjectsForWorkOutsideScope,
            bool shouldBundleFeasibleTreatments)
        {
            var workSummaryByBudgetModels = CreateWorkSummaryByBudgetModels(reportOutputData);
            var committedTreatments = new HashSet<string>();            

            SetupBudgetModelsAndCommittedTreatments(reportOutputData, selectableTreatments, workSummaryByBudgetModels, committedTreatments, shouldBundleFeasibleTreatments);

            var simulationTreatments = new List<(string Name, string AssetType, TreatmentCategory Category)>();
            foreach (var item in selectableTreatments)
            {
                simulationTreatments.Add((item.Name, item.AssetCategory, item.Category));
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
                
                PopulateYearlyCostCommittedProj(reportOutputData, budgetSummaryModel, yearlyCostCommittedProj, treatmentCategoryLookup, committedProjectList);

                // Inside iteration since each section has its own budget analysis section.
                var costBudgetsWorkSummary = new CostBudgetsWorkSummary();

                var costLengthPerSurfaceIdPerTreatmentPerYear = new Dictionary<int, Dictionary<string, Dictionary<int, (decimal treatmentCost, decimal compositeTreatmentCost, int length)>>>();
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
                        if (!costLengthPerSurfaceIdPerTreatmentPerYear.ContainsKey(year))
                        {
                            costLengthPerSurfaceIdPerTreatmentPerYear.Add(year,
                                new Dictionary<string, // treatmentName
                                Dictionary<int, // surfaceId
                            (decimal treatmentCost, decimal compositeTreatmentCost, int length)>>());
                        }
                        var treatmentData = costLengthPerSurfaceIdPerTreatmentPerYear[year];

                        if (!costAndLengthPerTreatmentGroupPerYear.ContainsKey(year))
                        {
                            costAndLengthPerTreatmentGroupPerYear.Add(year, new Dictionary<PavementTreatmentHelper.TreatmentGroup, (decimal treatmentCost, int length)>());
                        }
                        var treatmentGroupData = costAndLengthPerTreatmentGroupPerYear[year];

                        foreach (var treatment in simulationTreatments)
                        {
                            if (treatment.Name != PAMSConstants.NoTreatmentForWorkSummary)
                            {
                                var dict = new Dictionary<int, (decimal treatmentCost, decimal compositeTreatmentCost, int length)>
                                {
                                    { 0, (0, 0, 0) }
                                };
                                treatmentData.Add(treatment.Name, dict);
                                var treatmentGroup = PavementTreatmentHelper.GetTreatmentGroup(treatment.Name, simulationTreatments);
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
                    _pavementWorkSummaryComputationHelper.FillDataToUseInExcel(budgetSummaryModel, costLengthPerSurfaceIdPerTreatmentPerYear, costAndLengthPerTreatmentGroupPerYear, simulationTreatments);
                }

                var workTypeTotals = _pavementWorkSummaryComputationHelper.CalculateWorkTypeTotals(costLengthPerSurfaceIdPerTreatmentPerYear, simulationTreatments);
                costBudgetsWorkSummary.FillCostBudgetWorkSummarySectionsbyBudget(worksheet, currentCell, simulationYears,
                    yearlyBudgetAmount,
                    costLengthPerSurfaceIdPerTreatmentPerYear,
                    yearlyCostCommittedProj,
                    costAndLengthPerTreatmentGroupPerYear, // We only care about cost here
                    simulationTreatments, // This should be filtered by budget/year; do we already have this by this point?
                    workTypeTotals,                    
                    budgetSummaryModel,
                    reportOutputData,
                    committedProjectList,
                    committedProjectsForWorkOutsideScope,
                    shouldBundleFeasibleTreatments);

                // Finally, advance for next budget label
                currentCell.Row++;
            }
            worksheet.Cells.AutoFitColumns();
        }

        private void SetupBudgetModelsAndCommittedTreatments(SimulationOutput reportOutputData, IReadOnlyCollection<SelectableTreatment> selectableTreatments, List<WorkSummaryByBudgetModel> workSummaryByBudgetModels, HashSet<string> committedTreatments, bool shouldBundleFeasibleTreatments)
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
                            var treatmentConsiderations = section.TreatmentStatus == TreatmentStatus.Applied && section.TreatmentCause !=
                                                          TreatmentCause.CashFlowProject ? section.TreatmentConsiderations : keyCashFlowFundingDetails[crs];
                            var treatmentConsideration = shouldBundleFeasibleTreatments ?
                                                        treatmentConsiderations.FirstOrDefault() :
                                                        treatmentConsiderations.FirstOrDefault(_ => _.TreatmentName == section.AppliedTreatment);
                            var appliedTreatment = treatmentConsideration?.TreatmentName ?? section.AppliedTreatment;
                            var budgetAmount = (double)treatmentConsiderations.Sum(_ => _.FundingCalculationOutput?.AllocationMatrix.
                                               Where(b => b.BudgetName == summaryModel.BudgetName).
                                               Sum(bu => bu.AllocatedAmount) ?? 0);
                            var treatmentData = appliedTreatment.Contains("Bundle") ?
                                                selectableTreatments.FirstOrDefault(_ => appliedTreatment.Contains(_.Name)) :
                                                selectableTreatments.FirstOrDefault(_ => _.Name == appliedTreatment);
                            var category = section.AppliedTreatment.Contains("Bundle") ? TreatmentCategory.Bundled : treatmentData.Category;
                            var assetCategory = (string)treatmentData.AssetCategory;
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
                                Dictionary<string, string> treatmentCategoryLookup,
                                List<BaseCommittedProjectDTO> committedProjectList)
        {
            yearlyCostCommittedProj.Clear();

            foreach (var yearData in reportOutputData.Years)
            {
                // Populating yearlyCostCommittedProj dictionary
                if (!yearlyCostCommittedProj.ContainsKey(yearData.Year))
                {
                    yearlyCostCommittedProj[yearData.Year] = new Dictionary<string, (decimal, int, string, string)>();
                }

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
                            if (!yearlyCostCommittedProj[yearData.Year].ContainsKey(appliedTreatment))
                            {
                                var projectSource = committedProjectList.FirstOrDefault(_ => appliedTreatment.Contains(_.Treatment))?.ProjectSource.ToString() ?? string.Empty;
                                yearlyCostCommittedProj[yearData.Year].Add(appliedTreatment, (committedCost, 1, projectSource, treatmentCategory));
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
