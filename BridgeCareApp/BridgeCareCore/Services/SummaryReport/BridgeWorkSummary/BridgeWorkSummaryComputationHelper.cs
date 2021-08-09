using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.iAM.Analysis;

namespace BridgeCareCore.Services.SummaryReport.BridgeWorkSummary
{
    public class BridgeWorkSummaryComputationHelper
    {
        public int TotalInitialPoorBridgesCount(SimulationOutput reportOutputData) => CountAndAreaOfBridges.GetTotalInitialPoorCount(reportOutputData.InitialSectionSummaries);

        public int TotalSectionalPoorBridgesCount(SimulationYearDetail YearlyData) => CountAndAreaOfBridges.GetTotalSectionalPoorCount(YearlyData.Sections);

        public double TotalInitialPoorBridgesDeckArea(SimulationOutput reportOutputData) => CountAndAreaOfBridges.GetTotalInitialPoorDeckArea(reportOutputData.InitialSectionSummaries);

        public double CalculateTotalPoorBridgesDeckArea(SimulationYearDetail yearlyData) => CountAndAreaOfBridges.TotalPoorBridgesDeckArea(yearlyData.Sections);

        internal int TotalInitialBridgeGoodCount(SimulationOutput reportOutputData) => reportOutputData.InitialSectionSummaries.Where(_ => ConditionIsGood(_)).Count();

        internal int CalculateTotalBridgeGoodCount(SimulationYearDetail yearlyData) => yearlyData.Sections.Where(_ => ConditionIsGood(_)).Count();

        internal int TotalInitialBridgeClosedCount(SimulationOutput reportOutputData) => reportOutputData.InitialSectionSummaries.Where(_ => IsClosed(_)).Count();

        internal int CalculateTotalBridgeClosedCount(SimulationYearDetail yearlyData) => yearlyData.Sections.Where(_ => IsClosed(_)).Count();



        internal double CalculatePoorCountOrAreaForBPN(List<SectionDetail> sectionDetails, string bpn, bool isCount)
        {
            var postedBridges = sectionDetails.FindAll(b => b.ValuePerTextAttribute["BUS_PLAN_NETWORK"] == bpn);
            var selectedBridges = postedBridges.FindAll(section => ConditionIsPoor(section));
            if (isCount)
            {
                return selectedBridges.Count;
            }
            return selectedBridges.Sum(_ => _.ValuePerNumericAttribute["DECK_AREA"]);
        }

        internal double CalculatePoorCountOrAreaForBPN(List<SectionSummaryDetail> initialSectionSummaries, string bpn, bool isCount)
        {
            var postedBridges = initialSectionSummaries.FindAll(b => b.ValuePerTextAttribute["BUS_PLAN_NETWORK"] == bpn);
            var selectedBridges = postedBridges.FindAll(section => ConditionIsPoor(section));
            if (isCount)
            {
                return selectedBridges.Count;
            }
            return selectedBridges.Sum(_ => _.ValuePerNumericAttribute["DECK_AREA"]);
        }

        internal double CalculateMoneyNeededByBPN(List<SectionDetail> sectionDetails, string bpn)
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
            foreach (var section in yearlyData.Sections)
            {
                poorCount += ConditionIsPoor(section) ? 1 : 0;
            }
            return poorCount;
        }

        internal double TotalInitialGoodDeckArea(SimulationOutput reportOutputData)
        {
            double sum = 0;
            foreach (var initialSection in reportOutputData.InitialSectionSummaries)
            {
                var area = ConditionIsGood(initialSection) ? initialSection.ValuePerNumericAttribute["DECK_AREA"] : 0;
                sum += area;
            }

            return sum;
        }

        internal double TotalInitialPoorDeckArea(SimulationOutput reportOutputData) => CountAndAreaOfBridges.GetTotalInitialPoorDeckArea(reportOutputData.InitialSectionSummaries);

        internal double InitialTotalDeckArea(SimulationOutput reportOutputData)
        {
            double sum = 0;
            foreach (var initialSection in reportOutputData.InitialSectionSummaries)
            {
                sum += initialSection.ValuePerNumericAttribute["DECK_AREA"];
            }

            return sum;
        }


        internal double TotalInitialClosedDeckArea(SimulationOutput reportOutputData)
        {
            double sum = 0;
            foreach (var initialSection in reportOutputData.InitialSectionSummaries)
            {
                var area = IsClosed(initialSection) ? initialSection.ValuePerNumericAttribute["DECK_AREA"] : 0;
                sum += area;
            }

            return sum;
        }

        internal double CalculateTotalGoodDeckArea(SimulationYearDetail yearlyData)
        {
            double sum = 0;
            foreach (var section in yearlyData.Sections)
            {
                var area = ConditionIsGood(section) ? section.ValuePerNumericAttribute["DECK_AREA"] : 0;
                sum += area;
            }

            return sum;
        }

        internal double CalculateTotalClosedDeckArea(SimulationYearDetail yearlyData)
        {
            double sum = 0;
            foreach (var section in yearlyData.Sections)
            {
                var area = IsClosed(section) ? section.ValuePerNumericAttribute["DECK_AREA"] : 0;
                sum += area;
            }

            return sum;
        }
        internal double SectionalNHSBridgeGoodCountOrArea(List<SectionDetail> sectionDetails, bool isCount)
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

        internal double TotalNHSBridgeCountOrArea(List<SectionSummaryDetail> initialSectionSummaries, List<SectionDetail> sectionDetails, bool isCount)
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

        internal double SectionalNHSBridgePoorCountOrArea(List<SectionDetail> sectionDetails, bool isCount)
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

        internal double SectionalNHSBridgeClosedCountOrArea(List<SectionDetail> sectionDetails, bool isCount)
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

        internal double InitialNHSBridgePoorCountOrArea(List<SectionSummaryDetail> initialSectionSummaries, bool isCount)
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

        internal double InitialNHSBridgeClosedCountOrArea(List<SectionSummaryDetail> initialSectionSummaries, bool isCount)
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

        internal double InitialNHSBridgeGoodCountOrArea(List<SectionSummaryDetail> initialSectionSummaries, bool isCount)
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
            foreach (var section in yearlyData.Sections)
            {
                var area = ConditionIsPoor(section) ? section.ValuePerNumericAttribute["DECK_AREA"] : 0;
                sum += area;
            }

            return sum;
        }

        internal double CalculateTotalDeckArea(SimulationYearDetail yearlyData)
        {
            double sum = 0;
            foreach (var section in yearlyData.Sections)
            {
                sum += section.ValuePerNumericAttribute["DECK_AREA"];
            }

            return sum;
        }

        internal double CalculateClosedCountOrDeckAreaForBPN(List<SectionDetail> sectionDetails, string bpn, bool isCount)
        {
            var postedBridges = sectionDetails.FindAll(b => b.ValuePerTextAttribute["BUS_PLAN_NETWORK"] == bpn);
            var selectedBridges = postedBridges.FindAll(section => IsClosed(section));
            if (isCount)
            {
                return selectedBridges.Count;
            }
            return selectedBridges.Sum(_ => _.ValuePerNumericAttribute["DECK_AREA"]);
        }

        internal double CalculateClosedCountOrDeckAreaForBPN(List<SectionSummaryDetail> initialSectionSummaries, string bpn, bool isCount)
        {
            var postedBridges = initialSectionSummaries.FindAll(b => b.ValuePerTextAttribute["BUS_PLAN_NETWORK"] == bpn);
            var selectedBridges = postedBridges.FindAll(section => IsClosed(section));
            if (isCount)
            {
                return selectedBridges.Count;
            }
            return selectedBridges.Sum(_ => _.ValuePerNumericAttribute["DECK_AREA"]);
        }

        internal double CalculatePostedCountOrDeckAreaForBPN(List<SectionDetail> sectionDetails, string bpn, bool isCount)
        {
            var postedBridges = sectionDetails.FindAll(b => b.ValuePerTextAttribute["BUS_PLAN_NETWORK"] == bpn);
            var selectedBridges = postedBridges.FindAll(section => IsPosted(section));
            if (isCount)
            {
                return selectedBridges.Count;
            }
            return selectedBridges.Sum(_ => _.ValuePerNumericAttribute["DECK_AREA"]);
        }

        internal double CalculatePostedCountOrDeckAreaForBPN(List<SectionSummaryDetail> initialSectionSummaries, string bpn, bool isCount)
        {
            var postedBridges = initialSectionSummaries.FindAll(b => b.ValuePerTextAttribute["BUS_PLAN_NETWORK"] == bpn);           
            var selectedBridges = postedBridges.FindAll(section => IsPosted(section));
            if (isCount)
            {
                return selectedBridges.Count;
            }
            return selectedBridges.Sum(_ => _.ValuePerNumericAttribute["DECK_AREA"]);
        }

        internal double CalculatePostedCount(List<SectionDetail> sectionDetails) => sectionDetails.Count(_ => IsPosted(_));
        internal double CalculatePostedCount(List<SectionSummaryDetail> initialSectionSummaries) => initialSectionSummaries.Count(section => IsPosted(section));
        internal double CalculateClosedCount(List<SectionDetail> sectionDetails) => sectionDetails.Count(_ => IsClosed(_));
        internal double CalculateClosedCount(List<SectionSummaryDetail> initialSectionSummaries) => initialSectionSummaries.Count(_ => IsClosed(_));

        #region Private methods

        internal static bool ConditionIsGood(SectionSummaryDetail initialSection) => initialSection.ValuePerNumericAttribute["MINCOND"] >= 7;
        internal static bool ConditionIsGood(SectionDetail section) => section.ValuePerNumericAttribute["MINCOND"] >= 7;

        internal static bool ConditionIsFair(SectionSummaryDetail initialSection) => initialSection.ValuePerNumericAttribute["MINCOND"] < 7 && initialSection.ValuePerNumericAttribute["MINCOND"] >= 5;
        internal static bool ConditionIsFair(SectionDetail section) => section.ValuePerNumericAttribute["MINCOND"] < 7 && section.ValuePerNumericAttribute["MINCOND"] >= 5;

        internal static bool ConditionIsPoor(SectionSummaryDetail initialSection) => initialSection.ValuePerNumericAttribute["MINCOND"] < 5;
        internal static bool ConditionIsPoor(SectionDetail section) => section.ValuePerNumericAttribute["MINCOND"] < 5;

        internal static bool IsClosed(SectionSummaryDetail initialSection) => initialSection.ValuePerNumericAttribute["MINCOND"] < 3;
        internal static bool IsClosed(SectionDetail section) => section.ValuePerNumericAttribute["MINCOND"] < 3;

        internal static bool IsPosted(SectionSummaryDetail initialSection) => initialSection.ValuePerNumericAttribute["MINCOND"] >= 3 && initialSection.ValuePerNumericAttribute["SUP_SEEDED"] <= 4.1;
        internal static bool IsPosted(SectionDetail section) => section.ValuePerNumericAttribute["MINCOND"] >= 3 && section.ValuePerNumericAttribute["SUP_SEEDED"] <= 4.1;

        #endregion Private methods

        private static class CountAndAreaOfBridges
        {
            internal static int GetTotalInitialPoorCount(List<SectionSummaryDetail> initialSectionSummaries)
            {
                var count = 0;
                foreach (var initialSection in initialSectionSummaries)
                {
                    count += ConditionIsPoor(initialSection) ? 1 : 0;
                }
                return count;
            }

            internal static int GetTotalSectionalPoorCount(List<SectionDetail> sections)
            {
                var count = 0;
                foreach (var section in sections)
                {
                    count += ConditionIsPoor(section) ? 1 : 0;
                }
                return count;
            }

            internal static double GetTotalInitialPoorDeckArea(List<SectionSummaryDetail> initialSectionSummaries)
            {
                double sum = 0;
                foreach (var initialSection in initialSectionSummaries)
                {
                    var deckArea = ConditionIsPoor(initialSection) ? initialSection.ValuePerNumericAttribute["DECK_AREA"] : 0;
                    sum += deckArea;
                }

                return sum;
            }

            internal static double TotalPoorBridgesDeckArea(List<SectionDetail> sections)
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
