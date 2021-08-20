using System;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.Abstract;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.ScenarioEntities.TargetConditionGoal
{
    public class CriterionLibraryScenarioTargetConditionGoalEntity : BaseEntity
    {
        public Guid ScenarioTargetConditionGoalId { get; set; }

        public Guid CriterionLibraryId { get; set; }

        public virtual ScenarioTargetConditionGoalEntity ScenarioTargetConditionGoal { get; set; }

        public virtual CriterionLibraryEntity CriterionLibrary { get; set; }
    }
}
