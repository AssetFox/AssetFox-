using System;
using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.iAM.Analysis.Engine;
using AppliedResearchAssociates.iAM.Analysis;
using OfficeOpenXml;

using AppliedResearchAssociates.iAM.Reporting.Interfaces.PAMSSummaryReport;
using AppliedResearchAssociates.iAM.Reporting.Models.PAMSSummaryReport;

namespace AppliedResearchAssociates.iAM.Reporting.Services.PAMSSummaryReport.PavementWorkSummary
{
    public class PavementWorkSummary : IPavementWorkSummary
    {
        private CostBudgetsWorkSummary _costBudgetsWorkSummary;
        private TreatmentsWorkSummary _treatmentsWorkSummary;

        public PavementWorkSummary()
        {
            var workSummaryModel = new WorkSummaryModel();
            _costBudgetsWorkSummary = new CostBudgetsWorkSummary(workSummaryModel);
            if (_costBudgetsWorkSummary == null) { throw new ArgumentNullException(nameof(_costBudgetsWorkSummary)); }

            _treatmentsWorkSummary = new TreatmentsWorkSummary(workSummaryModel);
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
            //var yearlyCostCommittedProj = new Dictionary<int, Dictionary<string, (decimal treatmentCost, int bridgeCount)>>();
            //var countForCompletedProject = new Dictionary<int, Dictionary<string, int>>();
            //var countForCompletedCommittedProject = new Dictionary<int, Dictionary<string, int>>();

            FillDataToUseInExcel(reportOutputData, costAndLengthPerTreatmentPerYear); //, yearlyCostCommittedProj, countForCompletedProject, countForCompletedCommittedProject);


            //var costPerTreatmentPerYear = new Dictionary<int, Dictionary<string, decimal>>();
            _costBudgetsWorkSummary.FillCostBudgetWorkSummarySections(worksheet, currentCell, simulationYears, yearlyBudgetAmount, costAndLengthPerTreatmentPerYear, simulationTreatments);

            //var segmentMilesPerTreatmentPerYear = new Dictionary<int, Dictionary<string, (decimal treatmentCost, int bridgeCount)>>();

            _treatmentsWorkSummary.FillTreatmentsWorkSummarySections(worksheet, currentCell, simulationYears, costAndLengthPerTreatmentPerYear, simulationTreatments);

            var chartRowsModel = new ChartRowsModel(); // TODO: Get this fromto-be-implemented functions

            worksheet.Calculate();
            worksheet.Cells.AutoFitColumns();

            return chartRowsModel;
        }

        #region Private methods

        // TODO: private methods are direct cut/paste from BridgeWorkSummary; refactor/delete as necessary
        private void FillDataToUseInExcel(
            SimulationOutput reportOutputData,
            //Dictionary<int, Dictionary<string, decimal>> costPerBPNPerYear,
            Dictionary<int, Dictionary<string, (decimal treatmentCost, int count)>> costAndLengthPerTreatmentPerYear
            //Dictionary<int, Dictionary<string, (decimal treatmentCost, int count)>> yearlyCostCommittedProj,
            //Dictionary<int, Dictionary<string, int>> countForCompletedProject,
            //Dictionary<int, Dictionary<string, int>> countForCompletedCommittedProject
            )
        {
            //TODO: This is just cut-and-paste from BAMS, need to fix it for PAMS.

            //var isInitialYear = true;
            foreach (var yearData in reportOutputData.Years)
            {
                costAndLengthPerTreatmentPerYear.Add(yearData.Year, new Dictionary<string, (decimal treatmentCost, int count)>());
                //yearlyCostCommittedProj.Add(yearData.Year, new Dictionary<string, (decimal treatmentCost, int count)>());
                //costPerBPNPerYear.Add(yearData.Year, new Dictionary<string, decimal>());
                //countForCompletedProject.Add(yearData.Year, new Dictionary<string, int>());
                //countForCompletedCommittedProject.Add(yearData.Year, new Dictionary<string, int>());

                foreach (var section in yearData.Assets)
                {
                    var cost = section.TreatmentConsiderations.Sum(_ => _.BudgetUsages.Sum(b => b.CoveredCost));
                    PopulateWorkedOnCostAndLength(yearData.Year, section, costAndLengthPerTreatmentPerYear, cost);

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

        private void PopulateWorkedOnCostAndLength(int year, AssetDetail section,
            Dictionary<int, Dictionary<string, (decimal treatmentCost, int PavementCount)>> costAndLengthPerTreatmentPerYear, decimal cost)
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

        private void PopulateCompletedProjectCount(int year, AssetDetail section, Dictionary<int, Dictionary<string, int>> countForCompletedProject)
        {
            if (section.TreatmentCause == TreatmentCause.NoSelection)
            {
                //var culvert = BAMSConstants.CulvertPavementType;
                //// If Pavement type is culvert
                //if (section.ValuePerTextAttribute["Pavement_TYPE"] == culvert)
                //{
                //    AddKeyValue(countForCompletedProject[year], culvert, section.AppliedTreatment);
                //}
                //// If Pavement type is non culvert
                //else
                //{
                //    AddKeyValue(countForCompletedProject[year], BAMSConstants.NonCulvertPavementType, section.AppliedTreatment);
                //}
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
