using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.Analysis.Engine;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers
{
    public static class TargetConditionGoalDetailMapper
    {
        public static TargetConditionGoalDetailEntity ToEntity(
            this TargetConditionGoalDetail domain,
            Guid simulationYearDetailId)
        {
            var entity = new TargetConditionGoalDetailEntity
            {
                ActualValue = domain.ActualValue,
                GoalIsMet = domain.GoalIsMet,
                GoalName = domain.GoalName,
                SimulationYearDetailId = simulationYearDetailId,
                TargetValue = domain.TargetValue,
            };
            return entity;
        }

        public static List<TargetConditionGoalDetailEntity> ToEntityList(
            List<TargetConditionGoalDetail> domainList,
            Guid simulationYearDetailId)
        {
            var entities = new List<TargetConditionGoalDetailEntity>();
            foreach (var domain in domainList)
            {
                var entity = ToEntity(domain, simulationYearDetailId);
                entities.Add(entity);
            }
            return entities;
        }
    }
}
