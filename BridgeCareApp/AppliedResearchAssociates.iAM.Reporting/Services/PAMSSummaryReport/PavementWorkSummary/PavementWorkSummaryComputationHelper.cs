using System;
using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.iAM.Analysis;
using AppliedResearchAssociates.iAM.Analysis.Engine;
using AppliedResearchAssociates.iAM.DTOs.Enums;
using AppliedResearchAssociates.iAM.Reporting.Models.PAMSSummaryReport;
using OfficeOpenXml.FormulaParsing.Excel.Functions.DateTime;
using static System.Collections.Specialized.BitVector32;

namespace AppliedResearchAssociates.iAM.Reporting.Services.PAMSSummaryReport.PavementWorkSummary
{
    public static class SegmentLengthConvertorExtension
    {
        public static int FeetToMiles(this int feet) => feet / 5280;
        public static double FeetToMiles(this double feet) => feet / 5280.0;
        public static decimal FeetToMiles(this decimal feet) => feet / (decimal) 5280.0;
    }

    public static class PavementConditionExtensions
    {
        private static SummaryReportHelper _summaryReportHelper = new SummaryReportHelper();

        // InRange excludes the high value to avoid overlap
        private static bool InRange(this double value, double low, double high) =>
            low <= value && value < high;

        private static double IriCondition(this AssetDetail detail) =>
            _summaryReportHelper.checkAndGetValue<double>(detail.ValuePerNumericAttribute, "ROUGHNESS");

        public static bool IriConditionIsExcellent(this AssetDetail detail) =>
            detail.IriCondition().InRange(0, 60);

        public static bool IriConditionIsGood(this AssetDetail detail) =>
            detail.IriCondition().InRange(60, 95);

        public static bool IriConditionIsFair(this AssetDetail detail) =>
            detail.IriCondition().InRange(95, 170);

        public static bool IriConditionIsPoor(this AssetDetail detail) =>
            detail.IriCondition() >= 170;

        private static double OpiCondition(this AssetDetail detail) =>
            _summaryReportHelper.checkAndGetValue<double>(detail.ValuePerNumericAttribute, "OPI");

        public static bool OpiConditionIsExcellent(this AssetDetail detail) =>
            detail.OpiCondition().InRange(90, 100);

        public static bool OpiConditionIsGood(this AssetDetail detail) =>
            detail.OpiCondition().InRange(75, 90);

        public static bool OpiConditionIsFair(this AssetDetail detail) =>
            detail.OpiCondition().InRange(60, 75);

        public static bool OpiConditionIsPoor(this AssetDetail detail) =>
            detail.OpiCondition() < 60;
    }

    public class PavementWorkSummaryComputationHelper
    {
        private SummaryReportHelper _summaryReportHelper;

        public PavementWorkSummaryComputationHelper()
        {
            _summaryReportHelper = new SummaryReportHelper();
        }

        internal double CalculateSegmentMilesForBPNWithCondition(List<AssetSummaryDetail> initialSectionSummaries, string bpn, Func<AssetSummaryDetail, bool> conditionFunction)
        {
            var postedSegments = !String.IsNullOrEmpty(bpn) ? initialSectionSummaries.FindAll(b => _summaryReportHelper.checkAndGetValue<string>(b.ValuePerTextAttribute, "BUSIPLAN") == bpn) : initialSectionSummaries;
            var selectedSegments = postedSegments.FindAll(section => conditionFunction(section));
            return selectedSegments.Sum(_ => _summaryReportHelper.checkAndGetValue<double>(_.ValuePerNumericAttribute, "SEGMENT_LENGTH")).FeetToMiles();
        }

        internal double CalculateSegmentMilesForBPNWithCondition(List<AssetDetail> sectionSummaries, string bpn, Func<AssetDetail, bool> conditionFunction)
        {
            var postedSegments = !String.IsNullOrEmpty(bpn) ? sectionSummaries.FindAll(b => _summaryReportHelper.checkAndGetValue<string>(b.ValuePerTextAttribute, "BUSIPLAN") == bpn) : sectionSummaries;
            var selectedSegments = postedSegments.FindAll(section => conditionFunction(section));
            return selectedSegments.Sum(_ => _summaryReportHelper.checkAndGetValue<double>(_.ValuePerNumericAttribute, "SEGMENT_LENGTH")).FeetToMiles();
        }


        internal void FillDataToUseInExcel(
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

        internal void FillDataToUseInExcel(
            WorkSummaryByBudgetModel budgetSummaryModel,
            Dictionary<int, Dictionary<string, (decimal treatmentCost, int count)>> costAndLengthPerTreatmentPerYear,
            Dictionary<int, Dictionary<PavementTreatmentHelper.TreatmentGroup, (decimal treatmentCost, int length)>> costAndLengthPerTreatmentGroupPerYear
            )
        {
            foreach (var yearsData in budgetSummaryModel.YearlyData)
            {
                if (!costAndLengthPerTreatmentPerYear.ContainsKey(yearsData.Year))
                {
                    costAndLengthPerTreatmentPerYear.Add(yearsData.Year, new Dictionary<string, (decimal treatmentCost, int length)>());
                }
                var treatmentData = costAndLengthPerTreatmentPerYear[yearsData.Year];

                if (!costAndLengthPerTreatmentGroupPerYear.ContainsKey(yearsData.Year))
                {
                    costAndLengthPerTreatmentGroupPerYear.Add(yearsData.Year, new Dictionary<PavementTreatmentHelper.TreatmentGroup, (decimal treatmentCost, int length)>());
                }
                var treatmentGroupData = costAndLengthPerTreatmentGroupPerYear[yearsData.Year];

                PopulateTreatmentCostAndLength(yearsData, costAndLengthPerTreatmentPerYear);
                PopulateTreatmentGroupCostAndLength(yearsData, costAndLengthPerTreatmentGroupPerYear);
            }
        }
        private void PopulateTreatmentCostAndLength(
            YearsData yearsData,
            Dictionary<int, Dictionary<string, (decimal treatmentCost, int length)>> costAndLengthPerTreatmentPerYear
            )
        {
            int segmentLength = 0;

            if (!costAndLengthPerTreatmentPerYear[yearsData.Year].ContainsKey(yearsData.TreatmentName))
            {
                costAndLengthPerTreatmentPerYear[yearsData.Year].Add(yearsData.TreatmentName, ((decimal)yearsData.Amount, (int)segmentLength));
            }
        }

        private void PopulateTreatmentGroupCostAndLength(
            YearsData yearsData,
            Dictionary<int, Dictionary<PavementTreatmentHelper.TreatmentGroup, (decimal treatmentCost, int length)>> costAndLengthPerTreatmentPerYear
            )
        {
            var year = yearsData.Year;
            var treatmentGroup = PavementTreatmentHelper.GetTreatmentGroup(yearsData.TreatmentName);
            int segmentLength = 0;

            if (!costAndLengthPerTreatmentPerYear[year].ContainsKey(treatmentGroup))
            {
                costAndLengthPerTreatmentPerYear[year].Add(treatmentGroup, ((decimal)yearsData.Amount, (int)segmentLength));
            }
            else
            {
                var values = costAndLengthPerTreatmentPerYear[year][treatmentGroup];
                values.treatmentCost += (decimal) yearsData.Amount;
                values.length += (int)segmentLength;
                costAndLengthPerTreatmentPerYear[year][treatmentGroup] = values;
            }
        }


        private void PopulateTreatmentCostAndLength(
            int year,
            AssetDetail section,
            decimal cost,
            Dictionary<int, Dictionary<string, (decimal treatmentCost, int length)>> costAndLengthPerTreatmentPerYear
            )
        {
            var segmentLength = section.ValuePerNumericAttribute["SEGMENT_LENGTH"];
            if (!costAndLengthPerTreatmentPerYear[year].ContainsKey(section.AppliedTreatment))
            {
                costAndLengthPerTreatmentPerYear[year].Add(section.AppliedTreatment, (cost, (int)segmentLength));
            }
            else
            {
                var values = costAndLengthPerTreatmentPerYear[year][section.AppliedTreatment];
                values.treatmentCost += cost;
                values.length += (int)segmentLength;
                costAndLengthPerTreatmentPerYear[year][section.AppliedTreatment] = values;
            }
        }

        private void PopulateTreatmentGroupCostAndLength(
            int year,
            AssetDetail section,
            decimal cost,
            Dictionary<int, Dictionary<PavementTreatmentHelper.TreatmentGroup, (decimal treatmentCost, int length)>> costAndLengthPerTreatmentPerYear
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
                values.length += (int)segmentLength;
                costAndLengthPerTreatmentPerYear[year][treatmentGroup] = values;
            }
        }

        internal Dictionary<TreatmentCategory, SortedDictionary<int, (decimal treatmentCost, int length)>> CalculateWorkTypeTotals(
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

    }
}
