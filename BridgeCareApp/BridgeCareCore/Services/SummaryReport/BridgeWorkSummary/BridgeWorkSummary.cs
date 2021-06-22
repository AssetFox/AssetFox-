using System;
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
            var treatments = new SortedSet<string>();

            // cache to store total cost per treatment for a given year along with count of culvert
            // and non-culvert bridges
            var costPerBPNPerYear = new Dictionary<int,Dictionary<string, decimal>>();
            var costPerTreatmentPerYear = new Dictionary<int, Dictionary<string, (decimal treatmentCost, int bridgeCount)>>();
            var yearlyCostCommittedProj = new Dictionary<int, Dictionary<string, (decimal treatmentCost, int bridgeCount)>>();
            foreach (var yearData in reportOutputData.Years)
            {
                costPerTreatmentPerYear.Add(yearData.Year, new Dictionary<string, (decimal treatmentCost, int bridgeCount)>());
                yearlyCostCommittedProj.Add(yearData.Year, new Dictionary<string, (decimal treatmentCost, int bridgeCount)>());
                costPerBPNPerYear.Add(yearData.Year, new Dictionary<string, decimal>());
                foreach (var section in yearData.Sections)
                {
                    if (section.TreatmentCause == TreatmentCause.NoSelection) //|| section.TreatmentOptions.Count <= 0
                    {
                        continue;
                    }
                    if (!costPerBPNPerYear[yearData.Year].ContainsKey(section.ValuePerTextAttribute["BUS_PLAN_NETWORK"]))
                    {
                        costPerBPNPerYear[yearData.Year].Add(section.ValuePerTextAttribute["BUS_PLAN_NETWORK"], 0);
                    }

                    if (section.TreatmentCause == TreatmentCause.CommittedProject &&
                        section.AppliedTreatment.ToLower() != Properties.Resources.NoTreatment)
                    {
                        var commitedCost = section.TreatmentConsiderations.Sum(_ => _.BudgetUsages.Sum(b => b.CoveredCost));

                        if (!yearlyCostCommittedProj[yearData.Year].ContainsKey(section.AppliedTreatment))
                        {
                            yearlyCostCommittedProj[yearData.Year].Add(section.AppliedTreatment, (commitedCost, 1));
                        }
                        else
                        {
                            var treatmentCost = yearlyCostCommittedProj[yearData.Year][section.AppliedTreatment].treatmentCost + commitedCost;
                            var bridgeCount = yearlyCostCommittedProj[yearData.Year][section.AppliedTreatment].bridgeCount + 1;
                            var newCostAndCount = (treatmentCost, bridgeCount);
                            yearlyCostCommittedProj[yearData.Year][section.AppliedTreatment] = newCostAndCount;
                        }

                         costPerBPNPerYear[yearData.Year][section.ValuePerTextAttribute["BUS_PLAN_NETWORK"]] += commitedCost;

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

                    // Fill cost per BPN per Year
                    costPerBPNPerYear[yearData.Year][section.ValuePerTextAttribute["BUS_PLAN_NETWORK"]] += cost;

                    section.TreatmentOptions.ForEach(_ =>
                    {
                        if (!treatments.Contains(_.TreatmentName))
                        {
                            treatments.Add(_.TreatmentName);
                        }
                    });

                    section.TreatmentRejections.ForEach(_ =>
                    {
                        if (!treatments.Contains(_.TreatmentName))
                        {
                            treatments.Add(_.TreatmentName);
                        }
                    });
                }
            }

            #endregion Initial work to set some data, which will be used throughout the Work summary TAB

            _costBudgetsWorkSummary.FillCostBudgetWorkSummarySections(worksheet, currentCell, costPerTreatmentPerYear, yearlyCostCommittedProj,
                simulationYears, treatments, yearlyBudgetAmount, costPerBPNPerYear);

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
