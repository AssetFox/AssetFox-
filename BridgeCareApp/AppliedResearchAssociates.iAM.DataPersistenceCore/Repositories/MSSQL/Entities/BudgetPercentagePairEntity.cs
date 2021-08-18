using System;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.Abstract;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.ScenarioEntities.Budget;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.ScenarioEntities.BudgetPriority;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities
{
    public class BudgetPercentagePairEntity : BaseEntity
    {
        public Guid Id { get; set; }

        public Guid ScenarioBudgetId { get; set; }

        public Guid ScenarioBudgetPriorityId { get; set; }

        public decimal Percentage { get; set; }

        public virtual ScenarioBudgetEntity ScenarioBudget { get; set; }


        public virtual ScenarioBudgetPriorityEntity ScenarioBudgetPriority { get; set; }
    }
}
