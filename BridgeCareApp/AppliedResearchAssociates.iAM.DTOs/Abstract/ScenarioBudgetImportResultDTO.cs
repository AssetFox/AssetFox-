using System.Collections.Generic;

namespace AppliedResearchAssociates.iAM.DTOs.Abstract
{
    public class ScenarioBudgetImportResultDTO : WarningServiceResultDTO
    {
        public List<BudgetDTO> Budgets { get; set; }
    }
}
