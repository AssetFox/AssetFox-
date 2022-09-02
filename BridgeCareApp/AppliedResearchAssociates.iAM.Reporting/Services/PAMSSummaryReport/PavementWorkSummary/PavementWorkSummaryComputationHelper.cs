using System;
using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.iAM.Analysis.Engine;
using AppliedResearchAssociates.iAM.Reporting.Interfaces.PAMSSummaryReport;

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
        private static ISummaryReportHelper _summaryReportHelper = new SummaryReportHelper();

        // InRange excludes the high value to avoid overlap
        private static bool InRange(this double value, double low, double high) =>
            low <= value && value < high;

        private static double IriCondition(this AssetSummaryDetail detail) =>
            _summaryReportHelper.checkAndGetValue<double>(detail.ValuePerNumericAttribute, "ROUGHNESS");
        private static double IriCondition(this AssetDetail detail) =>
            _summaryReportHelper.checkAndGetValue<double>(detail.ValuePerNumericAttribute, "ROUGHNESS");

        public static bool IriConditionIsExcellent(this AssetSummaryDetail detail) =>
            detail.IriCondition().InRange(0,60);
        public static bool IriConditionIsExcellent(this AssetDetail detail) =>
            detail.IriCondition().InRange(0, 60);

        public static bool IriConditionIsGood(this AssetSummaryDetail detail) =>
            detail.IriCondition().InRange(60,95);
        public static bool IriConditionIsGood(this AssetDetail detail) =>
            detail.IriCondition().InRange(60, 95);

        public static bool IriConditionIsFair(this AssetSummaryDetail detail) =>
            detail.IriCondition().InRange(95, 170);
        public static bool IriConditionIsFair(this AssetDetail detail) =>
            detail.IriCondition().InRange(95, 170);

        public static bool IriConditionIsPoor(this AssetSummaryDetail detail) =>
            detail.IriCondition() >= 170;
        public static bool IriConditionIsPoor(this AssetDetail detail) =>
            detail.IriCondition() >= 170;


        private static double OpiCondition(this AssetSummaryDetail detail) =>
            _summaryReportHelper.checkAndGetValue<double>(detail.ValuePerNumericAttribute, "OPI");
        private static double OpiCondition(this AssetDetail detail) =>
            _summaryReportHelper.checkAndGetValue<double>(detail.ValuePerNumericAttribute, "OPI");


        public static bool OpiConditionIsExcellent(this AssetSummaryDetail detail) =>
            detail.OpiCondition().InRange(90,100);
        public static bool OpiConditionIsExcellent(this AssetDetail detail) =>
            detail.OpiCondition().InRange(90, 100);

        public static bool OpiConditionIsGood(this AssetSummaryDetail detail) =>
            detail.OpiCondition().InRange(75, 90);
        public static bool OpiConditionIsGood(this AssetDetail detail) =>
            detail.OpiCondition().InRange(75, 90);

        public static bool OpiConditionIsFair(this AssetSummaryDetail detail) =>
            detail.OpiCondition().InRange(60, 75);
        public static bool OpiConditionIsFair(this AssetDetail detail) =>
            detail.OpiCondition().InRange(60, 75);

        public static bool OpiConditionIsPoor(this AssetSummaryDetail detail) =>
            detail.OpiCondition() < 60;
        public static bool OpiConditionIsPoor(this AssetDetail detail) =>
            detail.OpiCondition() < 60;
    }

    public class PavementWorkSummaryComputationHelper
    {
        private ISummaryReportHelper _summaryReportHelper;

        public PavementWorkSummaryComputationHelper()
        {
            _summaryReportHelper = new SummaryReportHelper();
        }

        internal double CalculateSegmentMilesForBPNWithCondition(List<AssetSummaryDetail> initialSectionSummaries, string bpn, Func<AssetSummaryDetail, bool> conditionFunction)
        {
            var postedSegments = !String.IsNullOrEmpty(bpn) ? initialSectionSummaries.FindAll(b => _summaryReportHelper.checkAndGetValue<string>(b.ValuePerTextAttribute, "BUS_PLAN_NETWORK") == bpn) : initialSectionSummaries;
            var selectedSegments = postedSegments.FindAll(section => conditionFunction(section));
            return selectedSegments.Sum(_ => _summaryReportHelper.checkAndGetValue<double>(_.ValuePerNumericAttribute, "SEGMENT_LENGTH")).FeetToMiles();
        }

        internal double CalculateSegmentMilesForBPNWithCondition(List<AssetDetail> sectionSummaries, string bpn, Func<AssetDetail, bool> conditionFunction)
        {
            var postedSegments = !String.IsNullOrEmpty(bpn) ? sectionSummaries.FindAll(b => _summaryReportHelper.checkAndGetValue<string>(b.ValuePerTextAttribute, "BUS_PLAN_NETWORK") == bpn) : sectionSummaries;
            var selectedSegments = postedSegments.FindAll(section => conditionFunction(section));
            return selectedSegments.Sum(_ => _summaryReportHelper.checkAndGetValue<double>(_.ValuePerNumericAttribute, "SEGMENT_LENGTH")).FeetToMiles();
        }

    }
}
