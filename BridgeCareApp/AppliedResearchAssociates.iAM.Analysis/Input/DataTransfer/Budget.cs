using System.Collections.Generic;

namespace AppliedResearchAssociates.iAM.Analysis.Input.DataTransfer
{
    public sealed class Budget
    {
        public string Name { get; set; }

        public List<decimal> YearlyAmounts { get; set; }
    }
}
