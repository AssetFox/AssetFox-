using System;
using System.Collections.Generic;
using AppliedResearchAssociates.iAM.Analysis.Engine;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers
{
    public static class SimulationYearDetailMapper
    {
        public static SimulationYearDetailEntity ToEntity(
            SimulationYearDetail domain,
            Guid simulationOutputId,
            Dictionary<string, Guid> attributeIdLookup)
        {
            var id = Guid.NewGuid();
            var budgets = BudgetDetailMapper.ToEntityList(domain.Budgets, id);
            var deficientConditionGoalDetails = DeficientConditionGoalDetailMapper.ToEntityList(domain.DeficientConditionGoals, id, attributeIdLookup);
            var targetConditionGoalDetails = TargetConditionGoalDetailMapper.ToEntityList(domain.TargetConditionGoals, id, attributeIdLookup);
            var assets = AssetDetailMapper.ToEntityList(domain.Assets, id, attributeIdLookup);
            var entity = new SimulationYearDetailEntity
            {
                Id = id,
                Assets = assets,
                Budgets = budgets,
                ConditionOfNetwork = domain.ConditionOfNetwork,
                DeficientConditionGoalDetails = deficientConditionGoalDetails,
                SimulationOutputId = simulationOutputId,
                TargetConditionGoalDetails = targetConditionGoalDetails,
                Year = domain.Year,
            };
            return entity;
        }
    }
}
