namespace AppliedResearchAssociates.iAM.Analysis.Logic.Testing;

public class MultiYearFundingTests
{
    [Fact]
    public void _SanityCheck_TrivialFailure_NoMoney()
    {
        bool[,] allocationIsAllowed =
        {
            { true, true, true },
            { true, true, true },
            { true, true, true },
        };

        decimal[][] amountPerBudgetPerYear =
        [
            [0, 0, 0],
            [0, 0, 0],
            [0, 0, 0],
        ];

        decimal[] costPerTreatment = [100, 100, 100];
        decimal[] costPercentages = [30, 30, 40];

        var solved = Funding.TrySolve(
            allocationIsAllowed,
            amountPerBudgetPerYear,
            costPerTreatment,
            costPercentages,
            new(),
            out var actualSolution);

        Assert.False(solved);
    }

    [Fact]
    public void _SanityCheck_TrivialSuccess_UnlimitedSpending()
    {
        bool[,] allocationIsAllowed =
        {
            { true, false, true },
            { false, true, true },
            { true, true, false },
        };

        decimal[][] amountPerBudgetPerYear =
        [
            [0, 0, 0],
            [0, 0, 0],
            [0, 0, 0],
        ];

        decimal[] costPerTreatment = [100, 100, 100];
        decimal[] costPercentages = [30, 30, 40];

        var solved = Funding.TrySolve(
            allocationIsAllowed,
            amountPerBudgetPerYear,
            costPerTreatment,
            costPercentages,
            new()
            {
                UnlimitedSpending = true,
            },
            out var actualSolution);

        Assert.True(solved);

        decimal?[,] expectedYear0 =
        {
            { 30, null, 30 },
            { null, 30, 0 },
            { 0, 0, null },
        };

        decimal?[,] expectedYear1 =
        {
            { 30, null, 30 },
            { null, 30, 0 },
            { 0, 0, null },
        };

        decimal?[,] expectedYear2 =
        {
            { 40, null, 40 },
            { null, 40, 0 },
            { 0, 0, null },
        };

        decimal?[][,] expectedSolution = [expectedYear0, expectedYear1, expectedYear2];

        Assert.Equivalent(expectedSolution, actualSolution, true);
    }

    [Fact]
    public void MultiBudget_MultiTreatment_MultiFunding_WithoutCarryover()
    {
        bool[,] allocationIsAllowed =
        {
            { true, true, true },
            { true, true, true },
            { true, true, true },
        };

        decimal[][] amountPerBudgetPerYear =
        [
            [50, 50, 50],
            [50, 50, 50],
            [50, 50, 50],
        ];

        decimal[] costPerTreatment = [100, 100, 100];
        decimal[] costPercentages = [30, 30, 40];

        var solved = Funding.TrySolve(
            allocationIsAllowed,
            amountPerBudgetPerYear,
            costPerTreatment,
            costPercentages,
            new()
            {
                BudgetCarryoverIsAllowed = false,
                MultipleBudgetsCanFundEachTreatment = true,
            },
            out var actualSolution);

        Assert.True(solved);

        decimal?[,] expectedYear0 =
        {
            { 30, 0, 20 },
            { 0, 30, 10 },
            { 0, 0, 0 },
        };

        decimal?[,] expectedYear1 =
        {
            { 0, 30, 20 },
            { 30, 0, 10 },
            { 0, 0, 0 },
        };

        decimal?[,] expectedYear2 =
        {
            { 40, 0, 10 },
            { 0, 40, 10 },
            { 0, 0, 20 },
        };

        decimal?[][,] expectedSolution = [expectedYear0, expectedYear1, expectedYear2];

        Assert.Equivalent(expectedSolution, actualSolution, true);
    }

    [Fact]
    public void MultiBudget_SingleTreatment_MultiFunding_WithoutCarryover()
    {
        bool[,] allocationIsAllowed =
        {
            { true },
            { true },
            { true },
        };

        decimal[][] amountPerBudgetPerYear =
        [
            [10, 20, 10],
            [50, 50, 50],
            [20, 30, 50],
        ];

        decimal[] costPerTreatment = [100];
        decimal[] costPercentages = [30, 30, 40];

        var solved = Funding.TrySolve(
            allocationIsAllowed,
            amountPerBudgetPerYear,
            costPerTreatment,
            costPercentages,
            new()
            {
                BudgetCarryoverIsAllowed = false,
                MultipleBudgetsCanFundEachTreatment = true,
            },
            out var actualSolution);

        Assert.True(solved);

        decimal?[,] expectedYear0 =
        {
            { 10 },
            { 20 },
            { 0 },
        };

        decimal?[,] expectedYear1 =
        {
            { 30 },
            { 0 },
            { 0 },
        };

        decimal?[,] expectedYear2 =
        {
            { 20 },
            { 20 },
            { 0 },
        };

        decimal?[][,] expectedSolution = [expectedYear0, expectedYear1, expectedYear2];

        Assert.Equivalent(expectedSolution, actualSolution, true);
    }

    [Fact]
    public void SingleBudget_MultiTreatment_MultiFunding_WithCarryover()
    {
        bool[,] allocationIsAllowed =
        {
            { true, true, true },
        };

        decimal[][] amountPerBudgetPerYear =
        [
            [100],
            [100],
            [100],
        ];

        decimal[] costPerTreatment = [100, 100, 100];
        decimal[] costPercentages = [30, 30, 40];

        var solved = Funding.TrySolve(
            allocationIsAllowed,
            amountPerBudgetPerYear,
            costPerTreatment,
            costPercentages,
            new()
            {
                BudgetCarryoverIsAllowed = true,
                MultipleBudgetsCanFundEachTreatment = true,
            },
            out var actualSolution);

        Assert.True(solved);

        decimal?[,] expectedYear0 =
        {
            { 30, 30, 30 },
        };

        decimal?[,] expectedYear1 =
        {
            { 30, 30, 30 },
        };

        decimal?[,] expectedYear2 =
        {
            { 40, 40, 40 },
        };

        decimal?[][,] expectedSolution = [expectedYear0, expectedYear1, expectedYear2];

        Assert.Equivalent(expectedSolution, actualSolution, true);
    }
}
