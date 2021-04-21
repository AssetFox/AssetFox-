using AppliedResearchAssociates.iAM.DTOs.Abstract;

namespace AppliedResearchAssociates.iAM.DTOs
{
    public class BudgetAmountDTO : BaseDTO
    {
        public string BudgetName { get; set; }

        public int Year { get; set; }

        public decimal Value { get; set; }
    }
}
