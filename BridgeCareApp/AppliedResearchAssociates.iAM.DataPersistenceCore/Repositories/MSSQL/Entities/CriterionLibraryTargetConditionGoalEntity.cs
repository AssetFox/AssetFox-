using System;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities
{
    public class CriterionLibraryTargetConditionGoalEntity
    {
        public Guid TargetConditionGoalId { get; set; }

        public Guid CriterionLibraryId { get; set; }

        public virtual TargetConditionGoalEntity TargetConditionGoal { get; set; }

        public virtual CriterionLibraryEntity CriterionLibrary { get; set; }
    }
}
