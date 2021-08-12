using System;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.Abstract;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.ScenarioEntities.Budget
{
    public class CriterionLibraryScenarioBudgetEntity : BaseCriterionLibraryJoinEntity
    {
        public Guid ScenarioBudgetId { get; set; }

        public virtual ScenarioBudgetEntity ScenarioBudget { get; set; }
    }
}
