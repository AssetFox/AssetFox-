using System;
using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.iAM.Analysis.Engine;
using AppliedResearchAssociates.iAM.Analysis;
using OfficeOpenXml;

using AppliedResearchAssociates.iAM.Reporting.Interfaces.PAMSSummaryReport;
using AppliedResearchAssociates.iAM.Reporting.Models.PAMSSummaryReport;
using AppliedResearchAssociates.iAM.DTOs.Enums;

namespace AppliedResearchAssociates.iAM.Reporting.Services.PAMSSummaryReport.PavementWorkSummary
{
    public class PavementWorkSummary : IPavementWorkSummary
    {
        private CostBudgetsWorkSummary _costBudgetsWorkSummary;
        private TreatmentsWorkSummary _treatmentsWorkSummary;
        private IriConditionSummary _iriConditionSummary;
        private OpiConditionSummary _opiConditionSummary;


        public PavementWorkSummary()
        {
            var workSummaryModel = new WorkSummaryModel();
            _costBudgetsWorkSummary = new CostBudgetsWorkSummary(workSummaryModel);
            if (_costBudgetsWorkSummary == null) { throw new ArgumentNullException(nameof(_costBudgetsWorkSummary)); }

            _treatmentsWorkSummary = new TreatmentsWorkSummary(workSummaryModel);
            if (_treatmentsWorkSummary == null) { throw new ArgumentNullException(nameof(_treatmentsWorkSummary)); }

            _iriConditionSummary = new IriConditionSummary(workSummaryModel);
            if (_treatmentsWorkSummary == null) { throw new ArgumentNullException(nameof(_treatmentsWorkSummary)); }

            _opiConditionSummary = new OpiConditionSummary(workSummaryModel);
            if (_treatmentsWorkSummary == null) { throw new ArgumentNullException(nameof(_treatmentsWorkSummary)); }
        }

        public ChartRowsModel Fill(
            ExcelWorksheet worksheet,
            SimulationOutput reportOutputData,
            List<int> simulationYears,
            WorkSummaryModel workSummaryModel,
            Dictionary<string, Budget> yearlyBudgetAmount,
            IReadOnlyCollection<SelectableTreatment> selectableTreatments)
        {
            var currentCell = new CurrentCell { Row = 1, Column = 1 };

            // Getting list of treatments. It will be used in several places throughout this excel TAB
            var simulationTreatments = new List<(string Name, AssetCategory AssetType, TreatmentCategory Category)>();

            foreach (var item in selectableTreatments)
            {
                //if (item.Name.ToLower() == BAMSConstants.NoTreatment) continue;
                simulationTreatments.Add((item.Name, item.AssetCategory, item.Category));
            }
            simulationTreatments.Sort((a, b) => a.Item1.CompareTo(b.Item1));


            var costAndLengthPerTreatmentPerYear = new Dictionary<int, Dictionary<string, (decimal treatmentCost, int length)>>();
            var costAndLengthPerTreatmentGroupPerYear = new Dictionary<int, Dictionary<PavementTreatmentHelper.TreatmentGroup, (decimal treatmentCost, int length)>>();

            //var yearlyCostCommittedProj = new Dictionary<int, Dictionary<string, (decimal treatmentCost, int bridgeCount)>>();
            //var countForCompletedProject = new Dictionary<int, Dictionary<string, int>>();
            //var countForCompletedCommittedProject = new Dictionary<int, Dictionary<string, int>>();

            FillDataToUseInExcel(reportOutputData, costAndLengthPerTreatmentPerYear, costAndLengthPerTreatmentGroupPerYear); //, yearlyCostCommittedProj, countForCompletedProject, countForCompletedCommittedProject);
            var workTypeTotals = CalculateWorkTypeTotals(costAndLengthPerTreatmentPerYear, simulationTreatments);


            //var costPerTreatmentPerYear = new Dictionary<int, Dictionary<string, decimal>>();
            _costBudgetsWorkSummary.FillCostBudgetWorkSummarySections(worksheet, currentCell, simulationYears, yearlyBudgetAmount, costAndLengthPerTreatmentPerYear, costAndLengthPerTreatmentGroupPerYear, simulationTreatments, workTypeTotals);

            //var segmentMilesPerTreatmentPerYear = new Dictionary<int, Dictionary<string, (decimal treatmentCost, int bridgeCount)>>();

            _treatmentsWorkSummary.FillTreatmentsWorkSummarySections(worksheet, currentCell, simulationYears, costAndLengthPerTreatmentPerYear, costAndLengthPerTreatmentGroupPerYear, simulationTreatments, workTypeTotals);

            _iriConditionSummary.FillIriConditionSummarySection(worksheet, currentCell, simulationYears);

            _opiConditionSummary.FillOpiConditionSummarySection(worksheet, currentCell, simulationYears);
            var chartRowsModel = new ChartRowsModel(); // TODO: Get this fromto-be-implemented functions

            worksheet.Calculate();
            worksheet.Cells.AutoFitColumns();

            return chartRowsModel;
        }

        #region Private methods


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


        // TODO: private methods are direct cut/paste from BridgeWorkSummary; refactor/delete as necessary
        private void FillDataToUseInExcel(
            SimulationOutput reportOutputData,
            //Dictionary<int, Dictionary<string, decimal>> costPerBPNPerYear,
            Dictionary<int, Dictionary<string, (decimal treatmentCost, int count)>> costAndLengthPerTreatmentPerYear,
            Dictionary<int, Dictionary<PavementTreatmentHelper.TreatmentGroup, (decimal treatmentCost, int length)>> costAndLengthPerTreatmentGroupPerYear
            //Dictionary<int, Dictionary<string, (decimal treatmentCost, int count)>> yearlyCostCommittedProj,
            //Dictionary<int, Dictionary<string, int>> countForCompletedProject,
            //Dictionary<int, Dictionary<string, int>> countForCompletedCommittedProject
            )
        {
            //TODO: This is just cut-and-paste from BAMS, need to fix it for PAMS.

            //var isInitialYear = true;
            foreach (var yearData in reportOutputData.Years)
            {
                costAndLengthPerTreatmentPerYear.Add(yearData.Year, new Dictionary<string, (decimal treatmentCost, int length)>());
                costAndLengthPerTreatmentGroupPerYear.Add(yearData.Year, new Dictionary<PavementTreatmentHelper.TreatmentGroup, (decimal treatmentCost, int length)>());

                foreach (var section in yearData.Assets)
                {
                    var cost = section.TreatmentConsiderations.Sum(_ => _.BudgetUsages.Sum(b => b.CoveredCost));
                    PopulateTreatmentCostAndLength(yearData.Year, section, cost, costAndLengthPerTreatmentPerYear);
                    PopulateTreatmentGroupCostAndLength(yearData.Year, section, cost, costAndLengthPerTreatmentGroupPerYear);

                    //if (section.TreatmentCause == TreatmentCause.CommittedProject &&
                    //    section.AppliedTreatment.ToLower() != PAMSConstants.NoTreatment)
                    //var commitedCost = section.TreatmentConsiderations.Sum(_ => _.BudgetUsages.Sum(b => b.CoveredCost));

                    //    if (!yearlyCostCommittedProj[yearData.Year].ContainsKey(section.AppliedTreatment))
                    //    {
                    //        yearlyCostCommittedProj[yearData.Year].Add(section.AppliedTreatment, (commitedCost, 1));
                    //    }
                    //    else
                    //    {
                    //        var treatmentCost = yearlyCostCommittedProj[yearData.Year][section.AppliedTreatment].treatmentCost + commitedCost;
                    //        var count = yearlyCostCommittedProj[yearData.Year][section.AppliedTreatment].count + 1;
                    //        yearlyCostCommittedProj[yearData.Year][section.AppliedTreatment] = (treatmentCost, count);
                    //    }

                    //    //costPerBPNPerYear[yearData.Year][section.ValuePerTextAttribute["BUS_PLAN_NETWORK"]] += commitedCost;

                    //    // Adding count for completed committed project
                    //    if (!countForCompletedCommittedProject[yearData.Year].ContainsKey(section.AppliedTreatment))
                    //    {
                    //        countForCompletedCommittedProject[yearData.Year].Add(section.AppliedTreatment, 1);
                    //    }
                    //    else
                    //    {
                    //        countForCompletedCommittedProject[yearData.Year][section.AppliedTreatment] += 1;
                    //    }

                    //PopulateCompletedProjectCount(yearData.Year, section, countForCompletedProject);

                    //RemoveBridgesForCashFlowedProj(countForCompletedProject, section, isInitialYear, yearData.Year);

                    // Fill cost per BPN per Year
                    //costPerBPNPerYear[yearData.Year][section.ValuePerTextAttribute["BUS_PLAN_NETWORK"]] += cost;
                }
                //isInitialYear = false;
            }
        }

        private void PopulateTreatmentCostAndLength(
            int year,
            AssetDetail section,
            decimal cost,
            Dictionary<int, Dictionary<string, (decimal treatmentCost, int PavementCount)>> costAndLengthPerTreatmentPerYear
            )
        {
            var segmentLength = section.ValuePerNumericAttribute["SEGMENT_LENGTH"];
            if (!costAndLengthPerTreatmentPerYear[year].ContainsKey(section.AppliedTreatment))
            {
                costAndLengthPerTreatmentPerYear[year].Add(section.AppliedTreatment, (cost, (int) segmentLength));
            }
            else
            {
                var values = costAndLengthPerTreatmentPerYear[year][section.AppliedTreatment];
                values.treatmentCost += cost;
                values.PavementCount += (int) segmentLength;
                costAndLengthPerTreatmentPerYear[year][section.AppliedTreatment] = values;
            }
        }

        private void PopulateTreatmentGroupCostAndLength(
            int year,
            AssetDetail section,
            decimal cost,
            Dictionary<int, Dictionary<PavementTreatmentHelper.TreatmentGroup, (decimal treatmentCost, int PavementCount)>> costAndLengthPerTreatmentPerYear
            )
        {
            var segmentLength = section.ValuePerNumericAttribute["SEGMENT_LENGTH"];

            var treatmentGroup = PavementTreatmentHelper.GetTreatmentGroup(section.AppliedTreatment);

            if (!costAndLengthPerTreatmentPerYear[year].ContainsKey(treatmentGroup))
            {
                costAndLengthPerTreatmentPerYear[year].Add(treatmentGroup, (cost, (int)segmentLength));
            }
            else
            {
                var values = costAndLengthPerTreatmentPerYear[year][treatmentGroup];
                values.treatmentCost += cost;
                values.PavementCount += (int)segmentLength;
                costAndLengthPerTreatmentPerYear[year][treatmentGroup] = values;
            }
        }

        private void RemovePavementsForCashFlowedProj(Dictionary<int, Dictionary<string, int>> countForCompletedProject,
            AssetDetail section, bool isInitialYear, int year)
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

        private void AddKeyValueForWorkedOn(Dictionary<string, (decimal treatmentCost, int PavementCount)> workedOnProj, string PavementType, string appliedTreatment,
            decimal cost)
        {
            var key = $"{PavementType}_{appliedTreatment}";
            if (!workedOnProj.ContainsKey(key))
            {
                workedOnProj.Add(key, (cost, 1));
            }
            else
            {
                var values = workedOnProj[key];
                values.PavementCount += 1;
                values.treatmentCost += cost;
                workedOnProj[key] = values;
            }
        }

        private void AddKeyValue(Dictionary<string, int> completedProj, string PavementType, string appliedTreatment)
        {
            var key = $"{PavementType}_{appliedTreatment}";
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
