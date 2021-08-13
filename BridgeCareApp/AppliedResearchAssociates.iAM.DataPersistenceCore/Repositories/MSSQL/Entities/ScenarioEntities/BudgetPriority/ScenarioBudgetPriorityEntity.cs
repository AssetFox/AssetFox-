using System;
using System.Collections.Generic;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.Abstract;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.ScenarioEntities.Budget;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.ScenarioEntities.BudgetPriority
{
    public class ScenarioBudgetPriorityEntity : BaseBudgetPriorityEntity
    {
        public ScenarioBudgetPriorityEntity() => BudgetPercentagePairs = new HashSet<BudgetPercentagePairEntity>();

        public Guid SimulationId { get; set; }

        public virtual SimulationEntity Simulation { get; set; }

        public virtual ICollection<BudgetPercentagePairEntity> BudgetPercentagePairs { get; set; }

        public virtual CriterionLibraryScenarioBudgetPriorityEntity CriterionLibraryScenarioBudgetPriorityJoin { get; set; }
    }
}
