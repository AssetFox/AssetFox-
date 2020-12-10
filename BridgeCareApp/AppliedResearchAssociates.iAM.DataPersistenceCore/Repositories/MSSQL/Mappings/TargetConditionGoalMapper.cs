using System;
using System.Linq;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.Domains;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappings
{
    public static class TargetConditionGoalMapper
    {
        public static TargetConditionGoalEntity ToEntity(this TargetConditionGoal domain, Guid targetConditionGoalLibraryId, Guid attributeId) =>
            new TargetConditionGoalEntity
            {
                Id = domain.Id,
                TargetConditionGoalLibraryId = targetConditionGoalLibraryId,
                AttributeId = attributeId,
                Name = domain.Name,
                Target = domain.Target,
                Year = domain.Year
            };

        public static void CreateTargetConditionGoal(this TargetConditionGoalEntity entity, Simulation simulation)
        {
            var targetConditionGoal = simulation.AnalysisMethod.AddTargetConditionGoal();
            targetConditionGoal.Id = entity.Id;
            targetConditionGoal.Attribute = simulation.Network.Explorer.NumberAttributes
                .Single(_ => _.Name == entity.Attribute.Name);
            targetConditionGoal.Target = entity.Target;
            targetConditionGoal.Year = entity.Year;
            targetConditionGoal.Name = entity.Name;
            targetConditionGoal.Criterion.Expression =
                entity.CriterionLibraryTargetConditionGoalJoin?.CriterionLibrary.MergedCriteriaExpression ??
                string.Empty;

        }
    }
}
