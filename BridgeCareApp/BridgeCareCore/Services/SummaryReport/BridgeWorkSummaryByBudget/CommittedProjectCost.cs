using System;
using System.Collections.Generic;
using System.Drawing;
using BridgeCareCore.Interfaces.SummaryReport;
using BridgeCareCore.Models.SummaryReport;
using BridgeCareCore.Services.SummaryReport.BridgeWorkSummary;
using OfficeOpenXml;

namespace BridgeCareCore.Services.SummaryReport.BridgeWorkSummaryByBudget
{
    public class CommittedProjectCost
    {
        private readonly BridgeWorkSummaryCommon _bridgeWorkSummaryCommon;
        private readonly WorkTypeTotal _workTypeTotal;

        public CommittedProjectCost(BridgeWorkSummaryCommon bridgeWorkSummaryCommon, WorkTypeTotal workTypeTotal)
        {
            _bridgeWorkSummaryCommon = bridgeWorkSummaryCommon ?? throw new ArgumentNullException(nameof(bridgeWorkSummaryCommon));
            _workTypeTotal = workTypeTotal ?? throw new ArgumentNullException(nameof(workTypeTotal));
        }

        internal void FillCostOfCommittedWork(ExcelWorksheet worksheet, CurrentCell currentCell, List<int> simulationYears,
            List<YearsData> costForCommittedBudgets, HashSet<string> committedTreatments,
            Dictionary<int, double> totalBudgetPerYearForCommittedWork)
        {
            var startYear = simulationYears[0];
            currentCell.Row += 1;
            _bridgeWorkSummaryCommon.AddHeaders(worksheet, currentCell, simulationYears, "Cost of MPMS Work", "MPMS Work Type");

            currentCell.Row += 1;
            var startOfCommittedBudget = currentCell.Row;
            currentCell.Column = 1;
            var treatmentTracker = new Dictionary<string, int>();
            foreach (var treatment in committedTreatments)
            {
                worksheet.Cells[currentCell.Row, currentCell.Column].Value = treatment;
                treatmentTracker.Add(treatment, currentCell.Row);
                worksheet.Cells[currentCell.Row, currentCell.Column + 2, currentCell.Row,
                    currentCell.Column + 1 + simulationYears.Count].Value = 0.0;
                currentCell.Row += 1;
            }
            _workTypeTotal.MPMSpreservationCostPerYear.Clear();
            foreach (var item in costForCommittedBudgets)
            {
                var rowNum = treatmentTracker[item.Treatment];

                var cellToEnterCost = item.Year - startYear;
                var cellValue = worksheet.Cells[rowNum, currentCell.Column + cellToEnterCost + 2].Value;
                var totalAmount = 0.0;
                if (cellValue != null)
                {
                    totalAmount = (double)cellValue;
                }
                totalAmount += item.Amount;
                worksheet.Cells[rowNum, currentCell.Column + cellToEnterCost + 2].Value = totalAmount;

                FillWorkTypeTotals(item);
            }

            worksheet.Cells[currentCell.Row, currentCell.Column].Value = Properties.Resources.BridgeTotal;

            foreach (var totalCommittedBudget in totalBudgetPerYearForCommittedWork)
            {
                var cellToEnterTotalBridgeCost = totalCommittedBudget.Key - startYear;
                worksheet.Cells[currentCell.Row, currentCell.Column + cellToEnterTotalBridgeCost + 2].Value = totalCommittedBudget.Value;
            }
            ExcelHelper.ApplyBorder(worksheet.Cells[startOfCommittedBudget, currentCell.Column, currentCell.Row, simulationYears.Count + 2]);
            ExcelHelper.SetCustomFormat(worksheet.Cells[startOfCommittedBudget, currentCell.Column + 2, currentCell.Row, simulationYears.Count + 2], ExcelHelperCellFormat.NegativeCurrency);
            ExcelHelper.ApplyColor(worksheet.Cells[startOfCommittedBudget, currentCell.Column + 2, currentCell.Row, simulationYears.Count + 2], Color.DarkSeaGreen);

            ExcelHelper.ApplyColor(worksheet.Cells[currentCell.Row, currentCell.Column + 2, currentCell.Row, simulationYears.Count + 2], Color.FromArgb(84, 130, 53));
            ExcelHelper.SetTextColor(worksheet.Cells[currentCell.Row, currentCell.Column + 2, currentCell.Row, simulationYears.Count + 2], Color.White);
            currentCell.Row++;
        }

        private void FillWorkTypeTotals(YearsData item)
        {
            MPMSTreatmentMap.Map.TryGetValue(item.Treatment, out var treatment);
            switch (treatment)
            {
            case MPMSTreatmentName.Preservation:
                if (!_workTypeTotal.MPMSpreservationCostPerYear.ContainsKey(item.Year))
                {
                    _workTypeTotal.MPMSpreservationCostPerYear.Add(item.Year, 0);
                }
                if (!_workTypeTotal.TotalCostPerYear.ContainsKey(item.Year))
                {
                    _workTypeTotal.TotalCostPerYear.Add(item.Year, 0);
                }
                _workTypeTotal.MPMSpreservationCostPerYear[item.Year] += item.Amount;
                _workTypeTotal.TotalCostPerYear[item.Year] += item.Amount;
                break;
            case MPMSTreatmentName.EmergencyRepair:
                if (!_workTypeTotal.MPMSEmergencyRepairCostPerYear.ContainsKey(item.Year))
                {
                    _workTypeTotal.MPMSEmergencyRepairCostPerYear.Add(item.Year, 0);
                }
                if (!_workTypeTotal.TotalCostPerYear.ContainsKey(item.Year))
                {
                    _workTypeTotal.TotalCostPerYear.Add(item.Year, 0);
                }
                _workTypeTotal.MPMSEmergencyRepairCostPerYear[item.Year] += item.Amount;
                _workTypeTotal.TotalCostPerYear[item.Year] += item.Amount;
                break;
            case MPMSTreatmentName.Rehabilitation:
            case MPMSTreatmentName.Repair:
                if (!_workTypeTotal.MPMSEmergencyRepairCostPerYear.ContainsKey(item.Year))
                {
                    _workTypeTotal.MPMSEmergencyRepairCostPerYear.Add(item.Year, 0);
                }
                if (!_workTypeTotal.TotalCostPerYear.ContainsKey(item.Year))
                {
                    _workTypeTotal.TotalCostPerYear.Add(item.Year, 0);
                }
                _workTypeTotal.MPMSEmergencyRepairCostPerYear[item.Year] += item.Amount;
                _workTypeTotal.TotalCostPerYear[item.Year] += item.Amount;
                break;
            case MPMSTreatmentName.Removal:
            case MPMSTreatmentName.Replacement:
                if (!_workTypeTotal.MPMSReplacementCostPerYear.ContainsKey(item.Year))
                {
                    _workTypeTotal.MPMSReplacementCostPerYear.Add(item.Year, 0);
                }
                if (!_workTypeTotal.TotalCostPerYear.ContainsKey(item.Year))
                {
                    _workTypeTotal.TotalCostPerYear.Add(item.Year, 0);
                }
                _workTypeTotal.MPMSReplacementCostPerYear[item.Year] += item.Amount;
                _workTypeTotal.TotalCostPerYear[item.Year] += item.Amount;
                break;
            default:
                if (!_workTypeTotal.OtherCostPerYear.ContainsKey(item.Year))
                {
                    _workTypeTotal.OtherCostPerYear.Add(item.Year, 0);
                }
                if (!_workTypeTotal.TotalCostPerYear.ContainsKey(item.Year))
                {
                    _workTypeTotal.TotalCostPerYear.Add(item.Year, 0);
                }
                _workTypeTotal.OtherCostPerYear[item.Year] += item.Amount;
                _workTypeTotal.TotalCostPerYear[item.Year] += item.Amount;
                break;
            }
        }
    }
}
