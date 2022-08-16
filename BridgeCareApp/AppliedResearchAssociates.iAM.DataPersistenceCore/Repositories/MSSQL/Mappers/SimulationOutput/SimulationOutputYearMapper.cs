using System;
using System.Collections.Generic;
using AppliedResearchAssociates.iAM.Analysis.Engine;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers
{
    public static class SimulationYearDetailMapper
    {
        public static SimulationYearDetailEntity ToEntityWithoutAssets(
            SimulationYearDetail domain,
            Guid simulationOutputId,
            Dictionary<string, Guid> attributeIdLookup)
        {
            var id = Guid.NewGuid();
            var budgets = BudgetDetailMapper.ToEntityList(domain.Budgets, id);
            var deficientConditionGoalDetails = DeficientConditionGoalDetailMapper.ToEntityList(domain.DeficientConditionGoals, id, attributeIdLookup);
            var targetConditionGoalDetails = TargetConditionGoalDetailMapper.ToEntityList(domain.TargetConditionGoals, id, attributeIdLookup);
            var entity = new SimulationYearDetailEntity
            {
                Id = id,
                Budgets = budgets,
                ConditionOfNetwork = domain.ConditionOfNetwork,
                DeficientConditionGoalDetails = deficientConditionGoalDetails,
                SimulationOutputId = simulationOutputId,
                TargetConditionGoalDetails = targetConditionGoalDetails,
                Year = domain.Year,
            };
            return entity;
        }

        public static SimulationYearDetail ToDomainWithoutAssets(SimulationYearDetailEntity entity)
        {
            var domain = new SimulationYearDetail(entity.Year)
            {
                ConditionOfNetwork = entity.ConditionOfNetwork,
            };
            var budgets = BudgetDetailMapper.ToDomainList(entity.Budgets);
            domain.Budgets.AddRange(budgets);
            var deficientConditionGoals = DeficientConditionGoalDetailMapper.ToDomainList(entity.DeficientConditionGoalDetails);
            domain.DeficientConditionGoals.AddRange(deficientConditionGoals);
            var targetConditionGoals = TargetConditionGoalDetailMapper.ToDomainList(entity.TargetConditionGoalDetails);
            domain.TargetConditionGoals.AddRange(targetConditionGoals);
            return domain;
        }

        public static SimulationYearDetail ToDomain(SimulationYearDetailEntity entity)
        {
            var domain = ToDomainWithoutAssets(entity);
            var assets = AssetDetailMapper.ToDomainList(entity.Assets);
            domain.Assets.AddRange(assets);
            return domain;
        }

        public static List<SimulationYearDetail> ToDomainList(ICollection<SimulationYearDetailEntity> entityList)
        {
            var domainList = new List<SimulationYearDetail>();
            foreach (var entity in entityList)
            {
                var domain = ToDomain(entity);
                domainList.Add(domain);
            }
            return domainList;
        }
    }
}
