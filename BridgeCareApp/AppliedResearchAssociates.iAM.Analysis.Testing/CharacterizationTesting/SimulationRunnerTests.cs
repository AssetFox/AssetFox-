using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
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
        var scenario = CreateExtremelyMinimalInput();

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
    public Task ExtremelyMinimalInput() => RunTest(CreateExtremelyMinimalInput());

    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        Converters = { new JsonStringEnumConverter() },
        Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        WriteIndented = true,
    };

    private static Scenario CreateExtremelyMinimalInput() => new()
    {
        AnalysisMethod = new()
        {
            BenefitAttributeName = "HEALTH",
            BudgetPriorities =
            {
                new()
                {
                    BudgetPercentagePairs =
                    {
                        new()
                        {
                            BudgetName = "Bag of money",
                            Percentage = 100,
                        },
                    },
                },
            },
            SpendingStrategy = DTOs.Enums.SpendingStrategy.UnlimitedSpending,
        },
        InvestmentPlan =
        {
            Budgets =
            {
                new()
                {
                    Name = "Bag of money",
                    YearlyAmounts = { 1_000_000 },
                },
            },
            FirstYearOfAnalysisPeriod = 2020,
            NumberOfYearsInAnalysisPeriod = 1,
        },
        Name = "Extremely minimal input",
        NameOfPassiveTreatment = "Forget about it",
        Network = new()
        {
            AttributeSystem =
            {
                NumberAttributes =
                {
                    new()
                    {
                        Name = "HEALTH",
                        IsDecreasingWithDeterioration = true,
                        MaximumValue = 100,
                        MinimumValue = 0,
                        DefaultValue = 50,
                    },
                },
            },
            MaintainableAssets =
            {
                new()
                {
                    Name = "LA 1",
                    NumberAttributeHistories =
                    {
                        new()
                        {
                            AttributeName = "AGE",
                            History =
                            {
                                new()
                                {
                                    Value = 20,
                                    Year = 2015,
                                },
                            },
                        },
                        new()
                        {
                            AttributeName = "HEALTH",
                            History =
                            {
                                new()
                                {
                                    Value = 90,
                                    Year = 2015,
                                },
                            },
                        },
                    },
                    SpatialWeightExpression = "1",
                },
            },
        },
        NumberOfYearsOfTreatmentOutlook = 100,
        PerformanceCurves =
        {
            new()
            {
                AttributeName = "HEALTH",
                EquationExpression = "(0,100)(100,0)",
            },
        },
        SelectableTreatments =
        {
            new()
            {
                Name = "Forget about it",
                Consequences =
                {
                    new()
                    {
                        AttributeName = "AGE",
                        ChangeExpression = "+1",
                    },
                },
            },
            new()
            {
                Name = "Eldritch wizardry",
                Consequences =
                {
                    new()
                    {
                        AttributeName = "HEALTH",
                        ChangeExpression = "+10",
                    },
                },
                Costs =
                {
                    new()
                    {
                        EquationExpression = "10000 * (100 - HEALTH)",
                    },
                },
                FeasibilityCriterionExpressions = { "AGE >= 0" },
                NamesOfUsableBudgets = { "Bag of money" },
                ShadowForAnyTreatment = 2,
                ShadowForSameTreatment = 5,
            },
        },
    };

    private static Task RunTest(Scenario scenario)
    {
        var input = scenario.ConvertOut();
        var runner = new SimulationRunner(input);
        runner.Run();
        var output = input.Results;
        var outputJson = JsonSerializer.Serialize(output, SerializerOptions);
        return Verifier.Verify(outputJson, "json").UseDirectory("Outputs");
    }
}
