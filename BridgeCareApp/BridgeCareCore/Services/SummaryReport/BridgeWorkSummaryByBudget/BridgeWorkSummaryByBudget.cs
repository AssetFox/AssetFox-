using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using AppliedResearchAssociates.iAM.Analysis;
using AppliedResearchAssociates.iAM.Domains;
using BridgeCareCore.Interfaces.SummaryReport;
using BridgeCareCore.Models.SummaryReport;
using BridgeCareCore.Services.SummaryReport.BridgeWorkSummary;
using OfficeOpenXml;

namespace BridgeCareCore.Services.SummaryReport.BridgeWorkSummaryByBudget
{
    public class BridgeWorkSummaryByBudget : IBridgeWorkSummaryByBudget
    {
        private readonly IExcelHelper _excelHelper;
        private readonly BridgeWorkSummaryCommon _bridgeWorkSummaryCommon;
        private readonly CulvertCost _culvertCost;
        private readonly BridgeWorkCost _bridgeWorkCost;

        public BridgeWorkSummaryByBudget(IExcelHelper excelHelper, BridgeWorkSummaryCommon bridgeWorkSummaryCommon,
            CulvertCost culvertCost, BridgeWorkCost bridgeWorkCost)
        {
            _excelHelper = excelHelper ?? throw new ArgumentNullException(nameof(excelHelper));
            _bridgeWorkSummaryCommon = bridgeWorkSummaryCommon ?? throw new ArgumentNullException(nameof(bridgeWorkSummaryCommon));
            _culvertCost = culvertCost ?? throw new ArgumentNullException(nameof(culvertCost));
            _bridgeWorkCost = bridgeWorkCost ?? throw new ArgumentNullException(nameof(bridgeWorkCost));
        }
        public void Fill(ExcelWorksheet worksheet, SimulationOutput reportOutputData,
            List<int> simulationYears, Dictionary<string, Budget> yearlyBudgetAmount)
        {
            var startYear = simulationYears[0];
            var currentCell = new CurrentCell { Row = 1, Column = 1 };

            // setting up model to store data. This will be used to fill up Bridge Work Summary By Budget TAB
            var workSummaryByBudgetData = new List<WorkSummaryByBudgetModel>();
            foreach (var item in reportOutputData.Years[0].Budgets)
            {
                workSummaryByBudgetData.Add(new WorkSummaryByBudgetModel
                {
                    Budget = item.BudgetName,
                    YearlyData = new List<YearsData>()
                });
            }

            foreach (var summaryData in workSummaryByBudgetData)
            {
                foreach (var yearData in reportOutputData.Years)
                {
                    foreach (var section in yearData.Sections)
                    {
                        if(section.TreatmentCause != TreatmentCause.NoSelection && section.TreatmentOptions.Count > 0)
                        {
                            var treatmentConsideration = section.TreatmentConsiderations.
                                Where(_ => _.TreatmentName == section.AppliedTreatment).FirstOrDefault();
                            var budgetAmount = (double)treatmentConsideration.BudgetUsages.Where(b => b.BudgetName == summaryData.Budget)
                                .Sum(_ => _.CoveredCost);

                            summaryData.YearlyData.Add(new YearsData
                            {
                                Year = yearData.Year,
                                Treatment = section.AppliedTreatment,
                                Amount = budgetAmount
                            });
                        }
                    }
                }
            }
            // Model setup complete. [TODO]: Make it efficient


            foreach (var summaryData in workSummaryByBudgetData)
            {
                //Filtering treatments for the given budget
                var totalCostPerYear = summaryData.YearlyData.Sum(_ => _.Amount);
                if(totalCostPerYear == 0)
                {
                    continue;
                }
                var costForCulvertBudget = summaryData.YearlyData
                                             .FindAll(_ => _.Treatment.Contains("culvert", StringComparison.OrdinalIgnoreCase));

                var costForBridgeBudgets = summaryData.YearlyData
                                             .FindAll(_ => !_.Treatment.Contains("culvert", StringComparison.OrdinalIgnoreCase));

                if (costForCulvertBudget.Count == 0 && costForBridgeBudgets.Count == 0)
                {
                    continue;
                }

                var totalBudgetPerYearForCulvert = new Dictionary<int, double>();
                var totalBudgetPerYearForBridgeWork = new Dictionary<int, double>();
                var totalBudgetPerYearForMPMS = new Dictionary<int, double>();

                var totalSpent = new List<(int year, double amount)>();

                // Filling up the total, "culvert" and "Bridge work" costs
                foreach (var year in simulationYears)
                {
                    var yearlyBudget = costForCulvertBudget.FindAll(_ => _.Year == year);
                    var culvertAmountSum = yearlyBudget.Sum(s => s.Amount);
                    totalBudgetPerYearForCulvert.Add(year, culvertAmountSum);

                    yearlyBudget.Clear();

                    yearlyBudget = costForBridgeBudgets.FindAll(_ => _.Year == year);
                    var budgetAmountSum = yearlyBudget.Sum(s => s.Amount);
                    totalBudgetPerYearForBridgeWork.Add(year, budgetAmountSum);

                    yearlyBudget.Clear();

                    totalSpent.Add((year, culvertAmountSum + budgetAmountSum));
                }

                currentCell.Column = 1;
                currentCell.Row += 2;
                worksheet.Cells[currentCell.Row, currentCell.Column].Value = summaryData.Budget;
                _excelHelper.MergeCells(worksheet, currentCell.Row, currentCell.Column, currentCell.Row, simulationYears.Count);
                _excelHelper.ApplyColor(worksheet.Cells[currentCell.Row, currentCell.Column, currentCell.Row, simulationYears.Count + 2], Color.Gray);
                currentCell.Row += 1;

                _culvertCost.FillCostOfCulvert(worksheet, currentCell, costForCulvertBudget, totalBudgetPerYearForCulvert, simulationYears);

                _bridgeWorkCost.FillCostOfBridgeWork(worksheet, currentCell, simulationYears, costForBridgeBudgets, totalBudgetPerYearForBridgeWork);

                currentCell.Row += 1;
                _bridgeWorkSummaryCommon.AddHeaders(worksheet, currentCell, simulationYears, "Total Budget", "Totals");
                currentCell.Row += 1;
                currentCell.Column = 1;
                var startOfTotalBudget = currentCell.Row;
                worksheet.Cells[currentCell.Row, currentCell.Column].Value = Properties.Resources.TotalSpent;

                foreach (var spentAmount in totalSpent)
                {
                    var cellFortotalSpentAmount = spentAmount.year - startYear;
                    worksheet.Cells[currentCell.Row, currentCell.Column + cellFortotalSpentAmount + 2].Value = spentAmount.amount;
                }
                _excelHelper.ApplyColor(worksheet.Cells[startOfTotalBudget, currentCell.Column + 2, currentCell.Row, simulationYears.Count + 2],
                    Color.FromArgb(84, 130, 53));
                _excelHelper.SetTextColor(worksheet.Cells[startOfTotalBudget, currentCell.Column + 2, currentCell.Row, simulationYears.Count + 2], Color.White);

                //[TODO: Rest of the code is dummy to fill up Total Bridge care budget. The actual data will come from an input obj]
                //var totalBridgeCareBudget = yearlyBudgetModels.FindAll(_ => _.BudgetName.Equals(budget.Replace("'", "")))
                //    .OrderBy(_ => _.Year).ToList();
                currentCell.Row += 2;
                currentCell.Column = 1;
                worksheet.Cells[currentCell.Row, currentCell.Column].Value = Properties.Resources.TotalBridgeCareBudget;

                var budgetDetails = yearlyBudgetAmount[summaryData.Budget];
                var yearTracker = 0;
                foreach (var item in budgetDetails.YearlyAmounts)
                {
                    var cellFortotalBudget = yearTracker;
                    worksheet.Cells[currentCell.Row, currentCell.Column + cellFortotalBudget + 2].Value = item.Value;
                    yearTracker++;
                }
                _excelHelper.ApplyBorder(worksheet.Cells[startOfTotalBudget, currentCell.Column, currentCell.Row, simulationYears.Count + 2]);
                _excelHelper.SetCustomFormat(worksheet.Cells[startOfTotalBudget, currentCell.Column + 2, currentCell.Row, simulationYears.Count + 2], "NegativeCurrency");

                _excelHelper.ApplyColor(worksheet.Cells[currentCell.Row, currentCell.Column + 2, currentCell.Row, simulationYears.Count + 2],
                    Color.FromArgb(84, 130, 53));
                _excelHelper.SetTextColor(worksheet.Cells[currentCell.Row, currentCell.Column + 2, currentCell.Row, simulationYears.Count + 2], Color.White);

                currentCell.Row += 1;
                _bridgeWorkSummaryCommon.AddHeaders(worksheet, currentCell, simulationYears, "Budget Analysis", "");
                currentCell.Row += 1;
                currentCell.Column = 1;
                worksheet.Cells[currentCell.Row, currentCell.Column].Value = Properties.Resources.RemainingBudget;
                worksheet.Cells[currentCell.Row + 1, currentCell.Column].Value = Properties.Resources.PercentBudgetSpentMPMS;
                worksheet.Cells[currentCell.Row + 2, currentCell.Column].Value = Properties.Resources.PercentBudgetSpentBAMS;

                yearTracker = 0;
                foreach (var budgetData in budgetDetails.YearlyAmounts)
                {
                    var perYearTotalSpent = totalSpent.Find(_ => _.year == startYear + yearTracker);
                    var cellFortotalBudget = yearTracker;
                    worksheet.Cells[currentCell.Row, currentCell.Column + cellFortotalBudget + 2].Value = budgetData.Value - (decimal)perYearTotalSpent.amount;

                    // Because we do not have committed project right now. The percentage will always be 0
                    worksheet.Cells[currentCell.Row + 1, currentCell.Column + cellFortotalBudget + 2].Value =  0;

                    worksheet.Cells[currentCell.Row + 2, currentCell.Column + cellFortotalBudget + 2].Value = 1 - 0;
                    yearTracker++;
                }

                _excelHelper.ApplyBorder(worksheet.Cells[currentCell.Row, currentCell.Column, currentCell.Row + 2, simulationYears.Count + 2]);

                _excelHelper.SetCustomFormat(worksheet.Cells[currentCell.Row + 1, currentCell.Column + 2,
                    currentCell.Row + 2, simulationYears.Count + 2], "Percentage");
                _excelHelper.SetCustomFormat(worksheet.Cells[currentCell.Row, currentCell.Column + 2,
                    currentCell.Row, simulationYears.Count + 2], "NegativeCurrency");

                _excelHelper.ApplyColor(worksheet.Cells[currentCell.Row, currentCell.Column + 2, currentCell.Row, simulationYears.Count + 2],
                    Color.Red);
                _excelHelper.ApplyColor(worksheet.Cells[currentCell.Row + 1, currentCell.Column + 2, currentCell.Row + 2, simulationYears.Count + 2], Color.FromArgb(248, 203, 173));
                currentCell.Row += 2;
            }
            worksheet.Cells.AutoFitColumns();
        }
    }
}
