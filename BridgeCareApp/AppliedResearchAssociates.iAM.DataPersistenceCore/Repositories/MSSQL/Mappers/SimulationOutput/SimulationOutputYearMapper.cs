using System;
using System.Collections.Generic;
using AppliedResearchAssociates.iAM.Analysis.Engine;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers
{
    public static class SimulationOutputYearMapper
    {
        public static SimulationYearDetailEntity ToEntity(
            SimulationYearDetail domain,
            Guid simulationOutputId,
            Dictionary<string, Guid> attributeIdLookup)
        {
            var id = Guid.NewGuid();
            var budgets = BudgetDetailMapper.ToEntityList(domain.Budgets, id);
            var deficientConditionGoalDetails = DeficientConditionGoalDetailMapper.ToEntityList(domain.DeficientConditionGoals, id);
            var assets = AssetDetailMapper.ToEntityList(domain.Assets, id, attributeIdLookup);
            var entity = new SimulationYearDetailEntity
            {
                Id = id,
                ConditionOfNetwork = domain.ConditionOfNetwork,
                SimulationOutputId = simulationOutputId,
                DeficientConditionGoalDetails = deficientConditionGoalDetails,
                Year = domain.Year,
                Budgets = budgets,
                Assets = assets,
            };
            return entity;
        }
    }
}
