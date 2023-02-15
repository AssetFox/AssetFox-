using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DTOs.Enums;

namespace AppliedResearchAssociates.iAM.UnitTestsCore
{
    public static class AnalysisMethodEntities
    {
        public static AnalysisMethodEntity TestAnalysis(Guid simulationId, Guid? id = null)
        {
            var resolveId = id ?? Guid.NewGuid();
            var returnValue = new AnalysisMethodEntity
            {
                Id = resolveId,
                SimulationId = simulationId,
                OptimizationStrategy = OptimizationStrategy.Benefit,
                SpendingStrategy = SpendingStrategy.NoSpending,
                ShouldApplyMultipleFeasibleCosts = false,
                ShouldDeteriorateDuringCashFlow = false,
                ShouldUseExtraFundsAcrossBudgets = false
            };
            return returnValue;
        }
    }
}
