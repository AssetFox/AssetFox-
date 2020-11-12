﻿using System;
using System.Collections.Generic;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities
{
    public class BudgetEntity
    {
        public BudgetEntity()
        {
            BudgetAmounts = new HashSet<BudgetAmountEntity>();
            BudgetPercentagePairs = new HashSet<BudgetPercentagePairEntity>();
            TreatmentBudgetJoins = new HashSet<TreatmentBudgetEntity>();
        }

        public Guid Id { get; set; }
        public Guid InvestmentPlanId { get; set; }

        public virtual InvestmentPlanEntity InvestmentPlan { get; set; }
        public virtual BudgetConditionEntity BudgetCondition { get; set; }
        public virtual ICollection<BudgetAmountEntity> BudgetAmounts { get; set; }
        public virtual ICollection<BudgetPercentagePairEntity> BudgetPercentagePairs { get; set; }
        public virtual ICollection<TreatmentBudgetEntity> TreatmentBudgetJoins { get; set; }
        public virtual ICollection<CommittedProjectEntity> CommittedProjects { get; set; }
    }
}
