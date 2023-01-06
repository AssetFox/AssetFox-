using System;
using System.Collections.Generic;
using AppliedResearchAssociates.iAM.DTOs;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Text;

namespace BridgeCareCore.Models
{
    public class InvestmentPagingSyncModel : PagingSyncModel<BudgetDTO>
    {
        public InvestmentPagingSyncModel() : base()
        {
            Investment = new InvestmentPlanDTO();
            Deletionyears = new List<int>();
            UpdatedBudgetAmounts = new Dictionary<string, List<BudgetAmountDTO>>();
            AddedBudgetAmounts = new Dictionary<string, List<BudgetAmountDTO>>();
            FirstYearAnalysisBudgetShift = 0;
        }
        public InvestmentPlanDTO Investment { get; set; }
        public List<int> Deletionyears { get; set; }
        public Dictionary<string, List<BudgetAmountDTO>> UpdatedBudgetAmounts { get; set; }
        public Dictionary<string, List<BudgetAmountDTO>> AddedBudgetAmounts { get; set; }
        public int FirstYearAnalysisBudgetShift { get; set; }
    }
}
