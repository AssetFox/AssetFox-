using System;
using System.Collections.Generic;
using System.Text;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.Domains;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappings
{
    public static class AnalysisMethodMapper
    {
        public static AnalysisMethodEntity ToEntity(this AnalysisMethod domain, Guid simulationId, Guid attributeId) =>
            new AnalysisMethodEntity
            {
                Id = Guid.NewGuid(),
                SimulationId = simulationId,
                AttributeId = attributeId,
                Description = domain.Description,
                OptimizationStrategy = domain.OptimizationStrategy,
                SpendingStrategy = domain.SpendingStrategy,
                ShouldApplyMultipleFeasibleCosts = domain.ShouldApplyMultipleFeasibleCosts,
                ShouldDeteriorateDuringCashFlow = domain.ShouldDeteriorateDuringCashFlow,
                ShouldUseExtraFundsAcrossBudgets = domain.ShouldUseExtraFundsAcrossBudgets
            };

        public static AnalysisMethod ToDomain(this AnalysisMethodEntity entity)
        {
            var domain = new AnalysisMethod(entity.Simulation.ToDomain())
            {
                Description = entity.Description,
                OptimizationStrategy = entity.OptimizationStrategy,
                SpendingStrategy = entity.SpendingStrategy,
                ShouldApplyMultipleFeasibleCosts = entity.ShouldApplyMultipleFeasibleCosts,
                ShouldDeteriorateDuringCashFlow = entity.ShouldDeteriorateDuringCashFlow,
                ShouldUseExtraFundsAcrossBudgets = entity.ShouldUseExtraFundsAcrossBudgets
            };

            if (entity.Attribute != null)
            {
                domain.Weighting = new NumberAttribute(entity.Attribute.Name);
            }

            return domain;
        }
    }
}
