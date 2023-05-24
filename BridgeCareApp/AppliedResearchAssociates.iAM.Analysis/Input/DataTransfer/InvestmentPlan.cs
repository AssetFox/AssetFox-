﻿using System.Collections.Generic;

namespace AppliedResearchAssociates.iAM.Analysis.Input.DataTransfer;

public sealed class InvestmentPlan
{
    public List<Budget> Budgets { get; set; } = new();

    public List<BudgetCondition> BudgetConditions { get; set; } = new();

    public List<CashFlowRule> CashFlowRules { get; set; } = new();

    public int FirstYearOfAnalysisPeriod { get; set; }

    public double InflationRatePercentage { get; set; }

    public decimal MinimumProjectCostLimit { get; set; }

    public int NumberOfYearsInAnalysisPeriod { get; set; }

    public bool ShouldAccumulateUnusedBudgetAmounts { get; set; }
}
