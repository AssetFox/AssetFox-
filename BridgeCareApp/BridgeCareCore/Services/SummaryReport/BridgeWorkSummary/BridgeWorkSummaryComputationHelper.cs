using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.Analysis;

namespace BridgeCareCore.Services.SummaryReport.BridgeWorkSummary
{
    public class BridgeWorkSummaryComputationHelper
    {
        public int TotalInitialPoorBridgesCount(SimulationOutput reportOutputData)
        {
            var count = 0;
            foreach (var initialSection in reportOutputData.InitialSectionSummaries)
            {
                count += initialSection.ValuePerNumericAttribute["MINCOND"] < 5 ? 1 : 0;
            }
            return count;
        }
        public int TotalSectionalPoorBridgesCount(SimulationYearDetail YearlyData)
        {
            var count = 0;
            foreach (var section in YearlyData.Sections)
            {
                count += section.ValuePerNumericAttribute["MINCOND"] < 5 ? 1 : 0;
            }
            return count;
        }

        public double TotalInitialPoorBridgesDeckArea(SimulationOutput reportOutputData)
        {
            double sum = 0;
            foreach (var initialSection in reportOutputData.InitialSectionSummaries)
            {
                var deckArea = initialSection.ValuePerNumericAttribute["MINCOND"] < 5 ? initialSection.ValuePerNumericAttribute["DECK_AREA"] : 0;
                sum += deckArea;
            }

            return sum;
        }
        public double CalculateTotalPoorBridgesDeckArea(SimulationYearDetail yearlyData)
        {
            double sum = 0;
            foreach (var section in yearlyData.Sections)
            {
                var deckArea = section.ValuePerNumericAttribute["MINCOND"] < 5 ? section.ValuePerNumericAttribute["DECK_AREA"] : 0;
                sum += deckArea;
            }

            return sum;
        }

        internal int TotalInitialBridgeGoodCount(SimulationOutput reportOutputData)
        {
            var initialGoodCount = 0;
            foreach (var intialSection in reportOutputData.InitialSectionSummaries)
            {
                initialGoodCount += intialSection.ValuePerNumericAttribute["MINCOND"] >= 7 ? 1 : 0;
            }
            return initialGoodCount;
        }

        internal int TotalInitialBridgePoorCount(SimulationOutput reportOutputData)
        {
            var initialPoorCount = 0;
            foreach (var intialSection in reportOutputData.InitialSectionSummaries)
            {
                initialPoorCount += intialSection.ValuePerNumericAttribute["MINCOND"] < 5 ? 1 : 0;
            }
            return initialPoorCount;
        }

        internal int CalculateTotalBridgeGoodCount(SimulationYearDetail yearlyData)
        {
            var goodCount = 0;
            foreach (var section in yearlyData.Sections)
            {
                goodCount += section.ValuePerNumericAttribute["MINCOND"] >= 7 ? 1 : 0;
            }
            return goodCount;
        }

        internal double CalculatePoorDeckAreaForBPN13(List<SectionDetail> sectionDetails, string bpn)
        {
            var postedBridges = sectionDetails.FindAll(b => b.ValuePerTextAttribute["BUS_PLAN_NETWORK"] == bpn);
            var selectedBridges = postedBridges.FindAll(_ => _.ValuePerNumericAttribute["MINCOND"] < 5);
            return selectedBridges.Sum(_ => _.ValuePerNumericAttribute["DECK_AREA"]);
        }
        internal double CalculatePoorDeckAreaForBPN2H(List<SectionDetail> sectionDetails)
        {
            var postedBridges = sectionDetails.FindAll(b => b.ValuePerTextAttribute["BUS_PLAN_NETWORK"] == "2"
            || b.ValuePerTextAttribute["BUS_PLAN_NETWORK"] == "H");

            var selectedBridges = postedBridges.FindAll(_ => _.ValuePerNumericAttribute["MINCOND"] < 5);
            return selectedBridges.Sum(_ => _.ValuePerNumericAttribute["DECK_AREA"]);
        }

        internal double CalculateMoneyNeededByBPN13(List<SectionDetail> sectionDetails, string bpn)
        {
            var filteredBPNBridges = sectionDetails.FindAll(b =>
            b.TreatmentCause != TreatmentCause.NoSelection &&
            b.TreatmentOptions.Count > 0 &&
            b.ValuePerTextAttribute["BUS_PLAN_NETWORK"] == bpn);

            var totalCost = filteredBPNBridges.Sum(_ => _.TreatmentOptions.FirstOrDefault(t =>
            t.TreatmentName == _.AppliedTreatment).Cost);
            return totalCost;
        }

        internal double CalculateMoneyNeededByBPN2H(List<SectionDetail> sectionDetails)
        {
            var filteredBPNBridges = sectionDetails.FindAll(
                b => b.TreatmentCause != TreatmentCause.NoSelection && b.TreatmentOptions.Count > 0 && (
                b.ValuePerTextAttribute["BUS_PLAN_NETWORK"] == "2" ||
                b.ValuePerTextAttribute["BUS_PLAN_NETWORK"] == "H")
                );
            var totalCost = filteredBPNBridges.Sum(_ => _.TreatmentOptions.FirstOrDefault(t =>
            t.TreatmentName == _.AppliedTreatment).Cost);

            return totalCost;
        }
        internal double CalculateMoneyNeededByRemainingBPN(List<SectionDetail> sectionDetails)
        {
            var filteredBPNBridges = sectionDetails.FindAll(
                b => b.TreatmentCause != TreatmentCause.NoSelection && b.TreatmentOptions.Count > 0 &&
                b.ValuePerTextAttribute["BUS_PLAN_NETWORK"] != "2" &&
                b.ValuePerTextAttribute["BUS_PLAN_NETWORK"] != "H" && b.ValuePerTextAttribute["BUS_PLAN_NETWORK"] != "1" &&
                b.ValuePerTextAttribute["BUS_PLAN_NETWORK"] != "3"
            );
            var totalCost = filteredBPNBridges.Sum(_ => _.TreatmentOptions.FirstOrDefault(t =>
            t.TreatmentName == _.AppliedTreatment).Cost);

            return totalCost;
        }

        internal double CalculatePoorDeckAreaForRemainingBPN(List<SectionDetail> sectionDetails)
        {
            var postedBridges = sectionDetails.FindAll(
                b => b.ValuePerTextAttribute["BUS_PLAN_NETWORK"] != "2" &&
                b.ValuePerTextAttribute["BUS_PLAN_NETWORK"] != "H" && b.ValuePerTextAttribute["BUS_PLAN_NETWORK"] != "1" &&
                b.ValuePerTextAttribute["BUS_PLAN_NETWORK"] != "3"
            );
            var selectedBridges = postedBridges.FindAll(_ => _.ValuePerNumericAttribute["MINCOND"] < 5);

            return selectedBridges.Sum(_ => _.ValuePerNumericAttribute["DECK_AREA"]);
        }

        internal int CalculateTotalBridgePoorCount(SimulationYearDetail yearlyData)
        {
            var poorCount = 0;
            foreach (var section in yearlyData.Sections)
            {
                poorCount += section.ValuePerNumericAttribute["MINCOND"] < 5 ? 1 : 0;
            }
            return poorCount;
        }

        internal double TotalInitialGoodDeckArea(SimulationOutput reportOutputData)
        {
            double sum = 0;
            foreach (var initialSection in reportOutputData.InitialSectionSummaries)
            {
                var area = initialSection.ValuePerNumericAttribute["MINCOND"] >= 7 ? initialSection.ValuePerNumericAttribute["DECK_AREA"] : 0;
                sum += area;
            }

            return sum;
        }

        internal double TotalInitialPoorDeckArea(SimulationOutput reportOutputData)
        {
            double sum = 0;
            foreach (var initialSection in reportOutputData.InitialSectionSummaries)
            {
                var area = initialSection.ValuePerNumericAttribute["MINCOND"] < 5 ? initialSection.ValuePerNumericAttribute["DECK_AREA"] : 0;
                sum += area;
            }

            return sum;
        }

        internal double InitialTotalDeckArea(SimulationOutput reportOutputData)
        {
            double sum = 0;
            foreach (var initialSection in reportOutputData.InitialSectionSummaries)
            {
                sum += initialSection.ValuePerNumericAttribute["DECK_AREA"];
            }

            return sum;
        }

        internal double CalculateTotalGoodDeckArea(SimulationYearDetail yearlyData)
        {
            double sum = 0;
            foreach (var section in yearlyData.Sections)
            {
                var area = section.ValuePerNumericAttribute["MINCOND"] >= 7 ? section.ValuePerNumericAttribute["DECK_AREA"] : 0;
                sum += area;
            }

            return sum;
        }

        internal double SectionalNHSBridgeGoodCountOrArea(List<SectionDetail> sectionDetails, bool isCount)
        {
            var filteredSection = sectionDetails.FindAll(_ => int.TryParse(_.ValuePerTextAttribute["NHS_IND"], out var numericValue)
             && numericValue > 0);
            var goodSections = filteredSection.FindAll(_ => _.ValuePerNumericAttribute["MINCOND"] >= 7);
            if (isCount)
            {
               return goodSections.Count;
            }
            return goodSections.Sum(_ => _.ValuePerNumericAttribute["DECK_AREA"]);
        }

        internal double TotalNHSBridgeCountOrArea(List<SectionSummaryDetail> initialSectionSummaries, List<SectionDetail> sectionDetails, bool isCount)
        {
            if(initialSectionSummaries != null)
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
            var poorSections = filteredSection.FindAll(_ => _.ValuePerNumericAttribute["MINCOND"] < 5);
            if (isCount)
            {
                return poorSections.Count;
            }
            return poorSections.Sum(_ => _.ValuePerNumericAttribute["DECK_AREA"]);
        }

        internal double InitialNHSBridgePoorCountOrArea(List<SectionSummaryDetail> initialSectionSummaries, bool isCount)
        {
            var filteredSection = initialSectionSummaries.FindAll(_ => int.TryParse(_.ValuePerTextAttribute["NHS_IND"], out var numericValue)
            && numericValue > 0);
            var poorSections = filteredSection.FindAll(_ => _.ValuePerNumericAttribute["MINCOND"] < 5);
            if (isCount)
            {
                return poorSections.Count;
            }
            return poorSections.Sum(_ => _.ValuePerNumericAttribute["DECK_AREA"]);
        }

        internal double InitialNHSBridgeGoodCountOrArea(List<SectionSummaryDetail> initialSectionSummaries, bool isCount)
        {
            var filteredSection = initialSectionSummaries.FindAll(_ => int.TryParse(_.ValuePerTextAttribute["NHS_IND"], out var numericValue)
            && numericValue > 0);
            var goodSections = filteredSection.FindAll(_ => _.ValuePerNumericAttribute["MINCOND"] >= 7);
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
                var area = section.ValuePerNumericAttribute["MINCOND"] < 5 ? section.ValuePerNumericAttribute["DECK_AREA"] : 0;
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
    }
}
