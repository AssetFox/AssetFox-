using System;
using System.Collections.Generic;
using System.Drawing;
using BridgeCareCore.Models.SummaryReport;
using BridgeCareCore.Services.SummaryReport.BridgeWorkSummary;
using OfficeOpenXml;

namespace BridgeCareCore.Services.SummaryReport.BridgeWorkSummaryByBudget
{
    public class BridgeWorkCost
    {
        private readonly BridgeWorkSummaryCommon _bridgeWorkSummaryCommon;
        public BridgeWorkCost(BridgeWorkSummaryCommon bridgeWorkSummaryCommon)
        {
            _bridgeWorkSummaryCommon = bridgeWorkSummaryCommon ?? throw new ArgumentNullException(nameof(bridgeWorkSummaryCommon));
        }

        internal void FillCostOfBridgeWork(ExcelWorksheet worksheet, CurrentCell currentCell, List<int> simulationYears,
            List<YearsData> costForBridgeBudgets, Dictionary<int, double> totalBudgetPerYearForBridgeWork, WorkTypeTotal workTypeTotal)
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

                FillForWorkTypeTotals(item, workTypeTotal);
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

        private void FillForWorkTypeTotals(YearsData data, WorkTypeTotal workTypeTotal)
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
                if (!workTypeTotal.BAMSPreservationCostPerYear.ContainsKey(data.Year))
                {
                    workTypeTotal.BAMSPreservationCostPerYear.Add(data.Year, 0);
                }
                if (!workTypeTotal.TotalCostPerYear.ContainsKey(data.Year))
                {
                    workTypeTotal.TotalCostPerYear.Add(data.Year, 0);
                }
                workTypeTotal.BAMSPreservationCostPerYear[data.Year] += data.Amount;
                workTypeTotal.TotalCostPerYear[data.Year] += data.Amount;
                break;
            case BAMSTreatmentName.DeckReplacement:
            case BAMSTreatmentName.SubstructureRehab:
            case BAMSTreatmentName.SuperstructureRepRehab:
                if (!workTypeTotal.BAMSRehabCostPerYear.ContainsKey(data.Year))
                {
                    workTypeTotal.BAMSRehabCostPerYear.Add(data.Year, 0);
                }
                if (!workTypeTotal.TotalCostPerYear.ContainsKey(data.Year))
                {
                    workTypeTotal.TotalCostPerYear.Add(data.Year, 0);
                }
                workTypeTotal.BAMSRehabCostPerYear[data.Year] += data.Amount;
                workTypeTotal.TotalCostPerYear[data.Year] += data.Amount;
                break;
            case BAMSTreatmentName.BridgeReplacement:
                if (!workTypeTotal.BAMSReplacementCostPerYear.ContainsKey(data.Year))
                {
                    workTypeTotal.BAMSReplacementCostPerYear.Add(data.Year, 0);
                }
                if (!workTypeTotal.TotalCostPerYear.ContainsKey(data.Year))
                {
                    workTypeTotal.TotalCostPerYear.Add(data.Year, 0);
                }
                workTypeTotal.BAMSReplacementCostPerYear[data.Year] += data.Amount;
                workTypeTotal.TotalCostPerYear[data.Year] += data.Amount;
                break;
            default:
                if (!workTypeTotal.OtherCostPerYear.ContainsKey(data.Year))
                {
                    workTypeTotal.OtherCostPerYear.Add(data.Year, 0);
                }
                if (!workTypeTotal.TotalCostPerYear.ContainsKey(data.Year))
                {
                    workTypeTotal.TotalCostPerYear.Add(data.Year, 0);
                }
                workTypeTotal.OtherCostPerYear[data.Year] += data.Amount;
                workTypeTotal.TotalCostPerYear[data.Year] += data.Amount;
                break;
            }
        }
    }
}
