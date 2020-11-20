using System;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.Domains;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappings
{
    public static class TargetConditionGoalMapper
    {
        public static TargetConditionGoalEntity ToEntity(this TargetConditionGoal domain, Guid targetConditionGoalLibraryId, Guid attributeId) =>
            new TargetConditionGoalEntity
            {
                Id = Guid.NewGuid(),
                TargetConditionGoalLibraryId = targetConditionGoalLibraryId,
                AttributeId = attributeId,
                Name = domain.Name,
                Target = domain.Target,
                Year = domain.Year
            };

        public static void ToSimulationAnalysisDomain(this TargetConditionGoalEntity entity,
            AnalysisMethod analysisMethod)
        {
            var targetConditionGoal = analysisMethod.AddTargetConditionGoal();
            targetConditionGoal.Attribute = (NumberAttribute)Convert
                .ChangeType(entity.Attribute.ToDomain().ToSimulationAnalysisAttribute(), typeof(NumberAttribute));
            targetConditionGoal.Target = entity.Target;
            targetConditionGoal.Year = entity.Year;
            targetConditionGoal.Name = entity.Name;
            targetConditionGoal.Criterion.Expression =
                entity.CriterionLibraryTargetConditionGoalJoin?.CriterionLibrary.MergedCriteriaExpression ??
                string.Empty;

        }
    }
}
