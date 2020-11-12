using System;
using System.Collections.Generic;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities
{
    public class CashFlowRuleLibraryEntity
    {
        public CashFlowRuleLibraryEntity()
        {
            CashFlowRules = new HashSet<CashFlowRuleEntity>();
            CashFlowRuleLibrarySimulations = new HashSet<CashFlowRuleLibrarySimulationEntity>();
        }

        public Guid Id { get; set; }

        public virtual ICollection<CashFlowRuleEntity> CashFlowRules { get; set; }
        public virtual ICollection<CashFlowRuleLibrarySimulationEntity> CashFlowRuleLibrarySimulations { get; set; }
    }
}
