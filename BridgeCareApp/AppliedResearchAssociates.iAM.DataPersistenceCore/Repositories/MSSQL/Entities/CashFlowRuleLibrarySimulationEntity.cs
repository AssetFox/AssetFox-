using System;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities
{
    public class CashFlowRuleLibrarySimulationEntity
    {
        public Guid CashFlowRuleLibraryId { get; set; }

        public Guid SimulationId { get; set; }

        public virtual CashFlowRuleLibraryEntity CashFlowRuleLibrary { get; set; }

        public virtual SimulationEntity Simulation { get; set; }
    }
}
