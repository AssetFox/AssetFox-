using System;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.Abstract;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.ScenarioEntities.CashFlow
{
    public class CriterionLibraryScenarioCashFlowRuleEntity : BaseCriterionLibraryJoinEntity
    {
        public Guid ScenarioCashFlowRuleId { get; set; }

        public virtual ScenarioCashFlowRuleEntity ScenarioCashFlowRule { get; set; }
    }
}
