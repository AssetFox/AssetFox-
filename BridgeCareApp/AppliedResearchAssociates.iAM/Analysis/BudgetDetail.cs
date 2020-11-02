﻿using System;
using Newtonsoft.Json;

namespace AppliedResearchAssociates.iAM.Analysis
{
    public sealed class BudgetDetail
    {
        public BudgetDetail(Budget budget, decimal availableFunding)
        {
            BudgetName = budget?.Name ?? throw new ArgumentNullException(nameof(budget));
            AvailableFunding = availableFunding;
        }

        [JsonConstructor]
        public BudgetDetail(decimal availableFunding, string budgetName)
        {
            AvailableFunding = availableFunding;
            BudgetName = budgetName ?? throw new ArgumentNullException(nameof(budgetName));
        }

        public decimal AvailableFunding { get; }

        public string BudgetName { get; }
    }
}
