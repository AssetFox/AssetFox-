using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.Analysis.Engine;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers
{
    public static class DeficientConditionGoalDetailMapper
    {
        public static List<DeficientConditionGoalDetailEntity> ToEntityList(
            List<DeficientConditionGoalDetail> domainList,
            Guid simulationYearDetailId)
        {
            var entities = new List<DeficientConditionGoalDetailEntity>();
            foreach (var domain in domainList)
            {
                var entity = ToEntity(domain, simulationYearDetailId);
                entities.Add(entity);
            }
            return entities;
        }

        private static DeficientConditionGoalDetailEntity ToEntity(
            DeficientConditionGoalDetail domain,
            Guid simulationYearDetailId)
        {
            var entity = new DeficientConditionGoalDetailEntity
            {
                ActualDeficientPercentage = domain.ActualDeficientPercentage,
                AllowedDeficientPercentage = domain.AllowedDeficientPercentage,
                DeficientLimit = domain.DeficientLimit,
                GoalIsMet = domain.GoalIsMet,
                GoalName = domain.GoalName,
                SimulationYearDetailId = simulationYearDetailId,
            };
            return entity;
        }
    }
}
