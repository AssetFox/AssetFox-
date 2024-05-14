using System;
using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.iAM.Analysis.Engine;
using AppliedResearchAssociates.iAM.DTOs.Abstract;
using AppliedResearchAssociates.iAM.DTOs.Enums;
using static AppliedResearchAssociates.iAM.Reporting.Services.PAMSSummaryReport.PavementWorkSummary.PavementTreatmentHelper;
using WorkSummaryByBudgetModel = AppliedResearchAssociates.iAM.Reporting.Models.PAMSSummaryReport.WorkSummaryByBudgetModel;
using YearsData = AppliedResearchAssociates.iAM.Reporting.Models.PAMSSummaryReport.YearsData;

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
           low < value && value <= high;

        private static double IriCondition(this AssetDetail detail) =>
            _summaryReportHelper.checkAndGetValue<double>(detail.ValuePerNumericAttribute, "ROUGHNESS");

        public static bool IriConditionIsExcellent(this AssetDetail detail, string bpnKey) =>
            bpnKey switch
            {
                "1" => detail.IriCondition() <= 70,
                "2" => detail.IriCondition() <= 75,
                "3" => detail.IriCondition() <= 100,
                "4" => detail.IriCondition() <= 120,
                _ => false,
            };

        public static bool IriConditionIsGood(this AssetDetail detail, string bpnKey) =>
            bpnKey switch
            {
                "1" => detail.IriCondition().InRange(70, 100),
                "2" => detail.IriCondition().InRange(75, 120),
                "3" => detail.IriCondition().InRange(100, 150),
                "4" => detail.IriCondition().InRange(120, 170),
                _ => false,
            };

        public static bool IriConditionIsFair(this AssetDetail detail, string bpnKey) =>
            bpnKey switch
            {
                "1" => detail.IriCondition().InRange(100, 150),
                "2" => detail.IriCondition().InRange(120, 170),
                "3" => detail.IriCondition().InRange(150, 195),
                "4" => detail.IriCondition().InRange(170, 220),
                _ => false,
            };

        public static bool IriConditionIsPoor(this AssetDetail detail, string bpnKey) =>
            bpnKey switch
            {
                "1" => detail.IriCondition() > 150,
                "2" => detail.IriCondition() > 170,
                "3" => detail.IriCondition() > 195,
                "4" => detail.IriCondition() > 220,
                _ => false,
            };
        
        private static double OpiCondition(this AssetDetail detail) =>
            _summaryReportHelper.checkAndGetValue<double>(detail.ValuePerNumericAttribute, "OPI_CALCULATED");
                
        public static bool OpiConditionIsExcellent(this AssetDetail detail, string bpnKey) =>
            bpnKey switch
            {
                "1" => detail.OpiCondition() > 95,
                "2" => detail.OpiCondition() > 95,
                "3" => detail.OpiCondition() > 90,
                "4" => detail.OpiCondition() > 85,
                _ => false,
            };

        public static bool OpiConditionIsGood(this AssetDetail detail, string bpnKey) =>
            bpnKey switch
            {
                "1" => detail.OpiCondition().InRange(85, 95),
                "2" => detail.OpiCondition().InRange(80, 95),
                "3" => detail.OpiCondition().InRange(80, 90),
                "4" => detail.OpiCondition().InRange(70, 85),
                _ => false,
            };

        public static bool OpiConditionIsFair(this AssetDetail detail, string bpnKey) =>
            bpnKey switch
            {
                "1" => detail.OpiCondition().InRange(75, 85),
                "2" => detail.OpiCondition().InRange(70, 80),
                "3" => detail.OpiCondition().InRange(65, 80),
                "4" => detail.OpiCondition().InRange(59, 70),
                _ => false,
            };

        public static bool OpiConditionIsPoor(this AssetDetail detail, string bpnKey) =>
            bpnKey switch
            {
                "1" => detail.OpiCondition() <= 75,
                "2" => detail.OpiCondition() <= 70,
                "3" => detail.OpiCondition() <= 65,
                "4" => detail.OpiCondition() < 60,
                _ => false,
            };
    }

    public class PavementWorkSummaryComputationHelper
    {
        private SummaryReportHelper _summaryReportHelper;

        public PavementWorkSummaryComputationHelper()
        {
            _summaryReportHelper = new SummaryReportHelper();
        }

        internal double CalculateSegmentMilesForBPNWithCondition(List<AssetDetail> sectionSummaries, string bpn, Func<AssetDetail, bool> conditionFunction)
        {
            var postedSegments = !string.IsNullOrEmpty(bpn) ? sectionSummaries.FindAll(b => _summaryReportHelper.checkAndGetValue<string>(b.ValuePerTextAttribute, "BUSIPLAN") == bpn) : sectionSummaries;
            var selectedSegments = postedSegments.FindAll(section => conditionFunction(section));
            return selectedSegments.Sum(_ => _summaryReportHelper.checkAndGetValue<double>(_.ValuePerNumericAttribute, "SEGMENT_LENGTH")).FeetToMiles();
        }

        internal void FillDataToUseInExcel(SimulationOutput reportOutputData,
                Dictionary<int, Dictionary<string, (decimal treatmentCost, int bridgeCount, string projectSource, string treatmentCategory)>> yearlyCostCommittedProj,
                Dictionary<int, Dictionary<string, Dictionary<int, (decimal treatmentCost, decimal compositeTreatmentCost, int length)>>> costLengthPerSurfaceIdPerTreatmentPerYear,
                Dictionary<int, Dictionary<TreatmentGroup, (decimal treatmentCost, int length)>> costAndLengthPerTreatmentGroupPerYear,
                Dictionary<string, string> treatmentCategoryLookup,
                List<BaseCommittedProjectDTO> committedProjectsForWorkOutsideScope,
                List<(string Name, string AssetType, TreatmentCategory Category)> simulationTreatments)
        {
            foreach (var yearData in reportOutputData.Years)
            {
                costLengthPerSurfaceIdPerTreatmentPerYear.Add(yearData.Year, new Dictionary<string, Dictionary<int, (decimal treatmentCost, decimal compositeTreatmentCost, int length)>>());
                costAndLengthPerTreatmentGroupPerYear.Add(yearData.Year, new Dictionary<TreatmentGroup, (decimal treatmentCost, int length)>());
                yearlyCostCommittedProj[yearData.Year] = new Dictionary<string, (decimal treatmentCost, int bridgeCount, string projectSource, string treatmentCategory)>();
                foreach (var section in yearData.Assets)
                {
                    var cost = section.TreatmentConsiderations.Sum(_ => _.FundingCalculationOutput?.AllocationMatrix.Sum(b => b.AllocatedAmount) ?? 0);                    
                    var appliedTreatment = section.AppliedTreatment;
                    var treatmentCategory = section.AppliedTreatment.Contains("Bundle") ? PAMSConstants.Bundled : treatmentCategoryLookup[appliedTreatment];

                    if (section.TreatmentCause == TreatmentCause.CommittedProject &&
                        appliedTreatment.ToLower() != PAMSConstants.NoTreatment)
                    {
                        if (!yearlyCostCommittedProj[yearData.Year].ContainsKey(appliedTreatment))
                        {
                            yearlyCostCommittedProj[yearData.Year].Add(appliedTreatment, (cost, 1, section.ProjectSource, treatmentCategory));
                        }
                        else
                        {
                            var currentRecord = yearlyCostCommittedProj[yearData.Year][appliedTreatment];
                            var treatmentCost = currentRecord.treatmentCost + cost;
                            var bridgeCount = currentRecord.bridgeCount + 1;
                            var projectSource = currentRecord.projectSource;
                            yearlyCostCommittedProj[yearData.Year][appliedTreatment] = (treatmentCost, bridgeCount, projectSource, treatmentCategory);
                        }

                        // Remove from committedProjectsForWorkOutsideScope
                        var toRemove = committedProjectsForWorkOutsideScope.Where(_ => appliedTreatment.Contains(_.Treatment)); // Bundled has many treatment names under AppliedTreatment
                        if (toRemove != null)
                        {
                            committedProjectsForWorkOutsideScope.RemoveAll(_ => toRemove.Contains(_));
                        }
                        
                        continue;
                    }

                    PopulateTreatmentCostAndLength(yearData.Year, section, cost, costLengthPerSurfaceIdPerTreatmentPerYear);
                    PopulateTreatmentGroupCostAndLength(yearData.Year, section, cost, costAndLengthPerTreatmentGroupPerYear, simulationTreatments);
                }
            }
        }

        internal void FillDataToUseInExcel(WorkSummaryByBudgetModel budgetSummaryModel,
            Dictionary<int, Dictionary<string, Dictionary<int, (decimal treatmentCost, decimal compositeTreatmentCost, int length)>>> costLengthPerSurfaceIdPerTreatmentPerYear,
            Dictionary<int, Dictionary<TreatmentGroup, (decimal treatmentCost, int length)>> costAndLengthPerTreatmentGroupPerYear,
            List<(string TreatmentName, string AssetType, TreatmentCategory Category)> simulationTreatments
            )
        {
            foreach (var yearData in budgetSummaryModel.YearlyData)
            {
                if (!costLengthPerSurfaceIdPerTreatmentPerYear.ContainsKey(yearData.Year))
                {
                    costLengthPerSurfaceIdPerTreatmentPerYear.Add(yearData.Year, new Dictionary<string, Dictionary<int, (decimal treatmentCost, decimal compositeTreatmentCost, int length)>>());
                }
                var treatmentData = costLengthPerSurfaceIdPerTreatmentPerYear[yearData.Year];

                if (!costAndLengthPerTreatmentGroupPerYear.ContainsKey(yearData.Year))
                {
                    costAndLengthPerTreatmentGroupPerYear.Add(yearData.Year, new Dictionary<TreatmentGroup, (decimal treatmentCost, int length)>());
                }
                var treatmentGroupData = costAndLengthPerTreatmentGroupPerYear[yearData.Year];

                if (!yearData.isCommitted)
                {
                    PopulateTreatmentCostAndLength(yearData, costLengthPerSurfaceIdPerTreatmentPerYear);
                    PopulateTreatmentGroupCostAndLength(yearData, costAndLengthPerTreatmentGroupPerYear, simulationTreatments);
                }
            }
        }
        private void PopulateTreatmentCostAndLength(
            YearsData yearsData,
            Dictionary<int, Dictionary<string, Dictionary<int, (decimal treatmentCost, decimal compositeTreatmentCost, int length)>>> costLengthPerSurfaceIdPerTreatmentPerYear
            )
        {
            int segmentLength = 0;
            var year = yearsData.Year;
            var appliedTreatment = yearsData.TreatmentName;
            var surfaceId = yearsData.SurfaceId;
            var cost = (decimal)yearsData.Amount;
            var compositeTreatmentCost = surfaceId == 62 ? cost : 0;
            if (!costLengthPerSurfaceIdPerTreatmentPerYear[yearsData.Year].ContainsKey(yearsData.TreatmentName))
            {
                costLengthPerSurfaceIdPerTreatmentPerYear[year].Add(appliedTreatment,
                    new() { { surfaceId, (cost, compositeTreatmentCost, segmentLength) } });
            }        
            else
            {                
                var values = costLengthPerSurfaceIdPerTreatmentPerYear[year][appliedTreatment];
                if (!values.ContainsKey(surfaceId))
                {
                    values.Add(surfaceId, (cost, compositeTreatmentCost, segmentLength));

                    costLengthPerSurfaceIdPerTreatmentPerYear[year][appliedTreatment] = values;
                }
                else
                {
                    var surfaceIdValues = values[surfaceId];
                    surfaceIdValues.treatmentCost += cost;
                    surfaceIdValues.compositeTreatmentCost += compositeTreatmentCost;

                    values[surfaceId] = surfaceIdValues;
                }
            }
        }

        private void PopulateTreatmentGroupCostAndLength(
            YearsData yearsData,
            Dictionary<int, Dictionary<TreatmentGroup, (decimal treatmentCost, int length)>> costAndLengthPerTreatmentPerYear,
            List<(string TreatmentName, string AssetType, TreatmentCategory Category)> simulationTreatments)
        {
            var year = yearsData.Year;
            var treatmentGroup = GetTreatmentGroup(yearsData.TreatmentName, simulationTreatments);
            int segmentLength = 0;

            if (!costAndLengthPerTreatmentPerYear[year].ContainsKey(treatmentGroup))
            {
                costAndLengthPerTreatmentPerYear[year].Add(treatmentGroup, ((decimal)yearsData.Amount, (int)segmentLength.FeetToMiles()));
            }
            else
            {
                var values = costAndLengthPerTreatmentPerYear[year][treatmentGroup];
                values.treatmentCost += (decimal) yearsData.Amount;
                values.length += (int)segmentLength.FeetToMiles();
                costAndLengthPerTreatmentPerYear[year][treatmentGroup] = values;
            }
        }

        private static void PopulateTreatmentCostAndLength(
            int year,
            AssetDetail section,
            decimal cost,
            Dictionary<int, Dictionary<string, Dictionary<int, (decimal treatmentCost, decimal compositeTreatmentCost, int length)>>> costLengthPerSurfaceIdPerTreatmentPerYear
            )
        {            
            var surfaceId = (int)section.ValuePerNumericAttribute["SURFACEID"];
            var appliedTreatment = section.AppliedTreatment;
            var compositeTreatmentCost = surfaceId == 62 ? cost : 0;
            var segmentLength = section.ValuePerNumericAttribute["SEGMENT_LENGTH"];
            var segmentLengthInMiles = (int)segmentLength.FeetToMiles();
            if (!costLengthPerSurfaceIdPerTreatmentPerYear[year].ContainsKey(appliedTreatment))
            {
                costLengthPerSurfaceIdPerTreatmentPerYear[year].Add(appliedTreatment,
                    new() { { surfaceId, (cost, compositeTreatmentCost, segmentLengthInMiles) } });
            }
            else
            {                
                var values = costLengthPerSurfaceIdPerTreatmentPerYear[year][appliedTreatment];
                if (!values.ContainsKey(surfaceId))
                {
                    values.Add(surfaceId, (cost, compositeTreatmentCost, segmentLengthInMiles));
                    costLengthPerSurfaceIdPerTreatmentPerYear[year][appliedTreatment] = values;
                }
                else
                {
                    var surfaceIdValues = values[surfaceId];
                    surfaceIdValues.treatmentCost += cost;
                    surfaceIdValues.compositeTreatmentCost += compositeTreatmentCost;
                    surfaceIdValues.length += segmentLengthInMiles;
                    values[surfaceId] = surfaceIdValues;
                }
            }
        }

        private void PopulateTreatmentGroupCostAndLength(
            int year,
            AssetDetail section,
            decimal cost,
            Dictionary<int, Dictionary<TreatmentGroup, (decimal treatmentCost, int length)>> costAndLengthPerTreatmentPerYear,
            List<(string Name, string AssetType, TreatmentCategory Category)> simulationTreatments)
        {
            var segmentLength = section.ValuePerNumericAttribute["SEGMENT_LENGTH"];
            var treatmentGroup = GetTreatmentGroup(section.AppliedTreatment, simulationTreatments);
            if (!costAndLengthPerTreatmentPerYear[year].ContainsKey(treatmentGroup))
            {
                costAndLengthPerTreatmentPerYear[year].Add(treatmentGroup, (cost, (int)segmentLength.FeetToMiles()));
            }
            else
            {
                var values = costAndLengthPerTreatmentPerYear[year][treatmentGroup];
                values.treatmentCost += cost;
                values.length += (int)segmentLength.FeetToMiles();
                costAndLengthPerTreatmentPerYear[year][treatmentGroup] = values;
            }
        }

        internal Dictionary<TreatmentCategory, SortedDictionary<int, (decimal treatmentCost, int length)>> CalculateWorkTypeTotals(
            Dictionary<int, Dictionary<string, Dictionary<int, (decimal treatmentCost, decimal compositeTreatmentCost, int length)>>> costLengthPerSurfaceIdPerTreatmentPerYear,
            List<(string Name, string AssetType, TreatmentCategory Category)> simulationTreatments
            )
        {
            var workTypeTotals = new Dictionary<TreatmentCategory, SortedDictionary<int, (decimal treatmentCost, int length)>>();

            foreach (var yearlyValues in costLengthPerSurfaceIdPerTreatmentPerYear)
            {
                foreach (var treatment in simulationTreatments)
                {
                    decimal cost = 0;
                    int length = 0;

                    yearlyValues.Value.TryGetValue(treatment.Name, out var costAndLengthsPerSurfaceId);
                    if (costAndLengthsPerSurfaceId != null)
                    {
                        foreach (var value in costAndLengthsPerSurfaceId)
                        {
                            cost += value.Value.treatmentCost;
                            length += value.Value.length;
                        }
                    }
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

                foreach(var yearlyValue in yearlyValues.Value)
                {
                    var treatment = yearlyValue.Key;
                    if (treatment.Contains("Bundle"))
                    {
                        var category = TreatmentCategory.Bundled;
                        decimal cost = 0;
                        int length = 0;
                        var costAndLengthsPerSurfaceId = yearlyValue.Value;                        
                        foreach (var value in costAndLengthsPerSurfaceId)
                        {
                            cost += value.Value.treatmentCost;
                            length += value.Value.length;
                        }

                        if (!workTypeTotals.ContainsKey(category))
                        {
                            workTypeTotals.Add(category, new SortedDictionary<int, (decimal treatmentCost, int length)>()
                            {
                                { yearlyValues.Key, (cost, length) }
                            });
                        }
                        else
                        {
                            if (!workTypeTotals[category].ContainsKey(yearlyValues.Key))
                            {
                                workTypeTotals[category].Add(yearlyValues.Key, (0, 0));
                            }
                            var value = workTypeTotals[category][yearlyValues.Key];
                            value.treatmentCost += cost;
                            value.length += length;
                            workTypeTotals[category][yearlyValues.Key] = value;
                        }
                    }
                }
            }
            return workTypeTotals;
        }

    }
}
