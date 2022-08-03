using System;
using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.iAM.Analysis;
using OfficeOpenXml;
using AppliedResearchAssociates.iAM.Reporting.Models.PAMSSummaryReport;
using AppliedResearchAssociates.iAM.ExcelHelpers;
using System.Drawing;

namespace AppliedResearchAssociates.iAM.Reporting.Services.PAMSSummaryReport.PavementWorkSummary
{
    // Supporting classes for specific sections of report
    // TODO: Factor out to separate files before PR

    public class CostBudgetsWorkSummary
    {
        private WorkSummaryModel _workSummaryModel;
        private PavementWorkSummaryCommon _pavementWorkSummaryCommon;

        private Dictionary<int, decimal> TotalAsphaltSpent = new Dictionary<int, decimal>();
        private Dictionary<int, decimal> TotalCompositeSpent = new Dictionary<int, decimal>();
        private Dictionary<int, decimal> TotalConcreteSpent = new Dictionary<int, decimal>();

        private int AsphaltTotalRow = 0;
        private int CompositeTotalRow = 0;
        private int ConcreteTotalRow = 0;

        public CostBudgetsWorkSummary(WorkSummaryModel workSummaryModel)
        {
            _pavementWorkSummaryCommon = new PavementWorkSummaryCommon();
            _workSummaryModel = workSummaryModel;
        }

        Dictionary<TreatmentCategory, SortedDictionary<int, (decimal treatmentCost, int length)>> CalculateWorkTypeTotals(
            Dictionary<int, Dictionary<string, (decimal treatmentCost, int length)>> costAndLengthPerTreatmentPerYear,
            List<(string Name, AssetCategory AssetType, TreatmentCategory Category)> simulationTreatments
            )
        {
            var workTypeTotals = new Dictionary<TreatmentCategory, SortedDictionary<int, (decimal treatmentCost, int length)>>();

            foreach (var yearlyValues in costAndLengthPerTreatmentPerYear)
            {
                foreach (var treatment in simulationTreatments)
                {
                    decimal cost = 0;
                    int length = 0;

                    yearlyValues.Value.TryGetValue(treatment.Name, out var costAndLength);
                    cost = costAndLength.treatmentCost;
                    length = costAndLength.length;

                    if (!workTypeTotals.ContainsKey(treatment.Category))
                    {
                        workTypeTotals.Add(treatment.Category, new SortedDictionary<int, (decimal treatmentCost, int length)>()
                        {
                            { yearlyValues.Key, (cost, length) }
                        });
                    }
                    else
                    {
                        if (!workTypeTotals[treatment.Category].ContainsKey(yearlyValues.Key))
                        {
                            workTypeTotals[treatment.Category].Add(yearlyValues.Key, (0, 0));
                        }
                        var value = workTypeTotals[treatment.Category][yearlyValues.Key];
                        value.treatmentCost += cost;
                        value.length += length;
                        workTypeTotals[treatment.Category][yearlyValues.Key] = value;
                    }
                }
            }
            return workTypeTotals;
        }


        public void FillCostBudgetWorkSummarySections(
            ExcelWorksheet worksheet, CurrentCell currentCell, List<int> simulationYears,
            Dictionary<string, Budget> yearlyBudgetAmount,
            Dictionary<int, Dictionary<string, (decimal treatmentCost, int length)>> costAndLengthPerTreatmentPerYear,
            Dictionary<int, Dictionary<PavementTreatmentHelper.TreatmentGroup, (decimal treatmentCost, int length)>> costAndLengthPerTreatmentGroupPerYear,
            //Dictionary<int, Dictionary<string, (decimal treatmentCost, int bridgeCount)>> yearlyCostCommittedProj,
            //Dictionary<int, Dictionary<string, decimal>> bpnCostPerYear,
            List<(string Name, AssetCategory AssetType, TreatmentCategory Category)> simulationTreatments
            )
        {
            FillCostOfFullDepthAsphaltWorkSection(worksheet, currentCell, simulationYears, costAndLengthPerTreatmentPerYear, simulationTreatments);
            FillCostOfCompositeWork(worksheet, currentCell, simulationYears, costAndLengthPerTreatmentPerYear, simulationTreatments);
            FillCostOfConcreteWork(worksheet, currentCell, simulationYears, costAndLengthPerTreatmentPerYear, simulationTreatments);


            //var totalTreatmentGroups = FillTreatmentGroupTotalsSection(worksheet, currentCell, simulationYears, costAndLengthPerTreatmentGroupPerYear);
            FillTreatmentGroupTotalsSection(worksheet, currentCell, simulationYears, costAndLengthPerTreatmentGroupPerYear);

            var workTypeTotals = CalculateWorkTypeTotals(costAndLengthPerTreatmentPerYear, simulationTreatments);
            FillWorkTypeTotalsSection(worksheet, currentCell, simulationYears, workTypeTotals);


            //var workTypeTotalAggregated = new WorkTypeTotalAggregated
            //{
            //    WorkTypeTotalCulvert = workTypeTotalCulvert,
            //    WorkTypeTotalBridge = workTypeTotalBridge,
            //    WorkTypeTotalMPMS = workTypeTotalMPMS
            //};
            //var workTypeTotalRow = FillWorkTypeTotalsSection(worksheet, currentCell, simulationYears,
            //    yearlyBudgetAmount,
            //    workTypeTotalAggregated);
            var workTypeTotalRow = FillBudgetTotalSection(worksheet, currentCell, simulationYears);

            //FillRemainingBudgetSection(worksheet, simulationYears, currentCell, workTypeTotalRow);
            FillRemainingBudgetSection(worksheet, currentCell, simulationYears);
        }



        private List<(string Name, AssetCategory AssetType, TreatmentCategory Category)> GetAsphaltTreatments(List<(string Name, AssetCategory AssetType, TreatmentCategory Category)> allTreatments)
        {
            return allTreatments.Where(treatment => treatment.Name.ToLower().StartsWith((char)PavementTreatmentHelper.TreatmentGroupCategory.Bituminous)).ToList();
        }

        private List<(string Name, AssetCategory AssetType, TreatmentCategory Category)> GetConcreteTreatments(List<(string Name, AssetCategory AssetType, TreatmentCategory Category)> allTreatments)
        {
            return allTreatments.Where(treatment => treatment.Name.ToLower().StartsWith((char)PavementTreatmentHelper.TreatmentGroupCategory.Concrete)).ToList();
        }




        private Dictionary<TreatmentCategory, SortedDictionary<int, decimal>> FillCostOfFullDepthAsphaltWorkSection(
            ExcelWorksheet worksheet,
            CurrentCell currentCell,
            List<int> simulationYears,
            Dictionary<int, Dictionary<string, (decimal treatmentCost, int count)>> costAndCountPerTreatmentPerYear,
            List<(string Name, AssetCategory AssetType, TreatmentCategory Category)> simulationTreatments
            )
        {
            var headerRange = new Range(currentCell.Row, currentCell.Row + 1);
            _pavementWorkSummaryCommon.AddHeaders(worksheet, currentCell, simulationYears, "Cost of PAMS Full Depth Asphalt Work", "PAMS Full Depth Asphalt Work");

            var asphaltTreatments = GetAsphaltTreatments(simulationTreatments);

            var workTypeTotalFullDepthAsphalt = AddCostsOfFullDepthAsphaltWork(worksheet, simulationYears, currentCell, costAndCountPerTreatmentPerYear, asphaltTreatments);

            return workTypeTotalFullDepthAsphalt;
        }

        private Dictionary<TreatmentCategory, SortedDictionary<int, decimal>> AddCostsOfFullDepthAsphaltWork(
            ExcelWorksheet worksheet,
            List<int> simulationYears,
            CurrentCell currentCell,
            Dictionary<int, Dictionary<string, (decimal treatmentCost, int count)>> costAndCountPerTreatmentPerYear,
            List<(string Name, AssetCategory AssetType, TreatmentCategory Category)> simulationTreatments
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
                    //if (treatment.AssetType == AssetCategory.Culvert &&
                    //    !treatment.Name.Contains(BAMSConstants.NoTreatment, StringComparison.OrdinalIgnoreCase))
                    //{
                        yearlyValues.Value.TryGetValue(treatment.Name, out var asphaltCost);
                        worksheet.Cells[row++, column].Value = asphaltCost.treatmentCost;
                        asphaltTotalCost += asphaltCost.treatmentCost;
                        cost = asphaltCost.treatmentCost;

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
                    //}
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



        private void FillWorkTypeTotalFullDepthAsphalt(
            Dictionary<TreatmentCategory,
            SortedDictionary<int, decimal>> workTypeTotalFullDepthAsphalt,
            TreatmentCategory category,
            List<int> simulationYears,
            int currYear,
            decimal treatmentCost)
        {
            if (!workTypeTotalFullDepthAsphalt.ContainsKey(category))
            {
                workTypeTotalFullDepthAsphalt.Add(category, new SortedDictionary<int, decimal>());
                foreach (var year in simulationYears)
                {
                    workTypeTotalFullDepthAsphalt[category].Add(year, 0);
                }
                workTypeTotalFullDepthAsphalt[category][currYear] += treatmentCost;
            }
            else
            {
                workTypeTotalFullDepthAsphalt[category][currYear] += treatmentCost;
            }
        }
 

    private Dictionary<TreatmentCategory, SortedDictionary<int, decimal>> FillCostOfCompositeWork(
            ExcelWorksheet worksheet,
            CurrentCell currentCell,
            List<int> simulationYears,
            Dictionary<int, Dictionary<string, (decimal treatmentCost, int count)>> costAndCountPerTreatmentPerYear,
            List<(string Name, AssetCategory AssetType, TreatmentCategory Category)> simulationTreatments
            )
        {
            var headerRange = new Range(currentCell.Row, currentCell.Row + 1);
            _pavementWorkSummaryCommon.AddHeaders(worksheet, currentCell, simulationYears, "Cost of PAMS Composite Work", "PAMS Composite Work");

            var asphaltTreatments = GetAsphaltTreatments(simulationTreatments);

            var workTypeComposite = AddCostsOfCompositeWork(worksheet, simulationYears, currentCell, costAndCountPerTreatmentPerYear, asphaltTreatments);

            return workTypeComposite;
        }



        private Dictionary<TreatmentCategory, SortedDictionary<int, decimal>> AddCostsOfCompositeWork(
            ExcelWorksheet worksheet,
            List<int> simulationYears,
            CurrentCell currentCell,
            Dictionary<int, Dictionary<string, (decimal treatmentCost, int count)>> costAndCountPerTreatmentPerYear,
            List<(string Name, AssetCategory AssetType, TreatmentCategory Category)>
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
                    //if (treatment.AssetType == AssetCategory.Culvert &&
                    //    !treatment.Name.Contains(BAMSConstants.NoTreatment, StringComparison.OrdinalIgnoreCase))
                    //{
                    yearlyValues.Value.TryGetValue(treatment.Name, out var CompositeCost);
                    worksheet.Cells[row++, column].Value = CompositeCost.treatmentCost;
                    CompositeTotalCost += CompositeCost.treatmentCost;
                    cost = CompositeCost.treatmentCost;

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



        private void FillWorkTypeTotalComposite(
            Dictionary<TreatmentCategory,
            SortedDictionary<int, decimal>> workTypeTotalComposite,
            TreatmentCategory category,
            List<int> simulationYears,
            int currYear,
            decimal treatmentCost)
        {
            if (!workTypeTotalComposite.ContainsKey(category))
            {
                workTypeTotalComposite.Add(category, new SortedDictionary<int, decimal>());
                foreach (var year in simulationYears)
                {
                    workTypeTotalComposite[category].Add(year, 0);
                }
                workTypeTotalComposite[category][currYear] += treatmentCost;
            }
            else
            {
                workTypeTotalComposite[category][currYear] += treatmentCost;
            }
        }

        private Dictionary<TreatmentCategory, SortedDictionary<int, decimal>> FillCostOfConcreteWork(
            ExcelWorksheet worksheet,
            CurrentCell currentCell,
            List<int> simulationYears,
            Dictionary<int, Dictionary<string, (decimal treatmentCost, int count)>> costAndCountPerTreatmentPerYear,
            List<(string Name, AssetCategory AssetType, TreatmentCategory Category)> simulationTreatments
            )
        {
            var headerRange = new Range(currentCell.Row, currentCell.Row + 1);
            _pavementWorkSummaryCommon.AddHeaders(worksheet, currentCell, simulationYears, "Cost of PAMS Concrete Work", "PAMS Concrete Work");

            var concreteTreatments = GetConcreteTreatments(simulationTreatments);

            var workTypeConcrete = AddCostsOfConcreteWork(worksheet, simulationYears, currentCell, costAndCountPerTreatmentPerYear, concreteTreatments);

            return workTypeConcrete;
        }




        private Dictionary<TreatmentCategory, SortedDictionary<int, decimal>> AddCostsOfConcreteWork(
            ExcelWorksheet worksheet,
            List<int> simulationYears,
            CurrentCell currentCell,
            Dictionary<int, Dictionary<string, (decimal treatmentCost, int count)>> costAndCountPerTreatmentPerYear,
            List<(string Name, AssetCategory AssetType, TreatmentCategory Category)> simulationTreatments
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
                    //if (treatment.AssetType == AssetCategory.Culvert &&
                    //    !treatment.Name.Contains(BAMSConstants.NoTreatment, StringComparison.OrdinalIgnoreCase))
                    //{
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



        private void FillWorkTypeTotalConcrete(
            Dictionary<TreatmentCategory,
            SortedDictionary<int, decimal>> workTypeTotalConcrete,
            TreatmentCategory category,
            List<int> simulationYears,
            int currYear,
            decimal treatmentCost)
        {
            if (!workTypeTotalConcrete.ContainsKey(category))
            {
                workTypeTotalConcrete.Add(category, new SortedDictionary<int, decimal>());
                foreach (var year in simulationYears)
                {
                    workTypeTotalConcrete[category].Add(year, 0);
                }
                workTypeTotalConcrete[category][currYear] += treatmentCost;
            }
            else
            {
                workTypeTotalConcrete[category][currYear] += treatmentCost;
            }
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
            //var workTypeConcrete = new Dictionary<TreatmentCategory, SortedDictionary<int, decimal>>();
            if (simulationYears.Count <= 0)
            {
                return;// workTypeConcrete;
            }

            var headerRange = new Range(currentCell.Row, currentCell.Row + 1);
            _pavementWorkSummaryCommon.AddHeaders(worksheet, currentCell, simulationYears, "Total Budget", "PAMS Treatment Groups Totals");

            AddTreatmentGroupTotalDetails(worksheet, currentCell, costAndLengthPerTreatmentGroupPerYear, PavementTreatmentHelper.TreatmentGroupCategory.Bituminous);
            AddTreatmentGroupTotalDetails(worksheet, currentCell, costAndLengthPerTreatmentGroupPerYear, PavementTreatmentHelper.TreatmentGroupCategory.Concrete);
        }


        private Dictionary<TreatmentCategory, SortedDictionary<int, decimal>> FillWorkTypeTotalsSection(ExcelWorksheet worksheet, CurrentCell currentCell,
            List<int> simulationYears,
            Dictionary<TreatmentCategory, SortedDictionary<int, (decimal treatmentCost, int length)>> workTypeTotals
            )
        {
            var workTypesForReport = new List<TreatmentCategory> { TreatmentCategory.Maintenance, TreatmentCategory.Preservation, TreatmentCategory.Rehabilitation, TreatmentCategory.Replacement };
            var headerRange = new Range(currentCell.Row, currentCell.Row + 1);
            _pavementWorkSummaryCommon.AddHeaders(worksheet, currentCell, simulationYears, "Total Budget", "Work Type Totals", "Total (all years)");

            int startRow, startColumn, row, column;
            _pavementWorkSummaryCommon.SetRowColumns(currentCell, out startRow, out startColumn, out row, out column);

            var workTypeTitles = workTypesForReport.Select(tc => tc.ToSpreadsheetString()).ToList();

            _pavementWorkSummaryCommon.SetPavementTreatmentGroupsExcelString(worksheet, workTypeTitles, ref row, ref column);

            column++;
            var fromColumn = column + 1;

            row = startRow;
            foreach (var workType in workTypesForReport)
            {
                decimal rowTotal = 0;
                column = fromColumn;

                if (workTypeTotals.TryGetValue(workType, out var workTypeTotal))
                {
                    foreach(var year in simulationYears)
                    {
                        if (workTypeTotal.TryGetValue(year, out var costAndLength))
                        {
                            worksheet.Cells[row, column].Value = costAndLength.treatmentCost;
                            rowTotal += costAndLength.treatmentCost;
                        }
                        else
                        {
                            worksheet.Cells[row, column].Value = 0.0;
                        }
                        column = ++column;
                    }
                    worksheet.Cells[row, column].Value = rowTotal;
                }
                row++;
            }
            column = fromColumn + simulationYears.Count;
            row--;

            ExcelHelper.ApplyBorder(worksheet.Cells[startRow, startColumn, row, column]);
            ExcelHelper.SetCustomFormat(worksheet.Cells[startRow, fromColumn, row, column], ExcelHelperCellFormat.NegativeCurrency);

            ExcelHelper.ApplyColor(worksheet.Cells[startRow, fromColumn, row, column - 1], Color.FromArgb(84, 130, 53));
            ExcelHelper.SetTextColor(worksheet.Cells[startRow, fromColumn, row, column - 1], Color.White);

            ExcelHelper.ApplyColor(worksheet.Cells[startRow, fromColumn, row, column - 1], Color.FromArgb(217, 217, 217));

            //var workTypeComposite = AddCostsOfCompositeWork(worksheet, simulationYears, currentCell, costPerTreatmentPerYear, simulationTreatments);
            worksheet.Cells[startRow, column + 2].Style.Numberformat.Format = "#0.00%";
            worksheet.Cells[startRow, column + 3].Value = "Percentage spent on PRESERVATION";
            //return workTypeTotalCulvert;

            //var workTypeComposite = AddCostsOfCompositeWork(worksheet, simulationYears, currentCell, costPerTreatmentPerYear, simulationTreatments);
            worksheet.Cells[startRow + 1, column + 2].Style.Numberformat.Format = "#0.00%";
            worksheet.Cells[startRow, column + 3].Value = "Percentage spent on REHABILITATION";
            //return workTypeTotalCulvert;

            //var workTypeComposite = AddCostsOfCompositeWork(worksheet, simulationYears, currentCell, costPerTreatmentPerYear, simulationTreatments);
            worksheet.Cells[startRow, column + 2].Style.Numberformat.Format = "#0.00%";
            worksheet.Cells[startRow, column + 3].Value = "Percentage spent on REPLACEMENT";
            //return workTypeTotalCulvert;

            _pavementWorkSummaryCommon.UpdateCurrentCell(currentCell, ++row, column);


            return null;
        }

        private Dictionary<TreatmentCategory, SortedDictionary<int, decimal>> FillBudgetTotalSection(ExcelWorksheet worksheet, CurrentCell currentCell,
            List<int> simulationYears //,
                                      //Dictionary<int, Dictionary<string, (decimal treatmentCost, int bridgeCount)>> costPerTreatmentPerYear,
                                      //List<(string Name, AssetCategory AssetType, TreatmentCategory Category)> simulationTreatments
            )
        {
            var headerRange = new Range(currentCell.Row, currentCell.Row + 1);
            _pavementWorkSummaryCommon.AddHeaders(worksheet, currentCell, simulationYears, "", "Budget Total");
            //var workTypeComposite = AddCostsOfCompositeWork(worksheet, simulationYears, currentCell, costPerTreatmentPerYear, simulationTreatments);

            //return workTypeTotalCulvert;
            return null;
        }


        private void FillRemainingBudgetSection(ExcelWorksheet worksheet, CurrentCell currentCell, List<int> simulationYears)
             //int budgetTotalRow)
        {
            _pavementWorkSummaryCommon.AddHeaders(worksheet, currentCell, simulationYears, "Budget Analysis", "");

            worksheet.Cells[currentCell.Row, simulationYears.Count + 3].Value = "Total Remaining Budget (all years)";
            ExcelHelper.ApplyStyle(worksheet.Cells[currentCell.Row, simulationYears.Count + 3]);
            ExcelHelper.ApplyBorder(worksheet.Cells[currentCell.Row, simulationYears.Count + 3]);

            AddDetailsForRemainingBudget(worksheet, simulationYears, currentCell);//, budgetTotalRow);
        }

        private void AddDetailsForRemainingBudget(ExcelWorksheet worksheet, List<int> simulationYears, CurrentCell currentCell //,
                                                                                                                               //int budgetTotalRow)
             )
        {
            int startRow, startColumn, row, column;
            _pavementWorkSummaryCommon.SetRowColumns(currentCell, out startRow, out startColumn, out row, out column);
            worksheet.Cells[row++, column].Value = PAMSConstants.RemainingBudget;
            worksheet.Cells[row++, column].Value = PAMSConstants.PercentBudgetSpentPAMS;
            worksheet.Cells[row++, column].Value = PAMSConstants.PercentBudgetSpentMPMS;
            worksheet.Cells[row++, column].Value = PAMSConstants.PercentBudgetSpentSAP;
            column++;
            var fromColumn = column + 1;
            foreach (var year in simulationYears)
            {
                row = startRow;
                column = ++column;

                var totalSpent = 0.0;// Convert.ToDouble(worksheet.Cells[CulvertTotalRow, column].Value) +
                //    Convert.ToDouble(worksheet.Cells[BridgeTotalRow, column].Value) +
                //Convert.ToDouble(worksheet.Cells[CommittedTotalRow, column].Value);

                //worksheet.Cells[row, column].Value = Convert.ToDouble(worksheet.Cells[budgetTotalRow, column].Value) - totalSpent;
                //row++;

                //if (totalSpent != 0)
                //{
                //    worksheet.Cells[row, column].Formula = worksheet.Cells[CommittedTotalRow, column] + "/" + totalSpent;
                //}
                //else
                //{
                //    worksheet.Cells[row, column].Value = 0;
                //}
                row++;

                worksheet.Cells[row, column].Formula = 1 + "-" + worksheet.Cells[row - 1, column];
            }
            worksheet.Cells[startRow, column + 1].Formula = "SUM(" + worksheet.Cells[startRow, fromColumn, startRow, column] + ")";
            if (_workSummaryModel.AnnualizedAmount != 0)
            {
                worksheet.Cells[startRow, column + 2].Formula = worksheet.Cells[startRow, column + 1] + "/"
                    + (_workSummaryModel.AnnualizedAmount * simulationYears.Count);
            }

            worksheet.Cells[startRow, column + 2].Style.Numberformat.Format = "#0.00%";
            worksheet.Cells[startRow, column + 3].Value = "Percentage of Total Budget that was Unspent";

            ExcelHelper.ApplyBorder(worksheet.Cells[startRow, startColumn, row, column + 1]);

            ExcelHelper.SetCustomFormat(worksheet.Cells[row - 2, fromColumn, row - 2, column + 1], ExcelHelperCellFormat.NegativeCurrency);
            ExcelHelper.SetCustomFormat(worksheet.Cells[row - 1, fromColumn, row, column], ExcelHelperCellFormat.Percentage);

            ExcelHelper.ApplyColor(worksheet.Cells[startRow, fromColumn, startRow, column], Color.Red);
            ExcelHelper.ApplyColor(worksheet.Cells[startRow + 1, fromColumn, row, column], Color.FromArgb(248, 203, 173));
            ExcelHelper.ApplyColor(worksheet.Cells[row + 3, startColumn, row + 3, column], Color.DimGray);

            _pavementWorkSummaryCommon.UpdateCurrentCell(currentCell, row + 4, column);

        }
    }

}
