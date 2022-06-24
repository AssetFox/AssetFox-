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

        public PavementWorkSummary()
        {

        }

        public ChartRowsModel Fill(ExcelWorksheet worksheet, SimulationOutput reportOutputData)
        {
            var currentCell = new CurrentCell { Row = 1, Column = 1 };

            worksheet.Calculate();
            worksheet.Cells.AutoFitColumns();

            var chartRowsModel = new ChartRowsModel();

            return chartRowsModel;
        }

        //public ChartRowsModel Fill(ExcelWorksheet worksheet, SimulationOutput reportOutputData,
        //    List<int> simulationYears, WorkSummaryModel workSummaryModel, Dictionary<string, Budget> yearlyBudgetAmount,
        //    IReadOnlyCollection<SelectableTreatment> selectableTreatments)
        //{
        //    var currentCell = new CurrentCell { Row = 1, Column = 1 };

        //    worksheet.Calculate();
        //    worksheet.Cells.AutoFitColumns();

        //    var chartRowsModel = new ChartRowsModel();

        //    return chartRowsModel;
        //}

        #region Private methods

        private void FillDataToUseInExcel()
        {
 
        }

        private void PopulateWorkedOnCostAndCount(int year, AssetDetail section,
            Dictionary<int, Dictionary<string, (decimal treatmentCost, int PavementCount)>> costAndCountPerTreatmentPerYear, decimal cost)
        {
            if (section.TreatmentCause == TreatmentCause.NoSelection)
            {
                //var culvert = BAMSConstants.CulvertPavementType;
                //var nonCulvert = BAMSConstants.NonCulvertPavementType;
                //// If Pavement type is culvert
                //if (section.ValuePerTextAttribute["Pavement_TYPE"] == culvert)
                //{
                //    AddKeyValueForWorkedOn(costAndCountPerTreatmentPerYear[year], culvert, section.AppliedTreatment, cost);
                //}
                //// if Pavement is non-culvert
                //else
                //{
                //    AddKeyValueForWorkedOn(costAndCountPerTreatmentPerYear[year], nonCulvert, section.AppliedTreatment, cost);
                //}
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
                    values.PavementCount += 1;
                    costAndCountPerTreatmentPerYear[year][section.AppliedTreatment] = values;
                }
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
