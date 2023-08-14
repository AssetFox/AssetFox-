using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.Analysis.Engine;
using AppliedResearchAssociates.iAM.Analysis.Input.DataTransfer;
using VerifyXunit;
using Xunit;

namespace AppliedResearchAssociates.iAM.Analysis.Testing.CharacterizationTesting;

[UsesVerify]
public class SimulationRunnerTests
{
    [Fact]
    public Task CommittedProjectBeforeAnalysisPeriod()
    {
        var scenario = InputCreation.CreateExtremelyMinimalInput();

        scenario.CommittedProjects.Add(new()
        {
            AssetID = scenario.Network.MaintainableAssets.Find(asset => asset.Name == "LA 1").ID,
            Year = 2018,
            ShadowForAnyTreatment = 5,
            ShadowForSameTreatment = 10,
            NameOfUsableBudget = scenario.InvestmentPlan.Budgets.First().Name,
            Cost = 100,
            Name = "Lovecraftian Horror",
            Consequences =
            {
                new()
                {
                    AttributeName = "HEALTH",
                    ChangeExpression = "+50",
                },
            },
        });

        return RunTest(scenario);
    }

    [Fact]
    public Task ExtremelyMinimalInput() => RunTest(InputCreation.CreateExtremelyMinimalInput());

    [Theory]
    [ClassData(typeof(ScenarioJsonFilePaths))]
    public Task JsonFileInputs(string fileName)
    {
        var path = Path.Combine(ScenarioJsonFilePaths.ScenarioJsonFolderPath, fileName);
        var scenario = InputCreation.CreateInputFromJsonFile(path);
        return RunTest(scenario, fileName);
    }

    private static Task RunTest(Scenario scenario, string parametersText = null)
    {
        var input = scenario.ConvertOut();
        var runner = new SimulationRunner(input);
        runner.Run();
        var output = input.Results;
        var outputJson = JsonSerializer.Serialize(output, Serialization.Options);
        var result = Verifier.Verify(outputJson, "json").UseDirectory("Outputs");
        return parametersText is null ? result : result.UseTextForParameters(parametersText);
    }
}
