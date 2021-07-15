using System;
using System.Collections.Generic;
using System.Drawing;
using BridgeCareCore.Interfaces.SummaryReport;
using BridgeCareCore.Models.SummaryReport;
using BridgeCareCore.Services.SummaryReport.BridgeWorkSummary;
using OfficeOpenXml;

namespace BridgeCareCore.Services.SummaryReport.BridgeWorkSummaryByBudget
{
    public class BridgeWorkCost
    {
        private readonly BridgeWorkSummaryCommon _bridgeWorkSummaryCommon;
        private readonly WorkTypeTotal _workTypeTotal;
        public BridgeWorkCost(BridgeWorkSummaryCommon bridgeWorkSummaryCommon, WorkTypeTotal workTypeTotal)
        {
            _bridgeWorkSummaryCommon = bridgeWorkSummaryCommon ?? throw new ArgumentNullException(nameof(bridgeWorkSummaryCommon));
            _workTypeTotal = workTypeTotal ?? throw new ArgumentNullException(nameof(workTypeTotal));
        }

        internal void FillCostOfBridgeWork(ExcelWorksheet worksheet, CurrentCell currentCell, List<int> simulationYears,
            List<YearsData> costForBridgeBudgets, Dictionary<int, double> totalBudgetPerYearForBridgeWork)
        {
            var startYear = simulationYears[0];
            currentCell.Row += 1;
            _bridgeWorkSummaryCommon.AddHeaders(worksheet, currentCell, simulationYears, "Cost of BAMS Bridge Work", "BAMS Bridge Work Type");

            currentCell.Row += 1;
            var startOfBridgeBudget = currentCell.Row;
            currentCell.Column = 1;
            var treatmentTracker = new Dictionary<string, int>();
            foreach (var treatment in costForBridgeBudgets)
            {
                if (!treatmentTracker.ContainsKey(treatment.Treatment))
                {
                    treatmentTracker.Add(treatment.Treatment, currentCell.Row);
                    worksheet.Cells[currentCell.Row, currentCell.Column].Value = treatment.Treatment;

                    worksheet.Cells[currentCell.Row, currentCell.Column + 2, currentCell.Row,
                    currentCell.Column + 1 + simulationYears.Count].Value = 0.0;
                    currentCell.Row += 1;
                }
            }

            _workTypeTotal.BAMSPreservationCostPerYear.Clear();
            foreach (var item in costForBridgeBudgets)
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

                FillForWorkTypeTotals(item);
            }

            worksheet.Cells[currentCell.Row, currentCell.Column].Value = Properties.Resources.BridgeTotal;

            foreach (var totalBridgeBudget in totalBudgetPerYearForBridgeWork)
            {
                var cellToEnterTotalBridgeCost = totalBridgeBudget.Key - startYear;
                worksheet.Cells[currentCell.Row, currentCell.Column + cellToEnterTotalBridgeCost + 2].Value = totalBridgeBudget.Value;
            }
            ExcelHelper.ApplyBorder(worksheet.Cells[startOfBridgeBudget, currentCell.Column, currentCell.Row, simulationYears.Count + 2]);
            ExcelHelper.SetCustomFormat(worksheet.Cells[startOfBridgeBudget, currentCell.Column + 2, currentCell.Row, simulationYears.Count + 2], ExcelHelperCellFormat.NegativeCurrency);
            ExcelHelper.ApplyColor(worksheet.Cells[startOfBridgeBudget, currentCell.Column + 2, currentCell.Row, simulationYears.Count + 2], Color.DarkSeaGreen);

            ExcelHelper.ApplyColor(worksheet.Cells[currentCell.Row, currentCell.Column + 2, currentCell.Row, simulationYears.Count + 2], Color.FromArgb(84, 130, 53));
            ExcelHelper.SetTextColor(worksheet.Cells[currentCell.Row, currentCell.Column + 2, currentCell.Row, simulationYears.Count + 2], Color.White);
        }

        private void FillForWorkTypeTotals(YearsData data)
        {
            BamsTreatmentMap.Map.TryGetValue(data.Treatment, out var treatment);
            switch (treatment)
            {
            case BAMSTreatmentName.CountyMaintenanceDeckWork:
            case BAMSTreatmentName.CountyMaintenanceSuperstructureWork:
            case BAMSTreatmentName.CountyMaintenanceSubstructureWork:
            case BAMSTreatmentName.BituminousOverlay:
            case BAMSTreatmentName.StructuralOverlayJointsCoatings:
            case BAMSTreatmentName.EpoxyJointGlandsCoatings:
            case BAMSTreatmentName.PaintingJointSpotZone:
            case BAMSTreatmentName.PaintingFull:
                if (!_workTypeTotal.BAMSPreservationCostPerYear.ContainsKey(data.Year))
                {
                    _workTypeTotal.BAMSPreservationCostPerYear.Add(data.Year, 0);
                }
                if (!_workTypeTotal.TotalCostPerYear.ContainsKey(data.Year))
                {
                    _workTypeTotal.TotalCostPerYear.Add(data.Year, 0);
                }
                _workTypeTotal.BAMSPreservationCostPerYear[data.Year] += data.Amount;
                _workTypeTotal.TotalCostPerYear[data.Year] += data.Amount;
                break;
            case BAMSTreatmentName.DeckReplacement:
            case BAMSTreatmentName.SubstructureRehab:
            case BAMSTreatmentName.SuperstructureRepRehab:
                if (!_workTypeTotal.BAMSRehabCostPerYear.ContainsKey(data.Year))
                {
                    _workTypeTotal.BAMSRehabCostPerYear.Add(data.Year, 0);
                }
                if (!_workTypeTotal.TotalCostPerYear.ContainsKey(data.Year))
                {
                    _workTypeTotal.TotalCostPerYear.Add(data.Year, 0);
                }
                _workTypeTotal.BAMSRehabCostPerYear[data.Year] += data.Amount;
                _workTypeTotal.TotalCostPerYear[data.Year] += data.Amount;
                break;
            case BAMSTreatmentName.BridgeReplacement:
                if (!_workTypeTotal.BAMSReplacementCostPerYear.ContainsKey(data.Year))
                {
                    _workTypeTotal.BAMSReplacementCostPerYear.Add(data.Year, 0);
                }
                if (!_workTypeTotal.TotalCostPerYear.ContainsKey(data.Year))
                {
                    _workTypeTotal.TotalCostPerYear.Add(data.Year, 0);
                }
                _workTypeTotal.BAMSReplacementCostPerYear[data.Year] += data.Amount;
                _workTypeTotal.TotalCostPerYear[data.Year] += data.Amount;
                break;
            default:
                if (!_workTypeTotal.OtherCostPerYear.ContainsKey(data.Year))
                {
                    _workTypeTotal.OtherCostPerYear.Add(data.Year, 0);
                }
                if (!_workTypeTotal.TotalCostPerYear.ContainsKey(data.Year))
                {
                    _workTypeTotal.TotalCostPerYear.Add(data.Year, 0);
                }
                _workTypeTotal.OtherCostPerYear[data.Year] += data.Amount;
                _workTypeTotal.TotalCostPerYear[data.Year] += data.Amount;
                break;
            }
        }
    }
}
