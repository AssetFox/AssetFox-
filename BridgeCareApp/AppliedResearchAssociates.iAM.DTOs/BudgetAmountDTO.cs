using AppliedResearchAssociates.iAM.DTOs.Abstract;

namespace AppliedResearchAssociates.iAM.DTOs
{
    /// <summary>
    /// .
    /// </summary>
    public class BudgetAmountDTO : BaseDTO
    {
        /// <summary>
        /// .
        /// </summary>
        public string BudgetName { get; set; }

        /// <summary>
        /// .
        /// </summary>
        public int Year { get; set; }

        /// <summary>
        /// .
        /// </summary>
        public decimal Value { get; set; }
    }
}
