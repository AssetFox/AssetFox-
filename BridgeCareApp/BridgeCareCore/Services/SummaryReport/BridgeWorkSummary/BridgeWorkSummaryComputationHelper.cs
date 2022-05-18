using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.iAM.Analysis.Engine;

namespace BridgeCareCore.Services.SummaryReport.BridgeWorkSummary
{
    public class BridgeWorkSummaryComputationHelper
    {
        public int TotalInitialPoorBridgesCount(SimulationOutput reportOutputData) => CountAndAreaOfBridges.GetTotalInitialPoorCount(reportOutputData.InitialAssetSummaries);

        public int TotalSectionalPoorBridgesCount(SimulationYearDetail YearlyData) => CountAndAreaOfBridges.GetTotalSectionalPoorCount(YearlyData.Assets);

        public double TotalInitialPoorBridgesDeckArea(SimulationOutput reportOutputData) => CountAndAreaOfBridges.GetTotalInitialPoorDeckArea(reportOutputData.InitialAssetSummaries);

        public double CalculateTotalPoorBridgesDeckArea(SimulationYearDetail yearlyData) => CountAndAreaOfBridges.TotalPoorBridgesDeckArea(yearlyData.Assets);

        internal int TotalInitialBridgeGoodCount(SimulationOutput reportOutputData) => reportOutputData.InitialAssetSummaries.Where(_ => ConditionIsGood(_)).Count();

        internal int CalculateTotalBridgeGoodCount(SimulationYearDetail yearlyData) => yearlyData.Assets.Where(_ => ConditionIsGood(_)).Count();

        internal int TotalInitialBridgeClosedCount(SimulationOutput reportOutputData) => reportOutputData.InitialAssetSummaries.Where(_ => IsClosed(_)).Count();

        internal int CalculateTotalBridgeClosedCount(SimulationYearDetail yearlyData) => yearlyData.Assets.Where(_ => IsClosed(_)).Count();



        internal double CalculatePoorCountOrAreaForBPN(List<AssetDetail> sectionDetails, string bpn, bool isCount)
        {
            var postedBridges = sectionDetails.FindAll(b => b.ValuePerTextAttribute["BUS_PLAN_NETWORK"] == bpn);
            var selectedBridges = postedBridges.FindAll(section => ConditionIsPoor(section));
            if (isCount)
            {
                return selectedBridges.Count;
            }
            return selectedBridges.Sum(_ => _.ValuePerNumericAttribute["DECK_AREA"]);
        }

        internal double CalculatePoorCountOrAreaForBPN(List<AssetSummaryDetail> initialSectionSummaries, string bpn, bool isCount)
        {
            var postedBridges = initialSectionSummaries.FindAll(b => b.ValuePerTextAttribute["BUS_PLAN_NETWORK"] == bpn);
            var selectedBridges = postedBridges.FindAll(section => ConditionIsPoor(section));
            if (isCount)
            {
                return selectedBridges.Count;
            }
            return selectedBridges.Sum(_ => _.ValuePerNumericAttribute["DECK_AREA"]);
        }

        internal double CalculateMoneyNeededByBPN(List<AssetDetail> sectionDetails, string bpn)
        {
            var filteredBPNBridges = sectionDetails.FindAll(b =>
            b.TreatmentCause != TreatmentCause.NoSelection &&
            b.TreatmentOptions.Count > 0 &&
            b.ValuePerTextAttribute["BUS_PLAN_NETWORK"] == bpn);

            var totalCost = filteredBPNBridges.Sum(_ => _.TreatmentOptions.FirstOrDefault(t =>
            t.TreatmentName == _.AppliedTreatment).Cost);
            return totalCost;
        }


        internal int CalculateTotalBridgePoorCount(SimulationYearDetail yearlyData)
        {
            var poorCount = 0;
            foreach (var section in yearlyData.Assets)
            {
                poorCount += ConditionIsPoor(section) ? 1 : 0;
            }
            return poorCount;
        }

        internal double TotalInitialGoodDeckArea(SimulationOutput reportOutputData)
        {
            double sum = 0;
            foreach (var initialSection in reportOutputData.InitialAssetSummaries)
            {
                var area = ConditionIsGood(initialSection) ? initialSection.ValuePerNumericAttribute["DECK_AREA"] : 0;
                sum += area;
            }

            return sum;
        }

        internal double TotalInitialPoorDeckArea(SimulationOutput reportOutputData) => CountAndAreaOfBridges.GetTotalInitialPoorDeckArea(reportOutputData.InitialAssetSummaries);

        internal double InitialTotalDeckArea(SimulationOutput reportOutputData)
        {
            double sum = 0;
            foreach (var initialSection in reportOutputData.InitialAssetSummaries)
            {
                sum += initialSection.ValuePerNumericAttribute["DECK_AREA"];
            }

            return sum;
        }


        internal double TotalInitialClosedDeckArea(SimulationOutput reportOutputData)
        {
            double sum = 0;
            foreach (var initialSection in reportOutputData.InitialAssetSummaries)
            {
                var area = IsClosed(initialSection) ? initialSection.ValuePerNumericAttribute["DECK_AREA"] : 0;
                sum += area;
            }

            return sum;
        }

        internal double CalculateTotalGoodDeckArea(SimulationYearDetail yearlyData)
        {
            double sum = 0;
            foreach (var section in yearlyData.Assets)
            {
                var area = ConditionIsGood(section) ? section.ValuePerNumericAttribute["DECK_AREA"] : 0;
                sum += area;
            }

            return sum;
        }

        internal double CalculateTotalClosedDeckArea(SimulationYearDetail yearlyData)
        {
            double sum = 0;
            foreach (var section in yearlyData.Assets)
            {
                var area = IsClosed(section) ? section.ValuePerNumericAttribute["DECK_AREA"] : 0;
                sum += area;
            }

            return sum;
        }
        internal double SectionalNHSBridgeGoodCountOrArea(List<AssetDetail> sectionDetails, bool isCount)
        {
            var filteredSection = sectionDetails.FindAll(_ => int.TryParse(_.ValuePerTextAttribute["NHS_IND"], out var numericValue)
             && numericValue > 0);
            var goodSections = filteredSection.FindAll(s => ConditionIsGood(s));
            if (isCount)
            {
                return goodSections.Count;
            }
            return goodSections.Sum(_ => _.ValuePerNumericAttribute["DECK_AREA"]);
        }

        internal double TotalNHSBridgeCountOrArea(List<AssetSummaryDetail> initialSectionSummaries, List<AssetDetail> sectionDetails, bool isCount)
        {
            if (initialSectionSummaries != null)
            {
                var initialSections = initialSectionSummaries.FindAll(_ => int.TryParse(_.ValuePerTextAttribute["NHS_IND"], out var numericValue)
            && numericValue > 0);
                if (isCount)
                {
                    return initialSections.Count;
                }
                return initialSections.Sum(_ => _.ValuePerNumericAttribute["DECK_AREA"]);
            }

            var secitons = sectionDetails.FindAll(_ => int.TryParse(_.ValuePerTextAttribute["NHS_IND"], out var numericValue)
            && numericValue > 0);
            if (isCount)
            {
                return secitons.Count;
            }
            return secitons.Sum(_ => _.ValuePerNumericAttribute["DECK_AREA"]);
        }

        internal double SectionalNHSBridgePoorCountOrArea(List<AssetDetail> sectionDetails, bool isCount)
        {
            var filteredSection = sectionDetails.FindAll(_ => int.TryParse(_.ValuePerTextAttribute["NHS_IND"], out var numericValue)
            && numericValue > 0);
            var poorSections = filteredSection.FindAll(section => ConditionIsPoor(section));
            if (isCount)
            {
                return poorSections.Count;
            }
            return poorSections.Sum(_ => _.ValuePerNumericAttribute["DECK_AREA"]);
        }

        internal double SectionalNHSBridgeClosedCountOrArea(List<AssetDetail> sectionDetails, bool isCount)
        {
            var filteredSection = sectionDetails.FindAll(_ => int.TryParse(_.ValuePerTextAttribute["NHS_IND"], out var numericValue)
            && numericValue > 0);
            var closedSections = filteredSection.FindAll(section => IsClosed(section));
            if (isCount)
            {
                return closedSections.Count;
            }
            return closedSections.Sum(_ => _.ValuePerNumericAttribute["DECK_AREA"]);
        }

        internal double InitialNHSBridgePoorCountOrArea(List<AssetSummaryDetail> initialSectionSummaries, bool isCount)
        {
            var filteredSection = initialSectionSummaries.FindAll(_ => int.TryParse(_.ValuePerTextAttribute["NHS_IND"], out var numericValue)
            && numericValue > 0);
            var poorSections = filteredSection.FindAll(s => ConditionIsPoor(s));
            if (isCount)
            {
                return poorSections.Count;
            }
            return poorSections.Sum(_ => _.ValuePerNumericAttribute["DECK_AREA"]);
        }

        internal double InitialNHSBridgeClosedCountOrArea(List<AssetSummaryDetail> initialSectionSummaries, bool isCount)
        {
            var filteredSection = initialSectionSummaries.FindAll(_ => int.TryParse(_.ValuePerTextAttribute["NHS_IND"], out var numericValue)
            && numericValue > 0);
            var closedSections = filteredSection.FindAll(s => IsClosed(s));
            if (isCount)
            {
                return closedSections.Count;
            }
            return closedSections.Sum(_ => _.ValuePerNumericAttribute["DECK_AREA"]);
        }

        internal double InitialNHSBridgeGoodCountOrArea(List<AssetSummaryDetail> initialSectionSummaries, bool isCount)
        {
            var filteredSection = initialSectionSummaries.FindAll(_ => int.TryParse(_.ValuePerTextAttribute["NHS_IND"], out var numericValue)
            && numericValue > 0);
            var goodSections = filteredSection.FindAll(s => ConditionIsGood(s));
            if (isCount)
            {
                return goodSections.Count;
            }
            return goodSections.Sum(_ => _.ValuePerNumericAttribute["DECK_AREA"]);
        }

        internal double CalculateTotalPoorDeckArea(SimulationYearDetail yearlyData)
        {
            double sum = 0;
            foreach (var section in yearlyData.Assets)
            {
                var area = ConditionIsPoor(section) ? section.ValuePerNumericAttribute["DECK_AREA"] : 0;
                sum += area;
            }

            return sum;
        }

        internal double CalculateTotalDeckArea(SimulationYearDetail yearlyData)
        {
            double sum = 0;
            foreach (var section in yearlyData.Assets)
            {
                sum += section.ValuePerNumericAttribute["DECK_AREA"];
            }

            return sum;
        }

        internal double CalculateClosedCountOrDeckAreaForBPN(List<AssetDetail> sectionDetails, string bpn, bool isCount)
        {
            var postedBridges = sectionDetails.FindAll(b => b.ValuePerTextAttribute["BUS_PLAN_NETWORK"] == bpn);
            var selectedBridges = postedBridges.FindAll(section => IsClosed(section));
            if (isCount)
            {
                return selectedBridges.Count;
            }
            return selectedBridges.Sum(_ => _.ValuePerNumericAttribute["DECK_AREA"]);
        }

        internal double CalculateClosedCountOrDeckAreaForBPN(List<AssetSummaryDetail> initialSectionSummaries, string bpn, bool isCount)
        {
            var postedBridges = initialSectionSummaries.FindAll(b => b.ValuePerTextAttribute["BUS_PLAN_NETWORK"] == bpn);
            var selectedBridges = postedBridges.FindAll(section => IsClosed(section));
            if (isCount)
            {
                return selectedBridges.Count;
            }
            return selectedBridges.Sum(_ => _.ValuePerNumericAttribute["DECK_AREA"]);
        }

        internal double CalculatePostedCountOrDeckAreaForBPN(List<AssetDetail> sectionDetails, string bpn, bool isCount)
        {
            var postedBridges = sectionDetails.FindAll(b => b.ValuePerTextAttribute["BUS_PLAN_NETWORK"] == bpn);
            var selectedBridges = postedBridges.FindAll(section => IsPosted(section));
            if (isCount)
            {
                return selectedBridges.Count;
            }
            return selectedBridges.Sum(_ => _.ValuePerNumericAttribute["DECK_AREA"]);
        }

        internal double CalculatePostedCountOrDeckAreaForBPN(List<AssetSummaryDetail> initialSectionSummaries, string bpn, bool isCount)
        {
            var postedBridges = initialSectionSummaries.FindAll(b => b.ValuePerTextAttribute["BUS_PLAN_NETWORK"] == bpn);
            var selectedBridges = postedBridges.FindAll(section => IsPosted(section));
            if (isCount)
            {
                return selectedBridges.Count;
            }
            return selectedBridges.Sum(_ => _.ValuePerNumericAttribute["DECK_AREA"]);
        }

        internal double CalculatePostedCount(List<AssetDetail> sectionDetails) => sectionDetails.Count(_ => IsPosted(_));
        internal double CalculatePostedCount(List<AssetSummaryDetail> initialSectionSummaries) => initialSectionSummaries.Count(section => IsPosted(section));
        internal double CalculateClosedCount(List<AssetDetail> sectionDetails) => sectionDetails.Count(_ => IsClosed(_));
        internal double CalculateClosedCount(List<AssetSummaryDetail> initialSectionSummaries) => initialSectionSummaries.Count(_ => IsClosed(_));

        #region Private methods

        internal static bool ConditionIsGood(AssetSummaryDetail initialSection) => initialSection.ValuePerNumericAttribute["MINCOND"] >= 7;
        internal static bool ConditionIsGood(AssetDetail section) => section.ValuePerNumericAttribute["MINCOND"] >= 7;

        internal static bool ConditionIsFair(AssetSummaryDetail initialSection) => initialSection.ValuePerNumericAttribute["MINCOND"] < 7 && initialSection.ValuePerNumericAttribute["MINCOND"] >= 5;
        internal static bool ConditionIsFair(AssetDetail section) => section.ValuePerNumericAttribute["MINCOND"] < 7 && section.ValuePerNumericAttribute["MINCOND"] >= 5;

        internal static bool ConditionIsPoor(AssetSummaryDetail initialSection) => initialSection.ValuePerNumericAttribute["MINCOND"] < 5;
        internal static bool ConditionIsPoor(AssetDetail section) => section.ValuePerNumericAttribute["MINCOND"] < 5;

        internal static bool IsClosed(AssetSummaryDetail initialSection) => initialSection.ValuePerNumericAttribute["MINCOND"] < 3;
        internal static bool IsClosed(AssetDetail section) => section.ValuePerNumericAttribute["MINCOND"] < 3;

        internal static bool IsPosted(AssetSummaryDetail initialSection) => initialSection.ValuePerNumericAttribute["MINCOND"] >= 3 && initialSection.ValuePerNumericAttribute["SUP_SEEDED"] <= 4.1;
        internal static bool IsPosted(AssetDetail section) => section.ValuePerNumericAttribute["MINCOND"] >= 3 && section.ValuePerNumericAttribute["SUP_SEEDED"] <= 4.1;

        #endregion Private methods

        private static class CountAndAreaOfBridges
        {
            internal static int GetTotalInitialPoorCount(List<AssetSummaryDetail> initialSectionSummaries)
            {
                var count = 0;
                foreach (var initialSection in initialSectionSummaries)
                {
                    count += ConditionIsPoor(initialSection) ? 1 : 0;
                }
                return count;
            }

            internal static int GetTotalSectionalPoorCount(List<AssetDetail> sections)
            {
                var count = 0;
                foreach (var section in sections)
                {
                    count += ConditionIsPoor(section) ? 1 : 0;
                }
                return count;
            }

            internal static double GetTotalInitialPoorDeckArea(List<AssetSummaryDetail> initialSectionSummaries)
            {
                double sum = 0;
                foreach (var initialSection in initialSectionSummaries)
                {
                    var deckArea = ConditionIsPoor(initialSection) ? initialSection.ValuePerNumericAttribute["DECK_AREA"] : 0;
                    sum += deckArea;
                }

                return sum;
            }

            internal static double TotalPoorBridgesDeckArea(List<AssetDetail> sections)
            {
                double sum = 0;
                foreach (var section in sections)
                {
                    var deckArea = ConditionIsPoor(section) ? section.ValuePerNumericAttribute["DECK_AREA"] : 0;
                    sum += deckArea;
                }

                return sum;
            }
        }
    }
}
