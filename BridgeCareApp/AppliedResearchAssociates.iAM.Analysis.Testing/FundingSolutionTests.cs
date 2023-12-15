using System;
using AppliedResearchAssociates.iAM.Analysis.Engine;
using Xunit;

namespace AppliedResearchAssociates.iAM.Analysis.Testing;

public class FundingSolutionTests
{
    #region Degenerate inputs

    [Fact]
    public void MultipleBudgets_SingleTreatment_MultiFunding_Solved()
    {
        bool[,] allocationIsAllowed =
        {
            { true, },
            { true, },
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
            15,
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
            { 10, },
            { 5, },
            { 0, },
        };

        Assert.Equivalent(expected, solution, true);
    }

    [Fact]
    public void MultipleBudgets_SingleTreatment_MultiFunding_Unsolved()
    {
        bool[,] allocationIsAllowed =
        {
            { true, },
            { true, },
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
            35,
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
    public void MultipleBudgets_SingleTreatment_NoAllocation()
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
    public void MultipleBudgets_SingleTreatment_SingleFunding_Solved()
    {
        bool[,] allocationIsAllowed =
        {
            { false, },
            { true, },
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
            5,
        };

        var solved = Funding.TrySolve(
            allocationIsAllowed,
            budgetAmounts,
            treatmentCosts,
            false,
            out var solution);

        Assert.True(solved);

        decimal?[,] expected =
        {
            { null, },
            { 5, },
            { 0, },
        };

        Assert.Equivalent(expected, solution, true);
    }

    [Fact]
    public void MultipleBudgets_SingleTreatment_SingleFunding_Unsolved()
    {
        bool[,] allocationIsAllowed =
        {
            { false, },
            { true, },
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
            15,
        };

        var solved = Funding.TrySolve(
            allocationIsAllowed,
            budgetAmounts,
            treatmentCosts,
            false,
            out var solution);

        Assert.False(solved);
    }

    [Fact]
    public void SingleBudget_MultipleTreatments()
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

        decimal?[,] expected =
        {
            { 1, 2, 3, },
        };

        Assert.Equivalent(expected, solution, true);
        Assert.Equal(1, solution[0, 0]);
        Assert.Equal(2, solution[0, 1]);
        Assert.Equal(3, solution[0, 2]);
    }

    [Fact]
    public void SingleBudget_MultipleTreatments_MissingAllocation()
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
    public void SingleBudget_SingleTreatment()
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

        decimal?[,] expected =
        {
            { 1, },
        };

        Assert.Equivalent(expected, solution, true);
    }

    #endregion

    #region Multiple budgets, multiple treatments, multi-funding (LP)

    [Fact]
    public void MultipleBudgets_MultipleTreatments_MultiFunding_Minimal_Solved()
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

    // to-do: allocation column false

    // to-do: allocation row false

    #endregion

    #region Multiple budgets, multiple treatments, single-funding (MIP)

    #endregion
}
