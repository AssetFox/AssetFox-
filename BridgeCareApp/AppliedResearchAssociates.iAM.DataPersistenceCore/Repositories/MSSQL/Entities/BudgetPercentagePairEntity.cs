using System;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.Abstract;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.LibraryEntities.Budget;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.ScenarioEntities.Budget;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities
{
    public class BudgetPercentagePairEntity : BaseEntity
    {
        public Guid Id { get; set; }

        //public Guid? BudgetId { get; set; }

        public Guid ScenarioBudgetId { get; set; }

        public Guid BudgetPriorityId { get; set; }

        public decimal Percentage { get; set; }

        //public virtual BudgetEntity Budget { get; set; }

        public virtual ScenarioBudgetEntity ScenarioBudget { get; set; }

        public virtual BudgetPriorityEntity BudgetPriority { get; set; }
    }
}
