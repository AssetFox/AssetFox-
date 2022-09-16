using System;
using System.Linq;
using AppliedResearchAssociates.iAM.Analysis.Engine;
using AppliedResearchAssociates.iAM.TestHelpers.Extensions;
using AppliedResearchAssociates.iAM.UnitTestsCore.TestUtils;
using Newtonsoft.Json;
using AppliedResearchAssociates.iAM.Common;

using Xunit;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests
{
    public static class SimulationOutputAssertions
    {
        public static void SameSimulationOutput(SimulationOutput expected, SimulationOutput actual)
        {
            Assert.Equal(expected.InitialConditionOfNetwork, actual.InitialConditionOfNetwork);
            Assert.Equal(expected.InitialAssetSummaries.Count, actual.InitialAssetSummaries.Count);
            expected.InitialAssetSummaries.Sort(a => a.AssetId);
            actual.InitialAssetSummaries.Sort(a => a.AssetId);
            for (int i=0; i<expected.InitialAssetSummaries.Count; i++)
            {
                AssetSummaryDetailAssertions.Same(expected.InitialAssetSummaries[i], actual.InitialAssetSummaries[i]);
            }
            Assert.Equal(expected.Years.Count, actual.Years.Count);
            foreach (var expectedYear in expected.Years)
            {
                var actualYear = actual.Years.Single(y => y.Year == expectedYear.Year);
                SimulationOutputYearAssertions.Same(expectedYear, actualYear);
            }
        }
    }
}
