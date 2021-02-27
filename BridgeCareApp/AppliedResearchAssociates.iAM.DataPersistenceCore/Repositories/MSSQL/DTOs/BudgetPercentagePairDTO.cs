using System;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.DTOs.Abstract;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.DTOs
{
    public class BudgetPercentagePairDTO : BaseDTO
    {
        public decimal Percentage { get; set; }
        public Guid BudgetId { get; set; }
        public string BudgetName { get; set; }
    }
}
