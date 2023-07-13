using System;
using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.iAM.Analysis;
using OfficeOpenXml;
using AppliedResearchAssociates.iAM.Reporting.Models.PAMSSummaryReport;
using AppliedResearchAssociates.iAM.ExcelHelpers;
using System.Drawing;
using AppliedResearchAssociates.iAM.DTOs.Enums;
using AppliedResearchAssociates.iAM.Reporting.Services.PAMSSummaryReport.StaticContent;

namespace AppliedResearchAssociates.iAM.Reporting.Services.PAMSSummaryReport.PavementWorkSummary
{
    public class CostBudgetsWorkSummary
    {
        private PavementWorkSummaryCommon _pavementWorkSummaryCommon;

        private Dictionary<int, decimal> TotalAsphaltSpent = new Dictionary<int, decimal>();
        private Dictionary<int, decimal> TotalCompositeSpent = new Dictionary<int, decimal>();
        private Dictionary<int, decimal> TotalConcreteSpent = new Dictionary<int, decimal>();

        private int AsphaltTotalRow = 0;
        private int CompositeTotalRow = 0;
        private int ConcreteTotalRow = 0;

        public CostBudgetsWorkSummary()
        {
            _pavementWorkSummaryCommon = new PavementWorkSummaryCommon();
        }


        public ChartRowsModel FillCostBudgetWorkSummarySections(
            ExcelWorksheet worksheet, CurrentCell currentCell, List<int> simulationYears,
            Dictionary<string, Budget> yearlyBudgetAmount,
            Dictionary<int, Dictionary<string, (decimal treatmentCost, decimal compositeTreatmentCost, int length)>> costAndLengthPerTreatmentPerYear,
            Dictionary<int, Dictionary<PavementTreatmentHelper.TreatmentGroup, (decimal treatmentCost, int length)>> costAndLengthPerTreatmentGroupPerYear,
            List<(string TreatmentName, DTOs.Enums.AssetCategory AssetType, TreatmentCategory Category)> simulationTreatments,
            Dictionary<TreatmentCategory, SortedDictionary<int, (decimal treatmentCost, int length)>> workTypeTotals
            )
        {
            FillCostOfFullDepthAsphaltWorkSection(worksheet, currentCell, simulationYears, costAndLengthPerTreatmentPerYear, simulationTreatments);
            FillCostOfCompositeWork(worksheet, currentCell, simulationYears, costAndLengthPerTreatmentPerYear, simulationTreatments);
            FillCostOfConcreteWork(worksheet, currentCell, simulationYears, costAndLengthPerTreatmentPerYear, simulationTreatments);
            FillTreatmentGroupTotalsSection(worksheet, currentCell, simulationYears, costAndLengthPerTreatmentGroupPerYear);
            FillWorkTypeTotalsSection(worksheet, currentCell, simulationYears, workTypeTotals, yearlyBudgetAmount, out var totalSpendingRow);
            FillBudgetTotalSection(worksheet, currentCell, simulationYears, yearlyBudgetAmount);
            FillBudgetAnalysisSection(worksheet, currentCell, simulationYears, yearlyBudgetAmount, totalSpendingRow);

            var chartRowsModel = new ChartRowsModel();
            return chartRowsModel;
        }


        private Dictionary<TreatmentCategory, SortedDictionary<int, decimal>> FillCostOfFullDepthAsphaltWorkSection(
            ExcelWorksheet worksheet,
            CurrentCell currentCell,
            List<int> simulationYears,
            Dictionary<int, Dictionary<string, (decimal treatmentCost, decimal compositeTreatmentCost, int count)>> costAndCountPerTreatmentPerYear,
            List<(string TreatmentName, DTOs.Enums.AssetCategory AssetType, TreatmentCategory Category)> simulationTreatments
            )
        {
            _pavementWorkSummaryCommon.AddHeaders(worksheet, currentCell, simulationYears, "Cost of PAMS Full Depth Asphalt Work", "PAMS Full Depth Asphalt Work");

            var asphaltTreatments = _pavementWorkSummaryCommon.GetNoTreatments(simulationTreatments).Concat(_pavementWorkSummaryCommon.GetAsphaltTreatments(simulationTreatments)).ToList();

            var workTypeTotalFullDepthAsphalt = AddCostsOfFullDepthAsphaltWork(worksheet, simulationYears, currentCell, costAndCountPerTreatmentPerYear, asphaltTreatments);

            return workTypeTotalFullDepthAsphalt;
        }

        private Dictionary<TreatmentCategory, SortedDictionary<int, decimal>> AddCostsOfFullDepthAsphaltWork(
            ExcelWorksheet worksheet,
            List<int> simulationYears,
            CurrentCell currentCell,
            Dictionary<int, Dictionary<string, (decimal treatmentCost, decimal compositeTreatmentCost, int count)>> costAndCountPerTreatmentPerYear,
            List<(string Name, DTOs.Enums.AssetCategory AssetType, TreatmentCategory Category)> simulationTreatments
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

            worksheet.Cells[row++, column].Value = PAMSConstants.AsphaltTotal;
            column++;
            var fromColumn = column + 1;
            // Filling in the cost per treatment per year in the excel TAB
            foreach (var yearlyValues in costAndCountPerTreatmentPerYear)
            {
                row = startRow;
                column = ++column;
                decimal asphaltTotalCost = 0;

                foreach (var treatment in simulationTreatments)
                {
                    decimal cost = 0;
                    yearlyValues.Value.TryGetValue(treatment.Name, out var asphaltCost);
                    worksheet.Cells[row++, column].Value = asphaltCost.treatmentCost - asphaltCost.compositeTreatmentCost;
                    asphaltTotalCost += (asphaltCost.treatmentCost - asphaltCost.compositeTreatmentCost);
                    cost = asphaltCost.treatmentCost - asphaltCost.compositeTreatmentCost;

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

            AsphaltTotalRow = asphaltTotalRow;
            return workTypeFullDepthAsphalt;
        }
 
    private Dictionary<TreatmentCategory, SortedDictionary<int, decimal>> FillCostOfCompositeWork(
            ExcelWorksheet worksheet,
            CurrentCell currentCell,
            List<int> simulationYears,
            Dictionary<int, Dictionary<string, (decimal treatmentCost, decimal compositeTreatmentCost, int count)>> costAndCountPerTreatmentPerYear,
            List<(string Name, DTOs.Enums.AssetCategory AssetType, TreatmentCategory Category)> simulationTreatments
            )
        {
            var headerRange = new Range(currentCell.Row, currentCell.Row + 1);
            _pavementWorkSummaryCommon.AddHeaders(worksheet, currentCell, simulationYears, "Cost of PAMS Composite Work", "PAMS Composite Work");

            var asphaltTreatments = _pavementWorkSummaryCommon.GetNoTreatments(simulationTreatments).Concat(_pavementWorkSummaryCommon.GetAsphaltTreatments(simulationTreatments)).ToList();

            var workTypeComposite = AddCostsOfCompositeWork(worksheet, simulationYears, currentCell, costAndCountPerTreatmentPerYear, asphaltTreatments);

            return workTypeComposite;
        }



        private Dictionary<TreatmentCategory, SortedDictionary<int, decimal>> AddCostsOfCompositeWork(
            ExcelWorksheet worksheet,
            List<int> simulationYears,
            CurrentCell currentCell,
            Dictionary<int, Dictionary<string, (decimal treatmentCost, decimal compositeTreatmentCost, int count)>> costAndCountPerTreatmentPerYear,
            List<(string Name, DTOs.Enums.AssetCategory AssetType, TreatmentCategory Category)>
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

            worksheet.Cells[row++, column].Value = PAMSConstants.CompositeTotal;
            column++;
            var fromColumn = column + 1;
            // Filling in the cost per treatment per year in the excel TAB
            foreach (var yearlyValues in costAndCountPerTreatmentPerYear)
            {
                decimal CompositeTotalCost = 0;
                row = startRow;
                column = ++column;

                foreach (var treatment in simulationTreatments)
                {
                    decimal cost = 0;
                    yearlyValues.Value.TryGetValue(treatment.Name, out var CompositeCost);
                    worksheet.Cells[row++, column].Value = CompositeCost.compositeTreatmentCost;
                    CompositeTotalCost += CompositeCost.compositeTreatmentCost;
                    cost = CompositeCost.compositeTreatmentCost;

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
                    //}
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

            CompositeTotalRow = compositeTotalRow;
            return workTypeComposite;
        }

        private Dictionary<TreatmentCategory, SortedDictionary<int, decimal>> FillCostOfConcreteWork(
            ExcelWorksheet worksheet,
            CurrentCell currentCell,
            List<int> simulationYears,
            Dictionary<int, Dictionary<string, (decimal treatmentCost, decimal compositeTreatmentCost, int count)>> costAndCountPerTreatmentPerYear,
            List<(string Name, DTOs.Enums.AssetCategory AssetType, TreatmentCategory Category)> simulationTreatments
            )
        {
            var headerRange = new Range(currentCell.Row, currentCell.Row + 1);
            _pavementWorkSummaryCommon.AddHeaders(worksheet, currentCell, simulationYears, "Cost of PAMS Concrete Work", "PAMS Concrete Work");

            var concreteTreatments = _pavementWorkSummaryCommon.GetNoTreatments(simulationTreatments).Concat(_pavementWorkSummaryCommon.GetConcreteTreatments(simulationTreatments)).ToList();

            var workTypeConcrete = AddCostsOfConcreteWork(worksheet, simulationYears, currentCell, costAndCountPerTreatmentPerYear, concreteTreatments);

            return workTypeConcrete;
        }




        private Dictionary<TreatmentCategory, SortedDictionary<int, decimal>> AddCostsOfConcreteWork(
            ExcelWorksheet worksheet,
            List<int> simulationYears,
            CurrentCell currentCell,
            Dictionary<int, Dictionary<string, (decimal treatmentCost, decimal compositeTreatmentCost, int count)>> costAndCountPerTreatmentPerYear,
            List<(string Name, DTOs.Enums.AssetCategory AssetType, TreatmentCategory Category)> simulationTreatments
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

            worksheet.Cells[row++, column].Value = PAMSConstants.ConcreteTotal;
            column++;
            var fromColumn = column + 1;
            // Filling in the cost per treatment per year in the excel TAB
            foreach (var yearlyValues in costAndCountPerTreatmentPerYear)
            {
                decimal ConcreteTotalCost = 0;
                row = startRow;
                column = ++column;

                foreach (var treatment in simulationTreatments)
                {
                    decimal cost = 0;

                    yearlyValues.Value.TryGetValue(treatment.Name, out var ConcreteCost);
                    worksheet.Cells[row++, column].Value = ConcreteCost.treatmentCost;
                    ConcreteTotalCost += ConcreteCost.treatmentCost;
                    cost = ConcreteCost.treatmentCost;

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
                    //}
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

            ConcreteTotalRow = concreteTotalRow;
            return workTypeConcrete;
        }


        private void AddTreatmentGroupTotalDetails(ExcelWorksheet worksheet, CurrentCell currentCell,
            Dictionary<int, Dictionary<PavementTreatmentHelper.TreatmentGroup, (decimal treatmentCost, int length)>> costAndLengthPerTreatmentGroupPerYear,
            PavementTreatmentHelper.TreatmentGroupCategory treatmentGroupCategory)
        {
            int startRow, startColumn, row, column;
            _pavementWorkSummaryCommon.SetRowColumns(currentCell, out startRow, out startColumn, out row, out column);

            var treatmentGroups = PavementTreatmentHelper.GetListOfTreatmentGroupForCategory(treatmentGroupCategory);

            var prefix = PavementTreatmentHelper.GetTreatmentGroupString(treatmentGroupCategory) + " - ";
            var treatmentGroupTitles = treatmentGroups.Select(tg => prefix + tg.GroupDescription).ToList();

            _pavementWorkSummaryCommon.SetPavementTreatmentGroupsExcelString(worksheet, treatmentGroupTitles, ref row, ref column);

            column++;
            var fromColumn = column + 1;

            foreach (var yearlyValues in costAndLengthPerTreatmentGroupPerYear)
            {
                row = startRow;
                column = ++column;
                foreach (var treatmentGroup in treatmentGroups)
                {
                    yearlyValues.Value.TryGetValue(treatmentGroup, out var costAndLength);
                    worksheet.Cells[row, column].Value = costAndLength.treatmentCost;
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
            Dictionary<int, Dictionary<PavementTreatmentHelper.TreatmentGroup, (decimal treatmentCost, int length)>> costAndLengthPerTreatmentGroupPerYear
            )
        {
            if (simulationYears.Count <= 0)
            {
                return;// workTypeConcrete;
            }

            _pavementWorkSummaryCommon.AddHeaders(worksheet, currentCell, simulationYears, "Total Budget", "PAMS Treatment Groups Totals");

            AddTreatmentGroupTotalDetails(worksheet, currentCell, costAndLengthPerTreatmentGroupPerYear, PavementTreatmentHelper.TreatmentGroupCategory.Bituminous);
            AddTreatmentGroupTotalDetails(worksheet, currentCell, costAndLengthPerTreatmentGroupPerYear, PavementTreatmentHelper.TreatmentGroupCategory.Concrete);
        }


        private void FillWorkTypeTotalsSection(ExcelWorksheet worksheet, CurrentCell currentCell,
            List<int> simulationYears,
            Dictionary<TreatmentCategory, SortedDictionary<int, (decimal treatmentCost, int length)>> workTypeTotals,
            Dictionary<string, Budget> yearlyBudgetAmount,
            out int totalSpendingRow
            )
        {
            var workTypesForReport = new List<TreatmentCategory> { TreatmentCategory.Maintenance, TreatmentCategory.Preservation, TreatmentCategory.Rehabilitation, TreatmentCategory.Replacement };
            var headerRange = new Range(currentCell.Row, currentCell.Row + 1);
            _pavementWorkSummaryCommon.AddHeaders(worksheet, currentCell, simulationYears, "Total Budget", "Work Type Totals", "Total (all years)");

            int startRow, startColumn, row, column;
            _pavementWorkSummaryCommon.SetRowColumns(currentCell, out startRow, out startColumn, out row, out column);

            var workTypeTitles = workTypesForReport.Select(tc => tc.ToSpreadsheetString()).ToList();
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
                worksheet.Cells[row, column].Value = rowTotal;

                row++;
            }

            // Add Total Spent Row
            column = fromColumn;
            decimal totalSpentTotal = 0;
            foreach (var year in simulationYears)
            {
                totalSpentTotal += columnTotals[year];
                worksheet.Cells[row, column].Value = columnTotals[year];
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

            worksheet.Cells[startRow + 1, column + 1].Style.Numberformat.Format = "#0.00%";
            worksheet.Cells[startRow + 1, column + 2].Value = "Percentage spent on PRESERVATION";

            worksheet.Cells[startRow + 2, column + 1].Style.Numberformat.Format = "#0.00%";
            worksheet.Cells[startRow + 2, column + 2].Value = "Percentage spent on REHABILITATION";

            worksheet.Cells[startRow + 3, column + 1].Style.Numberformat.Format = "#0.00%";
            worksheet.Cells[startRow + 3, column + 2].Value = "Percentage spent on REPLACEMENT";

            row += 2;
            worksheet.Cells[row, 1].Value = "Total PAMS Budget";
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

        private void FillBudgetTotalSection(ExcelWorksheet worksheet, CurrentCell currentCell,
            List<int> simulationYears,
            Dictionary<string, Budget> yearlyBudgetAmount
            )
        {
            var headerRange = new Range(currentCell.Row, currentCell.Row + 1);
            _pavementWorkSummaryCommon.AddHeaders(worksheet, currentCell, simulationYears, "", "Budget Total");

            int startRow, startColumn, row, column;
            _pavementWorkSummaryCommon.SetRowColumns(currentCell, out startRow, out startColumn, out row, out column);

            var rowTitles = new List<string> { "PAMS", "MPMS", "SAP" };
            _pavementWorkSummaryCommon.SetPavementTreatmentGroupsExcelString(worksheet, rowTitles, ref row, ref column);

            column++;
            var fromColumn = column + 1;

            row = startRow;
            // TODO: Fill with actual values
            foreach (var rowTitle in rowTitles) // TODO: Iteration for spacing only; must change for actual values
            {
                column = fromColumn;
                foreach (var year in simulationYears)
                {
                    // TODO: Use correct values
                    worksheet.Cells[row, column].Value = 0.0;
                    column++;
                }
                row++;
            }


            ExcelHelper.ApplyBorder(worksheet.Cells[startRow, startColumn, row - 1, column - 1]);
            ExcelHelper.SetCustomFormat(worksheet.Cells[startRow, fromColumn, row - 1, column - 1], ExcelHelperCellFormat.NegativeCurrency);
            ExcelHelper.ApplyColor(worksheet.Cells[startRow, fromColumn, row - 1, column - 1], Color.FromArgb(84, 130, 53));
            ExcelHelper.SetTextColor(worksheet.Cells[startRow, fromColumn, row, column - 1], Color.White);

            _pavementWorkSummaryCommon.UpdateCurrentCell(currentCell, row, column);
        }


        private void FillBudgetAnalysisSection(ExcelWorksheet worksheet, CurrentCell currentCell,
            List<int> simulationYears,
            Dictionary<string, Budget> yearlyBudgetAmount,
            int totalSpendingRow)
        {
            _pavementWorkSummaryCommon.AddHeaders(worksheet, currentCell, simulationYears, "Budget Analysis", "", "Total Remaining Budget (all years)");

            AddDetailsForBudgetAnalysis(worksheet, simulationYears, currentCell, yearlyBudgetAmount, totalSpendingRow);
        }

        private void AddDetailsForBudgetAnalysis(ExcelWorksheet worksheet, List<int> simulationYears, CurrentCell currentCell, Dictionary<string, Budget> yearlyBudgetAmount, int totalSpendingRow)
        {
            int startRow, startColumn, row, column;
            _pavementWorkSummaryCommon.SetRowColumns(currentCell, out startRow, out startColumn, out row, out column);

            var rowTitles = new List<string> { PAMSConstants.RemainingBudget, PAMSConstants.PercentBudgetSpentPAMS, PAMSConstants.PercentBudgetSpentMPMS, PAMSConstants.PercentBudgetSpentSAP };
            _pavementWorkSummaryCommon.SetPavementTreatmentGroupsExcelString(worksheet, rowTitles, ref row, ref column);

            column++;
            var fromColumn = column + 1;
            foreach (var year in simulationYears)
            {
                row = startRow;
                column = ++column;
                var yearIndex = year - simulationYears[0];
                var yearlyBudget = yearlyBudgetAmount.Sum(x => x.Value.YearlyAmounts[yearIndex].Value);
                var totalSpending = worksheet.Cells[totalSpendingRow, column].Value;
                if (totalSpending is decimal spending)
                {
                    worksheet.Cells[row, column].Value = yearlyBudget - spending;

                    row++;

                    worksheet.Cells[row, column].Value = (yearlyBudget - spending) / yearlyBudget;
                }
                else
                {
                    worksheet.Cells[row, column].Value = yearlyBudget;
                }
            }
            worksheet.Cells[startRow, column + 1].Formula = "SUM(" + worksheet.Cells[startRow, fromColumn, startRow, column] + ")";

            worksheet.Cells[startRow, column + 2].Style.Numberformat.Format = "#0.00%";
            worksheet.Cells[startRow, column + 3].Value = "Percentage of Total Budget that was Unspent";

            ExcelHelper.ApplyBorder(worksheet.Cells[startRow, startColumn, startRow + 3, column]);

            ExcelHelper.SetCustomFormat(worksheet.Cells[startRow, fromColumn, startRow, column + 1], ExcelHelperCellFormat.NegativeCurrency);
            ExcelHelper.SetCustomFormat(worksheet.Cells[startRow + 1, fromColumn, startRow + 3, column], ExcelHelperCellFormat.Percentage);

            ExcelHelper.ApplyColor(worksheet.Cells[startRow, fromColumn, startRow, column], Color.Red);
            ExcelHelper.ApplyColor(worksheet.Cells[startRow + 1, fromColumn, startRow + 3, column], Color.FromArgb(248, 203, 173));

            ExcelHelper.ApplyBorder(worksheet.Cells[startRow, column + 1, startRow, column + 1]);
            ExcelHelper.ApplyColor(worksheet.Cells[startRow, column + 1, startRow + 3, column + 1], Color.FromArgb(217, 217, 217));



            ExcelHelper.ApplyColor(worksheet.Cells[startRow + 5, startColumn, startRow + 5, column], Color.FromArgb(89, 89, 89));

            _pavementWorkSummaryCommon.UpdateCurrentCell(currentCell, startRow + 6, column);

        }
    }

}
