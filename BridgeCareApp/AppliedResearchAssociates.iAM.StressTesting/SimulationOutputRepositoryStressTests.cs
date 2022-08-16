using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.Common;
using AppliedResearchAssociates.iAM.Analysis.Engine;
using AppliedResearchAssociates.iAM.TestHelpers;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests;
using AppliedResearchAssociates.iAM.UnitTestsCore.TestUtils;
using Newtonsoft.Json;
using Xunit;
using AppliedResearchAssociates.iAM.TestHelpers.Extensions;
using System.IO;

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

        
        private void SaveSimulationOutput_ThenLoad_Same(string filename, Func<SimulationOutput, SimulationOutput> preTransform = null)
        {
            if (preTransform == null)
            {
                preTransform = (SimulationOutput so) => so;
            }
            var text = FileReader.ReadAllTextInGitIgnoredFile(filename);
            var rawOutput = JsonConvert.DeserializeObject<SimulationOutput>(text);
            var simulationOutput = preTransform(rawOutput);
            var assetNameIdPairs = simulationOutput.InitialAssetSummaries.Select(a => AssetNameIdPairs.ForAssetSummaryDetail(a)).ToList();
            var assetSummary = simulationOutput.InitialAssetSummaries[0];
            var attributeNamesToIgnore = new List<string> { "AREA" };
            var numericAttributeNames = assetSummary.ValuePerNumericAttribute.Keys.Except(attributeNamesToIgnore).ToList();
            var textAttributeNames = assetSummary.ValuePerTextAttribute.Keys.ToList();
            var context = SimulationOutputCreationContextTestSetup.ContextWithObjectsInDatabase(_testHelper.UnitOfWork, assetNameIdPairs, numericAttributeNames, textAttributeNames, 5);
            _testHelper.UnitOfWork.SimulationOutputRepo.CreateSimulationOutput(context.SimulationId, simulationOutput);
            var loadedOutput = _testHelper.UnitOfWork.SimulationOutputRepo.GetSimulationOutput(context.SimulationId);
            Canonicalize(simulationOutput);
            Canonicalize(loadedOutput);
            var serializeOutput = JsonConvert.SerializeObject(simulationOutput, Formatting.Indented);
            var serializeLoaded = JsonConvert.SerializeObject(loadedOutput, Formatting.Indented);
            var outputFilename = filename.Substring(0, filename.IndexOf(".")) + "Output";
            var outputFilenameWithExtension = Path.ChangeExtension(outputFilename, "json");
            FileReader.WriteTextToGitIgnoredFile(outputFilenameWithExtension, serializeOutput);
            TrimmingAsserts(serializeOutput, serializeLoaded);
        }

        private static void TrimmingAsserts(string serializeOutput, string serializeLoaded)
        {
            var splitSerializeOutput = StringExtensions.ToLines(serializeOutput);
            var splitSerializeLoaded = StringExtensions.ToLines(serializeLoaded);
            var trimmedSerializedOutput = splitSerializeOutput.Select(str => str.Trim().TrimEnd(',')).ToList();
            var trimmedSerializedLoaded = splitSerializeLoaded.Select(str => str.Trim().TrimEnd(',')).ToList();
            trimmedSerializedOutput.Sort();
            trimmedSerializedLoaded.Sort();
            for (int i = 0; i < trimmedSerializedOutput.Count; i++)
            {
                var outputI = trimmedSerializedOutput[i];
                var loadedI = trimmedSerializedLoaded[i];
                Assert.Equal(outputI, loadedI);
            }
            Assert.Equal(serializeOutput.Length, serializeLoaded.Length);
        }

        [Fact]
        public void SaveLargeSimulationOutput_ThenLoad_Same()
        {
            SaveSimulationOutput_ThenLoad_Same(CannedSimulationOutput.Filename);
        }

    }
}
