using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AppliedResearchAssociates.iAM.Analysis.Engine;
using AppliedResearchAssociates.iAM.Common;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests;
using AppliedResearchAssociates.iAM.UnitTestsCore.TestUtils;
using Newtonsoft.Json;
using Xunit;

namespace AppliedResearchAssociates.iAM.StressTesting
{
    public class SimulationOutputRepositoryStressTests
    {
        private TestHelper _testHelper => TestHelper.Instance;

        private void Canonicalize(SimulationOutput simulationOutput)
        {
            simulationOutput.InitialAssetSummaries.Sort(a => a.AssetId.ToString());
            foreach (var year in simulationOutput.Years)
            {
                year.Assets.Sort(a => a.AssetId);
                foreach (var asset in year.Assets)
                {
                    asset.TreatmentOptions.Sort(to => to.TreatmentName);
                    asset.TreatmentConsiderations.Sort(tc => tc.TreatmentName);
                    asset.TreatmentRejections.Sort(tr => tr.TreatmentName);
                    asset.TreatmentSchedulingCollisions.Sort(tsc => tsc.NameOfUnscheduledTreatment);
                    foreach (var consideration in asset.TreatmentConsiderations)
                    {
                        consideration.CashFlowConsiderations.Sort(cfc => cfc.CashFlowRuleName);
                        consideration.BudgetUsages.Sort(bu => bu.BudgetName);
                    }
                }
            }
        }

        [Fact]
        public void SaveSimulationOutput522_ThenLoad_Same()
        {
            SaveSimulationOutput_ThenLoad_Same(CannedSimulationOutput.Filename522);
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
            var context = SimulationOutputCreationContextTestSetup.ContextWithObjectsInDatabase(_testHelper.UnitOfWork, assetNameIdPairs, numericAttributeNames, textAttributeNames, yearCount);
            _testHelper.UnitOfWork.SimulationOutputRepo.CreateSimulationOutput(context.SimulationId, simulationOutput);
            var loadedOutput = _testHelper.UnitOfWork.SimulationOutputRepo.GetSimulationOutput(context.SimulationId);
            Canonicalize(simulationOutput);
            Canonicalize(loadedOutput);
            var serializeOutput = JsonConvert.SerializeObject(simulationOutput, Formatting.Indented);
            var serializeLoaded = JsonConvert.SerializeObject(loadedOutput, Formatting.Indented);
            SimulationOutputAsserts.AssertCouldRepresentSameSimulationOutput(serializeOutput, serializeLoaded);
        }

        private void LoadSimulationOutput_Does(string filename, Guid simulationId)
        {
            var testHelper = new TestHelper(false);
            var simulationOutput = testHelper.UnitOfWork.SimulationOutputRepo.GetSimulationOutput(simulationId);
            var text = FileReader.ReadAllTextInGitIgnoredFile(filename);
            var outputFromFile = JsonConvert.DeserializeObject<SimulationOutput>(text);
            var serializeOutputFromFile = JsonConvert.SerializeObject(outputFromFile);
            var serializeLoaded = JsonConvert.SerializeObject(simulationOutput);
            SimulationOutputAsserts.AssertCouldRepresentSameSimulationOutput(serializeOutputFromFile, serializeLoaded);
        }

        /// <summary>This test checks a SimulationOutput. It saves it to the database, loads it back from the database,
        /// then checks that they are the same. For the test to run, you need a json-encoded SimulationOutput saved at the place
        /// where it tries to load the file. The full path for WJ's case is in the regular comment below this message.</summary> 
        // C:\Code\Infrastructure Asset Management\BridgeCareApp\AppliedResearchAssociates.iAM.StressTesting\GitIgnored\SimulationOutput.json
        [Fact]
        public void SaveSimulationOutput176_ThenLoad_Same()
        {
            SaveSimulationOutput_ThenLoad_Same(CannedSimulationOutput.Filename176);
        }



        [Fact(Skip = "Actually does reset the db, even though it tries not to. May not ever want this.")]
        public void LoadOutputFromDatabase_MatchesOutputFromFile()
        {
            var simulationIdString = "8F3B3D46-F294-404E-CDDC-08DA8AA13E89";
            var simulationId = Guid.Parse(simulationIdString);
            LoadSimulationOutput_Does(CannedSimulationOutput.Filename, simulationId);
        }
    }
}
