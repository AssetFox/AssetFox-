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
            Guid simulationYearDetailId,
            Dictionary<string, Guid> attributeIdLookup)
        {
            var id = Guid.NewGuid();
            var attributeId = attributeIdLookup[domain.AttributeName];
            var entity = new TargetConditionGoalDetailEntity
            {
                Id = id,
                ActualValue = domain.ActualValue,
                AttributeId = attributeId,
                GoalIsMet = domain.GoalIsMet,
                GoalName = domain.GoalName,
                SimulationYearDetailId = simulationYearDetailId,
                TargetValue = domain.TargetValue,
            };
            return entity;
        }

        public static List<TargetConditionGoalDetailEntity> ToEntityList(
            List<TargetConditionGoalDetail> domainList,
            Guid simulationYearDetailId,
            Dictionary<string, Guid> attributeIdLookup)
        {
            var entities = new List<TargetConditionGoalDetailEntity>();
            foreach (var domain in domainList)
            {
                var entity = ToEntity(domain, simulationYearDetailId, attributeIdLookup);
                entities.Add(entity);
            }
            return entities;
        }

        public static TargetConditionGoalDetail ToDomain(TargetConditionGoalDetailEntity entity, Dictionary<Guid, string> attributeNameLookup)
        {
            var attributeName = attributeNameLookup[entity.AttributeId];
            var domain = new TargetConditionGoalDetail
            {
                ActualValue = entity.ActualValue,
                AttributeName = attributeName,
                GoalIsMet = entity.GoalIsMet,
                GoalName = entity.GoalName,
                TargetValue = entity.TargetValue,
            };
            return domain;
        }

        public static List<TargetConditionGoalDetail> ToDomainList(ICollection<TargetConditionGoalDetailEntity> entityCollection, Dictionary<Guid, string> attributeNameLookup)
        {
            var domainList = new List<TargetConditionGoalDetail>();
            foreach (var entity in entityCollection)
            {
                var domain = ToDomain(entity, attributeNameLookup);
                domainList.Add(domain);
            }
            return domainList;
        }
    }
}
