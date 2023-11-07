using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.Analysis.Engine;
using AppliedResearchAssociates.iAM.Analysis.Input.DataTransfer;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.DTOs.Enums;
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
            NameOfUsableBudget = scenario.InvestmentPlan.Budgets.First().Name,
            Cost = 100,
            Name = "Lovecraftian Horror",
            NameOfTemplateTreatment = scenario.SelectableTreatments.Single(t => t.ForCommittedProjectsOnly).Name,
            ProjectSource = ProjectSourceDTO.Maintenance.ToString(),
        }) ;
        
        return RunTest(scenario);
    }

    [Fact]
    public Task ExtremelyMinimalInput() => RunTest(InputCreation.CreateExtremelyMinimalInput());

    [Theory]
    [ClassData(typeof(ScenarioJsonFileNames))]
    public Task JsonFileInput(string fileName)
    {
        var path = Path.Combine(ScenarioJsonFileNames.FolderPath, fileName);
        var scenario = InputCreation.CreateInputFromJsonFile(path);
        return RunTest(scenario, fileName);
    }

    [Theory]
    [ClassData(typeof(EnumValues<OptimizationStrategy>))]
    public Task OptimizationStrategyUsage(OptimizationStrategy strategy)
    {
        var scenario = InputCreation.CreateSmallButRealInput();
        scenario.AnalysisMethod.OptimizationStrategy = strategy;
        return RunTest(scenario, strategy.ToString());
    }

    [Theory]
    [ClassData(typeof(EnumValues<SpendingStrategy>))]
    public Task SpendingStrategyUsage(SpendingStrategy strategy)
    {
        var scenario = InputCreation.CreateSmallButRealInput();
        scenario.AnalysisMethod.SpendingStrategy = strategy;
        return RunTest(scenario, strategy.ToString());
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
