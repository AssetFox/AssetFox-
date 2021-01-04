﻿using System;
using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.iAM.Analysis;
using AppliedResearchAssociates.iAM.Domains;
using BridgeCareCore.Interfaces.SummaryReport;
using BridgeCareCore.Models.SummaryReport;
using OfficeOpenXml;

namespace BridgeCareCore.Services.SummaryReport.BridgeWorkSummary
{
    public class BridgeWorkSummary : IBridgeWorkSummary
    {
        private readonly CostBudgetsWorkSummary _costBudgetsWorkSummary;
        private readonly BridgesCulvertsWorkSummary _bridgesCulvertsWorkSummary;
        private readonly BridgeRateDeckAreaWorkSummary _bridgeRateDeckAreaWorkSummary;
        private readonly NHSBridgeDeckAreaWorkSummary _nhsBridgeDeckAreaWorkSummary;
        private readonly DeckAreaBridgeWorkSummary _deckAreaBridgeWorkSummary;
        private readonly PostedClosedBridgeWorkSummary _postedClosedBridgeWorkSummary;

        public BridgeWorkSummary(CostBudgetsWorkSummary costBudgetsWorkSummary,
            BridgesCulvertsWorkSummary bridgesCulvertsWorkSummary, BridgeRateDeckAreaWorkSummary bridgeRateDeckAreaWorkSummary,
            NHSBridgeDeckAreaWorkSummary nhsBridgeDeckAreaWorkSummary,
            DeckAreaBridgeWorkSummary deckAreaBridgeWorkSummary,
            PostedClosedBridgeWorkSummary postedClosedBridgeWorkSummary)
        {
            _bridgesCulvertsWorkSummary = bridgesCulvertsWorkSummary ?? throw new ArgumentNullException(nameof(bridgesCulvertsWorkSummary));
            _costBudgetsWorkSummary = costBudgetsWorkSummary ?? throw new ArgumentNullException(nameof(costBudgetsWorkSummary));
            _bridgeRateDeckAreaWorkSummary = bridgeRateDeckAreaWorkSummary ?? throw new ArgumentNullException(nameof(bridgeRateDeckAreaWorkSummary));
            _nhsBridgeDeckAreaWorkSummary = nhsBridgeDeckAreaWorkSummary ?? throw new ArgumentNullException(nameof(nhsBridgeDeckAreaWorkSummary));
            _deckAreaBridgeWorkSummary = deckAreaBridgeWorkSummary ?? throw new ArgumentNullException(nameof(deckAreaBridgeWorkSummary));
            _postedClosedBridgeWorkSummary = postedClosedBridgeWorkSummary ?? throw new ArgumentNullException(nameof(postedClosedBridgeWorkSummary));
        }
        public ChartRowsModel Fill(ExcelWorksheet worksheet, SimulationOutput reportOutputData,
            List<int> simulationYears, WorkSummaryModel workSummaryModel, Dictionary<string, Budget> yearlyBudgetAmount)
        {
            var currentCell = new CurrentCell { Row = 1, Column = 1 };

            #region Initial work to set some data, which will be used throughout the Work summary TAB
            // Getting list of treatments. It will be used in several places throughout this excel TAB
            var treatments = new List<string>();
            var singleSection = reportOutputData.Years[0].Sections[0];
            treatments = singleSection.TreatmentOptions.Select(_ => _.TreatmentName)
                .Union(singleSection.TreatmentRejections.Select(r => r.TreatmentName)).ToList();
            treatments.Sort();

            // cache to store total cost per treatment for a given year along with count of culvert and non-culvert bridges
            var costPerTreatmentPerYear = new Dictionary<int, Dictionary<string, (decimal treatmentCost, int bridgeCount)>>();
            foreach (var yearData in reportOutputData.Years)
            {
                costPerTreatmentPerYear.Add(yearData.Year, new Dictionary<string, (decimal treatmentCost, int bridgeCount)>());
                foreach (var section in yearData.Sections)
                {
                    if (section.TreatmentCause == TreatmentCause.NoSelection) //|| section.TreatmentOptions.Count <= 0
                    {
                        continue;
                    }
                    //[TODO] - ask Jake regarding cash flow project. It won't have anything in the TreartmentOptions barring 1st year
                    //var treatmentDetailOption = section.TreatmentOptions.Find(_ => _.TreatmentName == section.AppliedTreatment);
                    //var cost = treatmentDetailOption == null ? 0 : treatmentDetailOption.Cost;
                    var cost = section.TreatmentConsiderations.Sum(_ => _.BudgetUsages.Sum(b => b.CoveredCost));
                    if (!costPerTreatmentPerYear[yearData.Year].ContainsKey(section.AppliedTreatment))
                    {
                        costPerTreatmentPerYear[yearData.Year].Add(section.AppliedTreatment, (cost, 1));
                    }
                    else
                    {
                        var treatmentCost = costPerTreatmentPerYear[yearData.Year][section.AppliedTreatment].treatmentCost + cost;
                        var bridgeCount = costPerTreatmentPerYear[yearData.Year][section.AppliedTreatment].bridgeCount + 1;
                        var newCostAndCount = (treatmentCost, bridgeCount);
                        costPerTreatmentPerYear[yearData.Year][section.AppliedTreatment] = newCostAndCount;
                    }
                }
            }
            #endregion

            _costBudgetsWorkSummary.FillCostBudgetWorkSummarySections(worksheet, currentCell, costPerTreatmentPerYear,
                simulationYears, treatments, yearlyBudgetAmount);

            _bridgesCulvertsWorkSummary.FillBridgesCulvertsWorkSummarySections(worksheet, currentCell, costPerTreatmentPerYear, simulationYears, treatments);

            var chartRowsModel = _bridgeRateDeckAreaWorkSummary.FillBridgeRateDeckAreaWorkSummarySections(worksheet, currentCell,
                simulationYears, workSummaryModel, reportOutputData);

            _nhsBridgeDeckAreaWorkSummary.FillNHSBridgeDeckAreaWorkSummarySections(worksheet, currentCell, simulationYears, reportOutputData, chartRowsModel);

            chartRowsModel = _deckAreaBridgeWorkSummary.FillPoorDeckArea(worksheet, currentCell, simulationYears, reportOutputData, chartRowsModel);

            chartRowsModel = _postedClosedBridgeWorkSummary.FillMoneyNeededByBPN(worksheet, currentCell, simulationYears, reportOutputData, chartRowsModel);

            worksheet.Calculate();
            worksheet.Cells.AutoFitColumns();

            return chartRowsModel;
        }
    }
}