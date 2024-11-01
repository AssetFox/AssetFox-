using System.Text.Json;
using AppliedResearchAssociates.iAM.Analysis.Engine;
using AppliedResearchAssociates.iAM.Analysis.Input.DataTransfer;
using AppliedResearchAssociates.iAM.DTOs.Enums;
using AppliedResearchAssociates.Validation;
using Xunit.Abstractions;

namespace AppliedResearchAssociates.iAM.Analysis.Testing.CharacterizationTesting;

[UsesVerify]
public class SimulationRunnerTests(ITestOutputHelper outputHelper)
{
    #region facts

    [Fact]
    public Task CashFlowCommittedProject()
    {
        var scenario = InputCreation.CreateExtremelyMinimalInput();

        scenario.CommittedProjects.Add(new()
        {
            AssetID = scenario.Network.MaintainableAssets.Find(asset => asset.Name == "LA 1").ID,
            Year = 2020,
            NameOfUsableBudget = scenario.InvestmentPlan.Budgets.First().Name,
            Cost = 6_000_000,
            Name = "Lovecraftian Horror (Year 1)",
            NameOfTemplateTreatment = scenario.SelectableTreatments.First(t => t.ForCommittedProjectsOnly).Name,
        });

        scenario.CommittedProjects.Add(new()
        {
            AssetID = scenario.Network.MaintainableAssets.Find(asset => asset.Name == "LA 1").ID,
            Year = 2021,
            NameOfUsableBudget = scenario.InvestmentPlan.Budgets.First().Name,
            Cost = 6_000_000,
            Name = "Lovecraftian Horror (Year 2)",
            NameOfTemplateTreatment = scenario.SelectableTreatments.First(t => t.ForCommittedProjectsOnly).Name,
        });

        scenario.InvestmentPlan.CashFlowRules.Add(new()
        {
            Name = "Default Cash Flow Rule",
            DistributionRules =
            {
                new()
                {
                    CostCeiling = 5_000_000,
                    YearlyPercentages =
                    {
                        100
                    },
                },
            },
        });

        return RunTest(scenario);
    }

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
            NameOfTemplateTreatment = scenario.SelectableTreatments.First(t => t.ForCommittedProjectsOnly).Name,
        });

        return RunTest(scenario);
    }

    [Fact]
    public Task ExtremelyMinimalInput() => RunTest(InputCreation.CreateExtremelyMinimalInput());

    [Fact]
    public Task MultipleCommittedProjectsForOneAssetYear()
    {
        var scenario = InputCreation.CreateExtremelyMinimalInput();

        scenario.CommittedProjects.Add(new()
        {
            AssetID = scenario.Network.MaintainableAssets.Find(asset => asset.Name == "LA 1").ID,
            Year = 2020,
            NameOfUsableBudget = scenario.InvestmentPlan.Budgets.First().Name,
            Cost = 100,
            Name = "Lovecraftian Horror #1",
            NameOfTemplateTreatment = scenario.SelectableTreatments.First(t => t.ForCommittedProjectsOnly).Name,
        });

        scenario.CommittedProjects.Add(new()
        {
            AssetID = scenario.Network.MaintainableAssets.Find(asset => asset.Name == "LA 1").ID,
            Year = 2020,
            NameOfUsableBudget = scenario.InvestmentPlan.Budgets.First().Name,
            Cost = 100,
            Name = "Lovecraftian Horror #2",
            NameOfTemplateTreatment = scenario.SelectableTreatments
                .First(t => !t.ForCommittedProjectsOnly && t.Name != scenario.NameOfPassiveTreatment).Name,
        });

        return RunTest(scenario);
    }

    #endregion

    #region theories

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

    #endregion

    private Task RunTest(Scenario scenario, string parametersText = null)
    {
        var input = scenario.ConvertOut();

        foreach (var validationResult in input.GetAllValidationResults([]))
        {
            outputHelper.WriteLine($"[{validationResult.Status}] {validationResult.Message}");
        }

        var runner = new SimulationRunner(input);
        runner.Progress += (sender, e) => outputHelper.WriteLine(e.ToString());
        runner.SimulationLog += (sender, e) => outputHelper.WriteLine(e.MessageBuilder.ToString());
        runner.Run();

        var output = input.Results;
        var outputJson = JsonSerializer.Serialize(output, Serialization.Options);
        var result = Verify(outputJson, "json").UseDirectory("Outputs");
        return parametersText is null ? result : result.UseTextForParameters(parametersText);
    }
}
