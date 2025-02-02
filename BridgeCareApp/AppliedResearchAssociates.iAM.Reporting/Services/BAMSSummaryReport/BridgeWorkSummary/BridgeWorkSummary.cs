using System;
using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.iAM.Analysis.Engine;
using OfficeOpenXml;
using AppliedResearchAssociates.iAM.Reporting.Models.BAMSSummaryReport;
using AppliedResearchAssociates.iAM.DTOs.Enums;
using AppliedResearchAssociates.iAM.Reporting.Models;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DTOs.Abstract;
using AppliedResearchAssociates.iAM.DTOs;

namespace AppliedResearchAssociates.iAM.Reporting.Services.BAMSSummaryReport.BridgeWorkSummary
{
    public class BridgeWorkSummary
    {
        private CostBudgetsWorkSummary _costBudgetsWorkSummary;
        private BridgesCulvertsWorkSummary _bridgesCulvertsWorkSummary;
        private BridgeRateDeckAreaWorkSummary _bridgeRateDeckAreaWorkSummary;
        private NHSBridgeDeckAreaWorkSummary _nhsBridgeDeckAreaWorkSummary;
        private DeckAreaBridgeWorkSummary _deckAreaBridgeWorkSummary;
        private PostedClosedBridgeWorkSummary _postedClosedBridgeWorkSummary;
        private ProjectsCompletedCount _projectsCompletedCount;
        private ReportHelper _reportHelper;
        private readonly IUnitOfWork _unitOfWork;

        public BridgeWorkSummary(IList<string> Warnings, IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _bridgesCulvertsWorkSummary = new BridgesCulvertsWorkSummary(Warnings);
            var workSummaryModel = new WorkSummaryModel();
            _costBudgetsWorkSummary = new CostBudgetsWorkSummary(workSummaryModel);
            _bridgeRateDeckAreaWorkSummary = new BridgeRateDeckAreaWorkSummary(_unitOfWork);
            _nhsBridgeDeckAreaWorkSummary = new NHSBridgeDeckAreaWorkSummary(_unitOfWork);
            _deckAreaBridgeWorkSummary = new DeckAreaBridgeWorkSummary(_unitOfWork);
            _postedClosedBridgeWorkSummary = new PostedClosedBridgeWorkSummary(workSummaryModel, _unitOfWork);
            _projectsCompletedCount = new ProjectsCompletedCount(Warnings);            
            _reportHelper = new ReportHelper(_unitOfWork);
        }

        public ChartRowsModel Fill(ExcelWorksheet worksheet, SimulationOutput reportOutputData,
            List<int> simulationYears, WorkSummaryModel workSummaryModel, Dictionary<string, BudgetDTO> yearlyBudgets,
            List<TreatmentDTO> selectableTreatments, Dictionary<string, string> treatmentCategoryLookup,
            List<BaseCommittedProjectDTO> committedProjectsForWorkOutsideScope, bool shouldBundleFeasibleTreatments)
        {
            var currentCell = new CurrentCell { Row = 10, Column = 1 };

            #region Initial work to set some data, which will be used throughout the Work summary TAB

            // Getting list of treatments. It will be used in several places throughout this excel TAB
            var simulationTreatments = new List<(string Name, string AssetType, TreatmentCategory Category)>
            {
                (BAMSConstants.CulvertNoTreatment, "Culvert", TreatmentCategory.Other),
                (BAMSConstants.NonCulvertNoTreatment, "Bridge", TreatmentCategory.Other)
            };
            foreach (var item in selectableTreatments)
            {
                if (item.Name.ToLower() == BAMSConstants.NoTreatment) continue;
                var category = SummaryReportHelper.GetCategory(item.Category);
                simulationTreatments.Add((item.Name, item.AssetType, category));
            }
            simulationTreatments.Sort((a, b) => a.Item1.CompareTo(b.Item1));

            // cache to store total cost per treatment for a given year along with count of culvert
            // and non-culvert bridges
            var costPerBPNPerYear = new Dictionary<int, Dictionary<string, decimal>>();
            var costAndCountPerTreatmentPerYear = new Dictionary<int, Dictionary<string, (decimal treatmentCost, int bridgeCount)>>();
            var yearlyCostCommittedProj = new Dictionary<int, Dictionary<string, List<CommittedProjectMetaData>>>();
            var countForCompletedProject = new Dictionary<int, Dictionary<string, int>>();
            var countForCompletedCommittedProject = new Dictionary<int, Dictionary<string, int>>();
            FillDataToUseInExcel(reportOutputData, costPerBPNPerYear, costAndCountPerTreatmentPerYear, yearlyCostCommittedProj,
                countForCompletedProject, countForCompletedCommittedProject, treatmentCategoryLookup, committedProjectsForWorkOutsideScope, shouldBundleFeasibleTreatments);

            #endregion Initial work to set some data, which will be used throughout the Work summary TAB

            _costBudgetsWorkSummary.FillCostBudgetWorkSummarySections(worksheet, currentCell, costAndCountPerTreatmentPerYear, yearlyCostCommittedProj,
                simulationYears, yearlyBudgets, costPerBPNPerYear, simulationTreatments, committedProjectsForWorkOutsideScope, shouldBundleFeasibleTreatments);

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

        private void FillDataToUseInExcel(
         SimulationOutput reportOutputData, Dictionary<int, Dictionary<string, decimal>> costPerBPNPerYear,
         Dictionary<int, Dictionary<string, (decimal treatmentCost, int bridgeCount)>> costAndCountPerTreatmentPerYear,
         Dictionary<int, Dictionary<string, List<CommittedProjectMetaData>>> yearlyCostCommittedProj,
         Dictionary<int, Dictionary<string, int>> countForCompletedProject,
         Dictionary<int, Dictionary<string, int>> countForCompletedCommittedProject,
         Dictionary<string, string> treatmentCategoryLookup,
         List<BaseCommittedProjectDTO> committedProjectsForWorkOutsideScope,
         bool shouldBundleFeasibleTreatments)
        {
            var isInitialYear = true;
            Dictionary<double, List<TreatmentConsiderationDetail>> keyCashFlowFundingDetails = new();
            foreach (var yearData in reportOutputData.Years)
            {
                costAndCountPerTreatmentPerYear.Add(yearData.Year, new Dictionary<string, (decimal treatmentCost, int bridgeCount)>());
                yearlyCostCommittedProj[yearData.Year] = new Dictionary<string, List<CommittedProjectMetaData>>();
                costPerBPNPerYear.Add(yearData.Year, new Dictionary<string, decimal>());
                countForCompletedProject.Add(yearData.Year, new Dictionary<string, int>());
                countForCompletedCommittedProject.Add(yearData.Year, new Dictionary<string, int>());
                foreach (var section in yearData.Assets)
                {
                    var section_BRKEY = _reportHelper.CheckAndGetValue<double>(section.ValuePerNumericAttribute, "BRKEY_");
                    
                    //get business plan network
                    var busPlanNetwork = _reportHelper.CheckAndGetValue<string>(section.ValuePerTextAttribute, "BUS_PLAN_NETWORK");

                    if (!costPerBPNPerYear[yearData.Year].ContainsKey(busPlanNetwork))
                    {
                        costPerBPNPerYear[yearData.Year].Add(busPlanNetwork, 0);
                    }

                    // Build keyCashFlowFundingDetails                    
                    _reportHelper.BuildKeyCashFlowFundingDetails(yearData, section, section_BRKEY, keyCashFlowFundingDetails);

                    // If CF then use obj from keyCashFlowFundingDetails otherwise from section
                    var treatmentConsiderations = ((section.TreatmentCause == TreatmentCause.SelectedTreatment &&
                                                  section.TreatmentStatus == TreatmentStatus.Progressed) ||
                                                  (section.TreatmentCause == TreatmentCause.CashFlowProject &&
                                                  section.TreatmentStatus == TreatmentStatus.Progressed) ||
                                                  (section.TreatmentCause == TreatmentCause.CashFlowProject &&
                                                  section.TreatmentStatus == TreatmentStatus.Applied)) ?
                                                  keyCashFlowFundingDetails[section_BRKEY] :
                                                  section.TreatmentConsiderations ?? new();

                    var treatmentConsideration = shouldBundleFeasibleTreatments ?
                                                 treatmentConsiderations.FirstOrDefault(_ => _.FundingCalculationOutput != null &&
                                                    _.FundingCalculationOutput.AllocationMatrix.Any(_ => _.Year == yearData.Year) &&
                                                    section.AppliedTreatment.Contains(_.TreatmentName)) :
                                                 treatmentConsiderations.FirstOrDefault(_ => _.FundingCalculationOutput != null &&
                                                    _.FundingCalculationOutput.AllocationMatrix.Any(_ => _.Year == yearData.Year) &&
                                                    _.TreatmentName == section.AppliedTreatment);

                    var appliedTreatment = treatmentConsideration?.TreatmentName ?? section.AppliedTreatment;
                    var treatmentCategory = appliedTreatment.Contains("Bundle") ? BAMSConstants.Bundled : treatmentCategoryLookup[appliedTreatment];
                    var cost = treatmentConsideration?.FundingCalculationOutput?.AllocationMatrix?.
                               Where(_ => _.Year == yearData.Year).
                               Sum(b => b.AllocatedAmount) ?? 0;
                    cost = Math.Round(cost, 0);
                    if (section.TreatmentCause == TreatmentCause.CommittedProject &&
                        appliedTreatment.ToLower() != BAMSConstants.NoTreatment)
                    {
                        var committedCost = cost;
                        var committedProject = committedProjectsForWorkOutsideScope.FirstOrDefault(_ => appliedTreatment.Contains(_.Treatment) &&
                                                _.Year == yearData.Year && _.ProjectSource.ToString() == section.ProjectSource);
                        var projectSource = committedProject?.ProjectSource.ToString();
                        if (!yearlyCostCommittedProj[yearData.Year].ContainsKey(appliedTreatment))
                        {                            
                            var committedProjectMetaData = new List<CommittedProjectMetaData>() {
                                                                new() { TreatmentCost = committedCost,
                                                                    ProjectSource = projectSource,                                                                                            TreatmentCategory = treatmentCategory
                                                                }};
                            yearlyCostCommittedProj[yearData.Year].Add(appliedTreatment, committedProjectMetaData);
                        }
                        else
                        {
                            yearlyCostCommittedProj[yearData.Year][appliedTreatment].Add(new()
                            {
                                TreatmentCost = committedCost,
                                ProjectSource = projectSource,
                                TreatmentCategory = treatmentCategory // TODO Should this be committed proj's category in future?
                            });
                        }
                 
                        costPerBPNPerYear[yearData.Year][busPlanNetwork] += committedCost;

                        // Adding count for completed committed project
                        if (!countForCompletedCommittedProject[yearData.Year].ContainsKey(appliedTreatment))
                        {
                            countForCompletedCommittedProject[yearData.Year].Add(appliedTreatment, 1);
                        }
                        else
                        {
                            countForCompletedCommittedProject[yearData.Year][appliedTreatment] += 1;
                        }

                        // Remove from committedProjectsForWorkOutsideScope
                        // Bundled treatments have many treatment names under AppliedTreatment
                        var toRemove = committedProjectsForWorkOutsideScope.Where(_ => appliedTreatment.Contains(_.Treatment) &&
                                        _.Year == yearData.Year &&
                                        _.ProjectSource.ToString() == section.ProjectSource &&
                                        Math.Round(_.Cost, 0) == Convert.ToDouble(cost));
                        if (toRemove != null)
                        {
                            committedProjectsForWorkOutsideScope.RemoveAll(_ => toRemove.Contains(_));
                        }

                        continue;
                    }
                    //[TODO] - ask Jake regarding cash flow project. It won't have anything in the TreartmentOptions barring 1st year
                                        
                    PopulateWorkedOnCostAndCount(yearData.Year, section, costAndCountPerTreatmentPerYear, cost);

                    PopulateCompletedProjectCount(yearData.Year, section, countForCompletedProject);

                    RemoveBridgesForCashFlowedProj(countForCompletedProject, section, isInitialYear, yearData.Year);

                    // Fill cost per BPN per Year
                    costPerBPNPerYear[yearData.Year][busPlanNetwork] += cost;
                }
                isInitialYear = false;
            }
        }

        private void PopulateWorkedOnCostAndCount(int year, AssetDetail section,
            Dictionary<int, Dictionary<string, (decimal treatmentCost, int bridgeCount)>> costAndCountPerTreatmentPerYear, decimal cost)
        {
            if (section.TreatmentCause == TreatmentCause.NoSelection)
            {
                var culvert = BAMSConstants.CulvertBridgeType;
                var nonCulvert = BAMSConstants.NonCulvertBridgeType;
                // If Bridge type is culvert
                if (_reportHelper.CheckAndGetValue<string>(section.ValuePerTextAttribute, "BRIDGE_TYPE") == culvert)
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
                var appliedTreatment = section.AppliedTreatment;
                if (appliedTreatment.Contains("Bundle"))
                {
                    appliedTreatment = BAMSConstants.Bundled;
                }
                if (!costAndCountPerTreatmentPerYear[year].ContainsKey(appliedTreatment))
                {
                    costAndCountPerTreatmentPerYear[year].Add(appliedTreatment, (cost, 1));
                }
                else
                {
                    var values = costAndCountPerTreatmentPerYear[year][appliedTreatment];
                    values.treatmentCost += cost;
                    values.bridgeCount += 1;
                    costAndCountPerTreatmentPerYear[year][appliedTreatment] = values;
                }
            }
        }

        private void PopulateCompletedProjectCount(int year, AssetDetail section, Dictionary<int, Dictionary<string, int>> countForCompletedProject)
        {
            if (section.TreatmentCause == TreatmentCause.NoSelection)
            {
                var culvert = BAMSConstants.CulvertBridgeType;
                // If Bridge type is culvert
                if (_reportHelper.CheckAndGetValue<string>(section.ValuePerTextAttribute, "BRIDGE_TYPE") == culvert)
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
                var appliedTreatment = section.AppliedTreatment;
                if (appliedTreatment.Contains("Bundle"))
                {                    
                    appliedTreatment = BAMSConstants.Bundled;
                }
                if (!countForCompletedProject[year].ContainsKey(appliedTreatment))
                {
                    countForCompletedProject[year].Add(appliedTreatment, 1);
                }
                else
                {
                    countForCompletedProject[year][appliedTreatment] += 1;
                }
            }
        }

        private void RemoveBridgesForCashFlowedProj(Dictionary<int, Dictionary<string, int>> countForCompletedProject,
            AssetDetail section, bool isInitialYear, int year)
        {
            // to store "Projects completed"
            if (section.TreatmentCause == TreatmentCause.CashFlowProject && !isInitialYear)
            {
                // if current year status is TreatmentCause.CashFlowProject, then the previous year
                // is either 1st year of cashflow or somewhere in between, in both cases, we will
                // remove the previous year project as it has not been conmleted.
                var appliedTreatment = section.AppliedTreatment;
                if (appliedTreatment.Contains("Bundle"))
                {
                    appliedTreatment = BAMSConstants.Bundled;
                }
                countForCompletedProject[year - 1][appliedTreatment] -= 1;
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
