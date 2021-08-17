using System;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.Abstract;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.ScenarioEntities.BudgetPriority
{
    public class CriterionLibraryScenarioBudgetPriorityEntity : BaseCriterionLibraryJoinEntity
    {
        public Guid ScenarioBudgetPriorityId { get; set; }

        public virtual ScenarioBudgetPriorityEntity ScenarioBudgetPriority { get; set; }
    }
}
