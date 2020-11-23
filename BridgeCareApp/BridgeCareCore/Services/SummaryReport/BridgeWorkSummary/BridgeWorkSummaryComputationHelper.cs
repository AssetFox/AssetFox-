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
