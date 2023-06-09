using AppliedResearchAssociates.iAM.DTOs.Abstract;

namespace AppliedResearchAssociates.iAM.DTOs
{
    /// <summary>
    /// Funding for a specific budget in a specific year
    /// </summary>
    public class BudgetAmountDTO : BaseDTO
    {
        /// <summary>
        /// Name of the budget where the funding is used
        /// </summary>
        public string BudgetName { get; set; }

        /// <summary>
        /// Year the funding will be used
        /// </summary>
        public int Year { get; set; }

        /// <summary>
        /// Amount of funding available
        /// </summary>
        public decimal Value { get; set; }
    }
}
