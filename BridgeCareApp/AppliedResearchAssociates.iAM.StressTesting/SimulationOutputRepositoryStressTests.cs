using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.Analysis.Engine;
using AppliedResearchAssociates.iAM.TestHelpers;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests;
using AppliedResearchAssociates.iAM.UnitTestsCore.TestUtils;
using Newtonsoft.Json;
using Xunit;

namespace AppliedResearchAssociates.iAM.StressTesting
{
    public class SimulationOutputRepositoryStressTests
    {
        private TestHelper _testHelper => TestHelper.Instance;

        [Fact]
        public void SaveLargeSimulationOutput_ThenLoad_Same()
        {
            var text = FileReader.ReadAllTextInGitIgnoredFile(CannedSimulationOutput.Filename);
            var simulationOutput = JsonConvert.DeserializeObject<SimulationOutput>(text);
            var assetNameIdPairs = simulationOutput.InitialAssetSummaries.Select(a => AssetNameIdPairs.ForAssetSummaryDetail(a)).ToList();
            var assetSummary = simulationOutput.InitialAssetSummaries[0];
            var attributeNamesToIgnore = new List<string> { "AREA" };
            var numericAttributeNames = assetSummary.ValuePerNumericAttribute.Keys.Except(attributeNamesToIgnore).ToList();
            var textAttributeNames = assetSummary.ValuePerTextAttribute.Keys.ToList();
            var context = SimulationOutputCreationContextTestSetup.ContextWithObjectsInDatabase(_testHelper.UnitOfWork, assetNameIdPairs, numericAttributeNames, textAttributeNames, 5);
            _testHelper.UnitOfWork.SimulationOutputRepo.CreateSimulationOutput(context.SimulationId, simulationOutput);
            var loadedOutput = _testHelper.UnitOfWork.SimulationOutputRepo.GetSimulationOutput(context.SimulationId);
            ObjectAssertions.Equivalent(simulationOutput, loadedOutput);
        }
    }
}
