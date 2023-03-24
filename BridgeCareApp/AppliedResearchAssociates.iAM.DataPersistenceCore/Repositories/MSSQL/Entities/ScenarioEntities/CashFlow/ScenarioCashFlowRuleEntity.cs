using System;
using System.Collections;
using System.Collections.Generic;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.Abstract;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.ScenarioEntities.CashFlow
{
    public class ScenarioCashFlowRuleEntity : BaseCashFlowRuleEntity
    {
        public ScenarioCashFlowRuleEntity() =>
            ScenarioCashFlowDistributionRules = new HashSet<ScenarioCashFlowDistributionRuleEntity>();

        public Guid SimulationId { get; set; }

        public Guid LibraryId { get; set; }

        public bool IsModified { get; set; }

        public virtual SimulationEntity Simulation { get; set; }

        public virtual CriterionLibraryScenarioCashFlowRuleEntity CriterionLibraryScenarioCashFlowRuleJoin { get; set; }

        public virtual ICollection<ScenarioCashFlowDistributionRuleEntity> ScenarioCashFlowDistributionRules { get;
            set;
        }
    }
}
