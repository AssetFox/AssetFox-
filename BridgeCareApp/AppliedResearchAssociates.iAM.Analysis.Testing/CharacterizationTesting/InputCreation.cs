using System.IO;
using System.Text.Json;
using AppliedResearchAssociates.iAM.Analysis.Input.DataTransfer;

namespace AppliedResearchAssociates.iAM.Analysis.Testing.CharacterizationTesting;

public static class InputCreation
{
    public static Scenario CreateExtremelyMinimalInput() => new()
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

    public static Scenario CreateInputFromJsonFile(string path)
    {
        using var jsonFile = File.OpenRead(path);
        return JsonSerializer.Deserialize<Scenario>(jsonFile, Serialization.Options);
    }

    public static Scenario CreateSmallButRealInput()
    {
        var path = Path.Combine(
            ScenarioJsonFileNames.FolderPath,
            "2023-08-11 - iamtest - test scenario1.json");

        return CreateInputFromJsonFile(path);
    }
}
