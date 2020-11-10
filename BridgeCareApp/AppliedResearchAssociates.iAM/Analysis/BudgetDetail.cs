using System;
using AppliedResearchAssociates.iAM.Domains;

namespace AppliedResearchAssociates.iAM.Analysis
{
    public sealed class BudgetDetail
    {
        public BudgetDetail(Budget budget, decimal availableFunding)
        {
            BudgetName = budget?.Name ?? throw new ArgumentNullException(nameof(budget));
            AvailableFunding = availableFunding;
        }

        public decimal AvailableFunding { get; }

        public string BudgetName { get; }
    }
}
