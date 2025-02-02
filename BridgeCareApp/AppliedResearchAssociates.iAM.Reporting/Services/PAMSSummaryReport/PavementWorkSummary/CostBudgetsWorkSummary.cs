﻿using System;
using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.iAM.Analysis;
using OfficeOpenXml;
using AppliedResearchAssociates.iAM.Reporting.Models.PAMSSummaryReport;
using AppliedResearchAssociates.iAM.ExcelHelpers;
using System.Drawing;
using AppliedResearchAssociates.iAM.DTOs.Enums;
using AppliedResearchAssociates.iAM.Reporting.Services.PAMSSummaryReport.StaticContent;
using WorkTypeMap = AppliedResearchAssociates.iAM.Reporting.Services.BAMSSummaryReport.BridgeWorkSummary.WorkTypeMap;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.Analysis.Engine;
using AppliedResearchAssociates.iAM.DTOs.Abstract;
using static AppliedResearchAssociates.iAM.Reporting.Services.PAMSSummaryReport.PavementWorkSummary.PavementTreatmentHelper;
using AppliedResearchAssociates.iAM.Reporting.Models;
using CurrentCell = AppliedResearchAssociates.iAM.Reporting.Models.PAMSSummaryReport.CurrentCell;

namespace AppliedResearchAssociates.iAM.Reporting.Services.PAMSSummaryReport.PavementWorkSummary
{
    public class CostBudgetsWorkSummary
    {
        private PavementWorkSummaryCommon _pavementWorkSummaryCommon;

        private Dictionary<int, decimal> TotalAsphaltSpent = new();
        private Dictionary<int, decimal> TotalCompositeSpent = new();
        private Dictionary<int, decimal> TotalConcreteSpent = new();
        private Dictionary<int, decimal> TotalCommittedSpent = new();
        private Dictionary<int, decimal> TotalMPMSSpent = new();
        private Dictionary<int, decimal> TotalSAPSpent = new();
        private Dictionary<int, decimal> TotalProjectBuilderSpent = new();
        private bool ShouldBundleFeasibleTreatments;
        private int TotalSpentRow = 0;

        public CostBudgetsWorkSummary()
        {
            _pavementWorkSummaryCommon = new PavementWorkSummaryCommon();
        }

        public ChartRowsModel FillCostBudgetWorkSummarySections(
            ExcelWorksheet worksheet, CurrentCell currentCell, List<int> simulationYears,
            Dictionary<string, Budget> yearlyBudgetAmount,
            Dictionary<int, Dictionary<string, Dictionary<int, (decimal treatmentCost, decimal compositeTreatmentCost, int length)>>> costLengthPerSurfaceIdPerTreatmentPerYear,
            Dictionary<int, Dictionary<string, List<CommittedProjectMetaData>>> yearlyCostCommittedProj,
            Dictionary<int, Dictionary<TreatmentGroup, (decimal treatmentCost, int length)>> costAndLengthPerTreatmentGroupPerYear,
            List<(string TreatmentName, string AssetType, TreatmentCategory Category)> simulationTreatments,
            Dictionary<TreatmentCategory, SortedDictionary<int, (decimal treatmentCost, int length)>> workTypeTotals,
            ICollection<CommittedProject> committedProjects,
            List<BaseCommittedProjectDTO> committedProjectsForWorkOutsideScope,
            bool shouldBundleFeasibleTreatments)
        {
            ShouldBundleFeasibleTreatments = shouldBundleFeasibleTreatments;
            FillCostOfCommittedWorkSection(worksheet, currentCell, simulationYears, yearlyCostCommittedProj);
            FillCostOfMPMSWorkSection(worksheet, currentCell, simulationYears, yearlyCostCommittedProj);
            FillCostOfSAPWorkSection(worksheet, currentCell, simulationYears, yearlyCostCommittedProj);
            FillCostOfProjectBuilderWorkSection(worksheet, currentCell, simulationYears, yearlyCostCommittedProj);
            FillCostOfFullDepthAsphaltWorkSection(worksheet, currentCell, simulationYears, costLengthPerSurfaceIdPerTreatmentPerYear, simulationTreatments);
            FillCostOfCompositeWork(worksheet, currentCell, simulationYears, costLengthPerSurfaceIdPerTreatmentPerYear, simulationTreatments);
            FillCostOfConcreteWork(worksheet, currentCell, simulationYears, costLengthPerSurfaceIdPerTreatmentPerYear, simulationTreatments);
            FillTreatmentGroupTotalsSection(worksheet, currentCell, simulationYears, costAndLengthPerTreatmentGroupPerYear);
            var workTypeTotalsWorkOutsideScope = AddCostOfWorkOutsideScope(committedProjectsForWorkOutsideScope);
            FillWorkTypeTotalsSection(worksheet, currentCell, simulationYears, workTypeTotals, workTypeTotalsWorkOutsideScope, yearlyBudgetAmount, out var totalSpendingRow);
            var committedProjectsList = TrimCommittedProjects(committedProjects, committedProjectsForWorkOutsideScope);
            FillBudgetTotalSection(worksheet, currentCell, simulationYears, committedProjectsList, totalSpendingRow);
            FillBudgetAnalysisSection(worksheet, currentCell, simulationYears, yearlyBudgetAmount, committedProjectsList, totalSpendingRow);

            var chartRowsModel = new ChartRowsModel();
            return chartRowsModel;
        }

        private static List<CommittedProject> TrimCommittedProjects(ICollection<CommittedProject> committedProjects, List<BaseCommittedProjectDTO> committedProjectsForWorkOutsideScope)
        {
            var committedProjectsList = committedProjects.ToList();
            foreach (var project in committedProjectsForWorkOutsideScope)
            {
                var toRemove = committedProjectsList.FirstOrDefault(cp => cp.Year == project.Year && cp.Name == project.Treatment && cp.Cost == project.Cost && cp.ProjectSource == project.ProjectSource);
                if (toRemove != null)
                {
                    committedProjectsList.Remove(toRemove);
                }
            }

            return committedProjectsList;
        }

        public ChartRowsModel FillCostBudgetWorkSummarySectionsbyBudget(
            ExcelWorksheet worksheet, CurrentCell currentCell, List<int> simulationYears,
            Dictionary<string, Budget> yearlyBudgetAmount,
            Dictionary<int, Dictionary<string, Dictionary<int, (decimal treatmentCost, decimal compositeTreatmentCost, int length)>>> costLengthPerSurfaceIdPerTreatmentPerYear,
            Dictionary<int, Dictionary<string, List<CommittedProjectMetaData>>> yearlyCostCommittedProj,
            Dictionary<int, Dictionary<TreatmentGroup, (decimal treatmentCost, int length)>> costAndLengthPerTreatmentGroupPerYear,
            List<(string TreatmentName, string AssetType, TreatmentCategory Category)> simulationTreatments,
            Dictionary<TreatmentCategory, SortedDictionary<int, (decimal treatmentCost, int length)>> workTypeTotals,
            WorkSummaryByBudgetModel workSummaryByBudgetModel,
            SimulationOutput reportOutputData,
            List<BaseCommittedProjectDTO> committedProjectsForWorkOutsideScope,
            ICollection<CommittedProject> committedProjects,
            bool shouldBundleFeasibleTreatments)
        {
            var budgetAnalysisRow = currentCell.Row + 1;
            currentCell.Row += 11;

            ShouldBundleFeasibleTreatments = shouldBundleFeasibleTreatments;
            FillCostOfCommittedWorkSection(worksheet, currentCell, simulationYears, yearlyCostCommittedProj);
            FillCostOfMPMSWorkSection(worksheet, currentCell, simulationYears, yearlyCostCommittedProj);
            FillCostOfSAPWorkSection(worksheet, currentCell, simulationYears, yearlyCostCommittedProj);
            FillCostOfProjectBuilderWorkSection(worksheet, currentCell, simulationYears, yearlyCostCommittedProj);
            FillCostOfFullDepthAsphaltWorkSection(worksheet, currentCell, simulationYears, costLengthPerSurfaceIdPerTreatmentPerYear, simulationTreatments);
            FillCostOfCompositeWork(worksheet, currentCell, simulationYears, costLengthPerSurfaceIdPerTreatmentPerYear, simulationTreatments);
            FillCostOfConcreteWork(worksheet, currentCell, simulationYears, costLengthPerSurfaceIdPerTreatmentPerYear, simulationTreatments);
            FillTreatmentGroupTotalsSection(worksheet, currentCell, simulationYears, costAndLengthPerTreatmentGroupPerYear);
            var workTypeTotalsWorkOutsideScope = AddCostOfWorkOutsideScope(committedProjectsForWorkOutsideScope);
            FillWorkTypeTotalsSectionByBudget(worksheet, currentCell, simulationYears, workTypeTotals, workTypeTotalsWorkOutsideScope, yearlyBudgetAmount, out var totalSpendingRow, workSummaryByBudgetModel);
            FillBudgetTotalSectionByBudget(worksheet, currentCell, simulationYears, totalSpendingRow, workSummaryByBudgetModel, reportOutputData, committedProjects);

            var currentRow = currentCell.Row + 1;
            currentCell.Row = budgetAnalysisRow;
            FillBudgetAnalysisSectionByBudget(worksheet, currentCell, simulationYears, yearlyBudgetAmount, totalSpendingRow, workSummaryByBudgetModel, reportOutputData, committedProjects);
            currentCell.Row = currentRow;

            var chartRowsModel = new ChartRowsModel();
            return chartRowsModel;
        }

        private Dictionary<TreatmentCategory, SortedDictionary<int, decimal>> FillCostOfFullDepthAsphaltWorkSection(
            ExcelWorksheet worksheet,
            CurrentCell currentCell,
            List<int> simulationYears,
            Dictionary<int, Dictionary<string, Dictionary<int, (decimal treatmentCost, decimal compositeTreatmentCost, int length)>>> costLengthPerSurfaceIdPerTreatmentPerYear,
            List<(string TreatmentName, string AssetType, TreatmentCategory Category)> simulationTreatments
            )
        {
            _pavementWorkSummaryCommon.AddHeaders(worksheet, currentCell, simulationYears, "Cost of PAMS Full Depth Asphalt Work", "PAMS Full Depth Asphalt Work");

            var asphaltTreatments = _pavementWorkSummaryCommon.GetNoTreatments(simulationTreatments).Concat(_pavementWorkSummaryCommon.GetAsphaltTreatments(simulationTreatments)).ToList();

            var workTypeTotalFullDepthAsphalt = AddCostsOfFullDepthAsphaltWork(worksheet, simulationYears, currentCell, costLengthPerSurfaceIdPerTreatmentPerYear, asphaltTreatments);

            return workTypeTotalFullDepthAsphalt;
        }

        private Dictionary<TreatmentCategory, SortedDictionary<int, decimal>> AddCostsOfFullDepthAsphaltWork(
            ExcelWorksheet worksheet,
            List<int> simulationYears,
            CurrentCell currentCell,
            Dictionary<int, Dictionary<string, Dictionary<int, (decimal treatmentCost, decimal compositeTreatmentCost, int length)>>> costLengthPerSurfaceIdPerTreatmentPerYear,
            List<(string Name, string AssetType, TreatmentCategory Category)> simulationTreatments
            )
        {
            var workTypeFullDepthAsphalt = new Dictionary<TreatmentCategory, SortedDictionary<int, decimal>>();
            if (simulationYears.Count <= 0)
            {
                return workTypeFullDepthAsphalt;
            }
            int startRow, startColumn, row, column;
            _pavementWorkSummaryCommon.SetRowColumns(currentCell, out startRow, out startColumn, out row, out column);
            int asphaltTotalRow = 0;

            // filling in the treatments in the excel TAB
            _pavementWorkSummaryCommon.SetPavementTreatmentExcelString(worksheet, simulationTreatments, ref row, ref column);
            // Bundled Treatments
            if (ShouldBundleFeasibleTreatments)
            {
                worksheet.Cells[row++, column].Value = PAMSConstants.BundledTreatments;
            }
            worksheet.Cells[row++, column].Value = PAMSConstants.AsphaltTotal;
            column++;
            var fromColumn = column + 1;
            // Filling in the cost per treatment per year in the excel TAB
            foreach (var yearlyValues in costLengthPerSurfaceIdPerTreatmentPerYear)
            {
                row = startRow;
                column = ++column;
                decimal asphaltTotalCost = 0;

                foreach (var treatment in simulationTreatments)
                {
                    decimal cost = 0;
                    GetCosts(yearlyValues, treatment, out var treatmentCost, out var treatmentCompositeCost);
                    var treatmentAsphaltCost = treatmentCost - treatmentCompositeCost;
                    worksheet.Cells[row++, column].Value = treatmentAsphaltCost;
                    asphaltTotalCost += treatmentAsphaltCost;
                    cost = treatmentAsphaltCost;

                    if (!workTypeFullDepthAsphalt.ContainsKey(treatment.Category))
                    {
                        workTypeFullDepthAsphalt.Add(treatment.Category, new SortedDictionary<int, decimal>()
                        {
                            { yearlyValues.Key, cost }
                        });
                    }
                    else
                    {
                        if (!workTypeFullDepthAsphalt[treatment.Category].ContainsKey(yearlyValues.Key))
                        {
                            workTypeFullDepthAsphalt[treatment.Category].Add(yearlyValues.Key, 0);
                        }
                        workTypeFullDepthAsphalt[treatment.Category][yearlyValues.Key] += cost;
                    }
                }

                if (ShouldBundleFeasibleTreatments)
                {
                    decimal bundledCost = 0;
                    foreach (var yearlyValue in yearlyValues.Value)
                    {
                        var treatment = yearlyValue.Key;
                        if (treatment.Contains("Bundle"))
                        {
                            var category = TreatmentCategory.Bundled;
                            decimal cost = 0;
                            GetCostsInYearlyValue(yearlyValue.Value.Where(_ => _.Key < 62).ToList(), out var treatmentCost, out var compositeTreatmentCost);
                            cost = treatmentCost - compositeTreatmentCost;
                            bundledCost += cost;
                            asphaltTotalCost += cost;

                            if (!workTypeFullDepthAsphalt.ContainsKey(category))
                            {
                                workTypeFullDepthAsphalt.Add(category, new SortedDictionary<int, decimal>()
                                {
                                    { yearlyValues.Key, cost }
                                });
                            }
                            else
                            {
                                if (!workTypeFullDepthAsphalt[category].ContainsKey(yearlyValues.Key))
                                {
                                    workTypeFullDepthAsphalt[category].Add(yearlyValues.Key, 0);
                                }
                                workTypeFullDepthAsphalt[category][yearlyValues.Key] += cost;
                            }
                        }

                    }
                    worksheet.Cells[row++, column].Value = bundledCost;
                }
                
                worksheet.Cells[row, column].Value = asphaltTotalCost;
                asphaltTotalRow = row;
                TotalAsphaltSpent.Add(yearlyValues.Key, asphaltTotalCost);
            }
            // Format Data Cells
            ExcelHelper.ApplyBorder(worksheet.Cells[startRow, startColumn, row, column]);
            ExcelHelper.SetCustomFormat(worksheet.Cells[startRow, fromColumn, row, column], ExcelHelperCellFormat.NegativeCurrency);
            ExcelHelper.ApplyColor(worksheet.Cells[startRow, fromColumn, row, column], Color.FromArgb(198, 224, 180));

            // Format Total Cells
            ExcelHelper.ApplyColor(worksheet.Cells[asphaltTotalRow, fromColumn, asphaltTotalRow, column], Color.FromArgb(55, 86, 35));
            ExcelHelper.SetTextColor(worksheet.Cells[asphaltTotalRow, fromColumn, asphaltTotalRow, column], Color.White);
            _pavementWorkSummaryCommon.UpdateCurrentCell(currentCell, ++row, column);
            
            return workTypeFullDepthAsphalt;
        }        

        private Dictionary<TreatmentCategory, SortedDictionary<int, decimal>> FillCostOfCompositeWork(
                ExcelWorksheet worksheet,
                CurrentCell currentCell,
                List<int> simulationYears,
                Dictionary<int, Dictionary<string, Dictionary<int, (decimal treatmentCost, decimal compositeTreatmentCost, int length)>>> costLengthPerSurfaceIdPerTreatmentPerYear,
                List<(string Name, string AssetType, TreatmentCategory Category)> simulationTreatments
                )
        {
            var headerRange = new Range(currentCell.Row, currentCell.Row + 1);
            _pavementWorkSummaryCommon.AddHeaders(worksheet, currentCell, simulationYears, "Cost of PAMS Composite Work", "PAMS Composite Work");

            var asphaltTreatments = _pavementWorkSummaryCommon.GetNoTreatments(simulationTreatments).Concat(_pavementWorkSummaryCommon.GetAsphaltTreatments(simulationTreatments)).ToList();

            var workTypeComposite = AddCostsOfCompositeWork(worksheet, simulationYears, currentCell, costLengthPerSurfaceIdPerTreatmentPerYear, asphaltTreatments);

            return workTypeComposite;
        }

        private Dictionary<TreatmentCategory, SortedDictionary<int, decimal>> AddCostsOfCompositeWork(
            ExcelWorksheet worksheet,
            List<int> simulationYears,
            CurrentCell currentCell,
            Dictionary<int, Dictionary<string, Dictionary<int, (decimal treatmentCost, decimal compositeTreatmentCost, int length)>>> costLengthPerSurfaceIdPerTreatmentPerYear,
            List<(string Name, string AssetType, TreatmentCategory Category)>
            simulationTreatments
            )
        {
            var workTypeComposite = new Dictionary<TreatmentCategory, SortedDictionary<int, decimal>>();
            if (simulationYears.Count <= 0)
            {
                return workTypeComposite;
            }
            int startRow, startColumn, row, column;
            _pavementWorkSummaryCommon.SetRowColumns(currentCell, out startRow, out startColumn, out row, out column);
            int compositeTotalRow = 0;

            // filling in the treatments in the excel TAB
            _pavementWorkSummaryCommon.SetPavementTreatmentExcelString(worksheet, simulationTreatments, ref row, ref column);
            // Bundled Treatments
            if (ShouldBundleFeasibleTreatments)
            {
                worksheet.Cells[row++, column].Value = PAMSConstants.BundledTreatments;
            }
            worksheet.Cells[row++, column].Value = PAMSConstants.CompositeTotal;
            column++;
            var fromColumn = column + 1;
            // Filling in the cost per treatment per year in the excel TAB
            foreach (var yearlyValues in costLengthPerSurfaceIdPerTreatmentPerYear)
            {
                decimal CompositeTotalCost = 0;
                row = startRow;
                column = ++column;

                foreach (var treatment in simulationTreatments)
                {
                    decimal cost = 0;
                    GetCosts(yearlyValues, treatment, out var treatmentCost, out var treatmentCompositeCost);
                    worksheet.Cells[row++, column].Value = treatmentCompositeCost;
                    CompositeTotalCost += treatmentCompositeCost;
                    cost = treatmentCompositeCost;

                    if (!workTypeComposite.ContainsKey(treatment.Category))
                    {
                        workTypeComposite.Add(treatment.Category, new SortedDictionary<int, decimal>()
                        {
                            { yearlyValues.Key, cost }
                        });
                    }
                    else
                    {
                        if (!workTypeComposite[treatment.Category].ContainsKey(yearlyValues.Key))
                        {
                            workTypeComposite[treatment.Category].Add(yearlyValues.Key, 0);
                        }
                        workTypeComposite[treatment.Category][yearlyValues.Key] += cost;
                    }
                }

                if (ShouldBundleFeasibleTreatments)
                {
                    decimal bundledCost = 0;
                    foreach (var yearlyValue in yearlyValues.Value)
                    {
                        var treatment = yearlyValue.Key;
                        if (treatment.Contains("Bundle"))
                        {
                            var category = TreatmentCategory.Bundled;
                            decimal cost = 0;
                            GetCostsInYearlyValue(yearlyValue.Value.Where(_ => _.Key == 62).ToList(), out var treatmentCost, out var compositeTreatmentCost);
                            cost = compositeTreatmentCost;
                            bundledCost += cost;
                            CompositeTotalCost += cost;

                            if (!workTypeComposite.ContainsKey(category))
                            {
                                workTypeComposite.Add(category, new SortedDictionary<int, decimal>()
                            {
                                { yearlyValues.Key, cost }
                            });
                            }
                            else
                            {
                                if (!workTypeComposite[category].ContainsKey(yearlyValues.Key))
                                {
                                    workTypeComposite[category].Add(yearlyValues.Key, 0);
                                }
                                workTypeComposite[category][yearlyValues.Key] += cost;
                            }
                        }
                    }
                    worksheet.Cells[row++, column].Value = bundledCost;
                }
                
                worksheet.Cells[row, column].Value = CompositeTotalCost;
                compositeTotalRow = row;
                TotalCompositeSpent.Add(yearlyValues.Key, CompositeTotalCost);
            }
            ExcelHelper.ApplyBorder(worksheet.Cells[startRow, startColumn, row, column]);
            ExcelHelper.SetCustomFormat(worksheet.Cells[startRow, fromColumn, row, column], ExcelHelperCellFormat.NegativeCurrency);
            ExcelHelper.ApplyColor(worksheet.Cells[startRow, fromColumn, row, column], Color.FromArgb(198, 224, 180));

            ExcelHelper.ApplyColor(worksheet.Cells[compositeTotalRow, fromColumn, compositeTotalRow, column], Color.FromArgb(55, 86, 35));
            ExcelHelper.SetTextColor(worksheet.Cells[compositeTotalRow, fromColumn, compositeTotalRow, column], Color.White);
            _pavementWorkSummaryCommon.UpdateCurrentCell(currentCell, ++row, column);

            return workTypeComposite;
        }

        private Dictionary<TreatmentCategory, SortedDictionary<int, decimal>> FillCostOfConcreteWork(
            ExcelWorksheet worksheet,
            CurrentCell currentCell,
            List<int> simulationYears,
            Dictionary<int, Dictionary<string, Dictionary<int, (decimal treatmentCost, decimal compositeTreatmentCost, int length)>>> costLengthPerSurfaceIdPerTreatmentPerYear,
            List<(string Name, string AssetType, TreatmentCategory Category)> simulationTreatments
            )
        {
            var headerRange = new Range(currentCell.Row, currentCell.Row + 1);
            _pavementWorkSummaryCommon.AddHeaders(worksheet, currentCell, simulationYears, "Cost of PAMS Concrete Work", "PAMS Concrete Work");

            var concreteTreatments = _pavementWorkSummaryCommon.GetNoTreatments(simulationTreatments).Concat(_pavementWorkSummaryCommon.GetConcreteTreatments(simulationTreatments)).ToList();

            var workTypeConcrete = AddCostsOfConcreteWork(worksheet, simulationYears, currentCell, costLengthPerSurfaceIdPerTreatmentPerYear, concreteTreatments);

            return workTypeConcrete;
        }

        private Dictionary<TreatmentCategory, SortedDictionary<int, decimal>> AddCostsOfConcreteWork(
            ExcelWorksheet worksheet,
            List<int> simulationYears,
            CurrentCell currentCell,
            Dictionary<int, Dictionary<string, Dictionary<int, (decimal treatmentCost, decimal compositeTreatmentCost, int length)>>> costLengthPerSurfaceIdPerTreatmentPerYear,
            List<(string Name, string AssetType, TreatmentCategory Category)> simulationTreatments
            )
        {
            var workTypeConcrete = new Dictionary<TreatmentCategory, SortedDictionary<int, decimal>>();
            if (simulationYears.Count <= 0)
            {
                return workTypeConcrete;
            }
            int startRow, startColumn, row, column;
            _pavementWorkSummaryCommon.SetRowColumns(currentCell, out startRow, out startColumn, out row, out column);
            int concreteTotalRow = 0;

            // filling in the treatments in the excel TAB
            _pavementWorkSummaryCommon.SetPavementTreatmentExcelString(worksheet, simulationTreatments, ref row, ref column);
            // Bundled Treatments
            if (ShouldBundleFeasibleTreatments)
            {
                worksheet.Cells[row++, column].Value = PAMSConstants.BundledTreatments;
            }
            worksheet.Cells[row++, column].Value = PAMSConstants.ConcreteTotal;
            column++;
            var fromColumn = column + 1;
            // Filling in the cost per treatment per year in the excel TAB
            foreach (var yearlyValues in costLengthPerSurfaceIdPerTreatmentPerYear)
            {
                decimal ConcreteTotalCost = 0;
                row = startRow;
                column = ++column;

                foreach (var treatment in simulationTreatments)
                {
                    decimal cost = 0;
                    GetCosts(yearlyValues, treatment, out var treatmentCost, out var treatmentCompositeCost);
                    worksheet.Cells[row++, column].Value = treatmentCost;
                    ConcreteTotalCost += treatmentCost;
                    cost = treatmentCost;

                    if (!workTypeConcrete.ContainsKey(treatment.Category))
                    {
                        workTypeConcrete.Add(treatment.Category, new SortedDictionary<int, decimal>()
                        {
                            { yearlyValues.Key, cost }
                        });
                    }
                    else
                    {
                        if (!workTypeConcrete[treatment.Category].ContainsKey(yearlyValues.Key))
                        {
                            workTypeConcrete[treatment.Category].Add(yearlyValues.Key, 0);
                        }
                        workTypeConcrete[treatment.Category][yearlyValues.Key] += cost;
                    }
                }
                
                if (ShouldBundleFeasibleTreatments)
                {
                    decimal bundledCost = 0;
                    foreach (var yearlyValue in yearlyValues.Value)
                    {
                        var treatment = yearlyValue.Key;
                        if (treatment.Contains("Bundle"))
                        {
                            var category = TreatmentCategory.Bundled;
                            decimal cost = 0;
                            GetCostsInYearlyValue(yearlyValue.Value.Where(_ => _.Key > 62).ToList(), out var treatmentCost, out var compositeTreatmentCost);
                            cost = treatmentCost;
                            bundledCost += cost;
                            ConcreteTotalCost += cost;

                            if (!workTypeConcrete.ContainsKey(category))
                            {
                                workTypeConcrete.Add(category, new SortedDictionary<int, decimal>()
                            {
                                { yearlyValues.Key, cost }
                            });
                            }
                            else
                            {
                                if (!workTypeConcrete[category].ContainsKey(yearlyValues.Key))
                                {
                                    workTypeConcrete[category].Add(yearlyValues.Key, 0);
                                }
                                workTypeConcrete[category][yearlyValues.Key] += cost;
                            }
                        }
                    }
                    worksheet.Cells[row++, column].Value = bundledCost;
                }

                worksheet.Cells[row, column].Value = ConcreteTotalCost;
                concreteTotalRow = row;
                TotalConcreteSpent.Add(yearlyValues.Key, ConcreteTotalCost);
            }
            ExcelHelper.ApplyBorder(worksheet.Cells[startRow, startColumn, row, column]);
            ExcelHelper.SetCustomFormat(worksheet.Cells[startRow, fromColumn, row, column], ExcelHelperCellFormat.NegativeCurrency);
            ExcelHelper.ApplyColor(worksheet.Cells[startRow, fromColumn, row, column], Color.FromArgb(198, 224, 180));

            ExcelHelper.ApplyColor(worksheet.Cells[concreteTotalRow, fromColumn, concreteTotalRow, column], Color.FromArgb(55, 86, 35));
            ExcelHelper.SetTextColor(worksheet.Cells[concreteTotalRow, fromColumn, concreteTotalRow, column], Color.White);
            _pavementWorkSummaryCommon.UpdateCurrentCell(currentCell, ++row, column);

            return workTypeConcrete;
        }

        private void AddTreatmentGroupTotalDetails(ExcelWorksheet worksheet, CurrentCell currentCell,
            Dictionary<int, Dictionary<TreatmentGroup, (decimal treatmentCost, int length)>> costAndLengthPerTreatmentGroupPerYear,
            TreatmentGroupCategory treatmentGroupCategory)
        {
            int startRow, startColumn, row, column;
            _pavementWorkSummaryCommon.SetRowColumns(currentCell, out startRow, out startColumn, out row, out column);

            var treatmentGroups = GetListOfTreatmentGroupForCategory(treatmentGroupCategory);
            var prefix = GetTreatmentGroupString(treatmentGroupCategory) + " - ";
            var treatmentGroupTitles = treatmentGroups.Select(tg => prefix + tg.GroupDescription).Distinct().ToList();

            _pavementWorkSummaryCommon.SetPavementTreatmentGroupsExcelString(worksheet, treatmentGroupTitles, ref row, ref column);

            column++;
            var fromColumn = column + 1;
            var descriptions = treatmentGroups.Select(_ => _.GroupDescription).Distinct().ToList();
            foreach (var yearlyValues in costAndLengthPerTreatmentGroupPerYear)
            {
                row = startRow;
                column = ++column;                
                foreach (var description in descriptions)
                {
                    decimal treatmentCost = 0;
                    foreach (var treatmentGroup in treatmentGroups.Where(_ => _.GroupDescription.Equals(description)))
                    {
                        yearlyValues.Value.TryGetValue(treatmentGroup, out var costAndLength);
                        treatmentCost += costAndLength.treatmentCost;
                    }
                    worksheet.Cells[row, column].Value = treatmentCost;
                    row++;
                }
            }
            
            row--;

            ExcelHelper.ApplyBorder(worksheet.Cells[startRow, startColumn, row, column]);
            ExcelHelper.SetCustomFormat(worksheet.Cells[startRow, fromColumn, row, column], ExcelHelperCellFormat.NegativeCurrency);
            ExcelHelper.ApplyColor(worksheet.Cells[startRow, fromColumn, row, column], Color.FromArgb(84, 130, 53));
            ExcelHelper.SetTextColor(worksheet.Cells[startRow, fromColumn, row, column], Color.White);

            _pavementWorkSummaryCommon.UpdateCurrentCell(currentCell, ++row, column);
        }

        private void FillTreatmentGroupTotalsSection(ExcelWorksheet worksheet, CurrentCell currentCell,
            List<int> simulationYears,
            Dictionary<int, Dictionary<TreatmentGroup, (decimal treatmentCost, int length)>> costAndLengthPerTreatmentGroupPerYear
            )
        {
            if (simulationYears.Count <= 0)
            {
                return;// workTypeConcrete;
            }

            _pavementWorkSummaryCommon.AddHeaders(worksheet, currentCell, simulationYears, "Total Budget", "PAMS Treatment Groups Totals");

            AddTreatmentGroupTotalDetails(worksheet, currentCell, costAndLengthPerTreatmentGroupPerYear, TreatmentGroupCategory.Bituminous);
            AddTreatmentGroupTotalDetails(worksheet, currentCell, costAndLengthPerTreatmentGroupPerYear, TreatmentGroupCategory.Concrete);
            if (ShouldBundleFeasibleTreatments)
            {
                AddTreatmentGroupTotalDetails(worksheet, currentCell, costAndLengthPerTreatmentGroupPerYear, TreatmentGroupCategory.Bundled);
            }
        }

        private void FillWorkTypeTotalsSection(ExcelWorksheet worksheet, CurrentCell currentCell,
            List<int> simulationYears,
            Dictionary<TreatmentCategory, SortedDictionary<int, (decimal treatmentCost, int length)>> workTypeTotals,
            Dictionary<TreatmentCategory, SortedDictionary<int, decimal>> workTypeTotalsWorkOutsideScope,
            Dictionary<string, Budget> yearlyBudgetAmount,
            out int totalSpendingRow
            )
        {
            var workTypesForReport = new List<TreatmentCategory> { TreatmentCategory.Maintenance, TreatmentCategory.Preservation, TreatmentCategory.Rehabilitation, TreatmentCategory.Reconstruction, TreatmentCategory.WorkOutsideScope, TreatmentCategory.Bundled };
            var headerRange = new Range(currentCell.Row, currentCell.Row + 1);
            _pavementWorkSummaryCommon.AddHeaders(worksheet, currentCell, simulationYears, "Total Budget", "Work Type Totals", "Total (all years)");

            int startRow, startColumn, row, column;
            _pavementWorkSummaryCommon.SetRowColumns(currentCell, out startRow, out startColumn, out row, out column);

            var workTypeTitles = workTypesForReport.Select(tc => tc.ToSpreadsheetString())
                .ToList();
            workTypeTitles.Add("Total Spent");

            _pavementWorkSummaryCommon.SetPavementTreatmentGroupsExcelString(worksheet, workTypeTitles, ref row, ref column);

            column++;
            var fromColumn = column + 1;
            row = startRow;
            var columnTotals = new Dictionary<int, decimal>();

            foreach (var workType in workTypesForReport)
            {
                decimal rowTotal = 0;
                column = fromColumn;

                // WorkOutsideScope
                if (workType == TreatmentCategory.WorkOutsideScope)
                {
                    foreach (var year in simulationYears)
                    {
                        var workTypeTotalWorkOutsideScope = workTypeTotalsWorkOutsideScope.Where(_ => _.Key == TreatmentCategory.WorkOutsideScope).Select(_ => _.Value).FirstOrDefault(_ => _.ContainsKey(year));
                        if (workTypeTotalWorkOutsideScope != null)
                        {
                            var cost = workTypeTotalWorkOutsideScope[year];
                            worksheet.Cells[row, column].Value = cost;
                            rowTotal += cost;
                            columnTotals[year] += cost;
                        }
                        else
                        {
                            worksheet.Cells[row, column].Value = 0.0;
                        }
                        column++;
                    }
                }
                else
                {
                    var workTypeTotalExists = workTypeTotals.TryGetValue(workType, out var workTypeTotal);
                    foreach (var year in simulationYears)
                    {
                        if (!columnTotals.ContainsKey(year))
                        {
                            columnTotals.Add(year, 0);
                        }
                        if (workTypeTotalExists && (workTypeTotal.TryGetValue(year, out var costAndLength)))
                        {
                            worksheet.Cells[row, column].Value = costAndLength.treatmentCost;
                            rowTotal += costAndLength.treatmentCost;
                            columnTotals[year] += costAndLength.treatmentCost;
                        }
                        else
                        {
                            worksheet.Cells[row, column].Value = 0.0;
                        }
                        column++;
                    }
                }
                worksheet.Cells[row, column].Value = rowTotal;
                row++;
            }            

            // Add Total Spent Row
            column = fromColumn;
            decimal totalSpentTotal = 0;
            foreach (var year in simulationYears)
            {
                totalSpentTotal += columnTotals[year] + TotalCommittedSpent[year] + TotalMPMSSpent[year] + TotalSAPSpent[year] + TotalProjectBuilderSpent[year];
                worksheet.Cells[row, column].Value = columnTotals[year] + TotalCommittedSpent[year] + TotalMPMSSpent[year] + TotalSAPSpent[year] + TotalProjectBuilderSpent[year];
                column++;
            }
            totalSpendingRow = row;
            worksheet.Cells[row, column].Value = totalSpentTotal;

            column = fromColumn + simulationYears.Count;

            ExcelHelper.ApplyBorder(worksheet.Cells[startRow, startColumn, row, column]);
            ExcelHelper.SetCustomFormat(worksheet.Cells[startRow, fromColumn, row, column], ExcelHelperCellFormat.NegativeCurrency);

            ExcelHelper.ApplyColor(worksheet.Cells[startRow, fromColumn, row, column - 1], Color.FromArgb(84, 130, 53));
            ExcelHelper.SetTextColor(worksheet.Cells[startRow, fromColumn, row, column - 1], Color.White);

            ExcelHelper.ApplyColor(worksheet.Cells[startRow, column, row, column], Color.FromArgb(217, 217, 217));

            worksheet.Cells[startRow, column + 1].Formula = ExcelFormulas.Percentage(startRow, column, totalSpendingRow, column);
            worksheet.Cells[startRow, column + 1].Style.Numberformat.Format = "#0.00%";
            worksheet.Cells[startRow, column + 2].Value = "Percentage spent on MAINTENANCE";

            worksheet.Cells[startRow + 1, column + 1].Formula = ExcelFormulas.Percentage(startRow + 1, column, totalSpendingRow, column);
            worksheet.Cells[startRow + 1, column + 1].Style.Numberformat.Format = "#0.00%";
            worksheet.Cells[startRow + 1, column + 2].Value = "Percentage spent on PRESERVATION";

            worksheet.Cells[startRow + 2, column + 1].Formula = ExcelFormulas.Percentage(startRow + 2, column, totalSpendingRow, column);
            worksheet.Cells[startRow + 2, column + 1].Style.Numberformat.Format = "#0.00%";
            worksheet.Cells[startRow + 2, column + 2].Value = "Percentage spent on REHABILITATION";

            worksheet.Cells[startRow + 3, column + 1].Formula = ExcelFormulas.Percentage(startRow + 3, column, totalSpendingRow, column);
            worksheet.Cells[startRow + 3, column + 1].Style.Numberformat.Format = "#0.00%";
            worksheet.Cells[startRow + 3, column + 2].Value = "Percentage spent on RECONSTRUCTION";

            worksheet.Cells[startRow + 4, column + 1].Formula = ExcelFormulas.Percentage(startRow + 4, column, totalSpendingRow, column);
            worksheet.Cells[startRow + 4, column + 1].Style.Numberformat.Format = "#0.00%";
            worksheet.Cells[startRow + 4, column + 2].Value = "Percentage spent on WORK OUTSIDE SCOPE/JURISDICTION";

            // TODO : should we hide this based on setting?
            worksheet.Cells[startRow + 5, column + 1].Formula = ExcelFormulas.Percentage(startRow + 5, column, totalSpendingRow, column);
            worksheet.Cells[startRow + 5, column + 1].Style.Numberformat.Format = "#0.00%";
            worksheet.Cells[startRow + 5, column + 2].Value = "Percentage spent on BUNDLED";

            row += 2;
            worksheet.Cells[row, 1].Value = PAMSConstants.TotalWorkBudget;
            column = fromColumn;

            decimal annualBudget = 0;
            foreach (var year in simulationYears)
            {
                var yearIndex = year - simulationYears[0];
                var budgetTotal = yearlyBudgetAmount.Sum(x => x.Value.YearlyAmounts[yearIndex].Value);
                worksheet.Cells[row, column].Value = budgetTotal;
                column++;
                annualBudget += budgetTotal;
            }
            worksheet.Cells[row, column].Value = annualBudget; // Total cell

            ExcelHelper.ApplyBorder(worksheet.Cells[row, startColumn, row, column]);
            ExcelHelper.SetCustomFormat(worksheet.Cells[row, fromColumn, row, column], ExcelHelperCellFormat.NegativeCurrency);
            ExcelHelper.ApplyColor(worksheet.Cells[row, fromColumn, row, column], Color.FromArgb(0, 128, 0));
            ExcelHelper.SetTextColor(worksheet.Cells[row, fromColumn, row, column], Color.White);

            _pavementWorkSummaryCommon.UpdateCurrentCell(currentCell, row + 2, column);
        }

        private void FillWorkTypeTotalsSectionByBudget(ExcelWorksheet worksheet, CurrentCell currentCell,
            List<int> simulationYears,
            Dictionary<TreatmentCategory, SortedDictionary<int, (decimal treatmentCost, int length)>> workTypeTotals,
            Dictionary<TreatmentCategory, SortedDictionary<int, decimal>> workTypeTotalsWorkOutsideScope, Dictionary<string, Budget> yearlyBudgetAmount,
            out int totalSpendingRow,
            WorkSummaryByBudgetModel workSummaryByBudgetModel)
        {
            var workTypesForReport = new List<TreatmentCategory> { TreatmentCategory.Maintenance, TreatmentCategory.Preservation, TreatmentCategory.Rehabilitation, TreatmentCategory.Reconstruction, TreatmentCategory.WorkOutsideScope, TreatmentCategory.Bundled };
            var headerRange = new Range(currentCell.Row, currentCell.Row + 1);
            _pavementWorkSummaryCommon.AddHeaders(worksheet, currentCell, simulationYears, "Total Budget", "Work Type Totals", "Total (all years)");

            int startRow, startColumn, row, column;
            _pavementWorkSummaryCommon.SetRowColumns(currentCell, out startRow, out startColumn, out row, out column);

            var workTypeTitles = workTypesForReport.Select(tc => tc.ToSpreadsheetString())
                .ToList();
            workTypeTitles.Add("Total Spent");

            _pavementWorkSummaryCommon.SetPavementTreatmentGroupsExcelString(worksheet, workTypeTitles, ref row, ref column);

            column++;
            var fromColumn = column + 1;
            row = startRow;
            var columnTotals = new Dictionary<int, decimal>();

            foreach (var workType in workTypesForReport)
            {
                decimal rowTotal = 0;
                column = fromColumn;
                // WorkOutsideScope
                if (workType == TreatmentCategory.WorkOutsideScope)
                {
                    foreach (var year in simulationYears)
                    {
                        var workTypeTotalWorkOutsideScope = workTypeTotalsWorkOutsideScope.Where(_ => _.Key == TreatmentCategory.WorkOutsideScope).Select(_ => _.Value).FirstOrDefault(_ => _.ContainsKey(year));
                        if (workTypeTotalWorkOutsideScope != null)
                        {
                            var cost = workTypeTotalWorkOutsideScope[year];
                            worksheet.Cells[row, column].Value = cost;
                            rowTotal += cost;
                            columnTotals[year] += cost;
                        }
                        else
                        {
                            worksheet.Cells[row, column].Value = 0.0;
                        }
                        column++;
                    }
                }
                else
                {
                    var workTypeTotalExists = workTypeTotals.TryGetValue(workType, out var workTypeTotal);
                    foreach (var year in simulationYears)
                    {
                        if (!columnTotals.ContainsKey(year))
                        {
                            columnTotals.Add(year, 0);
                        }
                        if (workTypeTotalExists && (workTypeTotal.TryGetValue(year, out var costAndLength)))
                        {
                            worksheet.Cells[row, column].Value = costAndLength.treatmentCost;
                            rowTotal += costAndLength.treatmentCost;
                            columnTotals[year] += costAndLength.treatmentCost;
                        }
                        else
                        {
                            worksheet.Cells[row, column].Value = 0.0;
                        }
                        column++;
                    }                    
                }
                worksheet.Cells[row, column].Value = rowTotal;
                row++;
            }

            // Add Total Spent Row
            column = fromColumn;
            decimal totalSpentTotal = 0;
            foreach (var year in simulationYears)
            {
                totalSpentTotal += columnTotals[year] + TotalCommittedSpent[year] + TotalMPMSSpent[year] + TotalSAPSpent[year] + TotalProjectBuilderSpent[year];
                worksheet.Cells[row, column].Value = columnTotals[year] + TotalCommittedSpent[year] + TotalMPMSSpent[year] + TotalSAPSpent[year] + TotalProjectBuilderSpent[year];
                column++;
            }
            totalSpendingRow = row;
            worksheet.Cells[row, column].Value = totalSpentTotal;

            column = fromColumn + simulationYears.Count;

            ExcelHelper.ApplyBorder(worksheet.Cells[startRow, startColumn, row, column]);
            ExcelHelper.SetCustomFormat(worksheet.Cells[startRow, fromColumn, row, column], ExcelHelperCellFormat.NegativeCurrency);

            ExcelHelper.ApplyColor(worksheet.Cells[startRow, fromColumn, row, column - 1], Color.FromArgb(84, 130, 53));
            ExcelHelper.SetTextColor(worksheet.Cells[startRow, fromColumn, row, column - 1], Color.White);

            ExcelHelper.ApplyColor(worksheet.Cells[startRow, column, row, column], Color.FromArgb(217, 217, 217));
           
            worksheet.Cells[startRow, column + 1].Formula = ExcelFormulas.Percentage(startRow, column, totalSpendingRow, column);
            worksheet.Cells[startRow, column + 1].Style.Numberformat.Format = "#0.00%";
            worksheet.Cells[startRow, column + 2].Value = "Percentage spent on MAINTENANCE";

            worksheet.Cells[startRow + 1, column + 1].Formula = ExcelFormulas.Percentage(startRow + 1, column, totalSpendingRow, column);
            worksheet.Cells[startRow + 1, column + 1].Style.Numberformat.Format = "#0.00%";
            worksheet.Cells[startRow + 1, column + 2].Value = "Percentage spent on PRESERVATION";

            worksheet.Cells[startRow + 2, column + 1].Formula = ExcelFormulas.Percentage(startRow + 2, column, totalSpendingRow, column);
            worksheet.Cells[startRow + 2, column + 1].Style.Numberformat.Format = "#0.00%";
            worksheet.Cells[startRow + 2, column + 2].Value = "Percentage spent on REHABILITATION";

            worksheet.Cells[startRow + 3, column + 1].Formula = ExcelFormulas.Percentage(startRow + 3, column, totalSpendingRow, column);
            worksheet.Cells[startRow + 3, column + 1].Style.Numberformat.Format = "#0.00%";
            worksheet.Cells[startRow + 3, column + 2].Value = "Percentage spent on RECONSTRUCTION";

            worksheet.Cells[startRow + 4, column + 1].Formula = ExcelFormulas.Percentage(startRow + 4, column, totalSpendingRow, column);
            worksheet.Cells[startRow + 4, column + 1].Style.Numberformat.Format = "#0.00%";
            worksheet.Cells[startRow + 4, column + 2].Value = "Percentage spent on WORK OUTSIDE SCOPE/JURISDICTION";

            // TODO : should we hide this based on setting?
            worksheet.Cells[startRow + 5, column + 1].Formula = ExcelFormulas.Percentage(startRow + 5, column, totalSpendingRow, column);
            worksheet.Cells[startRow + 5, column + 1].Style.Numberformat.Format = "#0.00%";
            worksheet.Cells[startRow + 5, column + 2].Value = "Percentage spent on BUNDLED";

            row += 2;
            worksheet.Cells[row, 1].Value = PAMSConstants.TotalWorkBudget;
            column = fromColumn;

            decimal annualBudget = 0;
            foreach (var year in simulationYears)
            {
                var yearIndex = year - simulationYears.First(); 
                // Calculate the budget only for the specific budget name provided in workSummaryByBudgetModel
                if (yearlyBudgetAmount.TryGetValue(workSummaryByBudgetModel.BudgetName, out var budget) && budget.YearlyAmounts.Count > yearIndex)
                {
                    var budgetForYear = budget.YearlyAmounts[yearIndex].Value;
                    worksheet.Cells[row, column].Value = budgetForYear;
                    annualBudget += budgetForYear;
                }
                else
                {
                    worksheet.Cells[row, column].Value = 0;
                }
                column++;
            }
            worksheet.Cells[row, column].Value = annualBudget; // Total for the specific budget across all years

            ExcelHelper.ApplyBorder(worksheet.Cells[row, startColumn, row, column]);
            ExcelHelper.SetCustomFormat(worksheet.Cells[row, fromColumn, row, column], ExcelHelperCellFormat.NegativeCurrency);
            ExcelHelper.ApplyColor(worksheet.Cells[row, fromColumn, row, column], Color.FromArgb(0, 128, 0));
            ExcelHelper.SetTextColor(worksheet.Cells[row, fromColumn, row, column], Color.White);

            _pavementWorkSummaryCommon.UpdateCurrentCell(currentCell, row + 2, column);
        }
                
        private void FillBudgetTotalSection(ExcelWorksheet worksheet, CurrentCell currentCell, List<int> simulationYears, ICollection<CommittedProject> committedProjects, int totalSpendingRow)
        {
            var headerRange = new Range(currentCell.Row, currentCell.Row + 1);
            _pavementWorkSummaryCommon.AddHeaders(worksheet, currentCell, simulationYears, "", "Budget Total");

            int startRow, startColumn, pamsRow, row, column;
            _pavementWorkSummaryCommon.SetRowColumns(currentCell, out startRow, out startColumn, out row, out column);

            var rowTitles = new List<string> { "PAMS", "Committed", "MPMS", "SAP", "Project Builder" };
            _pavementWorkSummaryCommon.SetPavementTreatmentGroupsExcelString(worksheet, rowTitles, ref row, ref column);

            column++;
            var fromColumn = column + 1;

            pamsRow = startRow;
            row = pamsRow + 1;
            column = fromColumn;

            foreach (var year in simulationYears)
            {
                // Committed
                var committedBudgetTotal = committedProjects.Where(_ => _.Year == year && _.ProjectSource == ProjectSourceDTO.Committed).Select(_ => _.Cost).Sum();
                worksheet.Cells[row, column].Value = committedBudgetTotal;

                // MPMS based on committed projects
                var mpmsBudgetTotal = committedProjects.Where(_ => _.Year == year && _.ProjectSource == ProjectSourceDTO.MPMS).Select(_ => _.Cost).Sum();
                worksheet.Cells[row + 1, column].Value = mpmsBudgetTotal;

                // SAP
                var sapBudgetTotal = committedProjects.Where(_ => _.Year == year && _.ProjectSource == ProjectSourceDTO.SAP).Select(_ => _.Cost).Sum();
                worksheet.Cells[row + 2, column].Value = sapBudgetTotal;

                // Project Builder
                var projectBuilderBudgetTotal = committedProjects.Where(_ => _.Year == year && _.ProjectSource == ProjectSourceDTO.ProjectBuilder).Select(_ => _.Cost).Sum();
                worksheet.Cells[row + 3, column].Value = projectBuilderBudgetTotal;

                // PAMS based on treatments
                worksheet.Cells[pamsRow, column].Value = (decimal)worksheet.Cells[totalSpendingRow, column].Value - (decimal)committedBudgetTotal - (decimal)mpmsBudgetTotal - (decimal)sapBudgetTotal - (decimal)projectBuilderBudgetTotal;

                column++;
            }

            row += 4;

            ExcelHelper.ApplyBorder(worksheet.Cells[startRow, startColumn, row - 1, column - 1]);
            ExcelHelper.SetCustomFormat(worksheet.Cells[startRow, fromColumn, row - 1, column - 1], ExcelHelperCellFormat.NegativeCurrency);
            ExcelHelper.ApplyColor(worksheet.Cells[startRow, fromColumn, row - 1, column - 1], Color.FromArgb(84, 130, 53));
            ExcelHelper.SetTextColor(worksheet.Cells[startRow, fromColumn, row, column - 1], Color.White);

            _pavementWorkSummaryCommon.UpdateCurrentCell(currentCell, row, column);
        }

        private void FillBudgetTotalSectionByBudget(ExcelWorksheet worksheet,
            CurrentCell currentCell, List<int> simulationYears,
            int totalSpendingRow,
            WorkSummaryByBudgetModel workSummaryByBudgetModel,
            SimulationOutput reportOutputData,
            ICollection<CommittedProject> committedProjects)
        {
            var headerRange = new Range(currentCell.Row, currentCell.Row + 1);
            _pavementWorkSummaryCommon.AddHeaders(worksheet, currentCell, simulationYears, "", "Budget Total");

            int startRow, startColumn, pamsRow, row, column;
            _pavementWorkSummaryCommon.SetRowColumns(currentCell, out startRow, out startColumn, out row, out column);

            var rowTitles = new List<string> { "PAMS", "Committed", "MPMS", "SAP", "Project Builder" };
            _pavementWorkSummaryCommon.SetPavementTreatmentGroupsExcelString(worksheet, rowTitles, ref row, ref column);

            column++;
            var fromColumn = column + 1;

            pamsRow = startRow;
            row = pamsRow + 1;
            column = fromColumn;

            foreach (var year in simulationYears)
            {
                // Initialize budget totals for year
                decimal committedBudgetTotal = 0;
                decimal mpmsBudgetTotal = 0;
                decimal sapBudgetTotal = 0;
                decimal projectBuilderBudgetTotal = 0;

                foreach (var yearData in reportOutputData.Years.Where(y => y.Year == year))
                {
                    foreach (var section in yearData.Assets)
                    {
                        if (section.TreatmentCause == TreatmentCause.CommittedProject &&
                        section.AppliedTreatment.ToLower() != PAMSConstants.NoTreatment)
                        {
                            foreach (var consideration in section.TreatmentConsiderations)
                            {
                                foreach (var budgetUsage in consideration.FundingCalculationOutput?.AllocationMatrix.Where(bu => bu.BudgetName.Equals(workSummaryByBudgetModel.BudgetName, StringComparison.OrdinalIgnoreCase) && bu.Year == year))
                                {                                    
                                    var projectSource = committedProjects.FirstOrDefault(_ => _.Year == year &&
                                                                                         _.Name == budgetUsage.TreatmentName &&
                                                                                         _.Budget.Name == budgetUsage.BudgetName)?.ProjectSource;
                                    switch (projectSource)
                                    {
                                        case ProjectSourceDTO.Committed:
                                            committedBudgetTotal += budgetUsage.AllocatedAmount;
                                            break;
                                        case ProjectSourceDTO.MPMS:
                                            mpmsBudgetTotal += budgetUsage.AllocatedAmount;
                                            break;
                                        case ProjectSourceDTO.SAP:
                                                sapBudgetTotal += budgetUsage.AllocatedAmount;
                                                break;
                                        case ProjectSourceDTO.ProjectBuilder:
                                            projectBuilderBudgetTotal += budgetUsage.AllocatedAmount;
                                            break;
                                        default:
                                            break;
                                    }
                                }
                            }
                        }
                    }
                }

                worksheet.Cells[row, column].Value = committedBudgetTotal;
                worksheet.Cells[row + 1, column].Value = mpmsBudgetTotal;
                worksheet.Cells[row + 2, column].Value = sapBudgetTotal;
                worksheet.Cells[row + 3, column].Value = projectBuilderBudgetTotal;

                var pamsBudgetTotal = (decimal)worksheet.Cells[totalSpendingRow, column].Value - committedBudgetTotal - mpmsBudgetTotal - sapBudgetTotal - projectBuilderBudgetTotal;
                worksheet.Cells[pamsRow, column].Value = pamsBudgetTotal;

                column++;
            }

            row += 4;

            ExcelHelper.ApplyBorder(worksheet.Cells[startRow, startColumn, row - 1, column - 1]);
            ExcelHelper.SetCustomFormat(worksheet.Cells[startRow, fromColumn, row - 1, column - 1], ExcelHelperCellFormat.NegativeCurrency);
            ExcelHelper.ApplyColor(worksheet.Cells[startRow, fromColumn, row - 1, column - 1], Color.FromArgb(84, 130, 53));
            ExcelHelper.SetTextColor(worksheet.Cells[startRow, fromColumn, row, column - 1], Color.White);

            _pavementWorkSummaryCommon.UpdateCurrentCell(currentCell, row, column);
        }

        private Dictionary<TreatmentCategory, SortedDictionary<int, decimal>> AddCostsOfCommittedWork(ExcelWorksheet worksheet,
            List<int> simulationYears,
            CurrentCell currentCell,
            Dictionary<int, Dictionary<string, List<CommittedProjectMetaData>>> yearlyCostCommittedProj)
        {
            var workTypeTotalCommitted = new Dictionary<TreatmentCategory, SortedDictionary<int, decimal>>();
            if (simulationYears.Count <= 0)
            {
                return workTypeTotalCommitted;
            }
            var startYear = simulationYears[0];
            _pavementWorkSummaryCommon.SetRowColumns(currentCell, out var startRow, out var startColumn, out var row, out var column);
            currentCell.Column = column;
            var committedTotalRow = 0;
            var uniqueTreatments = new Dictionary<string, int>();            
            var map = WorkTypeMap.Map;

            // filling in the committed treatments in the excel TAB
            foreach (var yearlyItem in yearlyCostCommittedProj)
            {
                decimal committedTotalCost = 0;
                row = currentCell.Row;
                foreach (var data in yearlyItem.Value)
                { 
                    foreach (var committedProjectMetaData in data.Value)
                    {
                        if (committedProjectMetaData.ProjectSource == "Committed")
                        {
                            var key = data.Key.Contains("Bundle") ? data.Key : committedProjectMetaData.TreatmentCategory;
                            if (!uniqueTreatments.ContainsKey(key))
                            {
                                uniqueTreatments.Add(key, currentCell.Row);
                                worksheet.Cells[row++, column].Value = key;
                                var cellToEnterCost = yearlyItem.Key - startYear;
                                worksheet.Cells[uniqueTreatments[key], column + cellToEnterCost + 2].Value = committedProjectMetaData.TreatmentCost;
                                currentCell.Row += 1;
                            }
                            else
                            {
                                var cellToEnterCost = yearlyItem.Key - startYear;
                                var currentValue = worksheet.Cells[uniqueTreatments[key], column + cellToEnterCost + 2].Value;
                                decimal toAdd = currentValue == null ? 0 : Convert.ToDecimal(currentValue);
                                worksheet.Cells[uniqueTreatments[key], column + cellToEnterCost + 2].Value = committedProjectMetaData.TreatmentCost + toAdd;
                            }
                            committedTotalCost += committedProjectMetaData.TreatmentCost;

                            // setting up data for Work type totals
                            if (map.ContainsKey(committedProjectMetaData.TreatmentCategory))
                            {
                                var category = map[committedProjectMetaData.TreatmentCategory];
                                var treatmentCost = committedProjectMetaData.TreatmentCost;
                                var currYear = yearlyItem.Key;
                                FillWorkTypeTotalCommitted(workTypeTotalCommitted, category, simulationYears, currYear, treatmentCost);
                            }
                            else
                            {
                                var treatmentCost = committedProjectMetaData.TreatmentCost;
                                var currYear = yearlyItem.Key;
                                var category = TreatmentCategory.Other;
                                FillWorkTypeTotalCommitted(workTypeTotalCommitted, category, simulationYears, currYear, treatmentCost);

                            }
                        }
                    }
                }
                TotalCommittedSpent.Add(yearlyItem.Key, committedTotalCost);
            }

            column = currentCell.Column;
            worksheet.Cells[currentCell.Row, column].Value = PAMSConstants.CommittedTotal;
            column++;
            int firstTotalYear = TotalCommittedSpent.Count > 0 ? TotalCommittedSpent.Keys.Min() : startYear;
            var offsetForTotal = firstTotalYear - startYear;
            var fromColumn = column + offsetForTotal + 1;

            foreach (var cost in TotalCommittedSpent)
            {
                if (cost.Value == 0)
                {
                    worksheet.Cells[currentCell.Row, fromColumn++].Value = "-";
                }
                else
                {
                    worksheet.Cells[currentCell.Row, fromColumn++].Value = cost.Value;
                }
            }
            committedTotalRow = currentCell.Row;
            fromColumn = column + 1;
            var endColumn = simulationYears.Count + 2;

            ExcelHelper.ApplyBorder(worksheet.Cells[startRow, startColumn, committedTotalRow, endColumn]);
            ExcelHelper.SetCustomFormat(worksheet.Cells[startRow, fromColumn, committedTotalRow, endColumn], ExcelHelperCellFormat.NegativeCurrency);
            ExcelHelper.ApplyColor(worksheet.Cells[startRow, fromColumn, committedTotalRow, endColumn], Color.FromArgb(198, 224, 180));

            ExcelHelper.ApplyColor(worksheet.Cells[committedTotalRow, fromColumn, committedTotalRow, endColumn], Color.FromArgb(84, 130, 53));
            ExcelHelper.SetTextColor(worksheet.Cells[committedTotalRow, fromColumn, committedTotalRow, endColumn], Color.White);
            _pavementWorkSummaryCommon.UpdateCurrentCell(currentCell, committedTotalRow + 1, endColumn);

            return workTypeTotalCommitted;
        }

        private Dictionary<TreatmentCategory, SortedDictionary<int, decimal>> AddCostsOfMPMSWork(ExcelWorksheet worksheet,
            List<int> simulationYears,
            CurrentCell currentCell,
            Dictionary<int, Dictionary<string, List<CommittedProjectMetaData>>> yearlyCostCommittedProj)
        {
            var workTypeTotalMPMS = new Dictionary<TreatmentCategory, SortedDictionary<int, decimal>>();
            if (simulationYears.Count <= 0)
            {
                return workTypeTotalMPMS;
            }
            var startYear = simulationYears[0];
            _pavementWorkSummaryCommon.SetRowColumns(currentCell, out var startRow, out var startColumn, out var row, out var column);
            currentCell.Column = column;
            var committedTotalRow = 0;
            var uniqueTreatments = new Dictionary<string, int>();
            var map = WorkTypeMap.Map;

            // filling in the committed treatments in the excel TAB
            foreach (var yearlyItem in yearlyCostCommittedProj)
            {
                decimal committedTotalCost = 0;
                row = currentCell.Row;                
                foreach (var data in yearlyItem.Value)
                {
                    foreach (var committedProjectMetaData in data.Value)
                    {
                        if (committedProjectMetaData.ProjectSource == "MPMS")
                        {
                            var key = data.Key.Contains("Bundle") ? data.Key : committedProjectMetaData.TreatmentCategory;
                            if (!uniqueTreatments.ContainsKey(key))
                            {
                                uniqueTreatments.Add(key, currentCell.Row);
                                worksheet.Cells[row++, column].Value = key;
                                var cellToEnterCost = yearlyItem.Key - startYear;
                                worksheet.Cells[uniqueTreatments[key], column + cellToEnterCost + 2].Value = committedProjectMetaData.TreatmentCost;
                                currentCell.Row += 1;
                            }
                            else
                            {
                                var cellToEnterCost = yearlyItem.Key - startYear;
                                var currentValue = worksheet.Cells[uniqueTreatments[key], column + cellToEnterCost + 2].Value;
                                decimal toAdd = currentValue == null ? 0 : Convert.ToDecimal(currentValue);
                                worksheet.Cells[uniqueTreatments[key], column + cellToEnterCost + 2].Value = committedProjectMetaData.TreatmentCost + toAdd;
                            }
                            committedTotalCost += committedProjectMetaData.TreatmentCost;

                            // setting up data for Work type totals
                            if (map.ContainsKey(committedProjectMetaData.TreatmentCategory))
                            {
                                var category = map[committedProjectMetaData.TreatmentCategory];
                                var treatmentCost = committedProjectMetaData.TreatmentCost;
                                var currYear = yearlyItem.Key;
                                FillWorkTypeTotalMPMS(workTypeTotalMPMS, category, simulationYears, currYear, treatmentCost);
                            }
                            else
                            {
                                var treatmentCost = committedProjectMetaData.TreatmentCost;
                                var currYear = yearlyItem.Key;
                                var category = TreatmentCategory.Other;
                                FillWorkTypeTotalMPMS(workTypeTotalMPMS, category, simulationYears, currYear, treatmentCost);

                            }
                        }
                    }
                }
                TotalMPMSSpent.Add(yearlyItem.Key, committedTotalCost);
            }

            column = currentCell.Column;
            worksheet.Cells[currentCell.Row, column].Value = PAMSConstants.CommittedTotal;
            column++;
            int firstTotalYear = TotalMPMSSpent.Count > 0 ? TotalMPMSSpent.Keys.Min() : startYear;
            var offsetForTotal = firstTotalYear - startYear;
            var fromColumn = column + offsetForTotal + 1;


            foreach (var cost in TotalMPMSSpent)
            {
                if (cost.Value == 0)
                {
                    worksheet.Cells[currentCell.Row, fromColumn++].Value = "-";
                }
                else
                {
                    worksheet.Cells[currentCell.Row, fromColumn++].Value = cost.Value;
                }
            }
            committedTotalRow = currentCell.Row;
            fromColumn = column + 1;
            var endColumn = simulationYears.Count + 2;

            ExcelHelper.ApplyBorder(worksheet.Cells[startRow, startColumn, committedTotalRow, endColumn]);
            ExcelHelper.SetCustomFormat(worksheet.Cells[startRow, fromColumn, committedTotalRow, endColumn], ExcelHelperCellFormat.NegativeCurrency);
            ExcelHelper.ApplyColor(worksheet.Cells[startRow, fromColumn, committedTotalRow, endColumn], Color.FromArgb(198, 224, 180));

            ExcelHelper.ApplyColor(worksheet.Cells[committedTotalRow, fromColumn, committedTotalRow, endColumn], Color.FromArgb(84, 130, 53));
            ExcelHelper.SetTextColor(worksheet.Cells[committedTotalRow, fromColumn, committedTotalRow, endColumn], Color.White);
            _pavementWorkSummaryCommon.UpdateCurrentCell(currentCell, committedTotalRow + 1, endColumn);

            return workTypeTotalMPMS;
        }

        private Dictionary<TreatmentCategory, SortedDictionary<int, decimal>> AddCostsOfSAPWork(
            ExcelWorksheet worksheet,
            List<int> simulationYears,
            CurrentCell currentCell,
            Dictionary<int, Dictionary<string, List<CommittedProjectMetaData>>> yearlyCostSAPProj)
        {
            var workTypeTotalSAP = new Dictionary<TreatmentCategory, SortedDictionary<int, decimal>>();
            if (simulationYears.Count <= 0)
            {
                return workTypeTotalSAP;
            }

            var startYear = simulationYears[0];
            _pavementWorkSummaryCommon.SetRowColumns(currentCell, out var startRow, out var startColumn, out var row, out var column);
            currentCell.Column = column;
            var sapTotalRow = 0;

            var uniqueTreatments = new Dictionary<string, int>();
            var map = WorkTypeMap.Map;

            foreach (var yearlyItem in yearlyCostSAPProj)
            {
                decimal sapTotalCost = 0;
                row = currentCell.Row;

                foreach (var data in yearlyItem.Value)
                {
                    foreach (var committedProjectMetaData in data.Value)
                    {
                        if (committedProjectMetaData.ProjectSource == "SAP")
                        {
                            var key = data.Key.Contains("Bundle") ? data.Key : committedProjectMetaData.TreatmentCategory;
                            if (!uniqueTreatments.ContainsKey(key))
                            {
                                uniqueTreatments.Add(key, currentCell.Row);
                                worksheet.Cells[row++, column].Value = key;
                                var cellToEnterCost = yearlyItem.Key - startYear;
                                worksheet.Cells[uniqueTreatments[key], column + cellToEnterCost + 2].Value = committedProjectMetaData.TreatmentCost;
                                currentCell.Row += 1;
                            }
                            else
                            {
                                var cellToEnterCost = yearlyItem.Key - startYear;
                                var currentValue = worksheet.Cells[uniqueTreatments[key], column + cellToEnterCost + 2].Value;
                                decimal toAdd = currentValue == null ? 0 : Convert.ToDecimal(currentValue);
                                worksheet.Cells[uniqueTreatments[key], column + cellToEnterCost + 2].Value = committedProjectMetaData.TreatmentCost + toAdd;
                            }
                            sapTotalCost += committedProjectMetaData.TreatmentCost;

                            if (map.ContainsKey(committedProjectMetaData.TreatmentCategory))
                            {
                                var category = map[committedProjectMetaData.TreatmentCategory];
                                var treatmentCost = committedProjectMetaData.TreatmentCost;
                                var currYear = yearlyItem.Key;
                                FillWorkTypeTotalSAP(workTypeTotalSAP, category, simulationYears, currYear, treatmentCost);
                            }
                            else
                            {
                                var treatmentCost = committedProjectMetaData.TreatmentCost;
                                var currYear = yearlyItem.Key;
                                var category = TreatmentCategory.Other;
                                FillWorkTypeTotalSAP(workTypeTotalSAP, category, simulationYears, currYear, treatmentCost);
                            }
                        }
                    }
                }

                TotalSAPSpent.Add(yearlyItem.Key, sapTotalCost);
            }

            column = currentCell.Column;
            worksheet.Cells[currentCell.Row, column].Value = "SAP Total";
            column++;

            int firstTotalYear = TotalSAPSpent.Count > 0 ? TotalSAPSpent.Keys.Min() : startYear;
            var offsetForTotal = firstTotalYear - startYear;

            var fromColumn = column + 1 + offsetForTotal;

            foreach (var cost in TotalSAPSpent)
            {
                if (cost.Value == 0)
                {
                    worksheet.Cells[currentCell.Row, fromColumn++].Value = "-";
                }
                else
                {
                    worksheet.Cells[currentCell.Row, fromColumn++].Value = cost.Value;
                }
            }

            sapTotalRow = currentCell.Row;

            fromColumn = column + 1;

            var endColumn = startColumn + simulationYears.Count + 1;

            ExcelHelper.ApplyBorder(worksheet.Cells[startRow, startColumn, sapTotalRow, endColumn]);
            ExcelHelper.SetCustomFormat(worksheet.Cells[startRow, fromColumn, sapTotalRow, endColumn], ExcelHelperCellFormat.NegativeCurrency);
            ExcelHelper.ApplyColor(worksheet.Cells[startRow, fromColumn, sapTotalRow, endColumn], Color.FromArgb(198, 224, 180));

            ExcelHelper.ApplyColor(worksheet.Cells[sapTotalRow, fromColumn, sapTotalRow, endColumn], Color.FromArgb(84, 130, 53));
            ExcelHelper.SetTextColor(worksheet.Cells[sapTotalRow, fromColumn, sapTotalRow, endColumn], Color.White);

            _pavementWorkSummaryCommon.UpdateCurrentCell(currentCell, sapTotalRow + 1, endColumn);

            return workTypeTotalSAP;
        }

        private Dictionary<TreatmentCategory, SortedDictionary<int, decimal>> AddCostsOfProjectBuilderWork(
            ExcelWorksheet worksheet,
            List<int> simulationYears,
            CurrentCell currentCell,
            Dictionary<int, Dictionary<string, List<CommittedProjectMetaData>>> yearlyCostProjectBuilderProj)
        {
            var workTypeTotalProjectBuilder = new Dictionary<TreatmentCategory, SortedDictionary<int, decimal>>();
            if (simulationYears.Count <= 0)
            {
                return workTypeTotalProjectBuilder;
            }

            var startYear = simulationYears[0];
            _pavementWorkSummaryCommon.SetRowColumns(currentCell, out var startRow, out var startColumn, out var row, out var column);
            currentCell.Column = column;
            var projectBuilderTotalRow = 0;

            var uniqueTreatments = new Dictionary<string, int>();
            var map = WorkTypeMap.Map;

            foreach (var yearlyItem in yearlyCostProjectBuilderProj)
            {
                decimal projectBuilderTotalCost = 0;
                row = currentCell.Row;

                foreach (var data in yearlyItem.Value)
                {
                    foreach (var committedProjectMetaData in data.Value)
                    {
                        if (committedProjectMetaData.ProjectSource == "ProjectBuilder")
                        {
                            var key = data.Key.Contains("Bundle") ? data.Key : committedProjectMetaData.TreatmentCategory;
                            if (!uniqueTreatments.ContainsKey(key))
                            {
                                uniqueTreatments.Add(key, currentCell.Row);
                                worksheet.Cells[row++, column].Value = key;
                                var cellToEnterCost = yearlyItem.Key - startYear + 2;
                                worksheet.Cells[uniqueTreatments[key], column + cellToEnterCost].Value = committedProjectMetaData.TreatmentCost;
                                currentCell.Row += 1;
                            }
                            else
                            {
                                var cellToEnterCost = yearlyItem.Key - startYear + 2;
                                var currentValue = worksheet.Cells[uniqueTreatments[key], column + cellToEnterCost].Value;
                                decimal toAdd = currentValue == null ? 0 : Convert.ToDecimal(currentValue);
                                worksheet.Cells[uniqueTreatments[key], column + cellToEnterCost].Value = committedProjectMetaData.TreatmentCost = toAdd;
                            }
                            projectBuilderTotalCost += committedProjectMetaData.TreatmentCost;

                            if (map.ContainsKey(committedProjectMetaData.TreatmentCategory))
                            {
                                var category = map[committedProjectMetaData.TreatmentCategory];
                                var treatmentCost = committedProjectMetaData.TreatmentCost;
                                var currYear = yearlyItem.Key;
                                FillWorkTypeTotalProjectBuilder(workTypeTotalProjectBuilder, category, simulationYears, currYear, treatmentCost);
                            }
                            else
                            {
                                var treatmentCost = committedProjectMetaData.TreatmentCost;
                                var currYear = yearlyItem.Key;
                                var category = TreatmentCategory.Other;
                                FillWorkTypeTotalProjectBuilder(workTypeTotalProjectBuilder, category, simulationYears, currYear, treatmentCost);
                            }
                        }
                    }
                }

                TotalProjectBuilderSpent.Add(yearlyItem.Key, projectBuilderTotalCost);
            }

            column = currentCell.Column;
            worksheet.Cells[currentCell.Row, column].Value = "Project Builder Total";
            column++;
            int firstTotalYear = TotalProjectBuilderSpent.Count > 0 ? TotalProjectBuilderSpent.Keys.Min() : startYear;
            var offsetForTotal = firstTotalYear - startYear;
            var fromColumn = column + 1 + offsetForTotal;

            foreach (var cost in TotalProjectBuilderSpent)
            {
                if (cost.Value == 0)
                {
                    worksheet.Cells[currentCell.Row, fromColumn++].Value = "-";
                }
                else
                {
                    worksheet.Cells[currentCell.Row, fromColumn++].Value = cost.Value;
                }
            }

            projectBuilderTotalRow = currentCell.Row;
            fromColumn = column + 1;
            var endColumn = startColumn + simulationYears.Count + 1;

            ExcelHelper.ApplyBorder(worksheet.Cells[startRow, startColumn, projectBuilderTotalRow, endColumn]);
            ExcelHelper.SetCustomFormat(worksheet.Cells[startRow, fromColumn, projectBuilderTotalRow, endColumn], ExcelHelperCellFormat.NegativeCurrency);
            ExcelHelper.ApplyColor(worksheet.Cells[startRow, fromColumn, projectBuilderTotalRow, endColumn], Color.FromArgb(198, 224, 180));
            ExcelHelper.ApplyColor(worksheet.Cells[projectBuilderTotalRow, fromColumn, projectBuilderTotalRow, endColumn], Color.FromArgb(84, 130, 53));
            ExcelHelper.SetTextColor(worksheet.Cells[projectBuilderTotalRow, fromColumn, projectBuilderTotalRow, endColumn], Color.White);

            _pavementWorkSummaryCommon.UpdateCurrentCell(currentCell, projectBuilderTotalRow + 1, endColumn);

            return workTypeTotalProjectBuilder;
        }

        private void FillWorkTypeTotalCommitted(Dictionary<TreatmentCategory, SortedDictionary<int, decimal>> workTypeTotalCommitted,
            TreatmentCategory category, List<int> simulationYears, int currYear, decimal treatmentCost)
        {
            if (!workTypeTotalCommitted.ContainsKey(category))
            {
                workTypeTotalCommitted.Add(category, new SortedDictionary<int, decimal>());
                foreach (var year in simulationYears)
                {
                    workTypeTotalCommitted[category].Add(year, 0);
                }
                workTypeTotalCommitted[category][currYear] += treatmentCost;
            }
            else
            {
                workTypeTotalCommitted[category][currYear] += treatmentCost;
            }
        }

        private void FillWorkTypeTotalMPMS(Dictionary<TreatmentCategory, SortedDictionary<int, decimal>> workTypeTotalMPMS,
            TreatmentCategory category, List<int> simulationYears, int currYear, decimal treatmentCost)
        {
            if (!workTypeTotalMPMS.ContainsKey(category))
            {
                workTypeTotalMPMS.Add(category, new SortedDictionary<int, decimal>());
                foreach (var year in simulationYears)
                {
                    workTypeTotalMPMS[category].Add(year, 0);
                }
                workTypeTotalMPMS[category][currYear] += treatmentCost;
            }
            else
            {
                workTypeTotalMPMS[category][currYear] += treatmentCost;
            }
        }

        private void FillWorkTypeTotalSAP(Dictionary<TreatmentCategory, SortedDictionary<int, decimal>> workTypeTotalSAP,
            TreatmentCategory category, List<int> simulationYears, int currYear, decimal treatmentCost)
        {
            if (!workTypeTotalSAP.ContainsKey(category))
            {
                workTypeTotalSAP.Add(category, new SortedDictionary<int, decimal>());
                foreach (var year in simulationYears)
                {
                    workTypeTotalSAP[category].Add(year, 0);
                }
            }
            workTypeTotalSAP[category][currYear] += treatmentCost;
        }

        private void FillWorkTypeTotalProjectBuilder(Dictionary<TreatmentCategory, SortedDictionary<int, decimal>> workTypeTotalProjectBuilder,
            TreatmentCategory category, List<int> simulationYears, int currYear, decimal treatmentCost)
        {

            if (!workTypeTotalProjectBuilder.ContainsKey(category))
            {
                workTypeTotalProjectBuilder.Add(category, new SortedDictionary<int, decimal>());
                foreach (var year in simulationYears)
                {
                    workTypeTotalProjectBuilder[category].Add(year, 0);
                }
            }
            workTypeTotalProjectBuilder[category][currYear] += treatmentCost;
        }

        private Dictionary<TreatmentCategory, SortedDictionary<int, decimal>> FillCostOfCommittedWorkSection(ExcelWorksheet worksheet, CurrentCell currentCell,
              List<int> simulationYears,
              Dictionary<int, Dictionary<string, List<CommittedProjectMetaData>>> yearlyCostCommittedProj)
        {
            _pavementWorkSummaryCommon.AddHeaders(worksheet, currentCell, simulationYears, "Cost of Committed Work", "Committed Work Type");
            var workTypeTotalData = AddCostsOfCommittedWork(worksheet, simulationYears, currentCell, yearlyCostCommittedProj);
            return workTypeTotalData;
        }

        private Dictionary<TreatmentCategory, SortedDictionary<int, decimal>> FillCostOfMPMSWorkSection(ExcelWorksheet worksheet, CurrentCell currentCell,
               List<int> simulationYears,
               Dictionary<int, Dictionary<string, List<CommittedProjectMetaData>>> yearlyCostCommittedProj)
        {
            _pavementWorkSummaryCommon.AddHeaders(worksheet, currentCell, simulationYears, "Cost of MPMS Work", "MPMS Work Type");
            var workTypeTotalData = AddCostsOfMPMSWork(worksheet, simulationYears, currentCell, yearlyCostCommittedProj);
            return workTypeTotalData;
        }

        private Dictionary<TreatmentCategory, SortedDictionary<int, decimal>> FillCostOfSAPWorkSection(
            ExcelWorksheet worksheet,
            CurrentCell currentCell,
            List<int> simulationYears,
            Dictionary<int, Dictionary<string, List<CommittedProjectMetaData>>> yearlyCostCommittedProj)
        {
            _pavementWorkSummaryCommon.AddHeaders(worksheet, currentCell, simulationYears, "Cost of SAP Work", "SAP Work Type");
            var workTypeTotalDataSAP = AddCostsOfSAPWork(worksheet, simulationYears, currentCell, yearlyCostCommittedProj);
            return workTypeTotalDataSAP;
        }

        private Dictionary<TreatmentCategory, SortedDictionary<int, decimal>> FillCostOfProjectBuilderWorkSection(
            ExcelWorksheet worksheet,
            CurrentCell currentCell,
            List<int> simulationYears,
            Dictionary<int, Dictionary<string, List<CommittedProjectMetaData>>> yearlyCostCommittedProj)
        {
            _pavementWorkSummaryCommon.AddHeaders(worksheet, currentCell, simulationYears, "Cost of Project Builder", "Project Builder Work Type");
            var workTypeTotalDataProjectBuilder = AddCostsOfProjectBuilderWork(worksheet, simulationYears, currentCell, yearlyCostCommittedProj);
            return workTypeTotalDataProjectBuilder;
        }


        private void FillBudgetAnalysisSection(ExcelWorksheet worksheet,
            CurrentCell currentCell,
            List<int> simulationYears,
            Dictionary<string, Budget> yearlyBudgetAmount,
            ICollection<CommittedProject> committedProjects,
            int totalSpendingRow)
        {
            var currentRow = currentCell.Row;
            currentCell.Row = 1;
            _pavementWorkSummaryCommon.AddHeaders(worksheet, currentCell, simulationYears, "Budget Analysis", "", "Total Remaining Budget (all years)");

            AddDetailsForBudgetAnalysis(worksheet, simulationYears, currentCell, yearlyBudgetAmount, committedProjects, totalSpendingRow);
            currentCell.Row = currentRow;
        }

        private void FillBudgetAnalysisSectionByBudget(ExcelWorksheet worksheet,
            CurrentCell currentCell,
            List<int> simulationYears,
            Dictionary<string, Budget> yearlyBudgetAmount,
            int totalSpendingRow,
            WorkSummaryByBudgetModel workSummaryByBudgetModel,
            SimulationOutput reportOutputData,
            ICollection<CommittedProject> committedProjects)
        {
            _pavementWorkSummaryCommon.AddHeaders(worksheet, currentCell, simulationYears, "Budget Analysis", "", "Total Remaining Budget (all years)");

            AddDetailsForBudgetAnalysisByBudget(worksheet, simulationYears, currentCell, yearlyBudgetAmount,totalSpendingRow, workSummaryByBudgetModel, reportOutputData, committedProjects);
        }

        private void AddDetailsForBudgetAnalysis(ExcelWorksheet worksheet, List<int> simulationYears, CurrentCell currentCell, Dictionary<string, Budget> yearlyBudgetAmount, ICollection<CommittedProject> committedProjects, int totalSpendingRow)
        {
            int startRow, startColumn, row, column;
            _pavementWorkSummaryCommon.SetRowColumns(currentCell, out startRow, out startColumn, out row, out column);
                        
            var rowTitles = new List<string> { PAMSConstants.RemainingBudget, PAMSConstants.PercentBudgetSpentPAMS, PAMSConstants.PercentBudgetSpentCommitted, PAMSConstants.PercentBudgetSpentMPMS, PAMSConstants.PercentBudgetSpentSAP, PAMSConstants.PercentBudgetSpentProjectBuilder };
            _pavementWorkSummaryCommon.SetPavementTreatmentGroupsExcelString(worksheet, rowTitles, ref row, ref column);
                      
            column++;
            var fromColumn = column + 1;
            foreach (var year in simulationYears)
            {
                row = startRow;
                column = ++column;
                var yearIndex = year - simulationYears[0];

                // Calculate the MPMS, SAP, and Project Builder totals based on committed projects
                var committedBudgetTotal = committedProjects.Where(_ => _.Year == year && _.ProjectSource == ProjectSourceDTO.Committed).Select(_ => _.Cost).Sum();
                var mpmsBudgetTotal = committedProjects.Where(_ => _.Year == year && _.ProjectSource == ProjectSourceDTO.MPMS).Select(_ => _.Cost).Sum();
                var sapBudgetTotal = committedProjects.Where(_ => _.Year == year && _.ProjectSource == ProjectSourceDTO.SAP).Select(_ => _.Cost).Sum();
                var projectBuilderBudgetTotal = committedProjects.Where(_ => _.Year == year && _.ProjectSource == ProjectSourceDTO.ProjectBuilder).Select(_ => _.Cost).Sum();

                double cellValue = Convert.ToDouble(worksheet.Cells[totalSpendingRow, column].Value ?? 0.0);
                double pamsBudgetTotal = cellValue - committedBudgetTotal - mpmsBudgetTotal - sapBudgetTotal - projectBuilderBudgetTotal;

                // var totalBudget = pamsBudgetTotal + mpmsBudgetTotal + sapBudgetTotal + projectBuilderBudgetTotal;
                double yearlyBudget = Convert.ToDouble(yearlyBudgetAmount.Sum(x => x.Value.YearlyAmounts[yearIndex].Value));

                double totalSpending = Convert.ToDouble(worksheet.Cells[totalSpendingRow, column].Value ?? 0.0);

                // Calculate remaining budget
                var remainingBudget = yearlyBudget - totalSpending;
                worksheet.Cells[row, column].Value = remainingBudget;
                worksheet.Cells[row, column].Style.Numberformat.Format = "$#,##0.00";
                row++;

                double[] categoryBudgetTotals = new double[] { pamsBudgetTotal, committedBudgetTotal, mpmsBudgetTotal, sapBudgetTotal, projectBuilderBudgetTotal };
                for (int rowIndex = 0; rowIndex < 5; rowIndex++)
                {
                    // Calculate percentage
                    double percentage = categoryBudgetTotals[rowIndex] / yearlyBudget;
                    worksheet.Cells[row + rowIndex, column].Value = percentage;
                }

            }

            ExcelHelper.ApplyColor(worksheet.Cells[startRow, fromColumn, startRow, column], Color.Blue);
            ExcelHelper.SetTextColor(worksheet.Cells[startRow, fromColumn, startRow, column], Color.White);

            var projectBuilderColor = Color.FromArgb(248, 203, 173);
            for (int i = 1; i <= 5; i++) // Apply color to the percentage rows
            {
                ExcelHelper.ApplyColor(worksheet.Cells[startRow + i, fromColumn, startRow + i, column], projectBuilderColor);
            }

            worksheet.Cells[startRow, column + 1].Formula = "SUM(" + worksheet.Cells[startRow, fromColumn, startRow, column].Address + ")";
            worksheet.Cells[startRow, column + 1].Style.Numberformat.Format = "$#,##0.00";
            worksheet.Cells[startRow, column + 2].Formula = ExcelFormulas.Percentage(startRow, column + 1, totalSpendingRow + 2, column + 1);
            worksheet.Cells[startRow, column + 2].Style.Numberformat.Format = "#0.00%";
            worksheet.Cells[startRow, column + 3].Value = "Percentage of Total Budget that was Unspent";
                        
            ExcelHelper.ApplyBorder(worksheet.Cells[startRow, startColumn, startRow + 5, column]);
            ExcelHelper.ApplyBorder(worksheet.Cells[startRow, column + 1, startRow, column + 1]);
            ExcelHelper.ApplyColor(worksheet.Cells[startRow, column + 1, startRow + 5, column + 1], Color.FromArgb(217, 217, 217));
            ExcelHelper.SetCustomFormat(worksheet.Cells[startRow + 1, fromColumn, startRow + 5, column], ExcelHelperCellFormat.PercentDecimal4);

            _pavementWorkSummaryCommon.UpdateCurrentCell(currentCell, startRow + 6, column);
        }

        private void AddDetailsForBudgetAnalysisByBudget(
            ExcelWorksheet worksheet,
            List<int> simulationYears,
            CurrentCell currentCell,
            Dictionary<string, Budget> yearlyBudgetAmount,
            int totalSpendingRow,
            WorkSummaryByBudgetModel workSummaryByBudgetModel,
            SimulationOutput reportOutputData,
            ICollection<CommittedProject> committedProjects)
        {
            _pavementWorkSummaryCommon.SetRowColumns(currentCell, out int startRow, out int startColumn, out int row, out int column);

            var rowTitles = new List<string> {
                PAMSConstants.RemainingBudget,                
                PAMSConstants.PercentBudgetSpentPAMS,
                PAMSConstants.PercentBudgetSpentCommitted,
                PAMSConstants.PercentBudgetSpentMPMS,
                PAMSConstants.PercentBudgetSpentSAP,
                PAMSConstants.PercentBudgetSpentProjectBuilder
            };
            _pavementWorkSummaryCommon.SetPavementTreatmentGroupsExcelString(worksheet, rowTitles, ref row, ref column);

            var pamsRow = startRow;
            column++;
            var fromColumn = column + 1;
            foreach (var year in simulationYears)
            {
                row = startRow;
                column = ++column; // Move to the next column for the new year
                var yearIndex = year - simulationYears.First();

                decimal committedBudgetTotal = 0m;
                decimal mpmsBudgetTotal = 0m;
                decimal sapBudgetTotal = 0m;
                decimal projectBuilderBudgetTotal = 0m;

                // Loop through the report data to calculate totals based on the budget name
                foreach (var yearData in reportOutputData.Years.Where(y => y.Year == year))
                {
                    foreach (var section in yearData.Assets)
                    {
                        if (section.TreatmentCause == TreatmentCause.CommittedProject &&
                        section.AppliedTreatment.ToLower() != PAMSConstants.NoTreatment)
                        {
                            foreach (var consideration in section.TreatmentConsiderations)
                            {
                                foreach (var budgetUsage in consideration.FundingCalculationOutput?.AllocationMatrix.
                                        Where(bu => bu.BudgetName.Equals(workSummaryByBudgetModel.BudgetName, StringComparison.OrdinalIgnoreCase) &&
                                        bu.Year == year))
                                {
                                    var projectSource = committedProjects.FirstOrDefault(_ => _.Year == year &&
                                                                                         _.Name == budgetUsage.TreatmentName &&
                                                                                         _.Budget.Name == budgetUsage.BudgetName)?.ProjectSource;
                                    switch (projectSource)
                                    {
                                        case ProjectSourceDTO.Committed:
                                            committedBudgetTotal += budgetUsage.AllocatedAmount;
                                            break;
                                        case ProjectSourceDTO.MPMS:
                                            mpmsBudgetTotal += budgetUsage.AllocatedAmount;
                                            break;
                                        case ProjectSourceDTO.SAP:
                                            sapBudgetTotal += budgetUsage.AllocatedAmount;
                                            break;
                                        case ProjectSourceDTO.ProjectBuilder:
                                            projectBuilderBudgetTotal += budgetUsage.AllocatedAmount;
                                            break;
                                        default:
                                            break;
                                    }
                                }
                            }
                        }
                    }
                }

                decimal pamsBudgetTotal = Convert.ToDecimal(worksheet.Cells[totalSpendingRow, column].Value) - committedBudgetTotal - mpmsBudgetTotal - sapBudgetTotal - projectBuilderBudgetTotal;
                decimal yearlyBudget = Convert.ToDecimal(yearlyBudgetAmount[workSummaryByBudgetModel.BudgetName].YearlyAmounts[yearIndex].Value);
                decimal remainingBudget = yearlyBudget - (pamsBudgetTotal + committedBudgetTotal + mpmsBudgetTotal + sapBudgetTotal + projectBuilderBudgetTotal);
                worksheet.Cells[row, column].Value = Convert.ToDouble(remainingBudget);
                worksheet.Cells[row, column].Style.Numberformat.Format = "$#,##0.00";
                row++;

                decimal[] categoryBudgetTotals = { pamsBudgetTotal, committedBudgetTotal, mpmsBudgetTotal, sapBudgetTotal, projectBuilderBudgetTotal };
                for (int i = 0; i < categoryBudgetTotals.Length; i++)
                {
                    decimal percentage = yearlyBudget == 0 ? 0m : categoryBudgetTotals[i] / yearlyBudget;
                    worksheet.Cells[row + i, column].Value = percentage;
                }
            }            

            ExcelHelper.ApplyColor(worksheet.Cells[startRow, fromColumn, startRow, column], Color.Blue);
            ExcelHelper.SetTextColor(worksheet.Cells[startRow, fromColumn, startRow, column], Color.White);

            var projectBuilderColor = Color.FromArgb(248, 203, 173);
            for (int i = 1; i <= 5; i++)
            {
                ExcelHelper.ApplyColor(worksheet.Cells[startRow + i, fromColumn, startRow + i, column], projectBuilderColor);
            }

            worksheet.Cells[startRow, column + 1].Formula = "SUM(" + worksheet.Cells[startRow, fromColumn, startRow, column].Address + ")";
            worksheet.Cells[startRow, column + 1].Style.Numberformat.Format = "$#,##0.00";
            worksheet.Cells[startRow, column + 2].Formula = ExcelFormulas.Percentage(startRow, column + 1, totalSpendingRow + 2, column + 1);
            worksheet.Cells[startRow, column + 2].Style.Numberformat.Format = "#0.00%";
            worksheet.Cells[startRow, column + 3].Value = "Percentage of Total Budget that was Unspent";
            ExcelHelper.ApplyBorder(worksheet.Cells[startRow, startColumn, row + 4, column]);
            ExcelHelper.ApplyBorder(worksheet.Cells[startRow, column + 1, startRow, column + 1]);
            ExcelHelper.ApplyColor(worksheet.Cells[startRow, column + 1, startRow + 5, column + 1], Color.FromArgb(217, 217, 217));
            ExcelHelper.SetCustomFormat(worksheet.Cells[startRow + 1, fromColumn, startRow + 5, column], ExcelHelperCellFormat.PercentDecimal4);
            _pavementWorkSummaryCommon.UpdateCurrentCell(currentCell, startRow + 6, column);

        }

        private Dictionary<TreatmentCategory, SortedDictionary<int, decimal>> AddCostOfWorkOutsideScope(List<BaseCommittedProjectDTO> committedProjectsForWorkOutsideScope)
        {
            var workTypeTotalWorkOutsideScope = new Dictionary<TreatmentCategory, SortedDictionary<int, decimal>>();
            var category = TreatmentCategory.WorkOutsideScope;

            foreach (var committedProjectForWorkOutsideScope in committedProjectsForWorkOutsideScope)
            {
                var currYear = committedProjectForWorkOutsideScope.Year;
                var treatmentCost = Convert.ToDecimal(committedProjectForWorkOutsideScope.Cost);

                if (!workTypeTotalWorkOutsideScope.ContainsKey(category))
                {
                    workTypeTotalWorkOutsideScope.Add(category, new SortedDictionary<int, decimal>());
                }
                if (!workTypeTotalWorkOutsideScope[category].ContainsKey(currYear))
                {
                    workTypeTotalWorkOutsideScope[category].Add(currYear, 0);
                }
                workTypeTotalWorkOutsideScope[category][currYear] += treatmentCost;
            }

            return workTypeTotalWorkOutsideScope;
        }

        private static void GetCosts(KeyValuePair<int, Dictionary<string, Dictionary<int, (decimal treatmentCost, decimal compositeTreatmentCost, int length)>>> yearlyValues, (string Name, string AssetType, TreatmentCategory Category) treatment, out decimal treatmentCost, out decimal compositeTreatmentCost)
        {
            treatmentCost = 0;
            compositeTreatmentCost = 0;
            yearlyValues.Value.TryGetValue(treatment.Name, out var costAndLengthsPerSurfaceId);
            if (costAndLengthsPerSurfaceId != null)
            {
                foreach (var value in costAndLengthsPerSurfaceId)
                {
                    treatmentCost += value.Value.treatmentCost;
                    compositeTreatmentCost += value.Value.compositeTreatmentCost;
                }
            }
        }

        private static void GetCostsInYearlyValue(List<KeyValuePair<int, (decimal treatmentCost, decimal compositeTreatmentCost, int length)>> valuesPerSurfaceId, out decimal treatmentCost, out decimal compositeTreatmentCost)
        {
            treatmentCost = 0;
            compositeTreatmentCost = 0;
            foreach (var value in valuesPerSurfaceId)
            {
                treatmentCost += value.Value.treatmentCost;
                compositeTreatmentCost += value.Value.compositeTreatmentCost;
            }
        }
    }
}
