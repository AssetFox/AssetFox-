using System;
using System.Collections.Generic;
using System.Text;
using AppliedResearchAssociates.iAM.DTOs.Abstract;

namespace AppliedResearchAssociates.iAM.DTOs
{
    public class BudgetAmountDTOWithBudgetId
    {
        public BudgetAmountDTO BudgetAmount { get; set; }
        public Guid BudgetId { get; set; }
    }
}
