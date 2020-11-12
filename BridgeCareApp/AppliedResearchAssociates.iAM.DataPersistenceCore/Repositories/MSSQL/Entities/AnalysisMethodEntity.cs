using System;
using AppliedResearchAssociates.iAM.DataAccess;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities
{
    public class AnalysisMethodEntity
    {
        public Guid Id { get; set; }
        public Guid SimulationId { get; set; }
        public Guid? AttributeId { get; set; }
        public string Description { get; set; }
        public OptimizationStrategy OptimizationStrategy { get; set; }
        public SpendingStrategy SpendingStrategy { get; set; }
        public bool ShouldApplyMultipleFeasibleCosts { get; set; }

        public bool ShouldDeteriorateDuringCashFlow { get; set; }

        public bool ShouldUseExtraFundsAcrossBudgets { get; set; }


        public virtual SimulationEntity Simulation { get; set; }
        public virtual AttributeEntity Attribute { get; set; }
        public virtual BenefitEntity Benefit { get; set; }
        public virtual CriterionLibraryAnalysisMethodEntity CriterionLibraryAnalysisMethodJoin { get; set; }
    }
}
