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
            Guid simulationYearDetailId,
            Dictionary<string, Guid> attributeIdLookup)
        {
            var entities = new List<DeficientConditionGoalDetailEntity>();
            foreach (var domain in domainList)
            {
                var entity = ToEntity(domain, simulationYearDetailId, attributeIdLookup);
                entities.Add(entity);
            }
            return entities;
        }

        private static DeficientConditionGoalDetailEntity ToEntity(
            DeficientConditionGoalDetail domain,
            Guid simulationYearDetailId,
            Dictionary<string, Guid> attributeIdLookup)
        {
            var attributeId = attributeIdLookup[domain.AttributeName];
            var id = Guid.NewGuid();
            var entity = new DeficientConditionGoalDetailEntity
            {
                Id = id,
                ActualDeficientPercentage = domain.ActualDeficientPercentage,
                AllowedDeficientPercentage = domain.AllowedDeficientPercentage,
                AttributeId = attributeId,
                DeficientLimit = domain.DeficientLimit,
                GoalIsMet = domain.GoalIsMet,
                GoalName = domain.GoalName,
                SimulationYearDetailId = simulationYearDetailId,
            };
            return entity;
        }
    }
}
