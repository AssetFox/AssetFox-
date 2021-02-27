using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.DTOs.Abstract;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.DTOs
{
    public class BudgetAmountDTO : BaseDTO
    {
        public string BudgetName { get; set; }
        public int Year { get; set; }
        public decimal Value { get; set; }
    }
}
