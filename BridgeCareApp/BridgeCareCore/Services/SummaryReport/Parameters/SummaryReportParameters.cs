﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.Domains;
using BridgeCareCore.Interfaces.SummaryReport;
using BridgeCareCore.Models.SummaryReport;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace BridgeCareCore.Services.SummaryReport.Parameters
{
    public class SummaryReportParameters
    {
        private readonly IExcelHelper _excelHelper;
        private readonly UnitOfDataPersistenceWork _unitOfDataPersistenceWork;

        public SummaryReportParameters(IExcelHelper excelHelper, UnitOfDataPersistenceWork unitOfDataPersistenceWork)
        {
            _excelHelper = excelHelper ?? throw new ArgumentNullException(nameof(excelHelper));
            _unitOfDataPersistenceWork = unitOfDataPersistenceWork ?? throw new ArgumentNullException(nameof(unitOfDataPersistenceWork));
        }

        internal void Fill(ExcelWorksheet worksheet, int simulationYearsCount, ParametersModel parametersModel, Simulation simulation)
        {
            // Simulation Name format
            _excelHelper.MergeCells(worksheet, 1, 1, 1, 2);
            _excelHelper.MergeCells(worksheet, 1, 3, 1, 10);
            _excelHelper.ApplyColor(worksheet.Cells[1, 1, 1, 2], Color.Gray);

            worksheet.Cells["A1:B1"].Value = "Simulation Name";
            worksheet.Cells["C1:J1"].Value = simulation.Name;
            _excelHelper.ApplyColor(worksheet.Cells[1, 3, 1, 10], Color.FromArgb(142, 169, 219));
            _excelHelper.ApplyBorder(worksheet.Cells[1, 1, 1, 10]);
            // End of Simulation Name format

            // Simulation Comment
            _excelHelper.MergeCells(worksheet, 2, 1, 2, 2);
            _excelHelper.ApplyColor(worksheet.Cells[2, 1, 2, 2], Color.Gray);
            worksheet.Cells["A2:B2"].Value = "Simulation Comment";

            _excelHelper.MergeCells(worksheet, 2, 3, 2, 10);
            worksheet.Cells["C2:J2"].Value = simulation.AnalysisMethod.Description;
            _excelHelper.ApplyBorder(worksheet.Cells[2, 1, 2, 10]);

            FillData(worksheet, parametersModel, simulation.LastRun);

            FillSimulationDetails(worksheet, simulationYearsCount, simulation);
            FillAnalysisDetails(worksheet, simulation);
            FillJurisdictionCriteria(worksheet, simulation);
            var rowNum = FillPriorities(worksheet, simulation);
            FillBudgetSplitCriteria(worksheet, rowNum, simulation);
            FillInvestmentAndBudgetCriteria(worksheet, simulation);
            worksheet.Cells.AutoFitColumns(50);
        }

        #region

        private void FillData(ExcelWorksheet worksheet, ParametersModel parametersModel, DateTime lastRun)
        {
            var bpnValueCellTracker = new Dictionary<string, string>();
            var statusValueCellTracker = new Dictionary<string, string>();

            worksheet.Cells["A3"].Value = "BridgeCare Rules Creator:";
            worksheet.Cells["B3"].Value = "Central Office";
            worksheet.Cells["A4"].Value = "BridgeCare Rules Date:";
            worksheet.Cells["B4"].Value = "10/25/2019";
            _excelHelper.ApplyBorder(worksheet.Cells[3, 1, 4, 2]);

            worksheet.Cells["D3"].Value = "Simulation Last Run:";
            worksheet.Cells["E3"].Value = "10/25/2019";
            _excelHelper.ApplyBorder(worksheet.Cells[3, 4, 3, 5]);

            _excelHelper.MergeCells(worksheet, 6, 1, 6, 2);
            _excelHelper.ApplyColor(worksheet.Cells[6, 1, 6, 2], Color.Gray);
            worksheet.Cells["A6:B6"].Value = "NHS";
            worksheet.Cells["A7"].Value = "NHS";
            worksheet.Cells["A8"].Value = "Non-NHS";

            worksheet.Cells["B7"].Value = parametersModel.nHSModel.NHS;
            worksheet.Cells["B8"].Value = parametersModel.nHSModel.NonNHS;
            _excelHelper.ApplyBorder(worksheet.Cells[6, 1, 8, 2]);

            _excelHelper.MergeCells(worksheet, 10, 1, 10, 2);
            _excelHelper.ApplyColor(worksheet.Cells[10, 1, 10, 2], Color.Gray);
            worksheet.Cells["A10:B10"].Value = "6A19 BPN";
            worksheet.Cells["A11"].Value = "1";
            worksheet.Cells["A12"].Value = "2";
            worksheet.Cells["A13"].Value = "3";
            worksheet.Cells["A14"].Value = "4";
            worksheet.Cells["A15"].Value = "H";

            bpnValueCellTracker.Add("1", "B11");
            bpnValueCellTracker.Add("2", "B12");
            bpnValueCellTracker.Add("3", "B13");
            bpnValueCellTracker.Add("4", "B14");
            bpnValueCellTracker.Add("H", "B15");

            worksheet.Cells["A16"].Value = "L";
            worksheet.Cells["A17"].Value = "T";
            worksheet.Cells["A18"].Value = "D";
            worksheet.Cells["A19"].Value = "N";
            worksheet.Cells["A20"].Value = "Blank";

            bpnValueCellTracker.Add("L", "B16");
            bpnValueCellTracker.Add("T", "B17");
            bpnValueCellTracker.Add("D", "B18");
            bpnValueCellTracker.Add("N", "B19");
            bpnValueCellTracker.Add("Blank", "B20");

            foreach (var item in bpnValueCellTracker)
            {
                if (parametersModel.BPNValues.Contains(item.Key))
                {
                    worksheet.Cells[item.Value].Value = "Y";
                }
                else
                {
                    worksheet.Cells[item.Value].Value = "N";
                }
            }
            //worksheet.Cells["B16"].Value = "Y";
            //worksheet.Cells["B17"].Value = "Y";
            //worksheet.Cells["B18"].Value = "Y";
            //worksheet.Cells["B19"].Value = "Y";
            //worksheet.Cells["B20"].Value = "Y";
            _excelHelper.ApplyBorder(worksheet.Cells[10, 1, 20, 2]);

            _excelHelper.MergeCells(worksheet, 23, 1, 23, 2);
            _excelHelper.ApplyColor(worksheet.Cells[23, 1, 23, 2], Color.Gray);
            worksheet.Cells["A23:B23"].Value = "Bridge Length";
            worksheet.Cells["A24"].Value = "8-20";
            worksheet.Cells["A25"].Value = "NBIS Length";

            worksheet.Cells["B24"].Value = parametersModel.LengthBetween8and20;
            worksheet.Cells["B25"].Value = parametersModel.LengthGreaterThan20;

            _excelHelper.MergeCells(worksheet, 27, 1, 27, 2);
            _excelHelper.ApplyColor(worksheet.Cells[27, 1, 27, 2], Color.Gray);
            worksheet.Cells["A27:B27"].Value = "Status";
            worksheet.Cells["A28"].Value = "Open";
            worksheet.Cells["A29"].Value = "Closed";
            worksheet.Cells["A30"].Value = "P3";
            worksheet.Cells["A31"].Value = "Posted";

            statusValueCellTracker.Add("open", "B28");
            statusValueCellTracker.Add("closed", "B29");
            statusValueCellTracker.Add("posted", "B31");

            foreach (var item in statusValueCellTracker)
            {
                if (parametersModel.Status.Contains(item.Key))
                {
                    worksheet.Cells[item.Value].Value = "Y";
                }
                else
                {
                    worksheet.Cells[item.Value].Value = "N";
                }
            }
            worksheet.Cells["B30"].Value = parametersModel.P3 > 0 ? "Y" : "N";
            _excelHelper.ApplyBorder(worksheet.Cells[23, 1, 31, 2]);

            _excelHelper.MergeCells(worksheet, 14, 4, 14, 6);
            _excelHelper.ApplyColor(worksheet.Cells[14, 4, 14, 6], Color.Gray);
            worksheet.Cells["D14:F14"].Value = "5A21 Owner";

            _excelHelper.MergeCells(worksheet, 15, 4, 15, 5, false);
            worksheet.Cells["D15:E15"].Value = "01 - State Highway Agency";
            _excelHelper.MergeCells(worksheet, 16, 4, 16, 5, false);
            worksheet.Cells["D16:E16"].Value = "02 - County Highway Agency";
            _excelHelper.MergeCells(worksheet, 17, 4, 17, 5, false);
            worksheet.Cells["D17:E17"].Value = "03 - Town or Township Highway Agency";
            _excelHelper.MergeCells(worksheet, 18, 4, 18, 5, false);
            worksheet.Cells["D18:E18"].Value = "04 - City, Municipal Highway Agency or Borough";
            _excelHelper.MergeCells(worksheet, 19, 4, 19, 5, false);
            worksheet.Cells["D19:E19"].Value = "11 - State Park, Forest or Reservation Agency";
            _excelHelper.MergeCells(worksheet, 20, 4, 20, 5, false);
            worksheet.Cells["D20:E20"].Value = "12 - Local Park, Forest or Reservation Agency";
            _excelHelper.MergeCells(worksheet, 21, 4, 21, 5, false);
            worksheet.Cells["D21:E21"].Value = "21 - Other State Agency";
            _excelHelper.MergeCells(worksheet, 22, 4, 22, 5, false);
            worksheet.Cells["D22:E22"].Value = "25 - Other local Agency";
            _excelHelper.MergeCells(worksheet, 23, 4, 23, 5, false);
            worksheet.Cells["D23:E23"].Value = "26 - Private (Other than Railroad)";
            _excelHelper.MergeCells(worksheet, 24, 4, 24, 5, false);
            worksheet.Cells["D24:E24"].Value = "27 - Railroad";
            _excelHelper.MergeCells(worksheet, 25, 4, 25, 5, false);
            worksheet.Cells["D25:E25"].Value = "31 - State Toll Authority";
            _excelHelper.MergeCells(worksheet, 26, 4, 26, 5, false);
            worksheet.Cells["D26:E26"].Value = "32 - Local Toll Authority";
            _excelHelper.MergeCells(worksheet, 27, 4, 27, 5, false);
            worksheet.Cells["D27:E27"].Value = "60 - Other Federal Agencies";
            _excelHelper.MergeCells(worksheet, 28, 4, 28, 5, false);
            worksheet.Cells["D28:E28"].Value = "62 - Bureau of Indian Affairs";
            _excelHelper.MergeCells(worksheet, 29, 4, 29, 5, false);
            worksheet.Cells["D29:E29"].Value = "64 - U.S. Forest Service";
            _excelHelper.MergeCells(worksheet, 30, 4, 30, 5, false);
            worksheet.Cells["D30:E30"].Value = "66 - National Park Service";
            _excelHelper.MergeCells(worksheet, 31, 4, 31, 5, false);
            worksheet.Cells["D31:E31"].Value = "68 - Bureau of Land Management";
            _excelHelper.MergeCells(worksheet, 32, 4, 32, 5, false);
            worksheet.Cells["D32:E32"].Value = "69 - Bureau of Reclamation";
            _excelHelper.MergeCells(worksheet, 33, 4, 33, 5, false);
            worksheet.Cells["D33:E33"].Value = "70 - Military Reservation Corps Engineers";
            _excelHelper.MergeCells(worksheet, 34, 4, 34, 5, false);
            worksheet.Cells["D34:E34"].Value = "80 - Unknown";
            _excelHelper.MergeCells(worksheet, 35, 4, 35, 5, false);
            worksheet.Cells["D35:E35"].Value = "XX - Demolished/Replaced";

            var ownerCodeValueTracker = new Dictionary<string, string> { {"01", "F15"},{"02", "F16"},{"03","F17"},{"04","F18"},{"11","F19"},{"12","F20"},
                {"21","F21"},{"25","F22"},{"26","F23"},{"27","F24"},{"31","F25"},{"32","F26"},{"60","F27"},{"62","F28"},{"64","F29"},{"66","F30"},{"68","F31"},
                {"69","F32"},{"70","F33"},{"80","F34"},{"XX","F35"} };

            foreach (var item in ownerCodeValueTracker)
            {
                if (parametersModel.OwnerCode.Contains(item.Key))
                {
                    worksheet.Cells[item.Value].Value = "Y";
                }
                else
                {
                    worksheet.Cells[item.Value].Value = "N";
                }
            }
            _excelHelper.ApplyBorder(worksheet.Cells[14, 4, 35, 6]);

            _excelHelper.MergeCells(worksheet, 14, 8, 14, 10);
            _excelHelper.ApplyColor(worksheet.Cells[14, 8, 14, 10], Color.Gray);
            worksheet.Cells["H14:J14"].Value = "5C22 Functional Class";
            _excelHelper.MergeCells(worksheet, 15, 8, 15, 10);
            _excelHelper.ApplyColor(worksheet.Cells[15, 8, 15, 10], Color.DimGray);
            worksheet.Cells["H15:J15"].Value = "Rural";

            _excelHelper.MergeCells(worksheet, 16, 8, 16, 9, false);
            worksheet.Cells["H16:I16"].Value = "01 - Principal Arterial - Interstate";
            _excelHelper.MergeCells(worksheet, 17, 8, 17, 9, false);
            worksheet.Cells["H17:I17"].Value = "02 - Principal Arterial - Other";
            _excelHelper.MergeCells(worksheet, 18, 8, 18, 9, false);
            worksheet.Cells["H18:I18"].Value = "06 - Minor Arterial";
            _excelHelper.MergeCells(worksheet, 19, 8, 19, 9, false);
            worksheet.Cells["H19:I19"].Value = "07 - Major Collector";
            _excelHelper.MergeCells(worksheet, 20, 8, 20, 9, false);
            worksheet.Cells["H20:I20"].Value = "08 - Minor Collector";
            _excelHelper.MergeCells(worksheet, 21, 8, 21, 9, false);
            worksheet.Cells["H21:I21"].Value = "09 - Local";
            _excelHelper.MergeCells(worksheet, 22, 8, 22, 9, false);
            worksheet.Cells["H22:I22"].Value = "NN - Other";

            var functionalClassValueTracker = new Dictionary<string, string> { {"01","J16"},{"02","J17"},{"06","J18"},{"07","J19"},{"08","J20"},{"09","J21"},
                {"NN","J22"},{"11","J24"},{"12","J25"},{"14","J26"},{"16","J27"},{"17","J28"},{"19","J29"},{"99","J31"} };

            _excelHelper.MergeCells(worksheet, 23, 8, 23, 10);
            _excelHelper.ApplyColor(worksheet.Cells[23, 8, 23, 10], Color.DimGray);
            worksheet.Cells["H23:J23"].Value = "Urban";

            _excelHelper.MergeCells(worksheet, 24, 8, 24, 9, false);
            worksheet.Cells["H24:J24"].Value = "11 - Principal Arterial - Interstate";
            _excelHelper.MergeCells(worksheet, 25, 8, 25, 9, false);
            worksheet.Cells["H25:I25"].Value = "12 - Principal Arterial - Other Freeway & Expressways";
            _excelHelper.MergeCells(worksheet, 26, 8, 26, 9, false);
            worksheet.Cells["H26:I26"].Value = "14 - Other Principal Arterial";
            _excelHelper.MergeCells(worksheet, 27, 8, 27, 9, false);
            worksheet.Cells["H27:I27"].Value = "16 - Minor Arterial";
            _excelHelper.MergeCells(worksheet, 28, 8, 28, 9, false);
            worksheet.Cells["H28:I28"].Value = "17 - Collector";
            _excelHelper.MergeCells(worksheet, 29, 8, 29, 9, false);
            worksheet.Cells["H29:I29"].Value = "19 - Local";
            _excelHelper.MergeCells(worksheet, 30, 8, 30, 9, false);
            worksheet.Cells["H30:I30"].Value = "NN - Other";
            _excelHelper.MergeCells(worksheet, 31, 8, 31, 9, false);
            worksheet.Cells["H31:I31"].Value = "99 - Ramp";

            foreach (var item in functionalClassValueTracker)
            {
                if (parametersModel.FunctionalClass.Contains(item.Key))
                {
                    worksheet.Cells[item.Value].Value = "Y";
                }
                else
                {
                    worksheet.Cells[item.Value].Value = "N";
                }
            }
            worksheet.Cells["J30"].Value = worksheet.Cells["J22"].Value;
            _excelHelper.ApplyBorder(worksheet.Cells[14, 8, 31, 10]);
        }

        private void FillSimulationDetails(ExcelWorksheet worksheet, int yearCount, Simulation simulation)
        {
            _excelHelper.MergeCells(worksheet, 6, 6, 6, 8);
            _excelHelper.MergeCells(worksheet, 8, 6, 8, 7);
            _excelHelper.MergeCells(worksheet, 10, 6, 10, 7);
            _excelHelper.MergeCells(worksheet, 12, 6, 12, 7);

            _excelHelper.ApplyColor(worksheet.Cells[6, 6, 6, 8], Color.Gray);

            worksheet.Cells["F6:H6"].Value = "Investment:";
            worksheet.Cells["F8:G8"].Value = "Start Year:";
            worksheet.Cells["F10:G10"].Value = "Analysis Period:";
            worksheet.Cells["F12:G12"].Value = "Inflation Rate:";

            _excelHelper.ApplyBorder(worksheet.Cells[6, 6, 12, 8]);

            worksheet.Cells["H8"].Value = simulation.InvestmentPlan.FirstYearOfAnalysisPeriod; //StartYear;
            worksheet.Cells["H10"].Value = yearCount;
            worksheet.Cells["H12"].Value = simulation.InvestmentPlan.InflationRatePercentage; //inflationRate;

            _excelHelper.ApplyBorder(worksheet.Cells[8, 8, 12, 8]);
        }

        private void FillAnalysisDetails(ExcelWorksheet worksheet, Simulation simulation)
        {
            _excelHelper.MergeCells(worksheet, 6, 12, 6, 15);
            _excelHelper.MergeCells(worksheet, 8, 12, 8, 13);
            _excelHelper.MergeCells(worksheet, 10, 12, 10, 13);
            _excelHelper.MergeCells(worksheet, 12, 12, 12, 13);
            _excelHelper.MergeCells(worksheet, 14, 12, 14, 13);

            _excelHelper.ApplyBorder(worksheet.Cells[6, 12, 14, 15]);

            _excelHelper.MergeCells(worksheet, 8, 14, 8, 15, false);
            _excelHelper.MergeCells(worksheet, 10, 14, 10, 15, false);
            _excelHelper.MergeCells(worksheet, 12, 14, 12, 15, false);
            _excelHelper.MergeCells(worksheet, 14, 14, 14, 15, false);

            _excelHelper.ApplyBorder(worksheet.Cells[8, 14, 14, 15]);

            worksheet.Cells["L6:O6"].Value = "Analysis:";
            _excelHelper.ApplyColor(worksheet.Cells[6, 12, 6, 15], Color.Gray);
            worksheet.Cells["L8:M8"].Value = "Optimization:";
            worksheet.Cells["L10:M10"].Value = "Budget:";
            worksheet.Cells["L12:M12"].Value = "Weighting:";
            worksheet.Cells["L14:M14"].Value = "Benefit:";

            worksheet.Cells["N8:O8"].Value = simulation.AnalysisMethod.OptimizationStrategy;

            worksheet.Cells["N10:O10"].Value = simulation.AnalysisMethod.SpendingStrategy; //BudgetType;
            worksheet.Cells["N12:O12"].Value = simulation.AnalysisMethod.Weighting.Name; //WeightingAttribute;
            worksheet.Cells["N14:O14"].Value = simulation.AnalysisMethod.Benefit.Attribute.Name; //BenefitAttribute;
        }

        private int FillPriorities(ExcelWorksheet worksheet, Simulation simulation)
        {
            _excelHelper.MergeCells(worksheet, 19, 12, 19, worksheet.Dimension.End.Column);
            _excelHelper.ApplyColor(worksheet.Cells[19, 12, 19, worksheet.Dimension.End.Column], Color.Gray);
            _excelHelper.MergeCells(worksheet, 20, 13, 20, worksheet.Dimension.End.Column);

            _excelHelper.ApplyBorder(worksheet.Cells[20, 12, 20, worksheet.Dimension.End.Column]);

            worksheet.Cells["L19:Z19"].Value = "Analysis Priorites:";

            worksheet.Cells["L20"].Value = "Number";
            worksheet.Cells["M20"].Value = "Criteria:";

            var startingRow = 21;

            var priorites = simulation.AnalysisMethod.BudgetPriorities.OrderBy(_ => _.PriorityLevel);
            foreach (var item in priorites)
            {
                _excelHelper.MergeCells(worksheet, startingRow, 13, startingRow, worksheet.Dimension.End.Column, false);
                worksheet.Cells[startingRow, 12].Value = item.PriorityLevel;
                worksheet.Cells[startingRow, 12].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[startingRow, 12].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheet.Cells[startingRow, 13].Value = item.Criterion.Expression;
                worksheet.Row(startingRow).Height = 33;
                startingRow++;
            }
            _excelHelper.ApplyBorder(worksheet.Cells[21, 12, startingRow - 1, worksheet.Dimension.End.Column]);
            return startingRow;
        }

        private void FillJurisdictionCriteria(ExcelWorksheet worksheet, Simulation simulation)
        {
            _excelHelper.MergeCells(worksheet, 16, 12, 17, 13);
            _excelHelper.MergeCells(worksheet, 16, 14, 17, 26, false);

            _excelHelper.ApplyBorder(worksheet.Cells[16, 12, 17, 26]);

            worksheet.Cells["L16:M16"].Value = "Jurisdiction Criteria:";
            worksheet.Cells["N16:Z16"].Value = simulation.AnalysisMethod.Filter.Expression; //criteria;
        }

        private void FillBudgetSplitCriteria(ExcelWorksheet worksheet, int rowNum, Simulation simulation)
        {
            var currencyFormat = "_-$* #,##0.00_-;-$* #,##0.00_-;_-$* \"-\"??_-;_-@_-";
            rowNum++;
            var startingRow = rowNum;
            _excelHelper.MergeCells(worksheet, rowNum, 12, rowNum, 14);
            _excelHelper.ApplyColor(worksheet.Cells[rowNum, 12, rowNum, worksheet.Dimension.End.Column], Color.Gray);

            worksheet.Cells[rowNum, 12].Value = "Budget Split Criteria";
            worksheet.Cells[++rowNum, 12].Value = "Rank";
            worksheet.Cells[rowNum, 13].Value = "Amount";
            worksheet.Cells[rowNum, 14].Value = "Percentage";

            _excelHelper.ApplyBorder(worksheet.Cells[rowNum, 12, rowNum, 14]);

            foreach (var item in simulation.InvestmentPlan.CashFlowRules)
            {
                var i = 0;
                foreach(var rule in item.DistributionRules)
                {
                    i++;
                    worksheet.Cells[++rowNum, 12].Value = i;
                    worksheet.Cells[rowNum, 13].Style.Numberformat.Format = currencyFormat;
                    worksheet.Cells[rowNum, 13].Value = rule.CostCeiling;
                    worksheet.Cells[rowNum, 14].Value = rule.Expression;
                }
            }
            _excelHelper.ApplyBorder(worksheet.Cells[startingRow, 12, rowNum, 14]);
        }

        private void FillInvestmentAndBudgetCriteria(ExcelWorksheet worksheet, Simulation simulation)
        {
            var currencyFormat = "_-$* #,##0.00_-;-$* #,##0.00_-;_-$* \"-\"??_-;_-@_-";
            worksheet.Cells[38, 1].Value = "Years";
            worksheet.Cells[38, 2].Value = "Total Funding";
            _excelHelper.ApplyColor(worksheet.Cells[38, 2], Color.DimGray);
            _excelHelper.ApplyColor(worksheet.Cells[38, 1], Color.DimGray);

            var startingRowInvestment = 40;
            var startingBudgetHeaderColumn = 2;
            var nextBudget = 0;
            var investmentGrid = new SortedDictionary<int, Dictionary<string, decimal?>>();
            var startYear = simulation.InvestmentPlan.FirstYearOfAnalysisPeriod;
            foreach (var budgets in simulation.InvestmentPlan.Budgets)
            {
                var i = 0;
                foreach (var item in budgets.YearlyAmounts)
                {
                    if (!investmentGrid.ContainsKey(startYear + i))
                    {
                        investmentGrid.Add(startYear + i, new Dictionary<string, decimal?>() {
                            {budgets.Name, item.Value }
                        });
                    }
                    else
                    {
                        if (!investmentGrid[startYear + i].ContainsKey(budgets.Name))
                        {
                            investmentGrid[startYear + i].Add(budgets.Name, item.Value);
                        }
                        else
                        {
                            investmentGrid[startYear + i][budgets.Name] += item.Value;
                        }
                    }
                    i++;
                }
            }

            var firstRow = true;
            foreach (var item in investmentGrid)
            {
                worksheet.Cells[startingRowInvestment, 1].Value = item.Key;
                foreach (var budget in item.Value)
                {
                    if (firstRow == true)
                    {
                        worksheet.Cells[39, startingBudgetHeaderColumn + nextBudget].Value = budget.Key;
                        worksheet.Cells[startingRowInvestment, startingBudgetHeaderColumn + nextBudget].Value = budget.Value.Value;
                        worksheet.Cells[startingRowInvestment, startingBudgetHeaderColumn + nextBudget].Style.Numberformat.Format = currencyFormat;
                        nextBudget++;
                        continue;
                    }
                    for (var column = startingBudgetHeaderColumn; column <= item.Value.Count + 1; column++)
                    {
                        if (worksheet.Cells[39, column].Value.ToString() == budget.Key)
                        {
                            worksheet.Cells[startingRowInvestment, column].Style.Numberformat.Format = currencyFormat;
                            worksheet.Cells[startingRowInvestment, column].Value = budget.Value.Value;
                            break;
                        }
                    }
                }
                startingRowInvestment++;
                firstRow = false;
                nextBudget = 0;
            }
            _excelHelper.MergeCells(worksheet, 38, 1, 39, 1);
            _excelHelper.MergeCells(worksheet, 38, 2, 38, simulation.InvestmentPlan.Budgets.Count + 1);
            _excelHelper.ApplyBorder(worksheet.Cells[38, 1, startingRowInvestment - 1, simulation.InvestmentPlan.Budgets.Count + 1]);
            FillBudgetCriteria(worksheet, startingRowInvestment, simulation);
        }

        private void FillBudgetCriteria(ExcelWorksheet worksheet, int startingRowInvestment, Simulation simulation)
        {
            var rowToApplyBorder = startingRowInvestment + 2;
            worksheet.Cells[startingRowInvestment + 2, 1].Value = "Budget Criteria";
            _excelHelper.MergeCells(worksheet, startingRowInvestment + 2, 1, startingRowInvestment + 2, 5);
            _excelHelper.ApplyColor(worksheet.Cells[startingRowInvestment + 2, 1, startingRowInvestment + 2, 5], Color.Gray);

            worksheet.Cells[startingRowInvestment + 3, 1].Value = "Budget Name";
            worksheet.Cells[startingRowInvestment + 3, 2].Value = "Criteria";
            _excelHelper.MergeCells(worksheet, startingRowInvestment + 3, 2, startingRowInvestment + 3, 5);
            var cells = worksheet.Cells[startingRowInvestment + 3, 1, startingRowInvestment + 3, 2];
            _excelHelper.ApplyStyle(cells);
            foreach (var item in simulation.InvestmentPlan.BudgetConditions)
            {
                worksheet.Cells[startingRowInvestment + 4, 1].Value = item.Budget.Name;
                worksheet.Cells[startingRowInvestment + 4, 2].Value = item.Criterion.Expression;
                _excelHelper.MergeCells(worksheet, startingRowInvestment + 4, 2, startingRowInvestment + 4, 5, false);
                startingRowInvestment++;
            }
            _excelHelper.ApplyBorder(worksheet.Cells[rowToApplyBorder, 1, startingRowInvestment + 3, 5]);
        }

        #endregion
    }
}
