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
                simulationTreatments.Add((item.Name, item.AssetCategory, item.Category));
            }
            simulationTreatments.Sort((a, b) => a.Item1.CompareTo(b.Item1));


            var costAndLengthPerTreatmentPerYear = new Dictionary<int, Dictionary<string, (decimal treatmentCost, int length)>>();
            var costAndLengthPerTreatmentGroupPerYear = new Dictionary<int, Dictionary<PavementTreatmentHelper.TreatmentGroup, (decimal treatmentCost, int length)>>();

            FillDataToUseInExcel(reportOutputData, costAndLengthPerTreatmentPerYear, costAndLengthPerTreatmentGroupPerYear);
            var workTypeTotals = CalculateWorkTypeTotals(costAndLengthPerTreatmentPerYear, simulationTreatments);

            var chartRowsModel = _costBudgetsWorkSummary.FillCostBudgetWorkSummarySections(worksheet, currentCell, simulationYears, yearlyBudgetAmount, costAndLengthPerTreatmentPerYear, costAndLengthPerTreatmentGroupPerYear, simulationTreatments, workTypeTotals);
            chartRowsModel = _treatmentsWorkSummary.FillTreatmentsWorkSummarySections(worksheet, currentCell, simulationYears, costAndLengthPerTreatmentPerYear, costAndLengthPerTreatmentGroupPerYear, simulationTreatments, workTypeTotals, chartRowsModel);
            chartRowsModel = _iriConditionSummary.FillIriConditionSummarySection(worksheet, currentCell, reportOutputData, chartRowsModel);
            chartRowsModel = _opiConditionSummary.FillOpiConditionSummarySection(worksheet, currentCell, reportOutputData, chartRowsModel);

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


        private void FillDataToUseInExcel(
            SimulationOutput reportOutputData,
            Dictionary<int, Dictionary<string, (decimal treatmentCost, int count)>> costAndLengthPerTreatmentPerYear,
            Dictionary<int, Dictionary<PavementTreatmentHelper.TreatmentGroup, (decimal treatmentCost, int length)>> costAndLengthPerTreatmentGroupPerYear
            )
        {
            foreach (var yearData in reportOutputData.Years)
            {
                costAndLengthPerTreatmentPerYear.Add(yearData.Year, new Dictionary<string, (decimal treatmentCost, int length)>());
                costAndLengthPerTreatmentGroupPerYear.Add(yearData.Year, new Dictionary<PavementTreatmentHelper.TreatmentGroup, (decimal treatmentCost, int length)>());

                foreach (var section in yearData.Assets)
                {
                    var cost = section.TreatmentConsiderations.Sum(_ => _.BudgetUsages.Sum(b => b.CoveredCost));
                    PopulateTreatmentCostAndLength(yearData.Year, section, cost, costAndLengthPerTreatmentPerYear);
                    PopulateTreatmentGroupCostAndLength(yearData.Year, section, cost, costAndLengthPerTreatmentGroupPerYear);

                }
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


        #endregion Private methods
    }
}
