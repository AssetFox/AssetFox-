using System;
using AppliedResearchAssociates.iAM.Analysis.Engine;
using Xunit;

namespace AppliedResearchAssociates.iAM.Analysis.Testing;

public class FundingSolutionTests
{
    [Fact]
    public void Budgets1Treatments3()
    {
        bool[,] allocationIsAllowed =
        {
            { true, true, true, },
        };

        decimal[] budgetAmounts =
        {
            10,
        };

        decimal[] treatmentCosts =
        {
            1,
            2,
            3,
        };

        var solved = Funding.TrySolve(
            allocationIsAllowed,
            budgetAmounts,
            treatmentCosts,
            true,
            out var solution);

        Assert.True(solved);
        Assert.Equal(1, solution[0, 0]);
        Assert.Equal(2, solution[0, 1]);
        Assert.Equal(3, solution[0, 2]);
    }

    [Fact]
    public void Budgets1Treatments3WithMissingAllocation()
    {
        bool[,] allocationIsAllowed =
        {
            { true, false, true, },
        };

        decimal[] budgetAmounts =
        {
            10,
        };

        decimal[] treatmentCosts =
        {
            1,
            2,
            3,
        };

        var solved = Funding.TrySolve(
            allocationIsAllowed,
            budgetAmounts,
            treatmentCosts,
            true,
            out var solution);

        Assert.False(solved);
    }

    [Fact]
    public void Budgets3Treatments1()
    {
        bool[,] allocationIsAllowed =
        {
            { false, },
            { false, },
            { true, },
        };

        decimal[] budgetAmounts =
        {
            10,
            10,
            10,
        };

        decimal[] treatmentCosts =
        {
            1,
        };

        var solved = Funding.TrySolve(
            allocationIsAllowed,
            budgetAmounts,
            treatmentCosts,
            true,
            out var solution);

        Assert.True(solved);
        Assert.Equal(default, solution[0, 0]);
        Assert.Equal(default, solution[1, 0]);
        Assert.Equal(1, solution[2, 0]);
    }

    [Fact]
    public void Budgets3Treatments1WithNoAllocation()
    {
        bool[,] allocationIsAllowed =
        {
            { false, },
            { false, },
            { false, },
        };

        decimal[] budgetAmounts =
        {
            10,
            10,
            10,
        };

        decimal[] treatmentCosts =
        {
            1,
        };

        var solved = Funding.TrySolve(
            allocationIsAllowed,
            budgetAmounts,
            treatmentCosts,
            true,
            out var solution);

        Assert.False(solved);
    }

    [Fact]
    public void SanityCheckDegenerate()
    {
        bool[,] allocationIsAllowed =
        {
            { true, },
        };

        decimal[] budgetAmounts =
        {
            10,
        };

        decimal[] treatmentCosts =
        {
            1,
        };

        var solved = Funding.TrySolve(
            allocationIsAllowed,
            budgetAmounts,
            treatmentCosts,
            true,
            out var solution);

        Assert.True(solved);
        Assert.Equal(1, solution[0, 0]);
    }

    [Fact]
    public void SanityCheckLP()
    {
        bool[,] allocationIsAllowed =
        {
            { true, true, },
            { true, false, },
        };

        decimal[] budgetAmounts =
        {
            10,
            6,
        };

        decimal[] treatmentCosts =
        {
            8,
            8,
        };

        var solved = Funding.TrySolve(
            allocationIsAllowed,
            budgetAmounts,
            treatmentCosts,
            true,
            out var solution);

        Assert.True(solved);

        decimal?[,] expected =
        {
            { 2, 8, },
            { 6, default, },
        };

        Assert.Equivalent(expected, solution, true);
    }
}
