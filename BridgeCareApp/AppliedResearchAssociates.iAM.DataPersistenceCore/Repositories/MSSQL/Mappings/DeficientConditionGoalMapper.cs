using System;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.Domains;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappings
{
    public static class DeficientConditionGoalMapper
    {
        public static DeficientConditionGoalEntity ToEntity(this DeficientConditionGoal domain, Guid deficientConditionGoalLibraryId, Guid attributeId) =>
            new DeficientConditionGoalEntity
            {
                Id = Guid.NewGuid(),
                DeficientConditionGoalLibraryId = deficientConditionGoalLibraryId,
                AttributeId = attributeId,
                Name = domain.Name,
                AllowedDeficientPercentage = domain.AllowedDeficientPercentage,
                DeficientLimit = domain.DeficientLimit
            };

        public static void ToSimulationAnalysisDomain(this DeficientConditionGoalEntity entity,
            AnalysisMethod analysisMethod)
        {
            var deficientConditionGoal = analysisMethod.AddDeficientConditionGoal();
            deficientConditionGoal.Attribute = (NumberAttribute)Convert
                .ChangeType(entity.Attribute.ToDomain().ToSimulationAnalysisAttribute(), typeof(NumberAttribute));
            deficientConditionGoal.AllowedDeficientPercentage = entity.AllowedDeficientPercentage;
            deficientConditionGoal.DeficientLimit = entity.DeficientLimit;
            deficientConditionGoal.Name = entity.Name;
            deficientConditionGoal.Criterion.Expression =
                entity.CriterionLibraryDeficientConditionGoalJoin?.CriterionLibrary.MergedCriteriaExpression ??
                string.Empty;

        }
    }
}
