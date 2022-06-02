using System;
using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.iAM.Analysis.Engine;
using AppliedResearchAssociates.iAM.Analysis;
using OfficeOpenXml;

using AppliedResearchAssociates.iAM.Reporting.Interfaces.BAMSSummaryReport;
using AppliedResearchAssociates.iAM.Reporting.Models.BAMSSummaryReport;

namespace AppliedResearchAssociates.iAM.Reporting.Services.BAMSSummaryReport.BridgeWorkSummary
{
    public class BridgeWorkSummary : IBridgeWorkSummary
    {
        private CostBudgetsWorkSummary _costBudgetsWorkSummary;
        private BridgesCulvertsWorkSummary _bridgesCulvertsWorkSummary;
        private BridgeRateDeckAreaWorkSummary _bridgeRateDeckAreaWorkSummary;
        private NHSBridgeDeckAreaWorkSummary _nhsBridgeDeckAreaWorkSummary;
        private DeckAreaBridgeWorkSummary _deckAreaBridgeWorkSummary;
        private PostedClosedBridgeWorkSummary _postedClosedBridgeWorkSummary;
        private ProjectsCompletedCount _projectsCompletedCount;

        public BridgeWorkSummary()
        {
            _bridgesCulvertsWorkSummary = new BridgesCulvertsWorkSummary();
            if (_bridgesCulvertsWorkSummary == null) { throw new ArgumentNullException(nameof(_bridgesCulvertsWorkSummary)); }

            var workSummaryModel = new WorkSummaryModel();
            _costBudgetsWorkSummary = new CostBudgetsWorkSummary(workSummaryModel);
            if (_costBudgetsWorkSummary == null) { throw new ArgumentNullException(nameof(_costBudgetsWorkSummary)); }

            _bridgeRateDeckAreaWorkSummary = new BridgeRateDeckAreaWorkSummary();
            if (_bridgeRateDeckAreaWorkSummary == null) { throw new ArgumentNullException(nameof(_bridgeRateDeckAreaWorkSummary)); }

            _nhsBridgeDeckAreaWorkSummary = new NHSBridgeDeckAreaWorkSummary();
            if (_nhsBridgeDeckAreaWorkSummary == null) { throw new ArgumentNullException(nameof(_nhsBridgeDeckAreaWorkSummary)); }

            _deckAreaBridgeWorkSummary = new DeckAreaBridgeWorkSummary();
            if (_deckAreaBridgeWorkSummary == null) { throw new ArgumentNullException(nameof(_deckAreaBridgeWorkSummary)); }

            _postedClosedBridgeWorkSummary = new PostedClosedBridgeWorkSummary(workSummaryModel);
            if (_postedClosedBridgeWorkSummary == null) { throw new ArgumentNullException(nameof(_postedClosedBridgeWorkSummary)); }

            _projectsCompletedCount = new ProjectsCompletedCount();
            if (_projectsCompletedCount == null) { throw new ArgumentNullException(nameof(_projectsCompletedCount)); }
        }

        public ChartRowsModel Fill(ExcelWorksheet worksheet, SimulationOutput reportOutputData,
            List<int> simulationYears, WorkSummaryModel workSummaryModel, Dictionary<string, Budget> yearlyBudgetAmount,
            IReadOnlyCollection<SelectableTreatment> selectableTreatments)
        {
            var currentCell = new CurrentCell { Row = 1, Column = 1 };

            #region Initial work to set some data, which will be used throughout the Work summary TAB

            // Getting list of treatments. It will be used in several places throughout this excel TAB
            var simulationTreatments = new List<(string Name, AssetCategory AssetType, TreatmentCategory Category)>();
            simulationTreatments.Add((BAMSConstants.CulvertNoTreatment, AssetCategory.Culvert, TreatmentCategory.Other));
            simulationTreatments.Add((BAMSConstants.NonCulvertNoTreatment, AssetCategory.Bridge, TreatmentCategory.Other));
            foreach (var item in selectableTreatments)
            {
                if (item.Name.ToLower() == BAMSConstants.NoTreatment) continue;
                simulationTreatments.Add((item.Name, item.AssetCategory, item.Category));
            }
            simulationTreatments.Sort((a, b) => a.Item1.CompareTo(b.Item1));

            // cache to store total cost per treatment for a given year along with count of culvert
            // and non-culvert bridges
            var costPerBPNPerYear = new Dictionary<int, Dictionary<string, decimal>>();
            var costAndCountPerTreatmentPerYear = new Dictionary<int, Dictionary<string, (decimal treatmentCost, int bridgeCount)>>();
            var yearlyCostCommittedProj = new Dictionary<int, Dictionary<string, (decimal treatmentCost, int bridgeCount)>>();
            var countForCompletedProject = new Dictionary<int, Dictionary<string, int>>();
            var countForCompletedCommittedProject = new Dictionary<int, Dictionary<string, int>>();

            FillDataToUseInExcel(reportOutputData, costPerBPNPerYear, costAndCountPerTreatmentPerYear, yearlyCostCommittedProj,
                countForCompletedProject, countForCompletedCommittedProject);

            #endregion Initial work to set some data, which will be used throughout the Work summary TAB

            _costBudgetsWorkSummary.FillCostBudgetWorkSummarySections(worksheet, currentCell, costAndCountPerTreatmentPerYear, yearlyCostCommittedProj,
                simulationYears, yearlyBudgetAmount, costPerBPNPerYear, simulationTreatments);

            _bridgesCulvertsWorkSummary.FillBridgesCulvertsWorkSummarySections(worksheet, currentCell, costAndCountPerTreatmentPerYear, yearlyCostCommittedProj, simulationYears, simulationTreatments);
            _projectsCompletedCount.FillProjectCompletedCountSection(worksheet, currentCell, countForCompletedProject, countForCompletedCommittedProject, simulationYears, simulationTreatments);

            var chartRowsModel = _bridgeRateDeckAreaWorkSummary.FillBridgeRateDeckAreaWorkSummarySections(worksheet, currentCell,
                simulationYears, workSummaryModel, reportOutputData);

            _nhsBridgeDeckAreaWorkSummary.FillNHSBridgeDeckAreaWorkSummarySections(worksheet, currentCell, simulationYears, reportOutputData, chartRowsModel);

            chartRowsModel = _deckAreaBridgeWorkSummary.FillPoorDeckArea(worksheet, currentCell, simulationYears, reportOutputData, chartRowsModel);

            chartRowsModel = _postedClosedBridgeWorkSummary.FillPostedBridgesCountByBPN(worksheet, currentCell, simulationYears, reportOutputData, chartRowsModel);
            chartRowsModel = _postedClosedBridgeWorkSummary.FillPostedBridgesDeckAreaByBPN(worksheet, currentCell, simulationYears, reportOutputData, chartRowsModel);
            chartRowsModel = _postedClosedBridgeWorkSummary.FillClosedBridgesCountByBPN(worksheet, currentCell, simulationYears, reportOutputData, chartRowsModel);
            chartRowsModel = _postedClosedBridgeWorkSummary.FillClosedBridgesDeckAreaByBPN(worksheet, currentCell, simulationYears, reportOutputData, chartRowsModel);
            _postedClosedBridgeWorkSummary.FillPostedAndClosedBridgesTotalCount(worksheet, currentCell, simulationYears, reportOutputData, chartRowsModel);
            chartRowsModel = _postedClosedBridgeWorkSummary.FillMoneyNeededByBPN(worksheet, currentCell, simulationYears, reportOutputData, chartRowsModel);

            worksheet.Calculate();
            worksheet.Cells.AutoFitColumns();

            return chartRowsModel;
        }

        #region Private methods

        private void FillDataToUseInExcel(SimulationOutput reportOutputData, Dictionary<int, Dictionary<string, decimal>> costPerBPNPerYear,
            Dictionary<int, Dictionary<string, (decimal treatmentCost, int bridgeCount)>> costAndCountPerTreatmentPerYear,
            Dictionary<int, Dictionary<string, (decimal treatmentCost, int bridgeCount)>> yearlyCostCommittedProj, Dictionary<int, Dictionary<string, int>> countForCompletedProject,
            Dictionary<int, Dictionary<string, int>> countForCompletedCommittedProject)
        {
            var isInitialYear = true;
            foreach (var yearData in reportOutputData.Years)
            {
                costAndCountPerTreatmentPerYear.Add(yearData.Year, new Dictionary<string, (decimal treatmentCost, int bridgeCount)>());
                yearlyCostCommittedProj.Add(yearData.Year, new Dictionary<string, (decimal treatmentCost, int bridgeCount)>());
                costPerBPNPerYear.Add(yearData.Year, new Dictionary<string, decimal>());
                countForCompletedProject.Add(yearData.Year, new Dictionary<string, int>());
                countForCompletedCommittedProject.Add(yearData.Year, new Dictionary<string, int>());
                foreach (var section in yearData.Sections)
                {
                    if (!costPerBPNPerYear[yearData.Year].ContainsKey(section.ValuePerTextAttribute["BUS_PLAN_NETWORK"]))
                    {
                        costPerBPNPerYear[yearData.Year].Add(section.ValuePerTextAttribute["BUS_PLAN_NETWORK"], 0);
                    }

                    if (section.TreatmentCause == TreatmentCause.CommittedProject &&
                        section.AppliedTreatment.ToLower() != BAMSConstants.NoTreatment)
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

                        // Adding count for completed committed project
                        if (!countForCompletedCommittedProject[yearData.Year].ContainsKey(section.AppliedTreatment))
                        {
                            countForCompletedCommittedProject[yearData.Year].Add(section.AppliedTreatment, 1);
                        }
                        else
                        {
                            countForCompletedCommittedProject[yearData.Year][section.AppliedTreatment] += 1;
                        }

                        continue;
                    }
                    //[TODO] - ask Jake regarding cash flow project. It won't have anything in the TreartmentOptions barring 1st year

                    var cost = section.TreatmentConsiderations.Sum(_ => _.BudgetUsages.Sum(b => b.CoveredCost));
                    PopulateWorkedOnCostAndCount(yearData.Year, section, costAndCountPerTreatmentPerYear, cost);

                    PopulateCompletedProjectCount(yearData.Year, section, countForCompletedProject);

                    RemoveBridgesForCashFlowedProj(countForCompletedProject, section, isInitialYear, yearData.Year);

                    // Fill cost per BPN per Year
                    costPerBPNPerYear[yearData.Year][section.ValuePerTextAttribute["BUS_PLAN_NETWORK"]] += cost;
                }
                isInitialYear = false;
            }
        }

        private void PopulateWorkedOnCostAndCount(int year, SectionDetail section,
            Dictionary<int, Dictionary<string, (decimal treatmentCost, int bridgeCount)>> costAndCountPerTreatmentPerYear, decimal cost)
        {
            if (section.TreatmentCause == TreatmentCause.NoSelection)
            {
                var culvert = BAMSConstants.CulvertBridgeType;
                var nonCulvert = BAMSConstants.NonCulvertBridgeType;
                // If Bridge type is culvert
                if (section.ValuePerTextAttribute["BRIDGE_TYPE"] == culvert)
                {
                    AddKeyValueForWorkedOn(costAndCountPerTreatmentPerYear[year], culvert, section.AppliedTreatment, cost);
                }
                // if bridge is non-culvert
                else
                {
                    AddKeyValueForWorkedOn(costAndCountPerTreatmentPerYear[year], nonCulvert, section.AppliedTreatment, cost);
                }
            }
            // if applied treatment is other than No Treatment
            else
            {
                if (!costAndCountPerTreatmentPerYear[year].ContainsKey(section.AppliedTreatment))
                {
                    costAndCountPerTreatmentPerYear[year].Add(section.AppliedTreatment, (cost, 1));
                }
                else
                {
                    var values = costAndCountPerTreatmentPerYear[year][section.AppliedTreatment];
                    values.treatmentCost += cost;
                    values.bridgeCount += 1;
                    costAndCountPerTreatmentPerYear[year][section.AppliedTreatment] = values;
                }
            }
        }

        private void PopulateCompletedProjectCount(int year, SectionDetail section, Dictionary<int, Dictionary<string, int>> countForCompletedProject)
        {
            if (section.TreatmentCause == TreatmentCause.NoSelection)
            {
                var culvert = BAMSConstants.CulvertBridgeType;
                // If Bridge type is culvert
                if (section.ValuePerTextAttribute["BRIDGE_TYPE"] == culvert)
                {
                    AddKeyValue(countForCompletedProject[year], culvert, section.AppliedTreatment);
                }
                // If Bridge type is non culvert
                else
                {
                    AddKeyValue(countForCompletedProject[year], BAMSConstants.NonCulvertBridgeType, section.AppliedTreatment);
                }
            }
            else
            {
                if (!countForCompletedProject[year].ContainsKey(section.AppliedTreatment))
                {
                    countForCompletedProject[year].Add(section.AppliedTreatment, 1);
                }
                else
                {
                    countForCompletedProject[year][section.AppliedTreatment] += 1;
                }
            }
        }

        private void RemoveBridgesForCashFlowedProj(Dictionary<int, Dictionary<string, int>> countForCompletedProject,
            SectionDetail section, bool isInitialYear, int year)
        {
            // to store "Projects completed"
            if (section.TreatmentCause == TreatmentCause.CashFlowProject && !isInitialYear)
            {
                // if current year status is TreatmentCause.CashFlowProject, then the previous year
                // is either 1st year of cashflow or somewhere in between, in both cases, we will
                // remove the previous year project as it has not been conmleted.
                countForCompletedProject[year - 1][section.AppliedTreatment] -= 1;
            }
        }

        private void AddKeyValueForWorkedOn(Dictionary<string, (decimal treatmentCost, int bridgeCount)> workedOnProj, string bridgeType, string appliedTreatment,
            decimal cost)
        {
            var key = $"{bridgeType}_{appliedTreatment}";
            if (!workedOnProj.ContainsKey(key))
            {
                workedOnProj.Add(key, (cost, 1));
            }
            else
            {
                var values = workedOnProj[key];
                values.bridgeCount += 1;
                values.treatmentCost += cost;
                workedOnProj[key] = values;
            }
        }

        private void AddKeyValue(Dictionary<string, int> completedProj, string bridgeType, string appliedTreatment)
        {
            var key = $"{bridgeType}_{appliedTreatment}";
            if (!completedProj.ContainsKey(key))
            {
                completedProj.Add(key, 1);
            }
            else
            {
                completedProj[key] += 1;
            }
        }

        #endregion Private methods
    }
}