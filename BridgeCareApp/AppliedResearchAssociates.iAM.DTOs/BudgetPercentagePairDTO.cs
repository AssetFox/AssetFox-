using System;
using AppliedResearchAssociates.iAM.DTOs.Abstract;

namespace AppliedResearchAssociates.iAM.DTOs
{
    /// <summary>
    /// A budget and the amount that could be spent for a given priority
    /// level
    /// </summary>
    public class BudgetPercentagePairDTO : BaseDTO
    {
        /// <summary>
        /// The percentage of the budget that can be spent
        /// </summary>
        public decimal Percentage { get; set; }

        /// <summary>
        /// ID of the associated budget
        /// </summary>
        public Guid BudgetId { get; set; }

        /// <summary>
        /// Name of the associated budget
        /// </summary>
        public string BudgetName { get; set; }
    }
}
