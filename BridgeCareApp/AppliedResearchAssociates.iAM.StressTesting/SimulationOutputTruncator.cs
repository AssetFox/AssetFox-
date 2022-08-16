using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.Analysis.Engine;

namespace AppliedResearchAssociates.iAM.StressTesting
{
    public static class SimulationOutputTruncator
    {

        public static SimulationOutput Truncate(SimulationOutput simulationOutput, int take)
        {
            simulationOutput.InitialAssetSummaries.RemoveRange(take, simulationOutput.InitialAssetSummaries.Count - take);
            foreach (var year in simulationOutput.Years)
            {
                year.Assets.RemoveRange(take, year.Assets.Count - take);
            }
            return simulationOutput;
        }

        private static void TruncateSkipTake<T>(List<T> list, int skip, int take)
        {
            list.RemoveRange(0, skip);
            list.RemoveRange(take, list.Count - take);
        }

        public static SimulationOutput Truncate(SimulationOutput simulationOutput, int skip, int take)
        {
            TruncateSkipTake(simulationOutput.InitialAssetSummaries, skip, take);
            foreach (var year in simulationOutput.Years)
            {
                TruncateSkipTake(year.Assets, skip, take);
            }
            return simulationOutput;
        }

        public static Func<SimulationOutput, SimulationOutput> Truncate(int assetTake)
        {
            return simulationOutput
                => Truncate(simulationOutput, assetTake);
        }
        public static Func<SimulationOutput, SimulationOutput> Truncate(int assetSkip, int assetTake)
        {
            return simulationOutput
                => Truncate(simulationOutput, assetSkip, assetTake);
        }
    }
}
