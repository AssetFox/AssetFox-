using System.Collections.Generic;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.Abstract;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities
{
    public class CashFlowRuleLibraryEntity : LibraryEntity
    {
        public CashFlowRuleLibraryEntity()
        {
            CashFlowRules = new HashSet<CashFlowRuleEntity>();
            CashFlowRuleLibrarySimulationJoins = new HashSet<CashFlowRuleLibrarySimulationEntity>();
        }

        public virtual ICollection<CashFlowRuleEntity> CashFlowRules { get; set; }

        public virtual ICollection<CashFlowRuleLibrarySimulationEntity> CashFlowRuleLibrarySimulationJoins { get; set; }
    }
}
