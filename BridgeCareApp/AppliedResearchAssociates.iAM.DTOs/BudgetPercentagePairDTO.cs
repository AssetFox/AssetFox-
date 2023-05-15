using System;
using AppliedResearchAssociates.iAM.DTOs.Abstract;

namespace AppliedResearchAssociates.iAM.DTOs
{
    /// <summary>
    /// .
    /// </summary>
    public class BudgetPercentagePairDTO : BaseDTO
    {
        /// <summary>
        /// .
        /// </summary>
        public decimal Percentage { get; set; }

        /// <summary>
        /// .
        /// </summary>
        public Guid BudgetId { get; set; }

        /// <summary>
        /// .
        /// </summary>
        public string BudgetName { get; set; }
    }
}
