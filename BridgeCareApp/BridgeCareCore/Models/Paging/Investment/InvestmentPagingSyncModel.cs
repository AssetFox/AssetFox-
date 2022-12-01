using System;
using System.Collections.Generic;
using AppliedResearchAssociates.iAM.DTOs;

namespace BridgeCareCore.Models
{
    public class InvestmentPagingSyncModel
    {
        public InvestmentPagingSyncModel()
        {
            Investment = new InvestmentPlanDTO();
            BudgetsForDeletion = new List<Guid>();
            UpdatedBudgets = new List<BudgetDTO>();
            AddedBudgets = new List<BudgetDTO>();
            Deletionyears = new List<int>();
            UpdatedBudgetAmounts = new Dictionary<string, List<BudgetAmountDTO>>();
            AddedBudgetAmounts = new Dictionary<string, List<BudgetAmountDTO>>();
            FirstYearAnalysisBudgetShift = 0;
        }
        public InvestmentPlanDTO Investment { get; set; }
        public Guid? LibraryId { get; set; }
        public List<Guid> BudgetsForDeletion { get; set; }
        public List<BudgetDTO> UpdatedBudgets { get; set; }
        public List<BudgetDTO> AddedBudgets { get; set; }
        public List<int> Deletionyears { get; set; }
        public Dictionary<string, List<BudgetAmountDTO>> UpdatedBudgetAmounts { get; set; }
        public Dictionary<string, List<BudgetAmountDTO>> AddedBudgetAmounts { get; set; }
        public int FirstYearAnalysisBudgetShift { get; set; }
    }
}
