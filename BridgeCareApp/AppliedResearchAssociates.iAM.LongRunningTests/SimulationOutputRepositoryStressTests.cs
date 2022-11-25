using System;
using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.iAM.Analysis.Engine;
using AppliedResearchAssociates.iAM.Common;
using AppliedResearchAssociates.iAM.Common.PerformanceMeasurement;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests;
using AppliedResearchAssociates.iAM.UnitTestsCore.TestUtils;
using Newtonsoft.Json;
using Xunit;

namespace AppliedResearchAssociates.iAM.StressTesting
{
    public class SimulationOutputRepositoryStressTests
    {
        //[Fact]
        [Fact(Skip = "Takes about 30-50 minutes to run, provided the 522MB file exists.")]

        public void SaveSimulationOutput522_ThenLoad_Same()
        {
            SaveSimulationOutput_ThenLoad_Same(CannedSimulationOutput.Filename522);
        }

        [Fact]
        //[Fact(Skip = "Takes about 30-50 minutes to run, provided the 525MB file exists.")]
        public void SaveSimulationOutput525_ThenLoad_Same()
        {
            SaveSimulationOutput_ThenLoad_Same(CannedSimulationOutput.Filename525);
        }

        private void SaveSimulationOutput_ThenLoad_Same(string filename, Func<SimulationOutput, SimulationOutput> preTransform = null)
        {
            if (preTransform == null)
            {
                preTransform = (SimulationOutput so) => so;
            }
            var text = FileReader.ReadAllTextInGitIgnoredFile(filename);
            var rawOutput = JsonConvert.DeserializeObject<SimulationOutput>(text);
            var yearCount = rawOutput.Years.Count;
            var simulationOutput = preTransform(rawOutput);
            var assetNameIdPairs = simulationOutput.InitialAssetSummaries.Select(a => AssetNameIdPairs.ForAssetSummaryDetail(a)).ToList();
            var assetSummary = simulationOutput.InitialAssetSummaries[0];
            var attributeNamesToIgnore = new List<string> { "AREA" };
            var numericAttributeNames = assetSummary.ValuePerNumericAttribute.Keys.Except(attributeNamesToIgnore).ToList();
            var textAttributeNames = assetSummary.ValuePerTextAttribute.Keys.ToList();
            var context = SimulationOutputCreationContextTestSetup.ContextWithObjectsInDatabase(TestHelper.UnitOfWork, assetNameIdPairs, numericAttributeNames, textAttributeNames, yearCount);
            TestHelper.UnitOfWork.SimulationOutputRepo.CreateSimulationOutput(context.SimulationId, simulationOutput);
            var loadedOutput = TestHelper.UnitOfWork.SimulationOutputRepo.GetSimulationOutput(context.SimulationId);
            SimulationOutputAssertions.SameSimulationOutput(loadedOutput, simulationOutput);
        }

        /// <summary>This test checks a SimulationOutput. It saves it to the database, loads it back from the database,
        /// then checks that they are the same. For the test to run, you need a json-encoded SimulationOutput saved at the place
        /// where it tries to load the file. The full path for WJ's case is in the regular comment below this message.</summary> 
        // C:\Code\Infrastructure Asset Management\BridgeCareApp\AppliedResearchAssociates.iAM.StressTesting\GitIgnored\SimulationOutput.json
        //[Fact]
        [Fact (Skip ="Takes about 2-3 minutes to run. Needs the above file.")]
        public void SaveSimulationOutput176_ThenLoad_Same()
        {
            SaveSimulationOutput_ThenLoad_Same(CannedSimulationOutput.Filename176);
        }

        [Fact]
       // [Fact(Skip = "Takes about 2-3 minutes to run. Needs the above file.")]
        public void SaveSimulationOutput221_ThenLoad_Same()
        {
            SaveSimulationOutput_ThenLoad_Same(CannedSimulationOutput.Filename221);
        }

        [Fact (Skip ="Takes about 2 hours to run, assuming the 965Mb file exists.")]
        //[Fact]
        public void SaveSimulationOutput965_ThenLoad_Same()
        {
            SaveSimulationOutput_ThenLoad_Same(CannedSimulationOutput.Filename965);
        }
    }
}
